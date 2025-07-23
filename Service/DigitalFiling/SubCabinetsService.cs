using Dapper;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using TracePca.Data;
using TracePca.Dto.DigitalFiling;
using TracePca.Interface.DigitalFiling;

namespace TracePca.Service.DigitalFiling
{
    public class SubCabinetsService : SubCabinetsInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public SubCabinetsService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<DigitalFilingDropDownListDataDTO> LoadAllDDLDataAsync()
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                    cabinetQuery = @"SELECT CBN_ID AS ID, CBN_NAME AS Name FROM EDT_Cabinet a INNER JOIN Sad_Org_Structure b ON a.CBN_Department = b.Org_node WHERE CBN_Parent = -1 AND a.CBN_DelFlag = 'A'";
                    if (!string.IsNullOrWhiteSpace(allUserDept))
                    {
                        cabinetQuery += $" AND b.Org_Node IN ({allUserDept})";
                    }
                    cabinetQuery += " ORDER BY CBN_NAME";
                }
                else
                {
                    cabinetQuery = @"SELECT DISTINCT CBN_ID AS ID, CBN_NAME AS Name FROM view_cabpermissions WHERE CBN_Parent = -1 AND CBN_DelFlag = 'A'";
                    if (!string.IsNullOrWhiteSpace(allUserDept))
                    {
                        cabinetQuery += $" AND (CBN_Department IN ({allUserDept}) OR CBP_Department IN ({allUserDept}))";
                    }
                    cabinetQuery += " ORDER BY CBN_NAME";
                }

                var cabinets = await connection.QueryAsync<DFDropDownListData>(cabinetQuery);
                return cabinets.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading user cabinet dropdown data.", ex);
            }
        }

        public async Task<DigitalFilingDropDownListDataDTO> LoadPermissionUsersByDeptIdDDLAsync(int deptID)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = @"SELECT Usr_ID AS ID, Usr_FullName AS Name FROM Sad_UserDetails LEFT JOIN Sad_Org_Structure ON Org_node = Usr_DeptId WHERE Usr_DeptId = @DeptId AND USR_DutyStatus = 'A'";

                var userList = await connection.QueryAsync<DFDropDownListData>(query, new { DeptId = deptID });

                return new DigitalFilingDropDownListDataDTO
                {
                    CabinetUserPermissionList = userList?.ToList() ?? new List<DFDropDownListData>()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading user permission dropdown data.", ex);
            }
        }

        public async Task<List<SubCabinetDTO>> GetAllSubCabinetsByCabinetAndUserIdAsync(int userId, int cabinetId, string statusCode)
        {
            try
            {
                var result = new List<SubCabinetDTO>();

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                // TODO: Implement ExtendPermissions logic
                // objcab.LoadCabinetGrid(Session("AccessCode"), Session("AccessCodeID"), 0, sDeptID, Session("UserID"), ddlCabinet.SelectedValue)

                var userInfoQuery = "SELECT ISNULL(usr_IsSuperuser, 0) AS USR_IsSuperUser, ISNULL(usr_deptid, 0) AS USR_Department FROM sad_userdetails WHERE usr_id = @UserId";
                var userInfo = await connection.QueryFirstOrDefaultAsync(userInfoQuery, new { UserId = userId });
                int userType = userInfo?.USR_IsSuperUser ?? 0;
                int userParGrp = userInfo?.USR_Department ?? 0;

                string query;
                if (userType == 1)
                {
                    if (statusCode == "D")
                        query = "SELECT * FROM EDT_Cabinet WHERE Cbn_DelFlag = 'D' AND cbn_parent = @CabinetId Order by CBN_NAME Asc";
                    else if (statusCode == "W")
                        query = "SELECT * FROM EDT_Cabinet WHERE Cbn_DelFlag = 'W' AND cbn_parent = @CabinetId Order by CBN_NAME Asc";
                    else
                        query = "SELECT * FROM EDT_Cabinet WHERE Cbn_DelFlag != 'V' AND cbn_parent = @CabinetId Order by CBN_NAME Asc";
                }
                else
                {
                    if (statusCode == "D")
                        query = "SELECT * FROM EDT_Cabinet WHERE CBN_Department = @UserParGrp AND Cbn_DelFlag = 'D' AND cbn_parent = @CabinetId Order by CBN_NAME Asc";
                    else if (statusCode == "W")
                        query = "SELECT * FROM EDT_Cabinet WHERE CBN_Department = @UserParGrp AND Cbn_DelFlag = 'W' AND cbn_parent = @CabinetId Order by CBN_NAME Asc";
                    else
                        query = "SELECT * FROM EDT_Cabinet WHERE CBN_Department = @UserParGrp AND Cbn_DelFlag != 'V' AND cbn_parent = @CabinetId Order by CBN_NAME Asc";
                }

                var cabinets = (await connection.QueryAsync<dynamic>(query, new { UserParGrp = userParGrp, CabinetId = cabinetId })).ToList();

                var userIds = new HashSet<int>();
                var departmentIds = new HashSet<int>();
                foreach (var dr in cabinets)
                {
                    if (dr.CBN_CreatedBy > 0) userIds.Add(dr.CBN_CreatedBy);
                    if (dr.CBN_UpdatedBy > 0) userIds.Add(dr.CBN_UpdatedBy);
                    if (dr.CBN_ApprovedBy > 0) userIds.Add(dr.CBN_ApprovedBy);
                    if (dr.CBN_DeletedBy > 0) userIds.Add(dr.CBN_DeletedBy);
                    if (dr.CBN_RecalledBy > 0) userIds.Add(dr.CBN_RecalledBy);
                    departmentIds.Add((int)dr.CBN_Department);
                }

                var userNameQuery = "SELECT usr_id, usr_fullname FROM sad_userdetails WHERE usr_id IN @UserIds";
                var userNamesDict = (await connection.QueryAsync<(int usr_id, string usr_fullname)>(userNameQuery, new { UserIds = userIds })).ToDictionary(x => x.usr_id, x => x.usr_fullname);

                var departmentQuery = "SELECT Org_Node, Org_Name FROM Sad_Org_Structure WHERE Org_Node IN @DeptIds";
                var departmentDict = (await connection.QueryAsync<(int Org_Node, string Org_Name)>(departmentQuery, new { DeptIds = departmentIds })).ToDictionary(x => x.Org_Node, x => x.Org_Name);

                foreach (var dr in cabinets)
                {
                    string delFlagDesc = dr.Cbn_DelFlag switch
                    {
                        "A" => "Activated",
                        "D" => "De-Activated",
                        "W" => "Waiting for Approval",
                        _ => dr.Cbn_DelFlag
                    };

                    departmentDict.TryGetValue(dr.CBN_Department, out string departmentName);
                    userNamesDict.TryGetValue(dr.CBN_CreatedBy, out string createdByName);
                    userNamesDict.TryGetValue(dr.CBN_UpdatedBy, out string updatedByName);
                    userNamesDict.TryGetValue(dr.CBN_ApprovedBy, out string approvedByName);
                    userNamesDict.TryGetValue(dr.CBN_DeletedBy, out string deletedByName);
                    userNamesDict.TryGetValue(dr.CBN_RecalledBy, out string recalledByName);

                    var subCabinet = new SubCabinetDTO
                    {
                        CBN_ID = dr.CBN_ID,
                        CBN_Name = dr.CBN_Name,
                        CBN_Parent = dr.CBN_Parent,
                        CBN_Note = dr.CBN_Note,
                        CBN_UserID = dr.CBN_UserID,
                        CBN_Department = dr.CBN_Department,
                        CBN_DepartmentName = departmentName,
                        CBN_SubCabCount = dr.CBN_SubCabCount,
                        CBN_FolderCount = dr.CBN_FolderCount,
                        CBN_CreatedBy = dr.CBN_CreatedBy,
                        CBN_CreatedByName = createdByName,
                        CBN_CreatedOn = dr.CBN_CreatedOn,
                        CBN_UpdatedBy = dr.CBN_UpdatedBy,
                        CBN_UpdatedByName = updatedByName,
                        CBN_UpdatedOn = dr.CBN_UpdatedOn,
                        CBN_ApprovedBy = dr.CBN_ApprovedBy,
                        CBN_ApprovedByName = approvedByName,
                        CBN_ApprovedOn = dr.CBN_ApprovedOn,
                        CBN_DeletedBy = dr.CBN_DeletedBy,
                        CBN_DeletedByName = deletedByName,
                        CBN_DeletedOn = dr.CBN_DeletedOn,
                        CBN_RecalledBy = dr.CBN_RecalledBy,
                        CBN_RecalledByName = recalledByName,
                        CBN_RecalledOn = dr.CBN_RecalledOn,
                        CBN_DelFlag = delFlagDesc,
                        CBN_CompID = dr.CBN_CompID,
                        CBN_Retention = dr.CBN_Retention
                    };
                    result.Add(subCabinet);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all sub cabinet data.", ex);
            }
        }

        public async Task<string> UpdateSubCabinetStatusAsync(UpdateSubCabinetStatusRequestDTO request)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                if (request.StatusCode == "D")
                {
                    int extraPermissions = await CheckExtraPermissionsToCabinetAsync(request.UserId, request.CabinetId, "CBP_Delete");
                    if (extraPermissions == 0)
                    {
                        return "SubCabinet permission is not Assigned.";
                    }
                }

                string sql = "UPDATE EDT_Cabinet SET ";
                if (request.StatusCode == "D")
                    sql += "UPDATE EDT_Cabinet SET CBN_DelFlag = @DelFlag, CBN_DeletedBy = @UserId, CBN_DeletedOn = GETDATE(), CBN_Status = 'AD'";
                else if (request.StatusCode == "A")
                    sql += "UPDATE EDT_Cabinet SET CBN_DelFlag = @DelFlag, CBN_RecalledBy = @UserId, CBN_RecalledOn = GETDATE(), CBN_Status = 'AR'";
                else if (request.StatusCode == "W")
                    sql += "UPDATE EDT_Cabinet SET CBN_DelFlag = @DelFlag, CBN_ApprovedBy = @UserId, CBN_ApprovedOn = GETDATE(), CBN_Status = 'A'";
                else if (request.StatusCode == "AV")
                    sql += "CBN_DelFlag = @DelFlag, CBN_UpdatedBy = @UserId, CBN_UpdatedOn = GETDATE(), CBN_Status = 'AV'";
                sql += " WHERE CBN_ID = @SubCabinetId AND CBN_CompID = @CompId";
                var parameters = new { DelFlag = request.StatusCode, UserId = request.UserId, SubCabinetId = request.SubCabinetId, CompId = request.CompId };
                await connection.ExecuteAsync(sql, parameters);

                if (request.StatusCode == "W")
                {
                    var subCabSql = @"UPDATE EDT_Cabinet SET CBN_SubCabCount = (SELECT COUNT(CBN_ID) FROM EDT_Cabinet WHERE CBN_Parent = @CBN_ID AND CBN_DelFlag = 'A') WHERE CBN_ID = @CBN_ID AND CBN_CompID = @CompanyID";
                    var folderSql = @"UPDATE EDT_Cabinet SET CBN_FolderCount = (SELECT COUNT(Fol_folid) FROM edt_folder WHERE fol_cabinet IN (SELECT CBN_ID FROM EDT_Cabinet WHERE CBN_Parent = @CBN_ID AND CBN_DelFlag = 'A'
                                ))WHERE CBN_ID = @CBN_ID AND CBN_CompID = @CompId";
                    var param = new { CBN_ID = request.CabinetId, CompId = request.CompId };

                    await connection.ExecuteAsync(subCabSql, param);
                    await connection.ExecuteAsync(folderSql, param);
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
                throw new ApplicationException("An error occurred while updating sub cabinet status.", ex);
            }
        }

        public async Task<int> CheckExtraPermissionsToCabinetAsync(int userId, int cabinetId, string permTypes)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var userInfoQuery = @"SELECT USR_DeptID, usr_IsSuperuser FROM sad_userdetails WHERE usr_id = @UserId";
                var userInfo = await connection.QueryFirstOrDefaultAsync<(int USR_DeptID, int usr_IsSuperuser)>(userInfoQuery, new { UserId = userId });
                int userDeptId = userInfo.USR_DeptID;
                int userIsSuperUser = userInfo.usr_IsSuperuser;

                if (userIsSuperUser == 1)
                    return 1;

                var existCountQuery = @"SELECT COUNT(*) FROM EDT_Cabinet_Permission WHERE CBP_cabinet = @CabinetId";
                int existCount = await connection.ExecuteScalarAsync<int>(existCountQuery, new { CabinetId = cabinetId });
                if (existCount == 0)
                    return 0;

                var countOtherQuery = @"SELECT COUNT(*) FROM EDT_Cabinet_Permission WHERE CBP_cabinet = @CabinetId AND CBP_Department = @UserDeptId AND CBP_user = @UserId AND cbp_other = 1";
                int countOther = await connection.ExecuteScalarAsync<int>(countOtherQuery, new { CabinetId = cabinetId, UserDeptId = userDeptId, UserId = userId });
                if (countOther != 0)
                {
                    var userPermQuery = $@"SELECT {permTypes} AS ELevel, CBP_Other AS Permission FROM EDT_Cabinet_Permission WHERE CBP_user = @UserId AND CBP_PermissionType = 'U' AND CBP_cabinet = @CabinetId AND CBP_Department = @UserDeptId";
                    var userPerm = await connection.QueryFirstOrDefaultAsync<(int ELevel, int Permission)>(userPermQuery, new { UserId = userId, CabinetId = cabinetId, UserDeptId = userDeptId });
                    if (userPerm.Permission != 0)
                        return userPerm.ELevel;

                    return 0;
                }
                else
                {
                    var groupPermQuery = $@"SELECT {permTypes} FROM EDT_Cabinet_Permission WHERE CBP_user = 0 AND CBP_PermissionType = 'G' AND CBP_cabinet = @CabinetId AND CBP_Department = @UserDeptId";
                    int groupPerm = await connection.ExecuteScalarAsync<int>(groupPermQuery, new { CabinetId = cabinetId, UserDeptId = userDeptId });
                    return groupPerm;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<SubCabinetPermissionDTO> GetSubCabinetPermissionByLevelIdAsync(int compId, int departmentId, int userId, int subCabinetId)
        {
            try
            {
                await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var (isSuperUser, memberType) = await connection.QueryFirstOrDefaultAsync<(int usr_IsSuperuser, int USR_MemberType)>(
                    "SELECT ISNULL(usr_IsSuperuser,0) AS usr_IsSuperuser, ISNULL(USR_MemberType,0) AS USR_MemberType FROM sad_userdetails WHERE usr_id = @UserId",
                    new { UserId = userId });

                if (isSuperUser == 1 || memberType == 1)
                {
                    return new SubCabinetPermissionDTO
                    {
                        CBP_ID = 0,
                        CBP_PermissionType = "G",
                        CBP_Cabinet = subCabinetId,
                        CBP_User = userId,
                        CBP_Department = departmentId,
                        CBP_View = 1,
                        CBP_Create = 1,
                        CBP_Modify = 1,
                        CBP_Delete = 1,
                        CBP_Search = 1,
                        CBP_Index = 1,
                        CBP_Other = 1,
                        CBP_CreateFolder = 1
                    };
                }

                var dto = await connection.QueryFirstOrDefaultAsync<SubCabinetPermissionDTO>(
                     @"SELECT CBP_ID, CBP_PermissionType, CBP_Cabinet, CBP_User, CBP_Department, CBP_View, CBP_Create, CBP_Modify, CBP_Delete, CBP_Search, CBP_Index, CBP_Other, CBP_CreateFolder FROM EDT_Cabinet_Permission
                 WHERE CBP_CompID = @CompId AND CBP_Department = @DeptId AND CBP_User = @UserId AND CBP_Cabinet = @CabId;",
                     new { CompId = compId, DeptId = departmentId, UserId = userId, CabId = subCabinetId });

                return dto ?? new SubCabinetPermissionDTO();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting sub cabinet permission.", ex);
            }
        }

        public async Task<int> SaveOrUpdateSubCabinetAsync(SubCabinetDTO dto)
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var exists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM EDT_Cabinet WHERE CBN_CompID = @CompId AND CBN_Name = @Name AND CBN_ID <> @Id AND (CBN_DelFlag = 'A' OR CBN_DelFlag = 'W') AND CBN_Parent = @Parent",
                    new { CompId = dto.CBN_CompID, Name = dto.CBN_Name, Id = dto.CBN_ID, Parent = dto.CBN_Parent },
                    transaction);

                if (exists > 0)
                {
                    await transaction.RollbackAsync();
                    throw new ApplicationException("Sub Cabinet name already exists.");
                }

                int resultId;

                if (dto.CBN_ID == 0)
                {
                    resultId = await connection.ExecuteScalarAsync<int>(
                        "DECLARE @NewId INT = (SELECT ISNULL(MAX(CBN_ID), 0) + 1 FROM EDT_Cabinet);" +
                        "INSERT INTO EDT_Cabinet (CBN_ID, CBN_Name, CBN_Note, CBN_Parent, CBN_UserID, CBN_Department, CBN_SubCabCount, CBN_FolderCount, CBN_CreatedBy, CBN_CreatedOn, CBN_Status, CBN_DelFlag, CBN_CompID)" +
                        "VALUES (@NewId, @Name, @Note, @Parent, @UserID, @Department, 0, 0, @CreatedBy, GETDATE(), 'C', 'W', @CompID); SELECT @NewId;",
                        new
                        {
                            Name = dto.CBN_Name,
                            Note = dto.CBN_Note,
                            Parent = dto.CBN_Parent,
                            UserID = dto.CBN_CreatedBy,
                            CreatedBy = dto.CBN_CreatedBy,
                            Department = dto.CBN_Department,
                            CompID = dto.CBN_CompID
                        },
                        transaction);

                    await connection.ExecuteAsync(
                        "DECLARE @pkId INT = (SELECT ISNULL(MAX(CBP_ID), 0) + 1 FROM EDT_Cabinet_Permission);" +
                        "INSERT INTO EDT_Cabinet_Permission (CBP_ID, CBP_PermissionType, CBP_Cabinet, CBP_Department, CBP_User, CBP_View, CBP_Create, CBP_Modify, CBP_Delete, CBP_Search, CBP_Index, CBP_Other, CBP_CREATE_FOLDER)" +
                        "VALUES (@pkId, 'G', @SubCabinet, @Department, 0, 1, 1, 0, 0, 0, 0, 0, 0);",
                        new
                        {
                            SubCabinet = resultId,
                            Department = dto.CBN_Department
                        },
                        transaction);
                }
                else
                {
                    await connection.ExecuteAsync(
                        "UPDATE EDT_Cabinet SET CBN_Name = @Name, CBN_Note = @Note, CBN_Department = @Department, CBN_Parent = @Parent, CBN_UpdatedBy = @UpdatedBy, CBN_UpdatedOn = GETDATE(), CBN_Status = 'U'  WHERE CBN_ID = @Id AND CBN_CompID = @CompID;",
                        new
                        {
                            Name = dto.CBN_Name,
                            Note = dto.CBN_Note,
                            Department = dto.CBN_Department,
                            Parent = dto.CBN_Parent,
                            Id = dto.CBN_ID,
                            UpdatedBy = dto.CBN_UpdatedBy,
                            CompID = dto.CBN_CompID
                        },
                        transaction);

                    resultId = dto.CBN_ID;

                    if (dto.SubCabinetPermissionDetails != null)
                    {
                        if (dto.SubCabinetPermissionDetails.CBP_ID == 0)
                        {
                            await connection.ExecuteAsync(@"DECLARE @pkId INT = (SELECT ISNULL(MAX(CBP_ID), 0) + 1 FROM EDT_Cabinet_Permission);
                              INSERT INTO EDT_Cabinet_Permission (CBP_ID, CBP_PermissionType, CBP_Cabinet, CBP_User, CBP_Department, CBP_View, CBP_Create, CBP_Modify, CBP_Delete, CBP_Search, CBP_Index, CBP_Other, CBP_CREATE_FOLDER)
                              VALUES (@pkId, @PermissionType, @Cabinet, @User, @Department, @View, @Create, @Modify, @Delete, @Search, @Index, @Other, @CreateFolder);",
                                new
                                {
                                    PermissionType = dto.SubCabinetPermissionDetails.CBP_PermissionType ?? "G",
                                    Cabinet = dto.SubCabinetPermissionDetails.CBP_Cabinet,
                                    User = dto.SubCabinetPermissionDetails.CBP_User,
                                    Department = dto.SubCabinetPermissionDetails.CBP_Department,
                                    View = dto.SubCabinetPermissionDetails.CBP_View,
                                    Create = dto.SubCabinetPermissionDetails.CBP_Create,
                                    Modify = dto.SubCabinetPermissionDetails.CBP_Modify,
                                    Delete = dto.SubCabinetPermissionDetails.CBP_Delete,
                                    Search = dto.SubCabinetPermissionDetails.CBP_Search,
                                    Index = dto.SubCabinetPermissionDetails.CBP_Index,
                                    Other = dto.SubCabinetPermissionDetails.CBP_Other,
                                    CreateFolder = dto.SubCabinetPermissionDetails.CBP_CreateFolder
                                },
                                transaction
                            );
                        }
                        else
                        {
                            await connection.ExecuteAsync(@"UPDATE EDT_Cabinet_Permission SET CBP_PermissionType = @PermissionType, CBP_Cabinet = @Cabinet, CBP_User = @User, CBP_Department = @Department, CBP_View = @View,
                            CBP_Create = @Create, CBP_Modify = @Modify, CBP_Delete = @Delete, CBP_Search = @Search, CBP_Index = @Index, CBP_Other = @Other, CBP_CREATE_FOLDER = @CreateFolder WHERE CBP_ID = @Id;",
                                new
                                {
                                    Id = dto.SubCabinetPermissionDetails.CBP_ID,
                                    PermissionType = dto.SubCabinetPermissionDetails.CBP_PermissionType ?? "G",
                                    Cabinet = dto.SubCabinetPermissionDetails.CBP_Cabinet,
                                    User = dto.SubCabinetPermissionDetails.CBP_User,
                                    Department = dto.SubCabinetPermissionDetails.CBP_Department,
                                    View = dto.SubCabinetPermissionDetails.CBP_View,
                                    Create = dto.SubCabinetPermissionDetails.CBP_Create,
                                    Modify = dto.SubCabinetPermissionDetails.CBP_Modify,
                                    Delete = dto.SubCabinetPermissionDetails.CBP_Delete,
                                    Search = dto.SubCabinetPermissionDetails.CBP_Search,
                                    Index = dto.SubCabinetPermissionDetails.CBP_Index,
                                    Other = dto.SubCabinetPermissionDetails.CBP_Other,
                                    CreateFolder = dto.SubCabinetPermissionDetails.CBP_CreateFolder
                                },
                                transaction
                            );
                        }

                        if (dto.SubCabinetPermissionDetails.Entire_File_Plan == 1)
                        {
                            // TODO: Implement ExtendPermissions logic
                            // objCab.ExtendPermissions(objSrtPer, sCabSub, Session["AccessCode"], objSrtPer.cLvlType);
                        }
                    }
                }

                await connection.ExecuteAsync(
                    "UPDATE EDT_Cabinet SET CBN_SubCabCount = (SELECT COUNT(1) FROM EDT_Cabinet WHERE CBN_Parent = @CabId AND CBN_DelFlag = 'A') WHERE CBN_ID = @CabId AND CBN_CompID = @CompId;",
                    new { CabId = dto.CBN_Parent, CompId = dto.CBN_CompID },
                    transaction);

                await connection.ExecuteAsync(
                    "UPDATE EDT_Cabinet SET CBN_FolderCount = (SELECT COUNT(1) FROM EDT_Folder WHERE FOL_Cabinet IN (SELECT CBN_ID FROM EDT_Cabinet WHERE CBN_Parent = @CabId AND CBN_DelFlag = 'A')) WHERE CBN_ID = @CabId AND CBN_CompID = @CompId;",
                    new { CabId = dto.CBN_Parent, CompId = dto.CBN_CompID },
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

        //The implementation of the ExtendPermissions, GetFinalPermissions and LoadCabinetGrid functions is pending. These will be taken from the Cabinet module once Steffi's work is integrated, as the same logic will be used in both the Cabinet and Sub Cabinet modules
    }
}
