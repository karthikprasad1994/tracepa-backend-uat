using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using OpenAI;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.SuperMaster;
using TracePca.Interface.SuperMaster;
using static TracePca.Dto.SuperMaster.ExcelInformationDto;

namespace TracePca.Service.SuperMaster
{
    public class ExcelInformationService : ExcelInformationInterfaces
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExcelInformationService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //ValidateEmployeeMasters
        public async Task<object> ValidateExcelDataAsync(int CompId, List<SuperMasterValidateEmployeeDto> employees)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Setup Excel
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var duplicates = new List<SuperMasterValidateEmployeeDto>();
            var missingFields = new List<(string CustID, List<string> MissingFields)>();

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var emp in employees)
            {
                var missing = new List<string>();
                emp.Partner = string.IsNullOrWhiteSpace(emp.Partner) ? "No" : emp.Partner;

                if (string.IsNullOrWhiteSpace(emp.CustID)) missing.Add("CustID");
                if (string.IsNullOrWhiteSpace(emp.CustName)) missing.Add("CustName");
                if (string.IsNullOrWhiteSpace(emp.EmailID)) missing.Add("EmailID");
                if (string.IsNullOrWhiteSpace(emp.LoginName)) missing.Add("LoginName");
                if (string.IsNullOrWhiteSpace(emp.OfficePhoneNo)) missing.Add("OfficePhoneNo");
                if (string.IsNullOrWhiteSpace(emp.Designation)) missing.Add("Designation");
                if (string.IsNullOrWhiteSpace(emp.Partner)) missing.Add("Partner");

                if (missing.Any())
                {
                    missingFields.Add((emp.CustID ?? "UNKNOWN", missing));
                    continue;
                }

                // Duplicate check (CustName + EmailID)
                string key = $"{emp.CustName.Trim().ToLower()}|{emp.EmailID.Trim().ToLower()}";

                if (!seen.Add(key))
                {
                    duplicates.Add(new SuperMasterValidateEmployeeDto
                    {
                        CustID = emp.CustID,
                        CustName = emp.CustName,
                        EmailID = emp.EmailID
                    });
                }
            }
            return new
            {
                Duplicates = duplicates,
                MissingFields = missingFields.Select(x => new
                {
                    CustID = x.CustID,
                    MissingFields = x.MissingFields
                }).ToList()
            };
        }


        //SaveEmployeeMaster
        public async Task<List<int[]>> SuperMasterSaveEmployeeDetailsAsync(int CompId, List<SuperMasterSaveEmployeeMasterDto> employees)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var results = new List<int[]>();

                foreach (var objEmp in employees)
                {
                    // ✅ Check Designation ID validity
                    string checkQuery = @"
                SELECT COUNT(1)
                FROM SAD_GRPDESGN_General_Master
                WHERE Mas_ID = @DesignationID AND Mas_CompID = @CompId";

                    int exists = await connection.ExecuteScalarAsync<int>(
                        checkQuery,
                        new { DesignationID = objEmp.iUsrDesignation, CompId = CompId },
                        transaction: transaction
                    );

                    if (exists == 0)
                        throw new Exception($"Invalid designation ID for employee: {objEmp.sUsrFullName}");

                    int updateOrSave, oper;

                    using var command = new SqlCommand("spEmployeeMaster", connection, transaction);
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters (same as your existing code)
                    command.Parameters.AddWithValue("@Usr_ID", objEmp.iUserID);
                    command.Parameters.AddWithValue("@Usr_Node", objEmp.iUsrNode);
                    command.Parameters.AddWithValue("@Usr_Code", objEmp.sUsrCode ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_FullName", objEmp.sUsrFullName ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_LoginName", objEmp.sUsrLoginName ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Password", objEmp.sUsrPassword ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Email", objEmp.sUsrEmail ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Category", objEmp.iUsrSentMail);
                    command.Parameters.AddWithValue("@Usr_Suggetions", objEmp.iUsrSuggetions);
                    command.Parameters.AddWithValue("@usr_partner", objEmp.iUsrPartner);
                    command.Parameters.AddWithValue("@Usr_LevelGrp", objEmp.iUsrLevelGrp);
                    command.Parameters.AddWithValue("@Usr_DutyStatus", objEmp.sUsrDutyStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_PhoneNo", objEmp.sUsrPhoneNo ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_MobileNo", objEmp.sUsrMobileNo ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_OfficePhone", objEmp.sUsrOfficePhone ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_OffPhExtn", objEmp.sUsrOffPhExtn ?? string.Empty);

                    // ✅ Designation as ID
                    command.Parameters.AddWithValue("@Usr_Designation", objEmp.iUsrDesignation);

                    command.Parameters.AddWithValue("@Usr_CompanyID", objEmp.iUsrCompanyID);
                    command.Parameters.AddWithValue("@Usr_OrgnID", objEmp.iUsrOrgID);
                    command.Parameters.AddWithValue("@Usr_GrpOrUserLvlPerm", objEmp.iUsrGrpOrUserLvlPerm);
                    command.Parameters.AddWithValue("@Usr_Role", objEmp.iUsrRole);
                    command.Parameters.AddWithValue("@Usr_MasterModule", objEmp.iUsrMasterModule);
                    command.Parameters.AddWithValue("@Usr_AuditModule", objEmp.iUsrAuditModule);
                    command.Parameters.AddWithValue("@Usr_RiskModule", objEmp.iUsrRiskModule);
                    command.Parameters.AddWithValue("@Usr_ComplianceModule", objEmp.iUsrComplianceModule);
                    command.Parameters.AddWithValue("@Usr_BCMModule", objEmp.iUsrBCMmodule);
                    command.Parameters.AddWithValue("@Usr_DigitalOfficeModule", objEmp.iUsrDigitalOfficeModule);
                    command.Parameters.AddWithValue("@Usr_MasterRole", objEmp.iUsrMasterRole);
                    command.Parameters.AddWithValue("@Usr_AuditRole", objEmp.iUsrAuditRole);
                    command.Parameters.AddWithValue("@Usr_RiskRole", objEmp.iUsrRiskRole);
                    command.Parameters.AddWithValue("@Usr_ComplianceRole", objEmp.iUsrComplianceRole);
                    command.Parameters.AddWithValue("@Usr_BCMRole", objEmp.iUsrBCMRole);
                    command.Parameters.AddWithValue("@Usr_DigitalOfficeRole", objEmp.iUsrDigitalOfficeRole);
                    command.Parameters.AddWithValue("@Usr_CreatedBy", objEmp.iUsrCreatedBy);
                    command.Parameters.AddWithValue("@Usr_UpdatedBy", objEmp.iUsrCreatedBy);
                    command.Parameters.AddWithValue("@Usr_DelFlag", objEmp.sUsrFlag ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Status", objEmp.sUsrStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_IPAddress", objEmp.Usr_IPAdress ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_CompId", objEmp.iUsrCompID);
                    command.Parameters.AddWithValue("@Usr_Type", objEmp.sUsrType ?? string.Empty);
                    command.Parameters.AddWithValue("@usr_IsSuperuser", objEmp.iusr_IsSuperuser);
                    command.Parameters.AddWithValue("@USR_DeptID", objEmp.iUSR_DeptID);
                    command.Parameters.AddWithValue("@USR_MemberType", objEmp.iUSR_MemberType);
                    command.Parameters.AddWithValue("@USR_Levelcode", objEmp.iUSR_Levelcode);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    command.Parameters.Add(updateOrSaveParam);
                    command.Parameters.Add(operParam);

                    await command.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0);

                    results.Add(new[] { updateOrSave, oper });
                }

                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //ValidateClientDetails
        public async Task<object> ValidateClientDetailsAsync(int CompId, List<SuperMasterValidateClientDetailsDto> employees)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Setup Excel
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var duplicates = new List<SuperMasterValidateClientDetailsDto>();
            var missingFields = new List<(string CustID, List<string> MissingFields)>();

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var emp in employees)
            {
                var missing = new List<string>();

                if (string.IsNullOrWhiteSpace(emp.CustID)) missing.Add("CustID");
                if (string.IsNullOrWhiteSpace(emp.CustName)) missing.Add("CustName");
                if (string.IsNullOrWhiteSpace(emp.OrganisationType)) missing.Add("OrganisationType");
                if (string.IsNullOrWhiteSpace(emp.Address)) missing.Add("Address");
                if (string.IsNullOrWhiteSpace(emp.City)) missing.Add("City");
                if (string.IsNullOrWhiteSpace(emp.EmailID)) missing.Add("Email");
                if (string.IsNullOrWhiteSpace(emp.MobileNo)) missing.Add("MobileNo");
                if (string.IsNullOrWhiteSpace(emp.IndustryType)) missing.Add("LocationName");
                if (string.IsNullOrWhiteSpace(emp.LocationName)) missing.Add("ContactPerson");
                if (string.IsNullOrWhiteSpace(emp.ContactPerson)) missing.Add("ContactPerson");

                if (missing.Any())
                {
                    missingFields.Add((emp.CustID ?? "UNKNOWN", missing));
                    continue;
                }

                // Duplicate check (CustName + EmailID)
                string key = $"{emp.CustName.Trim().ToLower()}|{emp.EmailID.Trim().ToLower()}";

                if (!seen.Add(key))
                {
                    duplicates.Add(new SuperMasterValidateClientDetailsDto
                    {
                        CustID = emp.CustID,
                        CustName = emp.CustName,
                        EmailID = emp.EmailID
                    });
                }
            }
            return new
            {
                Duplicates = duplicates,
                MissingFields = missingFields.Select(x => new
                {
                    CustID = x.CustID,
                    MissingFields = x.MissingFields
                }).ToList()
            };
        }

        //SaveClientDetails
        public async Task<List<int[]>> SuperMasterSaveCustomerDetailsAsync(int CompId, List<SuperMasterSaveCustomerDto> customers)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var results = new List<int[]>();

                foreach (var objCust in customers)
                {

                    //Checks OrgType Fisrt
                    int orgTypeId = await connection.ExecuteScalarAsync<int?>(
                        @"SELECT TOP 1 Cmm_ID 
                  FROM Content_Management_Master 
                  WHERE cmm_Category = 'ORG' 
                    AND Cmm_CompID = @CompId 
                    AND cmm_Desc = @OrgType",
                        new { CompId, OrgType = objCust.OrgTypeName }, transaction
                    ) ?? 0;

                    if (orgTypeId == 0)
                    {
                        // Get next cmm_ID before inserting
                        int nextCmmId = await connection.ExecuteScalarAsync<int>(
                            "SELECT ISNULL(MAX(cmm_ID) + 1, 1) FROM Content_Management_Master",
                            transaction: transaction
                        );
                        //Doesn't exist → Insert new OrgType
                        orgTypeId = await connection.ExecuteScalarAsync<int>(
    @"INSERT INTO Content_Management_Master 
      (cmm_ID, cmm_Code, cmm_Category, Cmm_CompID, cmm_Desc, cmm_DelFlag)
      VALUES (@Cmm_ID, @cmm_Code, 'ORG', @CompId, @OrgType, 'A');
      SELECT @Cmm_ID;",
    new { Cmm_ID = nextCmmId, cmm_Code = objCust.Cmm_Code, CompId, OrgType = objCust.OrgTypeName },
    transaction
                        );
                    }
                    else
                    {
                        //Exists but ensure it's active
                        await connection.ExecuteAsync(
                            @"UPDATE Content_Management_Master
                      SET cmm_DelFlag = 'A'
                      WHERE Cmm_ID = @OrgTypeId",
                            new { OrgTypeId = orgTypeId }, transaction
                        );
                    }
                    objCust.CUST_ORGTYPEID = orgTypeId;

                    //Ensure Customer Code exists or activate it
                    var custRecord = await connection.QueryFirstOrDefaultAsync<(int Id, string DelFlag)>(
                        @"SELECT Cust_ID AS Id, CUST_DELFLG AS DelFlag
      FROM SAD_CUSTOMER_MASTER
      WHERE CUST_CODE = @CustCode",
                        new { CustCode = objCust.CUST_CODE }, transaction
                    );

                    if (custRecord.Id == 0) // Not found
                    {
                        objCust.CUST_CODE = await connection.ExecuteScalarAsync<string>(
                            @"SELECT 'CUST' + CAST(COALESCE(MAX(Cust_ID), 0) + 1 AS VARCHAR)
          FROM SAD_CUSTOMER_MASTER",
                            transaction: transaction
                        );
                    }
                    else
                    {
                        if (!string.Equals(custRecord.DelFlag, "A", StringComparison.OrdinalIgnoreCase))
                        {
                            await connection.ExecuteAsync(
                                @"UPDATE SAD_CUSTOMER_MASTER
              SET CUST_DELFLG = 'A'
              WHERE Cust_ID = @Id",
                                new { custRecord.Id }, transaction
                            );
                        }
                    }

                    //Save Customer Master Details via stored procedure
                    int updateOrSave, customerId;
                    using (var cmdCust = new SqlCommand("spSAD_CUSTOMER_MASTER", connection, transaction))
                    {
                        cmdCust.CommandType = CommandType.StoredProcedure;
                        cmdCust.Parameters.AddWithValue("@CUST_ID", objCust.CUST_ID);
                        cmdCust.Parameters.AddWithValue("@CUST_NAME", objCust.CUST_NAME ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_CODE", objCust.CUST_CODE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_WEBSITE", objCust.CUST_WEBSITE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_EMAIL", objCust.CUST_EMAIL ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_GROUPNAME", objCust.CUST_GROUPNAME ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_GROUPINDIVIDUAL", objCust.CUST_GROUPINDIVIDUAL);
                        cmdCust.Parameters.AddWithValue("@CUST_ORGTYPEID", objCust.CUST_ORGTYPEID);
                        cmdCust.Parameters.AddWithValue("@CUST_INDTYPEID", objCust.CUST_INDTYPEID);
                        cmdCust.Parameters.AddWithValue("@CUST_MGMTTYPEID", objCust.CUST_MGMTTYPEID);
                        cmdCust.Parameters.AddWithValue("@CUST_CommitmentDate", objCust.CUST_CommitmentDate);
                        cmdCust.Parameters.AddWithValue("@CUSt_BranchId", objCust.CUSt_BranchId ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_ADDRESS", objCust.CUST_COMM_ADDRESS ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_CITY", objCust.CUST_COMM_CITY ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_PIN", objCust.CUST_COMM_PIN ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_STATE", objCust.CUST_COMM_STATE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_COUNTRY", objCust.CUST_COMM_COUNTRY ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_FAX", objCust.CUST_COMM_FAX ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_TEL", objCust.CUST_COMM_TEL ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_Email", objCust.CUST_COMM_Email ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_ADDRESS", objCust.CUST_ADDRESS ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_CITY", objCust.CUST_CITY ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_PIN", objCust.CUST_PIN ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_STATE", objCust.CUST_STATE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COUNTRY", objCust.CUST_COUNTRY ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_FAX", objCust.CUST_FAX ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_TELPHONE", objCust.CUST_TELPHONE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_ConEmailID", objCust.CUST_ConEmailID ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_LOCATIONID", objCust.CUST_LOCATIONID ?? "0");
                        cmdCust.Parameters.AddWithValue("@CUST_TASKS", objCust.CUST_TASKS ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_ORGID", objCust.CUST_ORGID);
                        cmdCust.Parameters.AddWithValue("@CUST_CRBY", objCust.CUST_CRBY);
                        cmdCust.Parameters.AddWithValue("@CUST_UpdatedBy", objCust.CUST_UpdatedBy);
                        cmdCust.Parameters.AddWithValue("@CUST_BOARDOFDIRECTORS", objCust.CUST_BOARDOFDIRECTORS ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_DEPMETHOD", objCust.CUST_DEPMETHOD);
                        cmdCust.Parameters.AddWithValue("@CUST_IPAddress", objCust.CUST_IPAddress ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_CompID", objCust.CUST_CompID);
                        cmdCust.Parameters.AddWithValue("@CUST_Amount_Type", objCust.CUST_Amount_Type);
                        cmdCust.Parameters.AddWithValue("@CUST_RoundOff", objCust.CUST_RoundOff);
                        cmdCust.Parameters.AddWithValue("@Cust_DurtnId", objCust.Cust_DurtnId);
                        cmdCust.Parameters.AddWithValue("@Cust_FY", objCust.Cust_FY);


                        var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmdCust.Parameters.Add(updateOrSaveParam);
                        cmdCust.Parameters.Add(operParam);

                        await cmdCust.ExecuteNonQueryAsync();

                        updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                        customerId = (int)(operParam.Value ?? 0);
                    }

                    //Step 6: Save Locations
                    if (!string.IsNullOrWhiteSpace(objCust.LocationName))
                    {
                        int locationId;
                        using (var cmdLoc = new SqlCommand("spSAD_CUST_LOCATION", connection, transaction))
                        {
                            cmdLoc.CommandType = CommandType.StoredProcedure;
                            cmdLoc.Parameters.AddWithValue("@Mas_Id", objCust.Mas_Id);
                            cmdLoc.Parameters.AddWithValue("@Mas_code", objCust.Mas_code ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Description", objCust.LocationName ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_DelFlag", objCust.DelFlag ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CustID", customerId);
                            cmdLoc.Parameters.AddWithValue("@Mas_Loc_Address", objCust.Address ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_Person", objCust.ContactPerson ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_MobileNo", objCust.Mobile ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_LandLineNo", objCust.Landline ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_Email", objCust.Email ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@mas_Designation", objCust.Designation ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CRBY", objCust.CUST_CRBY);
                            cmdLoc.Parameters.AddWithValue("@Mas_UpdatedBy", objCust.CUST_UpdatedBy);
                            cmdLoc.Parameters.AddWithValue("@Mas_STATUS", "A");
                            cmdLoc.Parameters.AddWithValue("@Mas_IPAddress", objCust.CUST_IPAddress ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CompID", objCust.CUST_CompID);

                            var updateOrSaveLoc = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operLoc = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmdLoc.Parameters.Add(updateOrSaveLoc);
                            cmdLoc.Parameters.Add(operLoc);

                            await cmdLoc.ExecuteNonQueryAsync();
                            locationId = (int)(operLoc.Value ?? 0);
                        }

                        //Save Statutory Refs for this location
                        async Task SaveStatutoryRef(string desc, string value)
                        {
                            if (string.IsNullOrWhiteSpace(value))
                                return;

                            using var cmdStat = new SqlCommand("spSAD_CUST_Accounting_Template", connection, transaction);
                            cmdStat.CommandType = CommandType.StoredProcedure;
                            cmdStat.Parameters.AddWithValue("@Cust_PKID", 0);
                            cmdStat.Parameters.AddWithValue("@Cust_ID", customerId);
                            cmdStat.Parameters.AddWithValue("@Cust_Desc", desc);
                            cmdStat.Parameters.AddWithValue("@Cust_Value", value);
                            cmdStat.Parameters.AddWithValue("@Cust_Delflag", "A");
                            cmdStat.Parameters.AddWithValue("@Cust_Status", "A");
                            cmdStat.Parameters.AddWithValue("@Cust_AttchID", 0);
                            cmdStat.Parameters.AddWithValue("@Cust_CrBy", objCust.CUST_CRBY);
                            cmdStat.Parameters.AddWithValue("@Cust_UpdatedBy", objCust.CUST_UpdatedBy);
                            cmdStat.Parameters.AddWithValue("@Cust_IPAddress", objCust.CUST_IPAddress ?? string.Empty);
                            cmdStat.Parameters.AddWithValue("@Cust_Compid", objCust.CUST_CompID);
                            cmdStat.Parameters.AddWithValue("@Cust_LocationId", locationId);

                            var updateOrSaveStat = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operStat = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmdStat.Parameters.Add(updateOrSaveStat);
                            cmdStat.Parameters.Add(operStat);

                            await cmdStat.ExecuteNonQueryAsync();
                        }

                        await SaveStatutoryRef("CIN", objCust.CIN);
                        await SaveStatutoryRef("TAN", objCust.TAN);
                        await SaveStatutoryRef("GST", objCust.GST);
                    }

                    results.Add(new[] { updateOrSave, customerId });
                }

                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // SaveClientUser 
        public async Task<List<int[]>> SuperMasterSaveClientUserAsync(int CompId, List<SaveClientUserDto> employees)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var results = new List<int[]>();

                foreach (var objEmp in employees)
                {
                    // ✅ 1. Vendor check or create
                    const string checkVendorQuery = @"
                SELECT CUST_ID
                FROM SAD_CUSTOMER_MASTER
                WHERE CUST_EMAIL = @EmailId
                  AND CUST_CompID = @CompanyId";

                    var vendorId = await connection.ExecuteScalarAsync<int>(
                        checkVendorQuery,
                        new { EmailId = objEmp.EmailId, CompanyId = CompId },
                        transaction: transaction
                    );

                    if (vendorId == 0)
                    {
                        const string insertVendorQuery = @"
                    INSERT INTO SAD_CUSTOMER_MASTER (CUST_NAME, CUST_EMAIL, CUST_CompID)
                    OUTPUT INSERTED.CUST_ID
                    VALUES (@VendorName, @EmailId, @CompanyId)";

                        vendorId = await connection.ExecuteScalarAsync<int>(
                            insertVendorQuery,
                            new
                            {
                                VendorName = objEmp.VendorName ?? objEmp.EmailId,
                                EmailId = objEmp.EmailId,
                                CompanyId = CompId
                            },
                            transaction: transaction
                        );
                    }
                    objEmp.iUsrCompanyID = vendorId;

                    // ✅ 2. Designation check or create
                    const string checkDesignationQuery = @"
                SELECT Mas_ID
                FROM SAD_GRPDESGN_General_Master
                WHERE Mas_ID = @DesignationID
                  AND Mas_CompID = @CompId";

                    var designationId = await connection.ExecuteScalarAsync<int>(
                        checkDesignationQuery,
                        new { DesignationID = objEmp.iUsrDesignation, CompId = CompId },
                        transaction: transaction
                    );

                    if (designationId == 0)
                    {
                        const string insertDesignationQuery = @"
                    INSERT INTO SAD_GRPDESGN_General_Master (Mas_Description, Mas_CompID)
                    OUTPUT INSERTED.Mas_ID
                    VALUES (@DesignationName, @CompId)";

                        designationId = await connection.ExecuteScalarAsync<int>(
                            insertDesignationQuery,
                            new
                            {
                                DesignationName = objEmp.DesignationName ?? "New Designation",
                                CompId = CompId
                            },
                            transaction: transaction
                        );
                    }
                    objEmp.iUsrDesignation = designationId;

                    // ✅ 3. Save/Update Employee using stored procedure
                    int updateOrSave, oper;
                    using var command = new SqlCommand("spEmployeeMaster", connection, transaction);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Usr_ID", objEmp.iUserID);
                    command.Parameters.AddWithValue("@Usr_Node", objEmp.iUsrNode);
                    command.Parameters.AddWithValue("@Usr_Code", objEmp.sUsrCode ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_FullName", objEmp.sUsrFullName ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_LoginName", objEmp.sUsrLoginName ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Password", objEmp.sUsrPassword ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Email", objEmp.sUsrEmail ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Category", objEmp.iUsrSentMail);
                    command.Parameters.AddWithValue("@Usr_Suggetions", objEmp.iUsrSuggetions);
                    command.Parameters.AddWithValue("@usr_partner", objEmp.iUsrPartner);
                    command.Parameters.AddWithValue("@Usr_LevelGrp", objEmp.iUsrLevelGrp);
                    command.Parameters.AddWithValue("@Usr_DutyStatus", objEmp.sUsrDutyStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_PhoneNo", objEmp.sUsrPhoneNo ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_MobileNo", objEmp.sUsrMobileNo ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_OfficePhone", objEmp.sUsrOfficePhone ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_OffPhExtn", objEmp.sUsrOffPhExtn ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Designation", objEmp.iUsrDesignation);
                    command.Parameters.AddWithValue("@Usr_CompanyID", objEmp.iUsrCompanyID);
                    command.Parameters.AddWithValue("@Usr_OrgnID", objEmp.iUsrOrgID);
                    command.Parameters.AddWithValue("@Usr_GrpOrUserLvlPerm", objEmp.iUsrGrpOrUserLvlPerm);
                    command.Parameters.AddWithValue("@Usr_Role", objEmp.iUsrDesignation);
                    command.Parameters.AddWithValue("@Usr_MasterModule", objEmp.iUsrMasterModule);
                    command.Parameters.AddWithValue("@Usr_AuditModule", objEmp.iUsrAuditModule);
                    command.Parameters.AddWithValue("@Usr_RiskModule", objEmp.iUsrRiskModule);
                    command.Parameters.AddWithValue("@Usr_ComplianceModule", objEmp.iUsrComplianceModule);
                    command.Parameters.AddWithValue("@Usr_BCMModule", objEmp.iUsrBCMmodule);
                    command.Parameters.AddWithValue("@Usr_DigitalOfficeModule", objEmp.iUsrDigitalOfficeModule);
                    command.Parameters.AddWithValue("@Usr_MasterRole", objEmp.iUsrMasterRole);
                    command.Parameters.AddWithValue("@Usr_AuditRole", objEmp.iUsrAuditRole);
                    command.Parameters.AddWithValue("@Usr_RiskRole", objEmp.iUsrRiskRole);
                    command.Parameters.AddWithValue("@Usr_ComplianceRole", objEmp.iUsrComplianceRole);
                    command.Parameters.AddWithValue("@Usr_BCMRole", objEmp.iUsrBCMRole);
                    command.Parameters.AddWithValue("@Usr_DigitalOfficeRole", objEmp.iUsrDigitalOfficeRole);
                    command.Parameters.AddWithValue("@Usr_CreatedBy", objEmp.iUsrCreatedBy);
                    command.Parameters.AddWithValue("@Usr_UpdatedBy", objEmp.iUsrCreatedBy);
                    command.Parameters.AddWithValue("@Usr_DelFlag", objEmp.sUsrFlag ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Status", objEmp.sUsrStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_IPAddress", objEmp.Usr_IPAdress ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_CompId", objEmp.iUsrCompID);
                    command.Parameters.AddWithValue("@Usr_Type", objEmp.sUsrType ?? string.Empty);
                    command.Parameters.AddWithValue("@usr_IsSuperuser", objEmp.iusr_IsSuperuser);
                    command.Parameters.AddWithValue("@USR_DeptID", objEmp.iUSR_DeptID);
                    command.Parameters.AddWithValue("@USR_MemberType", objEmp.iUSR_MemberType);
                    command.Parameters.AddWithValue("@USR_Levelcode", objEmp.iUSR_Levelcode);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(updateOrSaveParam);
                    command.Parameters.Add(operParam);

                    await command.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0);

                    results.Add(new[] { updateOrSave, oper });
                }

                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

