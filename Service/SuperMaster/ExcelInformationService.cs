using System.Data;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
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

        //ValidateClientDetails
        public async Task<IEnumerable<SuperMasterValidateClientDetailsResult>> SuperMasterValidateClientDetailsExcelAsync(SuperMasterValidateClientDetailsResult file)
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

            var results = new List<SuperMasterValidateClientDetailsResult>();
            var customerNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var stream = new MemoryStream();
            //await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet == null)
                throw new Exception("Excel file does not contain a valid worksheet.");

            var mandatoryFields = new[]
            {
            "Customer Name", "Organisation Type", "Address", "City", "E-Mail", "Mobile No",
            "Business Reltn. Start Date", "Industry Type", "Professional Services Offered 1",
            "Location Name 1", "Contact Person 1", "Address 1"
            };

            var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var header = worksheet.Cells[1, col].Text.Trim().Replace("*", "");
                if (!string.IsNullOrEmpty(header))
                    headers[header] = col;
            }

            var missingColumns = mandatoryFields.Where(field => !headers.ContainsKey(field)).ToList();
            if (missingColumns.Any())
            {
                throw new Exception("Missing mandatory columns: " + string.Join(", ", missingColumns));
            }

            int customerCounter = 1;

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var result = new SuperMasterValidateClientDetailsResult
                {
                    RowNumber = row,
                    MissingFields = new List<string>(),
                    Data = new Dictionary<string, string>()
                };

                foreach (var column in mandatoryFields)
                {
                    string value = worksheet.Cells[row, headers[column]].Text.Trim();
                    result.Data[column] = value;

                    if (string.IsNullOrWhiteSpace(value))
                        result.MissingFields.Add(column);

                    if (column == "Customer Name")
                        result.CustomerName = value;

                    if (column == "E-Mail")
                        result.Email = value;
                }

                // Check for duplicates
                string nameKey = result.CustomerName?.ToLowerInvariant() ?? "";
                string emailKey = result.Email?.ToLowerInvariant() ?? "";

                bool isDuplicate = false;
                if (!string.IsNullOrEmpty(nameKey) && !customerNames.Add(nameKey))
                    isDuplicate = true;
                if (!string.IsNullOrEmpty(emailKey) && !emails.Add(emailKey))
                    isDuplicate = true;

                result.IsDuplicate = isDuplicate;

                if (!result.IsDuplicate && !result.MissingFields.Any())
                {
                    result.GeneratedCustomerId = $"CUST{customerCounter:D3}";
                    customerCounter++;
                }
                results.Add(result);
            }
            return results;
        }

        //SaveEmployeeMaster
        public async Task<int[]> SuperMasterSaveEmployeeDetailsAsync(int CompId, SuperMasterSaveEmployeeMasterDto objEmp)
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
        public async Task<int[]> SaveClientUserAsync(int CompId, SuperMasterSaveClientUserDto objCust)
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

                using var command = new SqlCommand("spSAD_CUSTOMER_DETAILS", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@CDET_ID", objCust.CDET_ID);
                command.Parameters.AddWithValue("@CDET_CUSTID", objCust.CDET_CUSTID);
                command.Parameters.AddWithValue("@CDET_STANDINGININDUSTRY", objCust.CDET_STANDINGININDUSTRY ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_PUBLICPERCEPTION", objCust.CDET_PUBLICPERCEPTION ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_GOVTPERCEPTION", objCust.CDET_GOVTPERCEPTION ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_LITIGATIONISSUES", objCust.CDET_LITIGATIONISSUES ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_PRODUCTSMANUFACTURED", objCust.CDET_PRODUCTSMANUFACTURED ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_SERVICESOFFERED", objCust.CDET_SERVICESOFFERED ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_TURNOVER", objCust.CDET_TURNOVER ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_PROFITABILITY", objCust.CDET_PROFITABILITY ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_FOREIGNCOLLABORATIONS", objCust.CDET_FOREIGNCOLLABORATIONS ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_EMPLOYEESTRENGTH", objCust.CDET_EMPLOYEESTRENGTH ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_PROFESSIONALSERVICES", objCust.CDET_PROFESSIONALSERVICES ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_GATHEREDBYAUDITFIRM", objCust.CDET_GATHEREDBYAUDITFIRM ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_LEGALADVISORS", objCust.CDET_LEGALADVISORS ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_AUDITINCHARGE", objCust.CDET_AUDITINCHARGE ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_FileNo", objCust.CDET_FileNo ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_CRBY", objCust.CDET_CRBY);
                command.Parameters.AddWithValue("@CDET_UpdatedBy", objCust.CDET_UpdatedBy);
                command.Parameters.AddWithValue("@CDET_STATUS", objCust.CDET_STATUS ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_IPAddress", objCust.CDET_IPAddress ?? string.Empty);
                command.Parameters.AddWithValue("@CDET_CompID", objCust.CDET_CompID);

                // Output Parameters
                var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

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

