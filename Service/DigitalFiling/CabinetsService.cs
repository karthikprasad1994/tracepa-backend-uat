using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFiling;
using TracePca.Interface.DigitalFiling;

namespace TracePca.Service.DigitalFiling
{
    public class CabinetsService : CabinetsInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public CabinetsService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<DigitalFilingDropDownListDataDTO> LoadDepartmentDDLAsync()
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = @"SELECT Org_node AS ID, Org_Name AS Name FROM Sad_Org_Structure WHERE Org_DelFlag = 'A' AND Org_LevelCode = 3 ORDER BY Org_Name";

                var departmentList = await connection.QueryAsync<DFDropDownListData>(query);

                return new DigitalFilingDropDownListDataDTO
                {
                    DepartmentList = departmentList?.ToList() ?? new List<DFDropDownListData>()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading department dropdown data", ex);
            }
        }

        public async Task<DigitalFilingDropDownListDataDTO> LoadCabinetUserPermissionDDLAsync(int deptID)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = @"SELECT Usr_ID AS ID, Usr_LoginName AS Name FROM Sad_UserDetails LEFT JOIN Sad_Org_Structure ON Org_node = Usr_DeptId WHERE Usr_DeptId = @DeptId AND USR_DutyStatus = 'A'";

                var userList = await connection.QueryAsync<DFDropDownListData>(query, new { DeptId = deptID });

                return new DigitalFilingDropDownListDataDTO
                {
                    CabinetUserPermissionList = userList?.ToList() ?? new List<DFDropDownListData>()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading user permission dropdown data", ex);
            }
        }

        public async Task<List<DFDropDownListData>> LoadAllUserCabinetDLLAsync(int compId, int userId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var deptQuery = @"SELECT DISTINCT Org_Node FROM Sad_Org_Structure LEFT JOIN Sad_UsersInOtherDept ON SUO_DeptID = Org_Node WHERE Org_DelFlag = 'A' AND Org_CompID = @CompID AND Org_LevelCode = 3";
                var departments = await connection.QueryAsync<int>(deptQuery, new { CompID = compId });
                var deptList = departments.ToList();
                string deptCsv = deptList.Any() ? string.Join(",", deptList) : "";

                var userInfoQuery = "SELECT USR_IsSuperUser, USR_MemberType FROM Sad_userdetails WHERE usr_id = @UserID";
                var userInfo = await connection.QueryFirstOrDefaultAsync(userInfoQuery, new { UserID = userId });
                int isSuperUser = userInfo?.USR_IsSuperUser ?? 0;
                int memberType = userInfo?.USR_MemberType ?? 0;

                string cabinetQuery;
                if (isSuperUser == 1 || memberType == 1)
                {
                    cabinetQuery = @"SELECT CBN_ID AS ID, CBN_NAME AS Name FROM edt_cabinet a INNER JOIN Sad_Org_Structure b ON a.CBN_Department = b.Org_node WHERE CBN_Parent = -1 AND a.CBN_DelFlag = 'A'";
                    if (!string.IsNullOrWhiteSpace(deptCsv))
                    {
                        cabinetQuery += $" AND b.Org_Node IN ({deptCsv})";
                    }
                    cabinetQuery += " ORDER BY CBN_NAME";
                }
                else
                {
                    cabinetQuery = @"SELECT DISTINCT CBN_ID AS ID, CBN_NAME AS Name FROM view_cabpermissions WHERE CBN_Parent = -1 AND CBN_DelFlag = 'A'";
                    if (!string.IsNullOrWhiteSpace(deptCsv))
                    {
                        cabinetQuery += $" AND (CBN_Department IN ({deptCsv}) OR CBP_Department IN ({deptCsv}))";
                    }
                    cabinetQuery += " ORDER BY CBN_NAME";
                }

                var cabinets = await connection.QueryAsync<DFDropDownListData>(cabinetQuery);
                return cabinets.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading user cabinet dropdown data", ex);
            }
        }

        public async Task<List<CabinetDto>> GetAllSubCabAsync(string status, int cabId, int userId)
        {
            try
            {
                var result = new List<CabinetDto>();

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var userInfoQuery = "SELECT ISNULL(usr_IsSuperuser, 0) AS USR_IsSuperUser, ISNULL(usr_deptid, 0) AS USR_Department FROM sad_userdetails WHERE usr_id = @UserId";
                var userInfo = await connection.QueryFirstOrDefaultAsync(userInfoQuery, new { UserId = userId });
                int userType = userInfo?.USR_IsSuperUser ?? 0;
                int userParGrp = userInfo?.USR_Department ?? 0;

                string query;
                if (userType != 1)
                {
                    if (status == "De-Activated")
                        query = "SELECT * FROM edt_cabinet WHERE CBN_Department = @UserParGrp AND Cbn_DelFlag = 'D' AND cbn_parent = @CabId";
                    else if (status == "Waiting for Approval")
                        query = "SELECT * FROM edt_cabinet WHERE CBN_Department = @UserParGrp AND Cbn_DelFlag = 'W' AND cbn_parent = @CabId";
                    else
                        query = "SELECT * FROM edt_cabinet WHERE CBN_Department = @UserParGrp AND Cbn_DelFlag != 'V' AND cbn_parent = @CabId";
                }
                else
                {
                    if (status == "De-Activated")
                        query = "SELECT * FROM edt_cabinet WHERE Cbn_DelFlag = 'D' AND cbn_parent = @CabId";
                    else if (status == "Waiting for Approval")
                        query = "SELECT * FROM edt_cabinet WHERE Cbn_DelFlag = 'W' AND cbn_parent = @CabId";
                    else
                        query = "SELECT * FROM edt_cabinet WHERE Cbn_DelFlag != 'V' AND cbn_parent = @CabId";
                }

                var cabinets = await connection.QueryAsync<dynamic>(query, new { UserParGrp = userParGrp, CabId = cabId });

                foreach (var dr in cabinets)
                {
                    string delFlagDesc = dr.Cbn_DelFlag switch
                    {
                        "A" => "Activated",
                        "D" => "De-Activated",
                        "W" => "Waiting for Approval",
                        _ => dr.Cbn_DelFlag
                    };

                    var groupNameQuery = "SELECT Org_Name FROM Sad_Org_Structure WHERE Org_Node = @GrpId";
                    string? groupName = await connection.ExecuteScalarAsync<string?>(groupNameQuery, new { GrpId = (int)dr.CBN_Department });

                    var createdByQuery = "SELECT usr_fullname FROM sad_userdetails WHERE usr_id = @CreatedById";
                    string? createdByName = await connection.ExecuteScalarAsync<string?>(createdByQuery, new { CreatedById = (int)dr.CBN_CreatedBy });

                    var cabinet = new CabinetDto
                    {
                        CBN_ID = dr.CBN_ID,
                        CBN_Name = dr.CBN_Name,
                        CBN_Parent = dr.CBN_Parent,
                        CBN_Note = dr.CBN_Note,
                        CBN_UserID = dr.CBN_UserID,
                        CBN_Department = dr.CBN_Department,
                        CBN_SubCabCount = dr.CBN_SubCabCount,
                        CBN_FolderCount = dr.CBN_FolderCount,
                        CBN_CreatedBy = dr.CBN_CreatedBy,
                        CBN_CreatedOn = dr.CBN_CreatedOn,
                        CBN_UpdatedBy = dr.CBN_UpdatedBy,
                        CBN_UpdatedOn = dr.CBN_UpdatedOn,
                        CBN_ApprovedBy = dr.CBN_ApprovedBy,
                        CBN_ApprovedOn = dr.CBN_ApprovedOn,
                        CBN_DeletedBy = dr.CBN_DeletedBy,
                        CBN_DeletedOn = dr.CBN_DeletedOn,
                        CBN_RecalledBy = dr.CBN_RecalledBy,
                        CBN_RecalledOn = dr.CBN_RecalledOn,
                        CBN_DelFlag = delFlagDesc,
                        CBN_CompID = dr.CBN_CompID,
                        CBN_Retention = dr.CBN_Retention
                    };
                    result.Add(cabinet);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all sub cabinet data", ex);
            }
        }

        public async Task<string> UpdateCabinetStatusAsync(UpdateCabinetStatusRequestDTO request)
        {
            int extraPermissions = await ExtraPermissionsToCabinetAsync(request.UserId, request.CabinetNode, "CBP_Delete");
            if (request.StatusCode == "D" && extraPermissions == 0)
            {
                return "SubCabinet Permission is not Assigned.";
            }

            await UpdateSubCabinetStatusAsync(request.CompId, request.StatusCode, request.CabinetNode, request.Flag, request.UserId);

            if (request.StatusCode == "W")
            {
                await UpdateSubCabinetCountDetailsAsync(request.CompId, 0, request.CabinetId);
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

        public async Task<int> ExtraPermissionsToCabinetAsync(int userId, int cabinetId, string permTypes)
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
                {
                    return 1;
                }

                var existCountQuery = @"SELECT COUNT(*) FROM edt_cabinet_Permission WHERE CBP_cabinet = @CabinetId";
                int existCount = await connection.ExecuteScalarAsync<int>(existCountQuery, new { CabinetId = cabinetId });
                if (existCount == 0)
                {
                    return 0;
                }

                var countOtherQuery = @"SELECT COUNT(*) FROM edt_cabinet_Permission WHERE CBP_cabinet = @CabinetId AND CBP_Department = @UserDeptId AND CBP_user = @UserId AND cbp_other = 1";
                int countOther = await connection.ExecuteScalarAsync<int>(countOtherQuery, new { CabinetId = cabinetId, UserDeptId = userDeptId, UserId = userId });
                if (countOther != 0)
                {
                    var userPermQuery = $@"SELECT {permTypes} AS ELevel, CBP_Other AS Permission FROM edt_cabinet_Permission WHERE CBP_user = @UserId AND CBP_PermissionType = 'U' AND CBP_cabinet = @CabinetId AND CBP_Department = @UserDeptId";
                    var userPerm = await connection.QueryFirstOrDefaultAsync<(int ELevel, int Permission)>(userPermQuery, new { UserId = userId, CabinetId = cabinetId, UserDeptId = userDeptId });
                    if (userPerm.Permission != 0)
                    {
                        return userPerm.ELevel;
                    }
                    return 0;
                }
                else
                {
                    var groupPermQuery = $@"SELECT {permTypes} FROM edt_cabinet_Permission WHERE CBP_user = 0 AND CBP_PermissionType = 'G' AND CBP_cabinet = @CabinetId AND CBP_Department = @UserDeptId";
                    int groupPerm = await connection.ExecuteScalarAsync<int>(groupPermQuery, new { CabinetId = cabinetId, UserDeptId = userDeptId });
                    return groupPerm;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateSubCabinetCountDetailsAsync(int compId, int departmentId, int cbnId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var subCabSql = @"UPDATE edt_cabinet SET CBN_SubCabCount = (SELECT COUNT(CBN_ID) FROM Edt_Cabinet WHERE CBN_Parent = @CBN_ID AND CBN_DelFlag = 'A') WHERE CBN_ID = @CBN_ID AND CBN_CompID = @CompanyID";
                var folderSql = @"UPDATE edt_cabinet SET CBN_FolderCount = (SELECT COUNT(Fol_folid) FROM edt_folder WHERE fol_cabinet IN (SELECT CBN_ID FROM Edt_Cabinet WHERE CBN_Parent = @CBN_ID AND CBN_DelFlag = 'A'
                                ))WHERE CBN_ID = @CBN_ID AND CBN_CompID = @CompId";

                var parameters = new
                {
                    CBN_ID = cbnId,
                    CompId = compId
                };

                await connection.ExecuteAsync(subCabSql, parameters);
                await connection.ExecuteAsync(folderSql, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateSubCabinetStatusAsync(int compId, string status, int subCabinetId, string delFlag, int userId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                string sql = "UPDATE edt_cabinet SET ";
                var parameters = new DynamicParameters();

                parameters.Add("@DelFlag", delFlag);
                parameters.Add("@UserId", userId);
                parameters.Add("@SubCabinetId", subCabinetId);
                parameters.Add("@CompId", compId);

                if (status == "D")
                {
                    sql += "CBN_DelFlag = @DelFlag, CBN_DeletedBy = @UserId, CBN_DeletedOn = GETDATE(), CBN_Status = 'AD'";
                }
                else if (status == "A")
                {
                    sql += "CBN_DelFlag = @DelFlag, CBN_RecalledBy = @UserId, CBN_RecalledOn = GETDATE(), CBN_Status = 'AR'";
                }
                else if (status == "W")
                {
                    sql += "CBN_DelFlag = @DelFlag, CBN_ApprovedBy = @UserId, CBN_ApprovedOn = GETDATE(), CBN_Status = 'A'";
                }
                else if (status == "AV")
                {
                    sql += "CBN_DelFlag = @DelFlag, CBN_UpdatedBy = @UserId, CBN_UpdatedOn = GETDATE(), CBN_Status = 'AV'";
                }

                sql += " WHERE CBN_ID = @SubCabinetId AND CBN_CompID = @CompId";

                await connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
