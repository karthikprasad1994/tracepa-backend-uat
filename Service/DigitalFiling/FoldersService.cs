using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using TracePca.Data;
using TracePca.Dto.DigitalFiling;
using TracePca.Interface.Audit;
using TracePca.Interface.DigitalFiling;
using TracePca.Service.DigitalFilling;

namespace TracePca.Service.DigitalFiling
{
    public class FoldersService : FoldersInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public FoldersService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _connectionString = GetConnectionStringFromSession();
        }
        private string GetConnectionStringFromSession()
        {
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connStr = _configuration.GetConnectionString(dbName);
            if (string.IsNullOrWhiteSpace(connStr))
                throw new Exception($"Connection string for '{dbName}' not found in configuration.");

            return connStr;
        }

        public async Task<DigitalFilingDropDownListDataDTO> LoadAllDDLDataAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var departmentListTask = connection.QueryAsync<DFDropDownListData>(
                    "SELECT Org_node AS ID, Org_Name AS Name FROM Sad_Org_Structure WHERE Org_DelFlag = 'A' AND Org_LevelCode = 3 ORDER BY Org_Name");

                var statusListTask = connection.QueryAsync<DFDropDownListData>(
                    "SELECT 0 AS ID, 'Activated' AS Name UNION ALL SELECT 1, 'De-Activated' UNION ALL SELECT 2, 'Waiting for Approval' UNION ALL SELECT 3, 'All'");

                var permissionLevelListTask = connection.QueryAsync<DFDropDownListData>(
                    "SELECT 1, 'User' UNION ALL SELECT 2, 'Group'");

                var permissionOptionsListTask = connection.QueryAsync<DFDropDownListData>(
                    "SELECT 1 AS ID, 'Create Folder' AS Name UNION ALL SELECT 2, 'Modify Sub Cabinet' UNION ALL SELECT 3, 'De-Activate Sub Cabinet' UNION ALL SELECT 4, 'Search' UNION ALL SELECT 5, 'Index' UNION ALL SELECT 6, 'View Sub Cabinet'");

                await Task.WhenAll(departmentListTask, statusListTask, permissionLevelListTask, permissionOptionsListTask);

                return new DigitalFilingDropDownListDataDTO
                {
                    DepartmentList = departmentListTask.Result.ToList(),
                    StatusList = statusListTask.Result.ToList(),
                    PermissionLevelList = permissionLevelListTask.Result.ToList(),
                    PermissionOptionsList = permissionOptionsListTask.Result.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all DDL data.", ex);
            }
        }

        public async Task<List<DFDropDownListData>> LoadCabinetsByUserIdDDLAsync(int compId, int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var deptQuery = @"SELECT DISTINCT Org_Node FROM Sad_Org_Structure LEFT JOIN Sad_UsersInOtherDept ON SUO_DeptID = Org_Node WHERE Org_DelFlag = 'A' AND Org_CompID = @CompID AND Org_LevelCode = 3";
                var departments = await connection.QueryAsync<int>(deptQuery, new { CompID = compId });
                var deptList = departments.ToList();
                string allUserDept = deptList.Any() ? string.Join(",", deptList) : "";

                var userInfoQuery = "SELECT USR_IsSuperUser, USR_MemberType FROM Sad_userdetails WHERE usr_id = @UserID";
                var userInfo = await connection.QueryFirstOrDefaultAsync(userInfoQuery, new { UserID = userId });
                int isSuperUser = userInfo?.USR_IsSuperUser ?? 0;
                int memberType = userInfo?.USR_MemberType ?? 0;

                string cabinetQuery;
                if (isSuperUser == 1 || memberType == 1)
                {
                    cabinetQuery = @"SELECT FOL_FolID AS ID, FOL_NAME AS Name FROM EDT_Folder a INNER JOIN Sad_Org_Structure b ON a.FOL_Department = b.Org_node WHERE FOL_Cabinet = -1 AND a.FOL_DelFlag = 'A'";
                    if (!string.IsNullOrWhiteSpace(allUserDept))
                    {
                        cabinetQuery += $" AND b.Org_Node IN ({allUserDept})";
                    }
                    cabinetQuery += " ORDER BY FOL_NAME";
                }
                else
                {
                    cabinetQuery = @"SELECT DISTINCT FOL_FolID AS ID, FOL_NAME AS Name FROM view_cabpermissions WHERE FOL_Cabinet = -1 AND FOL_DelFlag = 'A'";
                    if (!string.IsNullOrWhiteSpace(allUserDept))
                    {
                        cabinetQuery += $" AND (FOL_Department IN ({allUserDept}) OR EFP_Department IN ({allUserDept}))";
                    }
                    cabinetQuery += " ORDER BY FOL_NAME";
                }

                var cabinets = await connection.QueryAsync<DFDropDownListData>(cabinetQuery);
                return cabinets.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading user cabinet dropdown data.", ex);
            }
        }

        public async Task<DigitalFilingDropDownListDataDTO> LoadSubCabinetsByCabinetIdDDLAsync(int compId, int cabinetId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"Select FOL_FolID AS ID,FOL_NAME AS Name where FOL_Cabinet = @CabinetId and FOL_DelFlag ='A' and FOL_CompID = @CompId order by FOL_name";

                var userList = await connection.QueryAsync<DFDropDownListData>(query, new { CompId = compId, CabinetId = cabinetId });

                return new DigitalFilingDropDownListDataDTO
                {
                    SubCabinetList = userList?.ToList() ?? new List<DFDropDownListData>()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading sub cabinet dropdown data.", ex);
            }
        }


		public async Task<List<FolderDetailDTO>> GetAllFoldersDetailsBySubCabinetIdAsync(int subCabinetId, string statusCode)
		{
			try
			{
				string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
				if (string.IsNullOrEmpty(dbName))
					throw new Exception("CustomerCode is missing in session. Please log in again.");

				var connectionString = _configuration.GetConnectionString(dbName);

				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();



                //string query = @"SELECT 
                //                  distinct  A.FOL_FolID, 
                //                    A.FOL_Name, 
                //                    A.FOL_Note,
                //                    B.cbn_name AS FOL_SubCabinet,
                //                    (
                //                        SELECT COUNT(*) 
                //                        FROM edt_page P
                //                        WHERE P.pge_Folder = A.FOL_FolID 
                //                          AND P.pge_SubCabinet = A.FOL_Cabinet
                //                    ) AS FOL_Documents,
                //                    C.Usr_FullName as FOL_CreatedBy,
                //                    A.FOL_CreatedOn,
                //                    A.FOL_UpdatedBy,
                //                    A.FOL_UpdatedOn,
                //                    A.FOL_Status,
                //                    A.FOL_DelFlag
                //                FROM edt_Folder A 
                //                JOIN edt_Cabinet B ON A.FOL_Cabinet = B.cbn_ID  
                //                join sad_userDetails C on A.Fol_CreatedBy = C.Usr_ID
                //                WHERE A.FOL_Status = @statusCode 
                //                  AND A.FOL_Cabinet = @subCabinetId;";

                string query = @"SELECT DISTINCT A.FOL_FolID, A.FOL_Name, A.FOL_Note,B.cbn_name AS FOL_SubCabinet,
                                (SELECT COUNT(*) FROM edt_page P WHERE P.pge_Folder = A.FOL_FolID AND P.pge_SubCabinet = A.FOL_Cabinet) AS 
                                FOL_Documents,C.Usr_FullName AS FOL_CreatedBy,A.FOL_CreatedOn,A.FOL_UpdatedBy,
                                A.FOL_UpdatedOn,A.FOL_Status,A.FOL_DelFlag,(
                                SELECT STRING_AGG(DisplayPath + 'BITMAPS\'  
                                + CAST(FLOOR(CAST(AT.Atch_DocID AS numeric)/301) AS varchar) + '\' 
                                + CAST(AT.Atch_DocID AS varchar) + '.' + AT.ATCH_Ext, '| ')
                                FROM edt_page P LEFT JOIN edt_Attachments AT ON P.PGE_BaseName = AT.Atch_ID
                                CROSS JOIN (SELECT TOP 1 SAD_Config_Value AS DisplayPath FROM Sad_Config_Settings WHERE Sad_Config_Key = 'DisplayPath') cfg
                                WHERE P.pge_Folder = A.FOL_FolID
                                AND P.pge_SubCabinet = A.FOL_Cabinet) AS DocumentPath
                                FROM edt_Folder A 
                                JOIN edt_Cabinet B ON A.FOL_Cabinet = B.cbn_ID  
                                JOIN sad_userDetails C ON A.FOL_CreatedBy = C.Usr_ID
                                WHERE A.FOL_Status = @statusCode
                                AND A.FOL_Cabinet = @subCabinetId";
             
				var result = await connection.QueryAsync<FolderDetailDTO>(query, new
				{
					statusCode,
					subCabinetId
				});

				return result.ToList();
			}
			catch (Exception)
			{
				throw; // rethrow preserves stack trace
			}
		}

		//public async Task<List<FolderDTO>> GetAllFoldersBySubCabinetIdAsync(int subCabinetId, string statusCode)
		//      {
		//          try
		//          {
		//              var result = new List<FolderDTO>();

		//              using var connection = new SqlConnection(_connectionString);
		//              await connection.OpenAsync();

		//              // TODO: Implement ExtendPermissions logic
		//              // objclsFolders.LoadFolders(Session("AccessCode"), Session("AccessCodeID"), iSubCabId, Session("UserID"))

		//              string query;
		//              if (statusCode == "D")
		//                  query = "SELECT * FROM EDT_Folder WHERE FOL_DelFlag = 'D' AND FOL_Cabinet = @SubCabinetId Order by FOL_NAME Asc";
		//              else if (statusCode == "W")
		//                  query = "SELECT * FROM EDT_Folder WHERE FOL_DelFlag = 'W' AND FOL_Cabinet = @SubCabinetId Order by FOL_NAME Asc";
		//              else
		//                  query = "SELECT * FROM EDT_Folder WHERE FOL_DelFlag != 'V' AND FOL_Cabinet = @SubCabinetId Order by FOL_NAME Asc";

		//              var folders = (await connection.QueryAsync<dynamic>(query, new { SubCabinetId = subCabinetId })).ToList();

		//              var userIds = new HashSet<int>();
		//              var departmentIds = new HashSet<int>();
		//              foreach (var dr in folders)
		//              {
		//                  if (dr.FOL_CreatedBy > 0) userIds.Add(dr.FOL_CreatedBy);
		//                  if (dr.FOL_UpdatedBy > 0) userIds.Add(dr.FOL_UpdatedBy);
		//                  if (dr.FOL_ApprovedBy > 0) userIds.Add(dr.FOL_ApprovedBy);
		//                  if (dr.FOL_DeletedBy > 0) userIds.Add(dr.FOL_DeletedBy);
		//                  if (dr.FOL_RecalledBy > 0) userIds.Add(dr.FOL_RecalledBy);
		//                  departmentIds.Add((int)dr.FOL_Department);
		//              }

		//              var userNameQuery = "SELECT usr_id, usr_fullname FROM sad_userdetails WHERE usr_id IN @UserIds";
		//              var userNamesDict = (await connection.QueryAsync<(int usr_id, string usr_fullname)>(userNameQuery, new { UserIds = userIds })).ToDictionary(x => x.usr_id, x => x.usr_fullname);

		//              var departmentQuery = "SELECT Org_Node, Org_Name FROM Sad_Org_Structure WHERE Org_Node IN @DeptIds";
		//              var departmentDict = (await connection.QueryAsync<(int Org_Node, string Org_Name)>(departmentQuery, new { DeptIds = departmentIds })).ToDictionary(x => x.Org_Node, x => x.Org_Name);

		//              foreach (var dr in folders)
		//              {
		//                  string delFlagDesc = dr.FOL_DelFlag switch
		//                  {
		//                      "A" => "Activated",
		//                      "D" => "De-Activated",
		//                      "W" => "Waiting for Approval",
		//                      _ => dr.FOL_DelFlag
		//                  };

		//                  departmentDict.TryGetValue(Convert.ToInt32(dr.FOL_Department), out string departmentName);
		//                  userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_CreatedBy), out string createdByName);
		//                  userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_UpdatedBy), out string updatedByName);
		//                  userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_ApprovedBy), out string approvedByName);
		//                  userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_DeletedBy), out string deletedByName);
		//                  userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_RecalledBy), out string recalledByName);

		//                  var subCabinet = new FolderDTO
		//                  {
		//                      FOL_FolID = dr.FOL_FolID,
		//                      FOL_Name = dr.FOL_Name,
		//                      FOL_Cabinet = dr.FOL_Cabinet,
		//                      FOL_Note = dr.FOL_Note,
		//                      FOL_CreatedBy = dr.FOL_CreatedBy,
		//                      FOL_CreatedByName = createdByName,
		//                      FOL_CreatedOn = dr.FOL_CreatedOn,
		//                      FOL_UpdatedBy = dr.FOL_UpdatedBy,
		//                      FOL_UpdatedByName = updatedByName,
		//                      FOL_UpdatedOn = dr.FOL_UpdatedOn,
		//                      FOL_ApprovedBy = dr.FOL_ApprovedBy,
		//                      FOL_ApprovedByName = approvedByName,
		//                      FOL_ApprovedOn = dr.FOL_ApprovedOn,
		//                      FOL_DeletedBy = dr.FOL_DeletedBy,
		//                      FOL_DeletedByName = deletedByName,
		//                      FOL_DeletedOn = dr.FOL_DeletedOn,
		//                      FOL_RecalledBy = dr.FOL_RecalledBy,
		//                      FOL_RecalledByName = recalledByName,
		//                      FOL_RecalledOn = dr.FOL_RecalledOn,
		//                      FOL_DelFlag = delFlagDesc,
		//                      FOL_CompID = dr.FOL_CompID,
		//                  };
		//                  result.Add(subCabinet);
		//              }

		//              return result;
		//          }
		//          catch (Exception ex)
		//          {
		//              throw new ApplicationException("An error occurred while loading all folder data.", ex);
		//          }
		//      }



		public async Task<List<FolderDTO>> GetAllFoldersBySubCabinetIdAsync(int subCabinetId, string statusCode)
		{
			try
			{
				//var result = new List<FolderDTO>();

				using var connection = new SqlConnection(_connectionString);
				await connection.OpenAsync();
 
				string query;
				if (statusCode == "D")
					query = "SELECT * FROM EDT_Folder WHERE FOL_DelFlag = 'D' AND FOL_Cabinet = @SubCabinetId Order by FOL_NAME Asc";
				else if (statusCode == "W")
					query = "SELECT * FROM EDT_Folder WHERE FOL_DelFlag = 'W' AND FOL_Cabinet = @SubCabinetId Order by FOL_NAME Asc";
				else
					query = "SELECT * FROM EDT_Folder WHERE FOL_DelFlag != 'V' AND FOL_Cabinet = @SubCabinetId Order by FOL_NAME Asc";

				//var folders = (await connection.QueryAsync<dynamic>(query, new { SubCabinetId = subCabinetId })).ToList();

				//var userIds = new HashSet<int>();
				//var departmentIds = new HashSet<int>();
				//foreach (var dr in folders)
				//{
				//	if (dr.FOL_CreatedBy > 0) userIds.Add(dr.FOL_CreatedBy);
				//	if (dr.FOL_UpdatedBy > 0) userIds.Add(dr.FOL_UpdatedBy);
				//	if (dr.FOL_ApprovedBy > 0) userIds.Add(dr.FOL_ApprovedBy);
				//	if (dr.FOL_DeletedBy > 0) userIds.Add(dr.FOL_DeletedBy);
				//	if (dr.FOL_RecalledBy > 0) userIds.Add(dr.FOL_RecalledBy);
				//	departmentIds.Add((int)dr.FOL_Department);
				//}

				//var userNameQuery = "SELECT usr_id, usr_fullname FROM sad_userdetails WHERE usr_id IN @UserIds";
				//var userNamesDict = (await connection.QueryAsync<(int usr_id, string usr_fullname)>(userNameQuery, new { UserIds = userIds })).ToDictionary(x => x.usr_id, x => x.usr_fullname);

				//var departmentQuery = "SELECT Org_Node, Org_Name FROM Sad_Org_Structure WHERE Org_Node IN @DeptIds";
				//var departmentDict = (await connection.QueryAsync<(int Org_Node, string Org_Name)>(departmentQuery, new { DeptIds = departmentIds })).ToDictionary(x => x.Org_Node, x => x.Org_Name);

				//foreach (var dr in folders)
				//{
				//	string delFlagDesc = dr.FOL_DelFlag switch
				//	{
				//		"A" => "Activated",
				//		"D" => "De-Activated",
				//		"W" => "Waiting for Approval",
				//		_ => dr.FOL_DelFlag
				//	};

				//	departmentDict.TryGetValue(Convert.ToInt32(dr.FOL_Department), out string departmentName);
				//	userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_CreatedBy), out string createdByName);
				//	userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_UpdatedBy), out string updatedByName);
				//	userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_ApprovedBy), out string approvedByName);
				//	userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_DeletedBy), out string deletedByName);
				//	userNamesDict.TryGetValue(Convert.ToInt32(dr.FOL_RecalledBy), out string recalledByName);

				var result = await connection.QueryAsync<FolderDTO>(query, new
				{
					SubCabinetId = subCabinetId
				});

				return result.ToList();
                 
			}
			catch (Exception ex)
			{
				throw new ApplicationException("An error occurred while loading all folder data.", ex);
			}
		}

		public async Task<string> UpdateFolderStatusAsync(UpdateFolderStatusRequestDTO request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                if (request.StatusCode == "D")
                {
                    int extraPermissions = await CheckExtraPermissionsToFolderAsync(request.UserId, request.FolderId, "EFP_DEL_FOLDER");
                    if (extraPermissions == 0)
                    {
                        return "Folder permission is not Assigned.";
                    }
                }

                string sql = "UPDATE EDT_Folder SET ";
                if (request.StatusCode == "D")
                    sql += "FOL_DelFlag = @DelFlag, FOL_DeletedBy = @UserId, FOL_DeletedOn = GETDATE(), FOL_Status = 'AD'";
                else if (request.StatusCode == "A")
                    sql += "FOL_DelFlag = @DelFlag, FOL_RecalledBy = @UserId, FOL_RecalledOn = GETDATE(), FOL_Status = 'AR'";
                else if (request.StatusCode == "W")
                    sql += "FOL_DelFlag = @DelFlag, FOL_ApprovedBy = @UserId, FOL_ApprovedOn = GETDATE(), FOL_Status = 'A'";
                else if (request.StatusCode == "AV")
                    sql += "FOL_DelFlag = @DelFlag, FOL_UpdatedBy = @UserId, FOL_UpdatedOn = GETDATE(), FOL_Status = 'AV'";
                sql += " WHERE FOL_FolID = @FolderId AND FOL_CompID = @CompId";
                var parameters = new { DelFlag = request.StatusCode, UserId = request.UserId, FolderId = request.FolderId, CompId = request.CompId };
                await connection.ExecuteAsync(sql, parameters);

                if (request.StatusCode == "W")
                {
                    var cabSql = @"Update edt_cabinet set CBN_FolderCount=(SELECT COUNT(Fol_folid) from edt_folder where fol_cabinet in (Select CBN_id from Edt_Cabinet where CBN_Parent = @CabinetId And (CBN_DelFlag='A'))) where CBN_ID = @CabinetId and CBN_CompID = @CompId";
                    var SubCabSql = @"Update edt_cabinet set CBN_FolderCount=(select Count(Fol_folid) from edt_folder where fol_cabinet = @SubCabinetId and (FOL_Delflag='A')) where cbn_ID = @SubCabinetId and CBN_CompID = @CompId";
                    var param = new { CabinetId = request.CabinetId, SubCabinetId = request.SubCabinetId, CompId = request.CompId };

                    await connection.ExecuteAsync(cabSql, param);
                    await connection.ExecuteAsync(SubCabSql, param);
                }

                string message = request.StatusCode switch
                {
                    "D" => "Successfully De-Activated.",
                    "A" => "Successfully Activated.",
                    "W" => "Successfully Approved.",
                    _ => "Status updated."
                };
                return message;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating folder status.", ex);
            }
        }

        public async Task<int> CheckExtraPermissionsToFolderAsync(int userId, int folderId, string permTypes)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var userInfoQuery = @"SELECT USR_DeptID, usr_IsSuperuser FROM sad_userdetails WHERE usr_id = @UserId";
                var userInfo = await connection.QueryFirstOrDefaultAsync<(int USR_DeptID, int usr_IsSuperuser)>(userInfoQuery, new { UserId = userId });
                int userDeptId = userInfo.USR_DeptID;
                int userIsSuperUser = userInfo.usr_IsSuperuser;

                if (userIsSuperUser == 1)
                    return 1;

                var existCountQuery = @"SELECT COUNT(*) FROM EDT_Folder_Permission WHERE EFP_FolId = @FolderId";
                int existCount = await connection.ExecuteScalarAsync<int>(existCountQuery, new { FolderId = folderId });
                if (existCount == 0)
                    return 0;

                var countOtherQuery = @"SELECT COUNT(*) FROM EDT_Folder_Permission WHERE EFP_FolId = @FolderId AND EFP_GRPID = @UserDeptId AND EFP_USRID = @UserId AND EFP_Other = 1";
                int countOther = await connection.ExecuteScalarAsync<int>(countOtherQuery, new { FolderId = folderId, UserDeptId = userDeptId, UserId = userId });
                if (countOther != 0)
                {
                    var userPermQuery = $@"SELECT {permTypes} AS ELevel, EFP_Other AS Permission FROM EDT_Folder_Permission WHERE EFP_USRID = @UserId AND EFP_PTYPE = 'U' AND EFP_FolId = @FolderId AND EFP_GRPID = @UserDeptId";
                    var userPerm = await connection.QueryFirstOrDefaultAsync<(int ELevel, int Permission)>(userPermQuery, new { UserId = userId, FolderId = folderId, UserDeptId = userDeptId });
                    if (userPerm.Permission != 0)
                        return userPerm.ELevel;

                    return 0;
                }
                else
                {
                    var groupPermQuery = $@"SELECT {permTypes} FROM EDT_Folder_Permission WHERE EFP_USRID = 0 AND EFP_PTYPE = 'G' AND EFP_FolId = @FolderId AND EFP_GRPID = @UserDeptId";
                    int groupPerm = await connection.ExecuteScalarAsync<int>(groupPermQuery, new { FolderId = folderId, UserDeptId = userDeptId });
                    return groupPerm;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> SaveOrUpdateFolderAsync(FolderDTO dto)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var deptInfoQuery = "Select ISNULL(cbn_department, 0) AS cbn_department from EDT_Cabinet where cbn_id = @SubCabinet";
                var deptInfo = await connection.QueryFirstOrDefaultAsync(deptInfoQuery, new { SubCabinet = dto.FOL_SubCabinet });
                int deptId = deptInfo?.cbn_department ?? 0;

                var exists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM EDT_Folder WHERE FOL_CompID = @CompId AND FOL_Name = @Name AND FOL_FolID <> @Id AND (FOL_DelFlag = 'A' OR FOL_DelFlag = 'W') AND FOL_Cabinet = @Parent",
                    new { CompId = dto.FOL_CompID, Name = dto.FOL_Name, Id = dto.FOL_FolID, Parent = dto.FOL_Cabinet },
                    transaction);

                if (exists > 0)
                {
                    await transaction.RollbackAsync();
                    throw new ApplicationException("Folder name already exists.");
                }

                int resultId;

                if (dto.FOL_FolID == 0)
                {
                    resultId = await connection.ExecuteScalarAsync<int>(
                        "DECLARE @NewId INT = (SELECT ISNULL(MAX(FOL_FolID), 0) + 1 FROM EDT_Folder);" +
                        "INSERT INTO EDT_Folder (FOL_FolID, FOL_Name, FOL_Note, FOL_Cabinet, FOL_CreatedBy, FOL_CreatedOn, FOL_Status, FOL_DelFlag, FOL_CompID)" +
                        "VALUES (@NewId, @Name, @Note, @Parent, @CreatedBy, GETDATE(), 'C', 'W', @CompID); SELECT @NewId;",
                        new
                        {
                            Name = dto.FOL_Name,
                            Note = dto.FOL_Note,
                            Parent = dto.FOL_Cabinet,
                            CreatedBy = dto.FOL_CreatedBy,
                            CompID = dto.FOL_CompID
                        },
                        transaction);

                    await connection.ExecuteAsync(
                        "DECLARE @pkId INT = (SELECT ISNULL(MAX(EFP_ID), 0) + 1 FROM EDT_Folder_Permission);" +
                        "INSERT INTO EDT_Folder_Permission (EFP_ID, EFP_PTYPE, EFP_GRPID, EFP_USRID, EFP_INDEX, EFP_SEARCH, EFP_MOD_FOLDER, EFP_MOD_DOC, EFP_DEL_FOLDER, EFP_DEL_DOC, EFP_EXPORT, EFP_OTHER, EFP_CRT_DOC, EFP_VIEW_Fol, EFP_FolId)" +
                        "VALUES (@pkId, 'G', @Department, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, @Folder);",
                        new
                        {
                            Folder = resultId,
                            Department = deptId
                        },
                        transaction);
                }
                else
                {
                    await connection.ExecuteAsync(
                        "UPDATE EDT_Folder SET FOL_Name = @Name, FOL_Note = @Note, FOL_UpdatedBy = @UpdatedBy, FOL_UpdatedOn = GETDATE(), FOL_Status = 'U'  WHERE FOL_FolID = @Id AND FOL_CompID = @CompID;",
                        new
                        {
                            Name = dto.FOL_Name,
                            Note = dto.FOL_Note,
                            Id = dto.FOL_FolID,
                            UpdatedBy = dto.FOL_UpdatedBy,
                            CompID = dto.FOL_CompID
                        },
                        transaction);

                    resultId = dto.FOL_FolID;

                    if (dto.FolderPermissionDTODetails != null)
                    {
                        if (dto.FolderPermissionDTODetails.EFP_ID == 0)
                        {
                            await connection.ExecuteAsync(@"DECLARE @pkId INT = (SELECT ISNULL(MAX(EFP_ID), 0) + 1 FROM EDT_Folder_Permission);
                              INSERT INTO EDT_Folder_Permission (EFP_ID, EFP_PTYPE, EFP_GRPID, EFP_USRID, EFP_INDEX, EFP_SEARCH, EFP_MOD_FOLDER, EFP_MOD_DOC, EFP_DEL_FOLDER, EFP_DEL_DOC, EFP_EXPORT, EFP_OTHER, EFP_CRT_DOC, EFP_VIEW_Fol, EFP_FolId)
                              VALUES (@pkId, @PermissionType, @Department, @User, @Index, @Search, @ModifyFolder, @ModifyDoc, @DeleteFolder, @DeleteDoc, @Export, @CreateDoc, @ViewFolder, @FolderId);",
                                new
                                {
                                    PermissionType = dto.FolderPermissionDTODetails.EFP_PTYPE ?? "G",
                                    Department = dto.FolderPermissionDTODetails.EFP_GRPID,
                                    User = dto.FolderPermissionDTODetails.EFP_USRID,
                                    Index = dto.FolderPermissionDTODetails.EFP_INDEX,
                                    Search = dto.FolderPermissionDTODetails.EFP_SEARCH,
                                    ModifyFolder = dto.FolderPermissionDTODetails.EFP_MOD_FOLDER,
                                    ModifyDoc = dto.FolderPermissionDTODetails.EFP_MOD_DOC,
                                    DeleteFolder = dto.FolderPermissionDTODetails.EFP_DEL_FOLDER,
                                    DeleteDoc = dto.FolderPermissionDTODetails.EFP_DEL_DOC,
                                    Export = dto.FolderPermissionDTODetails.EFP_EXPORT,
                                    CreateDoc = dto.FolderPermissionDTODetails.EFP_CRT_DOC,
                                    ViewFolder = dto.FolderPermissionDTODetails.EFP_VIEW_Fol,
                                    FolderId = dto.FolderPermissionDTODetails.EFP_FolId
                                },
                                transaction
                            );
                        }
                        else
                        {
                            await connection.ExecuteAsync(@"UPDATE EDT_Folder_Permission SET EFP_PTYPE=@PermissionType, EFP_GRPID=@GroupId, EFP_USRID=@UserId, EFP_INDEX=@Index, EFP_SEARCH=@Search, EFP_MOD_FOLDER=@ModifyFolder,
                            EFP_MOD_DOC=@ModifyDoc, EFP_DEL_FOLDER=@DeleteFolder, EFP_DEL_DOC=@DeleteDoc, EFP_EXPORT=@Export, EFP_OTHER=@Other, EFP_CRT_DOC=@CreateDoc, EFP_VIEW_Fol=@ViewFolder, EFP_FolId=@FolderId WHERE EFP_ID=@Id;",
                                new
                                {
                                    Id = dto.FolderPermissionDTODetails.EFP_ID,
                                    PermissionType = dto.FolderPermissionDTODetails.EFP_PTYPE ?? "G",
                                    Department = dto.FolderPermissionDTODetails.EFP_GRPID,
                                    User = dto.FolderPermissionDTODetails.EFP_USRID,
                                    Index = dto.FolderPermissionDTODetails.EFP_INDEX,
                                    Search = dto.FolderPermissionDTODetails.EFP_SEARCH,
                                    ModifyFolder = dto.FolderPermissionDTODetails.EFP_MOD_FOLDER,
                                    ModifyDoc = dto.FolderPermissionDTODetails.EFP_MOD_DOC,
                                    DeleteFolder = dto.FolderPermissionDTODetails.EFP_DEL_FOLDER,
                                    DeleteDoc = dto.FolderPermissionDTODetails.EFP_DEL_DOC,
                                    Export = dto.FolderPermissionDTODetails.EFP_EXPORT,
                                    CreateDoc = dto.FolderPermissionDTODetails.EFP_CRT_DOC,
                                    ViewFolder = dto.FolderPermissionDTODetails.EFP_VIEW_Fol,
                                    FolderId = dto.FolderPermissionDTODetails.EFP_FolId
                                },
                                transaction
                            );
                        }
                    }
                }

                await connection.ExecuteAsync(
                    "Update edt_cabinet set CBN_FolderCount=(select count(Fol_folid) from edt_folder where fol_cabinet in (Select CBN_id from Edt_Cabinet where CBN_Parent = @CabId And (CBN_DelFlag='A'))) where CBN_ID = @CabId and CBN_CompID = @CompId",
                    new { CabId = dto.FOL_Cabinet, CompId = dto.FOL_CompID },
                    transaction);

                await connection.ExecuteAsync(
                    "Update edt_cabinet set CBN_FolderCount=(select Count(Fol_folid) from edt_folder where fol_cabinet = @SubCabId and (FOL_Delflag='A')) where cbn_ID = @SubCabId and CBN_CompID = @CompId;",
                    new { SubCabId = dto.FOL_SubCabinet, CompId = dto.FOL_CompID },
                    transaction);

                await transaction.CommitAsync();
                return resultId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //The implementation of the GetFinalPermissions and LoadCabinetGrid functions is pending. These will be taken from the Cabinet module once Steffi's work is integrated, as the same logic will be used in both the Cabinet and Folder modules
    }
}
