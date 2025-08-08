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
        public async Task<int[]> SuperMasterSaveCustomerDetailsAsync(int CompId, SuperMasterSaveClientDetailsDto objCust)
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
            using var transaction = connection.BeginTransaction();

            try
            {
                int updateOrSave, oper;

                using var command = new SqlCommand("spSAD_CUSTOMER_MASTER", connection, transaction);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@CUST_ID", objCust.CUST_ID);
                command.Parameters.AddWithValue("@CUST_NAME", objCust.CUST_NAME ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_CODE", objCust.CUST_CODE ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_WEBSITE", objCust.CUST_WEBSITE ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_EMAIL", objCust.CUST_EMAIL ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_GROUPNAME", objCust.CUST_GROUPNAME ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_GROUPINDIVIDUAL", objCust.CUST_GROUPINDIVIDUAL);
                command.Parameters.AddWithValue("@CUST_ORGTYPEID", objCust.CUST_ORGTYPEID);
                command.Parameters.AddWithValue("@CUST_INDTYPEID", objCust.CUST_INDTYPEID);
                command.Parameters.AddWithValue("@CUST_MGMTTYPEID", objCust.CUST_MGMTTYPEID);
                command.Parameters.AddWithValue("@CUST_CommitmentDate", objCust.CUST_CommitmentDate);
                command.Parameters.AddWithValue("@CUSt_BranchId", objCust.CUSt_BranchId ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COMM_ADDRESS", objCust.CUST_COMM_ADDRESS ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COMM_CITY", objCust.CUST_COMM_CITY ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COMM_PIN", objCust.CUST_COMM_PIN ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COMM_STATE", objCust.CUST_COMM_STATE ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COMM_COUNTRY", objCust.CUST_COMM_COUNTRY ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COMM_FAX", objCust.CUST_COMM_FAX ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COMM_TEL", objCust.CUST_COMM_TEL ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COMM_Email", objCust.CUST_COMM_Email ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_ADDRESS", objCust.CUST_ADDRESS ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_CITY", objCust.CUST_CITY ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_PIN", objCust.CUST_PIN ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_STATE", objCust.CUST_STATE ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_COUNTRY", objCust.CUST_COUNTRY ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_FAX", objCust.CUST_FAX ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_TELPHONE", objCust.CUST_TELPHONE ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_ConEmailID", objCust.CUST_ConEmailID ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_LOCATIONID", objCust.CUST_LOCATIONID ?? "0");
                command.Parameters.AddWithValue("@CUST_TASKS", objCust.CUST_TASKS ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_ORGID", objCust.CUST_ORGID);
                command.Parameters.AddWithValue("@CUST_CRBY", objCust.CUST_CRBY);
                command.Parameters.AddWithValue("@CUST_UpdatedBy", objCust.CUST_UpdatedBy);
                command.Parameters.AddWithValue("@CUST_BOARDOFDIRECTORS", objCust.CUST_BOARDOFDIRECTORS ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_DEPMETHOD", objCust.CUST_DEPMETHOD);
                command.Parameters.AddWithValue("@CUST_IPAddress", objCust.CUST_IPAddress ?? string.Empty);
                command.Parameters.AddWithValue("@CUST_CompID", objCust.CUST_CompID);
                command.Parameters.AddWithValue("@CUST_Amount_Type", objCust.CUST_Amount_Type);
                command.Parameters.AddWithValue("@CUST_RoundOff", objCust.CUST_RoundOff);
                command.Parameters.AddWithValue("@Cust_DurtnId", objCust.Cust_DurtnId);
                command.Parameters.AddWithValue("@Cust_FY", objCust.Cust_FY);

                var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                command.Parameters.Add(updateOrSaveParam);
                command.Parameters.Add(operParam);

                await command.ExecuteNonQueryAsync();

                updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                oper = (int)(operParam.Value ?? 0);

                transaction.Commit();

                return new[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //SaveClientUser
      
        public async Task<int[]> SuperMasterSaveClientUserAsync(int CompId, SaveClientUserDto objEmp)

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
                // ✅ Correct query using @DesignationID
                string checkQuery = @"
SELECT COUNT(1)
FROM SAD_GRPDESGN_General_Master
WHERE Mas_ID = @DesignationID AND Mas_CompID = @CompId";

                // ✅ Parameters must match the SQL query names exactly
                int exists = await connection.ExecuteScalarAsync<int>(
                    checkQuery,
                    new { DesignationID = objEmp.iUsrDesignation, CompId = CompId },
                    transaction: transaction
                );

                if (exists == 0)
                    throw new Exception("Invalid designation ID.");

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

                // ✅ Designation as resolved ID
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

                transaction.Commit();

                return new[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

