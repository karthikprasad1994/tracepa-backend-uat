using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Packaging.Ionic.Zip;
using OpenAI;
using System.Data;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Dto.SuperMaster;
using TracePca.Interface.SuperMaster;
using static TracePca.Dto.SuperMaster.ExcelInformationDto;
using System.Text.RegularExpressions;
using Microsoft.Playwright;


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

        //UploadEmployeeMasters
        public class EmployeeUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }

            public EmployeeUploadException(Dictionary<string, List<string>> errors)
                : base("Error processing employee master")
            {
                Errors = errors;
            }
        }
        public async Task<List<string>> UploadEmployeeDetailsAsync(int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            // ✅ Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Parse Excel
            List<UploadEmployeeMasterDto> employees;
            List<string> headerErrors = new List<string>();
            using (var stream = file.OpenReadStream())
            {
                employees = ParseExcelToEmployees(stream, out headerErrors);
            }

            // ✅ Step 3: Validate (only missing + invalids)
            var missingOrInvalidErrors = ValidateEmployees(employees);

            // ✅ Step 4: Collect duplicate errors separately
            var duplicateErrors = new List<UploadEmployeeMasterDto>();

            void AddDuplicateErrors(Func<UploadEmployeeMasterDto, string> keySelector, string fieldName)
            {
                duplicateErrors.AddRange(
                    employees.GroupBy(keySelector)
                        .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                        .Select(g => new UploadEmployeeMasterDto
                        {
                            EmpCode = g.First().EmpCode,
                            EmployeeName = g.First().EmployeeName,
                            ErrorMessage = $"Duplicate {fieldName} found: {g.Key}"
                        })
                );
            }

            AddDuplicateErrors(e => e.EmpCode, "EmpCode");
            AddDuplicateErrors(e => e.LoginName, "LoginName");
            AddDuplicateErrors(e => e.Email, "Email");
            AddDuplicateErrors(e => e.OfficePhoneNo, "OfficePhoneNo");

            // ✅ Step 5: Group errors into structured dictionary
            var finalErrors = new Dictionary<string, List<string>>();

            if (headerErrors.Any())
            {
                finalErrors["Missing column"] = headerErrors;
            }

            if (missingOrInvalidErrors.Any())
            {
                var groupedMissing = missingOrInvalidErrors
                    .GroupBy(e => new { e.EmpCode, e.EmployeeName })
                    .Select(g => $"{g.Key.EmployeeName} ({g.Key.EmpCode}): {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();

                finalErrors["Missing values"] = groupedMissing;
            }

            if (duplicateErrors.Any())
            {
                var groupedDuplicates = duplicateErrors
                    .GroupBy(e => new { e.EmpCode, e.EmployeeName })
                    .Select(g => $"{g.Key.EmployeeName} ({g.Key.EmpCode}): {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();

                finalErrors["Duplication"] = groupedDuplicates;
            }

            // ✅ Throw if any errors
            if (finalErrors.Any())
            {
                throw new EmployeeUploadException(finalErrors);
            }

            var results = new List<string>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var emp in employees)
                {

                    // Step 4: Validate mandatory fields
                    if (string.IsNullOrWhiteSpace(emp.EmpCode) ||
                        string.IsNullOrWhiteSpace(emp.EmployeeName) ||
                        string.IsNullOrWhiteSpace(emp.LoginName) ||
                        string.IsNullOrWhiteSpace(emp.Email) ||
                        string.IsNullOrWhiteSpace(emp.OfficePhoneNo) ||
                        string.IsNullOrWhiteSpace(emp.Designation) ||
                        emp.Partner == "yes" ||       // int check
                        string.IsNullOrWhiteSpace(emp.Role))
                    {
                        throw new Exception($"Mandatory fields missing for employee: {emp.EmployeeName}");
                    }



                    // Step 4: Ensure Designation exists

                    string designationSql = @"
            SELECT Mas_ID FROM SAD_GRPDESGN_General_Master
            WHERE UPPER(Mas_Description) = UPPER(@Name) AND Mas_CompID = @CompId";
                    int? designationId = await connection.ExecuteScalarAsync<int?>(
                        designationSql, new { Name = emp.Designation, CompId = compId }, transaction);

                    if (!designationId.HasValue)
                    {
                        string insertDesig = @"
                INSERT INTO SAD_GRPDESGN_General_Master (Mas_Description, Mas_CompID)
                VALUES (@Name, @CompId);
                SELECT CAST(SCOPE_IDENTITY() as int);";
                        designationId = await connection.ExecuteScalarAsync<int>(
                            insertDesig, new { Name = emp.Designation, CompId = compId }, transaction);
                    }

                    // Step 5: Ensure Role exists
                    string roleSql = @"
            SELECT Mas_ID FROM SAD_GrpOrLvl_General_Master
            WHERE UPPER(Mas_Description) = UPPER(@Name) AND Mas_CompID = @CompId";
                    int? roleId = await connection.ExecuteScalarAsync<int?>(
                        roleSql, new { Name = emp.Role, CompId = compId }, transaction);

                    if (!roleId.HasValue)
                    {
                        string insertRole = @"
                INSERT INTO SAD_GrpOrLvl_General_Master (Mas_Description, Mas_CompID)
                VALUES (@Name, @CompId);
                SELECT CAST(SCOPE_IDENTITY() as int);";
                        roleId = await connection.ExecuteScalarAsync<int>(
                            insertRole, new { Name = emp.Role, CompId = compId }, transaction);
                    }

                    // Step 6: Check if employee exists
                    string checkEmpSql = "SELECT COUNT(1) FROM Sad_UserDetails WHERE Usr_Code=@EmpCode AND Usr_CompId=@CompId";
                    bool exists = await connection.ExecuteScalarAsync<int>(
                        checkEmpSql, new { EmpCode = emp.EmpCode, CompId = compId }, transaction) > 0;

                    // Step 7: Insert/Update using SP
                    using var cmd = new SqlCommand("spEmployeeMaster", connection, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Usr_ID", (object?)emp.EmpId ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_Node", emp.EmpNode ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_Code", emp.EmpCode ?? "");
                    cmd.Parameters.AddWithValue("@Usr_FullName", emp.EmployeeName ?? "");
                    cmd.Parameters.AddWithValue("@Usr_LoginName", emp.LoginName ?? "");
                    cmd.Parameters.AddWithValue("@Usr_Password", emp.Password ?? "");
                    cmd.Parameters.AddWithValue("@Usr_Email", emp.Email ?? "");
                    cmd.Parameters.AddWithValue("@Usr_Category", emp.EmpCategory ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_Suggetions", emp.Suggestions ?? 0);
                    cmd.Parameters.AddWithValue("@usr_partner", emp.Partner);
                    cmd.Parameters.AddWithValue("@Usr_LevelGrp", emp.LevelGrp ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_DutyStatus", emp.DutyStatus ?? "A");
                    cmd.Parameters.AddWithValue("@Usr_PhoneNo", emp.PhoneNo ?? "");
                    cmd.Parameters.AddWithValue("@Usr_MobileNo", emp.MobileNo ?? "");
                    cmd.Parameters.AddWithValue("@Usr_OfficePhone", emp.OfficePhoneNo ?? "");
                    cmd.Parameters.AddWithValue("@Usr_OffPhExtn", emp.OfficePhoneExtn ?? "");
                    cmd.Parameters.AddWithValue("@Usr_Designation", designationId);
                    cmd.Parameters.AddWithValue("@Usr_CompanyID", emp.CompanyId ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_OrgnID", emp.OrgnId ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_GrpOrUserLvlPerm", emp.GrpOrUserLvlPerm ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_Role", roleId);

                    // ✅ Module flags
                    cmd.Parameters.AddWithValue("@Usr_MasterModule", emp.MasterModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_AuditModule", emp.AuditModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_RiskModule", emp.RiskModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_ComplianceModule", emp.ComplianceModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_BCMModule", emp.BCMModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_DigitalOfficeModule", emp.DigitalOfficeModule ?? 0);

                    // ✅ Role flags
                    cmd.Parameters.AddWithValue("@Usr_MasterRole", emp.MasterRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_AuditRole", emp.AuditRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_RiskRole", emp.RiskRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_ComplianceRole", emp.ComplianceRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_BCMRole", emp.BCMRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_DigitalOfficeRole", emp.DigitalOfficeRole ?? 0);

                    // ✅ Metadata
                    cmd.Parameters.AddWithValue("@Usr_CreatedBy", emp.CreatedBy ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_UpdatedBy", emp.UpdatedBy ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_DelFlag", emp.DelFlag ?? "A");
                    cmd.Parameters.AddWithValue("@Usr_Status", emp.Status ?? "N");
                    cmd.Parameters.AddWithValue("@Usr_IPAddress", emp.IPAddress ?? "");
                    cmd.Parameters.AddWithValue("@Usr_CompId", emp.CompId ?? compId);
                    cmd.Parameters.AddWithValue("@Usr_Type", emp.Type ?? "C");
                    cmd.Parameters.AddWithValue("@usr_IsSuperuser", emp.IsSuperuser ?? 0);
                    cmd.Parameters.AddWithValue("@USR_DeptID", emp.DeptID ?? 0);
                    cmd.Parameters.AddWithValue("@USR_MemberType", emp.MemberType ?? 0);
                    cmd.Parameters.AddWithValue("@USR_Levelcode", emp.Levelcode ?? 0);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(updateOrSaveParam);
                    cmd.Parameters.Add(operParam);

                    await cmd.ExecuteNonQueryAsync();

                    int updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    int oper = (int)(operParam.Value ?? 0);

                    string action = updateOrSave == 2 ? "Updated" : "Inserted";
                    results.Add($"{action} employee: {emp.EmpCode} - {emp.EmployeeName} (Usr_ID={oper})");
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

        //Parse Excel file into Employee DTO list
        private List<UploadEmployeeMasterDto> ParseExcelToEmployees(Stream fileStream, out List<string> headerErrors)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;

            // Expected headers (exactly from template)
            string[] expectedHeaders = new[]
            {
        "Emp Code","Employee Name","Login Name","E-Mail","Office Phone No","Designation","Partner","Role","Password",
        "LevelGrp","DutyStatus","PhoneNo","MobileNo","OffPhExtn","OrgID","GrpOrUserLvlPerm",
        "MasterModule","AuditModule","RiskModule","ComplianceModule","BCMModule","DigitalOfficeModule",
        "MasterRole","AuditRole","RiskRole","ComplianceRole","BCMRole","DigitalOfficeRole",
        "CreatedBy","UpdatedBy","DelFlag","Status","IPAddress","CompId","Type","IsSuperuser",
        "DeptID","MemberType","Levelcode","Suggestions"
    };

            headerErrors = new List<string>();

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                string actualHeader = worksheet.Cells[1, col].Text?.Trim();
                string expectedHeader = expectedHeaders[col - 1];

                if (!string.Equals(actualHeader, expectedHeader, StringComparison.OrdinalIgnoreCase))
                {
                    headerErrors.Add($"Expected header '{expectedHeader}' at column {col}, but found '{actualHeader}'");
                }
            }

            var employees = new List<UploadEmployeeMasterDto>();
            for (int row = 2; row <= rowCount; row++) // skip header
            {
                employees.Add(new UploadEmployeeMasterDto
                {
                    EmpCode = worksheet.Cells[row, 1].Text,   // Emp Code
                    EmployeeName = worksheet.Cells[row, 2].Text,   // Full Name
                    LoginName = worksheet.Cells[row, 3].Text,   // Login Name
                    Email = worksheet.Cells[row, 4].Text,   // Email
                    OfficePhoneNo = worksheet.Cells[row, 5].Text,   // Office Phone
                    Designation = worksheet.Cells[row, 6].Text,   // Designation
                    Partner = worksheet.Cells[row, 7].Text switch
                    {
                        "Yes" => "1",
                        "No" => "0",
                        _ => "0",
                    },
                    Role = worksheet.Cells[row, 8].Text,  // Role
                    Password = worksheet.Cells[row, 9].Text,

                    LevelGrp = int.TryParse(worksheet.Cells[row, 10].Text, out var levelGrp) ? levelGrp : 0,
                    DutyStatus = worksheet.Cells[row, 11].Text,
                    PhoneNo = worksheet.Cells[row, 12].Text,
                    MobileNo = worksheet.Cells[row, 13].Text,
                    OfficePhoneExtn = worksheet.Cells[row, 14].Text,
                    OrgnId = int.TryParse(worksheet.Cells[row, 15].Text, out var orgId) ? orgId : 0,
                    GrpOrUserLvlPerm = int.TryParse(worksheet.Cells[row, 16].Text, out var grpOfuser) ? grpOfuser : 0,
                    MasterModule = int.TryParse(worksheet.Cells[row, 17].Text, out var mm) ? mm : 0,
                    AuditModule = int.TryParse(worksheet.Cells[row, 18].Text, out var am) ? am : 0,
                    RiskModule = int.TryParse(worksheet.Cells[row, 19].Text, out var rm) ? rm : 0,
                    ComplianceModule = int.TryParse(worksheet.Cells[row, 20].Text, out var cm) ? cm : 0,
                    BCMModule = int.TryParse(worksheet.Cells[row, 21].Text, out var bcm) ? bcm : 0,
                    DigitalOfficeModule = int.TryParse(worksheet.Cells[row, 22].Text, out var dom) ? dom : 0,
                    MasterRole = int.TryParse(worksheet.Cells[row, 23].Text, out var mr) ? mr : 0,
                    AuditRole = int.TryParse(worksheet.Cells[row, 24].Text, out var ar) ? ar : 0,
                    RiskRole = int.TryParse(worksheet.Cells[row, 25].Text, out var rr) ? rr : 0,
                    ComplianceRole = int.TryParse(worksheet.Cells[row, 26].Text, out var cr) ? cr : 0,
                    BCMRole = int.TryParse(worksheet.Cells[row, 27].Text, out var br) ? br : 0,
                    DigitalOfficeRole = int.TryParse(worksheet.Cells[row, 28].Text, out var dor) ? dor : 0,
                    CreatedBy = int.TryParse(worksheet.Cells[row, 29].Text, out var createdBy) ? createdBy : 0,
                    UpdatedBy = int.TryParse(worksheet.Cells[row, 30].Text, out var updatedBy) ? updatedBy : 0,
                    DelFlag = worksheet.Cells[row, 31].Text,
                    Status = worksheet.Cells[row, 32].Text,
                    IPAddress = worksheet.Cells[row, 33].Text,
                    CompId = int.TryParse(worksheet.Cells[row, 34].Text, out var compId) ? compId : 0,
                    Type = worksheet.Cells[row, 35].Text,
                    IsSuperuser = int.TryParse(worksheet.Cells[row, 36].Text, out var su) ? su : 0,
                    DeptID = int.TryParse(worksheet.Cells[row, 37].Text, out var dept) ? dept : 0,
                    MemberType = int.TryParse(worksheet.Cells[row, 38].Text, out var mt) ? mt : 0,
                    Levelcode = int.TryParse(worksheet.Cells[row, 39].Text, out var lc) ? lc : 0,
                    Suggestions = int.TryParse(worksheet.Cells[row, 40].Text, out var sug2) ? sug2 : 0
                });
            }
            return employees;
        }

        // ✅ Combined Validation (mandatory fields + duplicates in one go)
        private List<UploadEmployeeMasterDto> ValidateEmployees(List<UploadEmployeeMasterDto> employees)
        {
            var errors = new List<UploadEmployeeMasterDto>();

            // 🔹 Mandatory field checks
            foreach (var emp in employees)
            {
                if (string.IsNullOrWhiteSpace(emp.EmpCode))
                    errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "EmpCode missing" });

                if (string.IsNullOrWhiteSpace(emp.EmployeeName))
                    errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "EmployeeName missing" });

                if (string.IsNullOrWhiteSpace(emp.LoginName))
                    errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "LoginName missing" });

                // ✅ Gmail format check
                if (string.IsNullOrWhiteSpace(emp.Email))
                {
                    errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "Email missing" });
                }
                else
                {
                    string trimmedEmail = emp.Email.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedEmail, @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "Invalid Gmail format. Must be like 'username@gmail.com'" });
                    }
                }

                // ✅ OfficePhoneNo check (10 digits only)
                if (string.IsNullOrWhiteSpace(emp.OfficePhoneNo))
                {
                    errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "OfficePhoneNo missing" });
                }
                else
                {
                    string phone = emp.OfficePhoneNo.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10}$"))
                    {
                        errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "OfficePhoneNo must be exactly 10 digits" });
                    }
                }

                if (string.IsNullOrWhiteSpace(emp.Designation))
                    errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "Designation missing" });

                if (string.IsNullOrWhiteSpace(emp.Partner))
                    errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "Partner missing" });

                if (string.IsNullOrWhiteSpace(emp.Role))
                    errors.Add(new UploadEmployeeMasterDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "Role missing" });
            }

            return errors;
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

        //UploadClientDetails
        public class ClientDetailsUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }

            public ClientDetailsUploadException(Dictionary<string, List<string>> errors)
                : base("Error processing client details")
            {
                Errors = errors;
            }
        }

        public async Task<List<string>> UploadClientDetailsAsync(int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");


            // ✅ Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");


            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Parse Excel
            List<UploadClientDetailsDto> clients;
            List<string> headerErrors = new List<string>();
            using (var stream = file.OpenReadStream())
            {
                clients = ParseExcelToClients(stream, out headerErrors);
            }

            // ✅ Validate missing/invalid values
            var missingOrInvalidErrors = ValidateClients(clients);

            // ✅ Duplicate errors
            var duplicateErrors = new List<UploadClientDetailsDto>();
            void AddDuplicateErrors(Func<UploadClientDetailsDto, string> keySelector, string fieldName)
            {
                duplicateErrors.AddRange(
                    clients.GroupBy(keySelector)
                        .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                        .Select(g => new UploadClientDetailsDto
                        {
                            CUST_CODE = g.First().CUST_CODE,
                            CUST_NAME = g.First().CUST_NAME,
                            ErrorMessage = $"Duplicate {fieldName} found: {g.Key}"
                        })
                );
            }

            AddDuplicateErrors(e => e.CUST_ID.ToString(), "CUST_ID");
            AddDuplicateErrors(c => c.CUST_EMAIL, "CUST_EMAIL");
            AddDuplicateErrors(c => c.CUST_TELPHONE, "CUST_TELPHONE");
            AddDuplicateErrors(c => c.CUST_ConEmailID, "CUST_ConEmailID");
            AddDuplicateErrors(c => c.CUST_CODE, "CUST_CODE");
            AddDuplicateErrors(c => c.CUST_WEBSITE, "CUST_WEBSITE");
            AddDuplicateErrors(c => c.CUST_FAX, "CUST_FAX");
            AddDuplicateErrors(c => c.CUST_COMM_Email, "CUST_COMM_Email");

            // ✅ Group errors
            var finalErrors = new Dictionary<string, List<string>>();

            if (headerErrors.Any())
                finalErrors["Missing column"] = headerErrors;

            if (missingOrInvalidErrors.Any())
            {
                var groupedMissing = missingOrInvalidErrors
                    .GroupBy(e => new { e.CUST_CODE, e.CUST_NAME })
                    .Select(g => $"{g.Key.CUST_NAME} ({g.Key.CUST_CODE}): {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();

                finalErrors["Missing values"] = groupedMissing;
            }

            if (duplicateErrors.Any())
            {
                var groupedDuplicates = duplicateErrors
                    .GroupBy(e => new { e.CUST_CODE, e.CUST_NAME })
                    .Select(g => $"{g.Key.CUST_NAME} ({g.Key.CUST_CODE}): {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();

                finalErrors["Duplication"] = groupedDuplicates;
            }

            if (finalErrors.Any())
                throw new ClientDetailsUploadException(finalErrors);

            // ✅ No validation errors → proceed with DB transaction

            var results = new List<string>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var client in clients)
                {// --- Quick mandatory check (fail-fast) ---
                    if (client.CUST_ID == 0 ||
                        string.IsNullOrWhiteSpace(client.CUST_NAME) ||
                        string.IsNullOrWhiteSpace(client.CUST_ORGTYPEID) ||
                        string.IsNullOrWhiteSpace(client.CUST_ADDRESS) ||
                        string.IsNullOrWhiteSpace(client.CUST_CITY) ||
                        string.IsNullOrWhiteSpace(client.CUST_EMAIL) ||
                        string.IsNullOrWhiteSpace(client.CUST_TELPHONE) ||
                        string.IsNullOrWhiteSpace(client.CUST_INDTYPEID) ||
                        string.IsNullOrWhiteSpace(client.CUST_LOCATIONID))
                    {
                        throw new Exception($"Mandatory fields missing for client: {client.CUST_NAME?.Trim() ?? "<unknown>"} ({client.CUST_CODE?.Trim() ?? "<no code>"})");
                    }

                    // Step 3: Ensure Industry Type exists
                    string indTypeSql = @"
SELECT Cmm_ID 
FROM Content_Management_Master 
WHERE cmm_Category = 'IND' 
  AND CMM_CompID = @CompId 
  AND UPPER(cmm_Desc) = UPPER(@Name) 
  AND cmm_DelFlag = 'A'";

                    int? indTypeId = await connection.ExecuteScalarAsync<int?>(
                        indTypeSql,
                        new { Name = client.CUST_INDTYPEID, CompId = client.CUST_CompID },
                        transaction
                    );

                    if (!indTypeId.HasValue)
                    {
                        string insertIndType = @"
                        INSERT INTO Content_Management_Master (cmm_ID, cmm_Code, cmm_Category, CMM_CompID, cmm_Desc, cmm_DelFlag)
                        SELECT ISNULL(MAX(cmm_ID), 0) + 1,
                               'IND' + CAST(ISNULL(MAX(cmm_ID), 0) + 1 AS VARCHAR),
                               'IND',
                               @CompId,
                               @Name,
                               'A'
                        FROM Content_Management_Master;

                        SELECT ISNULL(MAX(cmm_ID), 0) FROM Content_Management_Master;";

                        indTypeId = await connection.ExecuteScalarAsync<int>(
                            insertIndType,
                            new { Name = client.CUST_INDTYPEID, CompId = client.CUST_CompID },
                            transaction
                        );
                    }

                    client.CUST_INDTYPEID = indTypeId?.ToString();

                    //Step 4: Ensure Customer Code exists or activate it
                    var custRecord = await connection.QueryFirstOrDefaultAsync<(int Id, string DelFlag)>(
                        @"SELECT Cust_ID AS Id, CUST_DELFLG AS DelFlag
      FROM SAD_CUSTOMER_MASTER
      WHERE CUST_CODE = @CustCode",
                        new { CustCode = client.CUST_CODE }, transaction
                    );

                    if (custRecord.Id == 0) // Not found
                    {
                        client.CUST_CODE = await connection.ExecuteScalarAsync<string>(
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

                    // Step 5: Ensure Org Type exists
                    string orgTypeSql = @"
    SELECT Cmm_ID 
    FROM Content_Management_Master 
    WHERE cmm_Category = 'ORG' 
      AND Cmm_CompID = @CompId 
      AND UPPER(cmm_Desc) = UPPER(@Name) 
      AND cmm_DelFlag = 'A'";

                    int? orgTypeId = await connection.ExecuteScalarAsync<int?>(
                        orgTypeSql, new { Name = client.CUST_ORGTYPEID, CompId = client.CUST_CompID }, transaction);

                    if (!orgTypeId.HasValue)
                    {
                        string insertOrgType = @"
                        INSERT INTO Content_Management_Master (cmm_ID, cmm_Code, cmm_Category, Cmm_CompID, cmm_Desc, cmm_DelFlag)
                        SELECT ISNULL(MAX(cmm_ID), 0) + 1,
                         'ORG' + CAST(ISNULL(MAX(cmm_ID), 0) + 1 AS VARCHAR),
                         'ORG',
                         @CompId,
                         @Name,
                          'A'
                         FROM Content_Management_Master;
                         SELECT ISNULL(MAX(cmm_ID), 0) FROM Content_Management_Master;";

                        orgTypeId = await connection.ExecuteScalarAsync<int>(
                            insertOrgType, new { Name = client.CUST_ORGTYPEID, CompId = client.CUST_CompID }, transaction);
                    }

                    // ✅ Step 4: Call stored procedure
                    using var cmd = new SqlCommand("spSAD_CUSTOMER_MASTER", connection, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CUST_ID", client.CUST_ID);
                    cmd.Parameters.AddWithValue("@CUST_NAME", client.CUST_NAME ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_CODE", client.CUST_CODE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_WEBSITE", client.CUST_WEBSITE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_EMAIL", client.CUST_EMAIL ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_GROUPNAME", client.CUST_GROUPNAME ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_GROUPINDIVIDUAL", client.CUST_GROUPINDIVIDUAL);
                    cmd.Parameters.AddWithValue("@CUST_ORGTYPEID", orgTypeId);
                    cmd.Parameters.AddWithValue("@CUST_INDTYPEID", indTypeId);
                    cmd.Parameters.AddWithValue("@CUST_MGMTTYPEID", client.CUST_MGMTTYPEID);
                    cmd.Parameters.AddWithValue("@CUST_CommitmentDate", client.CUST_CommitmentDate);
                    cmd.Parameters.AddWithValue("@CUSt_BranchId", client.CUSt_BranchId ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_ADDRESS", client.CUST_COMM_ADDRESS ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_CITY", client.CUST_COMM_CITY ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_PIN", client.CUST_COMM_PIN ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_STATE", client.CUST_COMM_STATE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_COUNTRY", client.CUST_COMM_COUNTRY ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_FAX", client.CUST_COMM_FAX ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_TEL", client.CUST_COMM_TEL ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_Email", client.CUST_COMM_Email ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_ADDRESS", client.CUST_ADDRESS ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_CITY", client.CUST_CITY ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_PIN", client.CUST_PIN ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_STATE", client.CUST_STATE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COUNTRY", client.CUST_COUNTRY ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_FAX", client.CUST_FAX ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_TELPHONE", client.CUST_TELPHONE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_ConEmailID", client.CUST_ConEmailID ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_LOCATIONID", client.CUST_LOCATIONID ?? "0");
                    cmd.Parameters.AddWithValue("@CUST_TASKS", client.CUST_TASKS ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_ORGID", orgTypeId);
                    cmd.Parameters.AddWithValue("@CUST_CRBY", client.CUST_CRBY);
                    cmd.Parameters.AddWithValue("@CUST_UpdatedBy", client.CUST_UpdatedBy);
                    cmd.Parameters.AddWithValue("@CUST_BOARDOFDIRECTORS", client.CUST_BOARDOFDIRECTORS ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_DEPMETHOD", client.CUST_DEPMETHOD);
                    cmd.Parameters.AddWithValue("@CUST_IPAddress", client.CUST_IPAddress ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_CompID", client.CUST_CompID);
                    cmd.Parameters.AddWithValue("@CUST_Amount_Type", client.CUST_Amount_Type);
                    cmd.Parameters.AddWithValue("@CUST_RoundOff", client.CUST_RoundOff);
                    cmd.Parameters.AddWithValue("@Cust_DurtnId", client.Cust_DurtnId);
                    cmd.Parameters.AddWithValue("@Cust_FY", client.Cust_FY);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(updateOrSaveParam);
                    cmd.Parameters.Add(operParam);

                    await cmd.ExecuteNonQueryAsync();

                    int updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    int oper = (int)(operParam.Value ?? 0);

                    string action = updateOrSave == 2 ? "Updated" : "Inserted";
                    results.Add($"{action} client: {client.CUST_CODE} - {client.CUST_NAME} (Client_ID={oper})");

                    // Step 6: Save Locations
                    if (!string.IsNullOrWhiteSpace(client.LocationName))
                    {
                        int locationId;
                        using (var cmdLoc = new SqlCommand("spSAD_CUST_LOCATION", connection, transaction))
                        {
                            cmdLoc.CommandType = CommandType.StoredProcedure;
                            cmdLoc.Parameters.AddWithValue("@Mas_Id", client.Mas_Id);
                            cmdLoc.Parameters.AddWithValue("@Mas_code", client.Mas_code ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Description", client.LocationName ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_DelFlag", client.DelFlag ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CustID", client.CUST_ID);
                            cmdLoc.Parameters.AddWithValue("@Mas_Loc_Address", client.Address ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_Person", client.ContactPerson ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_MobileNo", client.Mobile ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_LandLineNo", client.Landline ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_Email", client.Email ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@mas_Designation", client.Designation ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CRBY", client.CUST_CRBY);
                            cmdLoc.Parameters.AddWithValue("@Mas_UpdatedBy", client.CUST_UpdatedBy);
                            cmdLoc.Parameters.AddWithValue("@Mas_STATUS", "A");
                            cmdLoc.Parameters.AddWithValue("@Mas_IPAddress", client.CUST_IPAddress ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CompID", client.CUST_CompID);

                            var updateOrSaveLoc = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operLoc = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmdLoc.Parameters.Add(updateOrSaveLoc);
                            cmdLoc.Parameters.Add(operLoc);

                            await cmdLoc.ExecuteNonQueryAsync();
                            locationId = (int)(operLoc.Value ?? 0);
                        }

                        // Step 7: Save Statutory Refs
                        async Task SaveStatutoryRef(string desc, string value)
                        {
                            if (string.IsNullOrWhiteSpace(value))
                                return;

                            using var cmdStat = new SqlCommand("spSAD_CUST_Accounting_Template", connection, transaction);
                            cmdStat.CommandType = CommandType.StoredProcedure;
                            cmdStat.Parameters.AddWithValue("@Cust_PKID", 0);
                            cmdStat.Parameters.AddWithValue("@Cust_ID", client.CUST_ID);
                            cmdStat.Parameters.AddWithValue("@Cust_Desc", desc);
                            cmdStat.Parameters.AddWithValue("@Cust_Value", value);
                            cmdStat.Parameters.AddWithValue("@Cust_Delflag", "A");
                            cmdStat.Parameters.AddWithValue("@Cust_Status", "A");
                            cmdStat.Parameters.AddWithValue("@Cust_AttchID", 0);
                            cmdStat.Parameters.AddWithValue("@Cust_CrBy", client.CUST_CRBY);
                            cmdStat.Parameters.AddWithValue("@Cust_UpdatedBy", client.CUST_UpdatedBy);
                            cmdStat.Parameters.AddWithValue("@Cust_IPAddress", client.CUST_IPAddress ?? string.Empty);
                            cmdStat.Parameters.AddWithValue("@Cust_Compid", client.CUST_CompID);
                            cmdStat.Parameters.AddWithValue("@Cust_LocationId", locationId);

                            var updateOrSaveStat = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operStat = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmdStat.Parameters.Add(updateOrSaveStat);
                            cmdStat.Parameters.Add(operStat);

                            await cmdStat.ExecuteNonQueryAsync();
                        }
                        await SaveStatutoryRef("CIN", client.CIN);
                        await SaveStatutoryRef("TAN", client.TAN);
                        await SaveStatutoryRef("GST", client.GST);
                    }
                    // keep your existing results.Add(...) logic
                    results.Add($"{updateOrSave}, {client.CUST_ID}");
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
        private List<UploadClientDetailsDto> ParseExcelToClients(Stream fileStream, out List<string> headerErrors)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            // ✅ Define expected headers (based on your template)
            string[] expectedHeaders =
            {
        "CustId","Cust Name","Cust Organisation Type","Cust Address","Cust City",
        "Cust E-mail","Cust Telephone","Cust IndustryType","Cust LocationName","ConEmailId/Contact Person",
        "Cust Code","Cust Website","Cust grpName","Cust GrpIndividual","Cust MgmTypeId",
        "Commitment Date","BranchId","Comm_Address","Comm_City","Comm_Pin",
        "Comm_State","Comm_Country","Comm_Fax","Comm_TelphoneNo","Comm_Email",
        "Cust  Pin","Cust State","Cust Country","Cust Fax","Cust Task",
        "Cust OrgId","Cust CrBy","Cust UpdatedBy","Cust Board Of Derectors","Cust Dep Method",
        "Cust Ip Address","Cust CompId","Cust Amount Type","Cust Round Off","Cust DurtnId","Cust Fy"
    };

            headerErrors = new List<string>();
            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                string actualHeader = worksheet.Cells[1, col].Text?.Trim();
                string expectedHeader = expectedHeaders[col - 1];

                if (!string.Equals(actualHeader, expectedHeader, StringComparison.OrdinalIgnoreCase))
                {
                    headerErrors.Add($"Expected header '{expectedHeader}' at column {col}, but found '{actualHeader}'");
                }
            }

            var clients = new List<UploadClientDetailsDto>();
            for (int row = 2; row <= rowCount; row++) // skip header
            {
                clients.Add(new UploadClientDetailsDto
                {
                    CUST_ID = int.TryParse(worksheet.Cells[row, 1].Text, out var custId) ? custId : 0,
                    CUST_NAME = worksheet.Cells[row, 2].Text,
                    CUST_ORGTYPEID = worksheet.Cells[row, 3].Text,
                    CUST_ADDRESS = worksheet.Cells[row, 4].Text,
                    CUST_CITY = worksheet.Cells[row, 5].Text,
                    CUST_EMAIL = worksheet.Cells[row, 6].Text,
                    CUST_TELPHONE = worksheet.Cells[row, 7].Text,
                    CUST_INDTYPEID = worksheet.Cells[row, 8].Text,
                    CUST_LOCATIONID = worksheet.Cells[row, 9].Text?.Trim(),
                    CUST_ConEmailID = worksheet.Cells[row, 10].Text?.Trim(),
                    CUST_CODE = worksheet.Cells[row, 11].Text?.Trim(),
                    CUST_WEBSITE = worksheet.Cells[row, 12].Text?.Trim(),
                    CUST_GROUPNAME = worksheet.Cells[row, 13].Text?.Trim(),
                    CUST_GROUPINDIVIDUAL = int.TryParse(worksheet.Cells[row, 14].Text, out var custGrpIndividual) ? custGrpIndividual : 0,
                    CUST_MGMTTYPEID = int.TryParse(worksheet.Cells[row, 15].Text, out var mgmTypeId) ? mgmTypeId : 0,
                    CUST_CommitmentDate = worksheet.Cells[row, 16].GetValue<DateTime?>(),
                    CUSt_BranchId = worksheet.Cells[row, 17].Text?.Trim(),
                    CUST_COMM_ADDRESS = worksheet.Cells[row, 18].Text?.Trim(),
                    CUST_COMM_CITY = worksheet.Cells[row, 19].Text?.Trim(),
                    CUST_COMM_PIN = worksheet.Cells[row, 20].Text?.Trim(),
                    CUST_COMM_STATE = worksheet.Cells[row, 21].Text?.Trim(),
                    CUST_COMM_COUNTRY = worksheet.Cells[row, 22].Text?.Trim(),
                    CUST_COMM_FAX = worksheet.Cells[row, 23].Text?.Trim(),
                    CUST_COMM_TEL = worksheet.Cells[row, 24].Text?.Trim(),
                    CUST_COMM_Email = worksheet.Cells[row, 25].Text?.Trim(),
                    CUST_PIN = worksheet.Cells[row, 26].Text?.Trim(),
                    CUST_STATE = worksheet.Cells[row, 27].Text?.Trim(),
                    CUST_COUNTRY = worksheet.Cells[row, 28].Text?.Trim(),
                    CUST_FAX = worksheet.Cells[row, 29].Text?.Trim(),
                    CUST_TASKS = worksheet.Cells[row, 30].Text?.Trim(),
                    CUST_ORGID = int.TryParse(worksheet.Cells[row, 31].Text, out var orgId) ? orgId : 0,
                    CUST_CRBY = int.TryParse(worksheet.Cells[row, 32].Text, out var createdBy) ? createdBy : 0,
                    CUST_UpdatedBy = int.TryParse(worksheet.Cells[row, 33].Text, out var updatedBy) ? updatedBy : 0,
                    CUST_BOARDOFDIRECTORS = worksheet.Cells[row, 34].Text?.Trim(),
                    CUST_DEPMETHOD = int.TryParse(worksheet.Cells[row, 35].Text, out var custDepMethod) ? custDepMethod : 0,
                    CUST_IPAddress = worksheet.Cells[row, 36].Text?.Trim(),
                    CUST_CompID = int.TryParse(worksheet.Cells[row, 37].Text, out var compId) ? compId : 0,
                    CUST_Amount_Type = int.TryParse(worksheet.Cells[row, 38].Text, out var custAmountType) ? custAmountType : 0,
                    CUST_RoundOff = int.TryParse(worksheet.Cells[row, 39].Text, out var custRoundff) ? custRoundff : 0,
                    Cust_DurtnId = int.TryParse(worksheet.Cells[row, 40].Text, out var durtnId) ? durtnId : 0,
                    Cust_FY = worksheet.Cells[row, 41].Text?.Trim(),

                });
            }

            return clients;
        }
        private List<UploadClientDetailsDto> ValidateClients(List<UploadClientDetailsDto> clients)
        {
            var errors = new List<UploadClientDetailsDto>();

            foreach (var client in clients)
            {
                if (client.CUST_ID <= 0)
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_ID missing" });

                if (string.IsNullOrWhiteSpace(client.CUST_NAME))
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_NAME missing" });

                if (string.IsNullOrWhiteSpace(client.CUST_ORGTYPEID))
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_ORGTYPEID missing" });

                if (string.IsNullOrWhiteSpace(client.CUST_ADDRESS))
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_ADDRESS missing" });

                if (string.IsNullOrWhiteSpace(client.CUST_CITY))
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_CITY missing" });

                // ✅ CUST_EMAIL check (must be a valid Gmail address)
                if (string.IsNullOrWhiteSpace(client.CUST_EMAIL))
                {
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = client.CUST_CODE,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "CUST_EMAIL missing"
                    });
                }
                else
                {
                    string trimmedEmail = client.CUST_EMAIL.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedEmail, @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "Invalid Gmail format. Must be like 'username@gmail.com'" });
                    }
                }

                // ✅ CUST_TELPHONE check (must be exactly 10 digits)
                if (string.IsNullOrWhiteSpace(client.CUST_TELPHONE))
                {
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_TELPHONE missing" });
                }
                else
                {
                    string phone = client.CUST_TELPHONE.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10}$"))
                    {
                        errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_TELPHONE must be exactly 10 digits" });
                    }
                }

                if (string.IsNullOrWhiteSpace(client.CUST_INDTYPEID))
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_INDTYPEID missing" });

                // ✅ CUST_ConEmailID check (must be a valid Gmail address)
                if (string.IsNullOrWhiteSpace(client.CUST_ConEmailID))
                {
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_ConEmailID missing" });
                }
                else
                {
                    string trimmedConEmail = client.CUST_ConEmailID.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedConEmail, @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "Invalid Gmail format for CUST_ConEmailID. Must be like 'username@gmail.com'" });
                    }
                }

                if (string.IsNullOrWhiteSpace(client.CUST_LOCATIONID))
                    errors.Add(new UploadClientDetailsDto { CUST_CODE = client.CUST_CODE, CUST_NAME = client.CUST_NAME, ErrorMessage = "CUST_LOCATIONID missing" });
            }
            return errors;
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
                    if (!string.IsNullOrWhiteSpace(objCust.OrgTypeName) && objCust.CUST_ORGTYPEID == 0)
                    {
                        int orgTypeId = await connection.ExecuteScalarAsync<int?>(
                            @"SELECT TOP 1 Cmm_ID 
          FROM Content_Management_Master 
          WHERE cmm_Category = 'ORG' 
            AND Cmm_CompID = @CompId 
            AND UPPER(cmm_Desc) = UPPER(@OrgType)",
                            new { CompId, OrgType = objCust.OrgTypeName }, transaction
                        ) ?? 0;

                        if (orgTypeId == 0)
                        {
                            // Get next cmm_ID before inserting
                            int nextCmmId = await connection.ExecuteScalarAsync<int>(
                                "SELECT ISNULL(MAX(cmm_ID) + 1, 1) FROM Content_Management_Master",
                                transaction: transaction
                            );

                            // Insert new OrgType
                            orgTypeId = await connection.ExecuteScalarAsync<int>(
                                @"INSERT INTO Content_Management_Master 
              (cmm_ID, cmm_Code, cmm_Category, Cmm_CompID, cmm_Desc, cmm_DelFlag)
              VALUES (@Cmm_ID, @cmm_Code, 'ORG', @CompId, @OrgType, 'A');
              SELECT @Cmm_ID;",
                                new
                                {
                                    Cmm_ID = nextCmmId,
                                    cmm_Code = objCust.Cmm_Code,
                                    CompId,
                                    OrgType = objCust.OrgTypeName
                                },
                                transaction
                            );
                        }
                        else
                        {
                            // Ensure it's active
                            await connection.ExecuteAsync(
                                @"UPDATE Content_Management_Master
              SET cmm_DelFlag = 'A'
              WHERE Cmm_ID = @OrgTypeId",
                                new { OrgTypeId = orgTypeId }, transaction
                            );
                        }

                        objCust.CUST_ORGTYPEID = orgTypeId;
                    }


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

        //UploadClientUser
        public class ClientUserUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }

            public ClientUserUploadException(Dictionary<string, List<string>> errors)
            {
                Errors = errors;
            }
        }
        public async Task<List<string>> UploadClientUserAsync(int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            // ✅ Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Parse Excel + Header Validation
            List<UploadClientUserDto> employees;
            List<string> headerErrors = new List<string>();
            using (var stream = file.OpenReadStream())
            {
                employees = ParseExcelToClientUser(stream, out headerErrors);
            }

            // ✅ Step 1: Validate (only missing + invalids)
            var missingOrInvalidErrors = ValidateClientUser(employees);

            // ✅ Step 2: Collect duplicate errors separately
            var duplicateErrors = new List<UploadClientUserDto>();

            void AddDuplicateErrors(Func<UploadClientUserDto, string> keySelector, string fieldName)
            {
                duplicateErrors.AddRange(
                    employees.GroupBy(keySelector)
                        .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                        .Select(g => new UploadClientUserDto
                        {
                            EmpCode = g.First().EmpCode,
                            EmployeeName = g.First().EmployeeName,
                            ErrorMessage = $"Duplicate {fieldName} found: {g.Key}"
                        })
                );
            }

            AddDuplicateErrors(e => e.EmpCode, "EmpCode");
            AddDuplicateErrors(e => e.LoginName, "LoginName");
            AddDuplicateErrors(e => e.Email, "Email");
            AddDuplicateErrors(e => e.PhoneNo, "PhoneNo");

            // ✅ Step 3: Group errors into structured dictionary
            var finalErrors = new Dictionary<string, List<string>>();

            if (headerErrors.Any())
            {
                finalErrors["Missing column"] = headerErrors;
            }

            if (missingOrInvalidErrors.Any())
            {
                var groupedMissing = missingOrInvalidErrors
                    .GroupBy(e => new { e.EmpCode, e.EmployeeName })
                    .Select(g => $"{g.Key.EmployeeName} ({g.Key.EmpCode}): {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();

                finalErrors["Missing values"] = groupedMissing;
            }

            if (duplicateErrors.Any())
            {
                var groupedDuplicates = duplicateErrors
                    .GroupBy(e => new { e.EmpCode, e.EmployeeName })
                    .Select(g => $"{g.Key.EmployeeName} ({g.Key.EmpCode}): {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();

                finalErrors["Duplication"] = groupedDuplicates;
            }

            // ✅ Throw structured error
            if (finalErrors.Any())
            {
                throw new ClientUserUploadException(finalErrors);
            }

            var results = new List<string>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var emp in employees)
                {
                    // Step 4: Validate mandatory fields
                    if (string.IsNullOrWhiteSpace(emp.EmpCode) ||
                        string.IsNullOrWhiteSpace(emp.EmployeeName) ||
                        string.IsNullOrWhiteSpace(emp.LoginName) ||
                        string.IsNullOrWhiteSpace(emp.Email) ||
                        string.IsNullOrWhiteSpace(emp.PhoneNo))
                    {
                        throw new Exception($"Mandatory fields missing for Client: {emp.EmployeeName}");
                    }

                    // Step 2: Ensure CompanyId exists
                    string customerSql = @"
SELECT CUST_ID 
FROM SAD_CUSTOMER_MASTER 
WHERE UPPER(CUST_NAME) = UPPER(@CustomerName) 
  AND CUST_CompID = @CompId";

                    int? CompanyId = await connection.ExecuteScalarAsync<int?>(
                        customerSql, new { CustomerName = emp.CompanyId, CompId = compId }, transaction);

                    if (!CompanyId.HasValue)
                    {
                        throw new Exception($"Customer '{emp.CompanyId}' does not exist in master table.");
                    }

                    emp.CompanyId = CompanyId.Value.ToString();

                    // Step 7: Check if ClientUser exists
                    string checkEmpSql = "SELECT COUNT(1) FROM Sad_UserDetails WHERE Usr_Code=@EmpCode AND Usr_CompId=@CompId";
                    bool exists = await connection.ExecuteScalarAsync<int>(
                        checkEmpSql, new { EmpCode = emp.EmpCode, CompId = compId }, transaction) > 0;

                    // Step 8: Insert/Update using SP
                    using var cmd = new SqlCommand("spEmployeeMaster", connection, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Usr_ID", (object?)emp.EmpId ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_Node", emp.EmpNode ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_Code", emp.EmpCode ?? "");
                    cmd.Parameters.AddWithValue("@Usr_FullName", emp.EmployeeName ?? "");
                    cmd.Parameters.AddWithValue("@Usr_LoginName", emp.LoginName ?? "");
                    cmd.Parameters.AddWithValue("@Usr_Password", emp.Password ?? "");
                    cmd.Parameters.AddWithValue("@Usr_Email", emp.Email ?? "");
                    cmd.Parameters.AddWithValue("@Usr_Category", emp.EmpCategory ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_Suggetions", emp.Suggestions ?? 0);
                    cmd.Parameters.AddWithValue("@usr_partner", emp.Partner);
                    cmd.Parameters.AddWithValue("@Usr_LevelGrp", emp.LevelGrp ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_DutyStatus", emp.DutyStatus ?? "A");
                    cmd.Parameters.AddWithValue("@Usr_PhoneNo", emp.PhoneNo ?? "");
                    cmd.Parameters.AddWithValue("@Usr_MobileNo", emp.MobileNo ?? "");
                    cmd.Parameters.AddWithValue("@Usr_OfficePhone", emp.OfficePhoneNo ?? "");
                    cmd.Parameters.AddWithValue("@Usr_OffPhExtn", emp.OfficePhoneExtn ?? "");
                    cmd.Parameters.AddWithValue("@Usr_Designation", emp.Designation ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_CompanyID", CompanyId);
                    cmd.Parameters.AddWithValue("@Usr_OrgnID", emp.OrgnId ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_GrpOrUserLvlPerm", emp.GrpOrUserLvlPerm ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_Role", emp.Role ?? 0);

                    // ✅ Module flags
                    cmd.Parameters.AddWithValue("@Usr_MasterModule", emp.MasterModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_AuditModule", emp.AuditModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_RiskModule", emp.RiskModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_ComplianceModule", emp.ComplianceModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_BCMModule", emp.BCMModule ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_DigitalOfficeModule", emp.DigitalOfficeModule ?? 0);

                    // ✅ Role flags
                    cmd.Parameters.AddWithValue("@Usr_MasterRole", emp.MasterRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_AuditRole", emp.AuditRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_RiskRole", emp.RiskRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_ComplianceRole", emp.ComplianceRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_BCMRole", emp.BCMRole ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_DigitalOfficeRole", emp.DigitalOfficeRole ?? 0);

                    // ✅ Metadata
                    cmd.Parameters.AddWithValue("@Usr_CreatedBy", emp.CreatedBy ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_UpdatedBy", emp.UpdatedBy ?? 0);
                    cmd.Parameters.AddWithValue("@Usr_DelFlag", emp.DelFlag ?? "A");
                    cmd.Parameters.AddWithValue("@Usr_Status", emp.Status ?? "N");
                    cmd.Parameters.AddWithValue("@Usr_IPAddress", emp.IPAddress ?? "");
                    cmd.Parameters.AddWithValue("@Usr_CompId", emp.CompId ?? compId);
                    cmd.Parameters.AddWithValue("@Usr_Type", emp.Type ?? "C");
                    cmd.Parameters.AddWithValue("@usr_IsSuperuser", emp.IsSuperuser ?? 0);
                    cmd.Parameters.AddWithValue("@USR_DeptID", emp.DeptID ?? 0);
                    cmd.Parameters.AddWithValue("@USR_MemberType", emp.MemberType ?? 0);
                    cmd.Parameters.AddWithValue("@USR_Levelcode", emp.Levelcode ?? 0);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(updateOrSaveParam);
                    cmd.Parameters.Add(operParam);

                    await cmd.ExecuteNonQueryAsync();

                    int updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    int oper = (int)(operParam.Value ?? 0);

                    string action = updateOrSave == 2 ? "Updated" : "Inserted";
                    results.Add($"{action} employee: {emp.EmpCode} - {emp.EmployeeName} (Usr_ID={oper})");
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

        //Parse Excel file into Employee DTO list
        private List<UploadClientUserDto> ParseExcelToClientUser(Stream fileStream, out List<string> headerErrors)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;

            // Expected headers (from template)
            string[] expectedHeaders = new[]
            {
        "Customer","Emp Code","Employee Name","Login Name","E-Mail","PhoneNo","Password","Partner","LevelGrp","DutyStatus",
        "MobileNo","OfficePhoneNo","OffPhExtn","Designation","OrgID","GrpOrUserLvlPerm","Role",
        "MasterModule","AuditModule","RiskModule","ComplianceModule","BCMModule","DigitalOfficeModule",
        "MasterRole","AuditRole","RiskRole","ComplianceRole","BCMRole","DigitalOfficeRole",
        "CreatedBy","UpdatedBy","DelFlag","Status","IPAddress","CompId","Type","IsSuperuser",
        "DeptID","MemberType","Levelcode","Suggestions"
    };

            headerErrors = new List<string>();

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                string actualHeader = worksheet.Cells[1, col].Text?.Trim();
                string expectedHeader = expectedHeaders[col - 1];

                if (!string.Equals(actualHeader, expectedHeader, StringComparison.OrdinalIgnoreCase))
                {
                    headerErrors.Add($"Expected header '{expectedHeader}' at column {col}, but found '{actualHeader}'");
                }
            }

            var employees = new List<UploadClientUserDto>();
            for (int row = 2; row <= rowCount; row++) // skip header
            {
                employees.Add(new UploadClientUserDto
                {
                    CompanyId = worksheet.Cells[row, 1].Text,
                    EmpCode = worksheet.Cells[row, 2].Text,
                    EmployeeName = worksheet.Cells[row, 3].Text,
                    LoginName = worksheet.Cells[row, 4].Text,
                    Email = worksheet.Cells[row, 5].Text,
                    PhoneNo = worksheet.Cells[row, 6].Text,
                    Password = worksheet.Cells[row, 7].Text,
                    Partner = int.TryParse(worksheet.Cells[row, 8].Text, out var partner) ? partner : 0,
                    LevelGrp = int.TryParse(worksheet.Cells[row, 9].Text, out var levelGrp) ? levelGrp : 0,
                    DutyStatus = worksheet.Cells[row, 10].Text,
                    MobileNo = worksheet.Cells[row, 11].Text,
                    OfficePhoneNo = worksheet.Cells[row, 12].Text,
                    OfficePhoneExtn = worksheet.Cells[row, 13].Text,
                    Designation = int.TryParse(worksheet.Cells[row, 14].Text, out var designation) ? designation : 0,
                    OrgnId = int.TryParse(worksheet.Cells[row, 15].Text, out var orgId) ? orgId : 0,
                    GrpOrUserLvlPerm = int.TryParse(worksheet.Cells[row, 16].Text, out var grpOfuser) ? grpOfuser : 0,
                    Role = int.TryParse(worksheet.Cells[row, 17].Text, out var role) ? role : 0,
                    MasterModule = int.TryParse(worksheet.Cells[row, 18].Text, out var mm) ? mm : 0,
                    AuditModule = int.TryParse(worksheet.Cells[row, 19].Text, out var am) ? am : 0,
                    RiskModule = int.TryParse(worksheet.Cells[row, 20].Text, out var rm) ? rm : 0,
                    ComplianceModule = int.TryParse(worksheet.Cells[row, 21].Text, out var cm) ? cm : 0,
                    BCMModule = int.TryParse(worksheet.Cells[row, 22].Text, out var bcm) ? bcm : 0,
                    DigitalOfficeModule = int.TryParse(worksheet.Cells[row, 23].Text, out var dom) ? dom : 0,
                    MasterRole = int.TryParse(worksheet.Cells[row, 24].Text, out var mr) ? mr : 0,
                    AuditRole = int.TryParse(worksheet.Cells[row, 25].Text, out var ar) ? ar : 0,
                    RiskRole = int.TryParse(worksheet.Cells[row, 26].Text, out var rr) ? rr : 0,
                    ComplianceRole = int.TryParse(worksheet.Cells[row, 27].Text, out var cr) ? cr : 0,
                    BCMRole = int.TryParse(worksheet.Cells[row, 28].Text, out var br) ? br : 0,
                    DigitalOfficeRole = int.TryParse(worksheet.Cells[row, 29].Text, out var dor) ? dor : 0,
                    CreatedBy = int.TryParse(worksheet.Cells[row, 30].Text, out var createdBy) ? createdBy : 0,
                    UpdatedBy = int.TryParse(worksheet.Cells[row, 31].Text, out var updatedBy) ? updatedBy : 0,
                    DelFlag = worksheet.Cells[row, 32].Text,
                    Status = worksheet.Cells[row, 33].Text,
                    IPAddress = worksheet.Cells[row, 34].Text,
                    CompId = int.TryParse(worksheet.Cells[row, 35].Text, out var compId) ? compId : 0,
                    Type = worksheet.Cells[row, 36].Text,
                    IsSuperuser = int.TryParse(worksheet.Cells[row, 37].Text, out var su) ? su : 0,
                    DeptID = int.TryParse(worksheet.Cells[row, 38].Text, out var dept) ? dept : 0,
                    MemberType = int.TryParse(worksheet.Cells[row, 39].Text, out var mt) ? mt : 0,
                    Levelcode = int.TryParse(worksheet.Cells[row, 40].Text, out var lc) ? lc : 0,
                    Suggestions = int.TryParse(worksheet.Cells[row, 41].Text, out var sug2) ? sug2 : 0
                });
            }

            return employees;
        }

        private List<UploadClientUserDto> ValidateClientUser(List<UploadClientUserDto> employees)
        {
            var errors = new List<UploadClientUserDto>();

            foreach (var emp in employees)
            {
                // ✅ Skip validation if the entire row is blank
                if (string.IsNullOrWhiteSpace(emp.CompanyId) &&
                    string.IsNullOrWhiteSpace(emp.EmpCode) &&
                    string.IsNullOrWhiteSpace(emp.EmployeeName) &&
                    string.IsNullOrWhiteSpace(emp.LoginName) &&
                    string.IsNullOrWhiteSpace(emp.Email) &&
                    string.IsNullOrWhiteSpace(emp.PhoneNo))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(emp.CompanyId))
                    errors.Add(new UploadClientUserDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "Customer missing" });

                if (string.IsNullOrWhiteSpace(emp.EmpCode))
                    errors.Add(new UploadClientUserDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "EmpCode missing" });

                if (string.IsNullOrWhiteSpace(emp.EmployeeName))
                    errors.Add(new UploadClientUserDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "EmployeeName missing" });

                if (string.IsNullOrWhiteSpace(emp.LoginName))
                    errors.Add(new UploadClientUserDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "LoginName missing" });

                // ✅ Email check
                if (string.IsNullOrWhiteSpace(emp.Email))
                {
                    errors.Add(new UploadClientUserDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "Email missing" });
                }
                else
                {
                    string trimmedEmail = emp.Email.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedEmail, @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadClientUserDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "Invalid Gmail format. Must be like 'username@gmail.com'" });
                    }
                }

                // ✅ PhoneNo check (10 digits only)
                if (string.IsNullOrWhiteSpace(emp.PhoneNo))
                {
                    errors.Add(new UploadClientUserDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "PhoneNo missing" });
                }
                else
                {
                    string phone = emp.PhoneNo.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10}$"))
                    {
                        errors.Add(new UploadClientUserDto { EmpCode = emp.EmpCode, EmployeeName = emp.EmployeeName, ErrorMessage = "PhoneNo must be exactly 10 digits" });
                    }
                }
            }
            return errors;
        }

        // SaveClientUser 
        public async Task<List<int[]>> SuperMasterSaveClientUserAsync(int CompId, List<SaveClientUserDto> clientUser)
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

                foreach (var objEmp in clientUser)
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

        //DownloadEmployeeMaster
        public EmployeeMasterResult GetEmployeeMasterExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core - Copy\\SampleExcels\\EmployeeMaster Template.xlsx";

            if (!File.Exists(filePath))
                return new EmployeeMasterResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "EmployeeMaster Template.xlsx";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new EmployeeMasterResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadClientDetails
        public ClientDetailsResult GetClientDetailsExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core - Copy\\SampleExcels\\ClientDetails Template.xlsx";

            if (!File.Exists(filePath))
                return new ClientDetailsResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "ClientDetails Template.xlsx";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new ClientDetailsResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadClientuser
        public ClientUserResult GetClientUserExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core - Copy\\SampleExcels\\ClientUser Template.xlsx";

            if (!File.Exists(filePath))
                return new ClientUserResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "ClientUser Template.xlsx";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new ClientUserResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadExcelTemplateFiles
        public ExcelTemplateResult GetExcelTemplate(string templateName)
        {
            var templates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Employee Master", @"C:\Users\Crcuial\Desktop\TracePA-Backend-Latest\tracepa-corebackend\SampleExcels\EmployeeMaster Template.xlsx" },
            { "Client Details", @"C:\Users\Crcuial\Desktop\TracePA-Backend-Latest\tracepa-corebackend\SampleExcels\ClientDetails Template.xlsx" },
            { "Client User", @"C:\Users\Crcuial\Desktop\TracePA-Backend-Latest\tracepa-corebackend\SampleExcels\ClientUser Template.xlsx" }
        };

            if (!templates.ContainsKey(templateName))
                return null;

            var filePath = templates[templateName];

            if (!File.Exists(filePath))
                return null;

            var bytes = File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            // Set content type dynamically based on extension
            var contentType = Path.GetExtension(filePath).ToLower() switch
            {
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                _ => "application/octet-stream"
            };

            return new ExcelTemplateResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        public class AuditTypeAndCheckpointsUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }
            public AuditTypeAndCheckpointsUploadException(Dictionary<string, List<string>> errors)
            {
                Errors = errors;
            }
        }

        public async Task<List<string>> UploadAuditTypeAndCheckpointsAsync(int compId, int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing.");

            var connectionString = _configuration.GetConnectionString(dbName);

            AuditTypeAndCheckpointParseResult parseResult;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var stream = file.OpenReadStream();
                parseResult = await ParseAuditTypeAndCheckpointExcelAndResolveAuditTypeAsync(stream, compId, userId, connection);
            }

            var rows = parseResult.Rows;
            var headerErrors = parseResult.HeaderErrors;
            var missingErrors = ValidateAuditChecklist(rows);

            var duplicateErrors = rows
                .GroupBy(r => new { r.ACM_AuditTypeID, r.ACM_Heading, r.ACM_Checkpoint })
                .Where(g => g.Count() > 1)
                .Select(g => $"Duplicate: Heading '{g.Key.ACM_Heading}' - Checkpoint '{g.Key.ACM_Checkpoint}'")
                .ToList();

            var finalErrors = new Dictionary<string, List<string>>();
            if (headerErrors.Any()) finalErrors["Missing column"] = headerErrors;
            if (missingErrors.Any()) finalErrors["Missing values"] = missingErrors;
            if (duplicateErrors.Any()) finalErrors["Duplication"] = duplicateErrors;

            if (finalErrors.Any())
                throw new AuditTypeAndCheckpointsUploadException(finalErrors);

            var failed = new List<string>();

            foreach (var dto in rows)
            {
                dto.ACM_CompId = compId;
                dto.ACM_CRBY = userId;
                dto.ACM_IPAddress = "127.0.0.1";

                if (!await SaveOrUpdateAuditTypeAndCheckpointAsync(dto))
                    failed.Add(dto.ACM_Checkpoint);
            }

            if (failed.Any())
            {
                return new List<string> { "Failed to save the following Audit Type And Checkpoints:" }
                    .Concat(failed).ToList();
            }

            return new List<string> { "Successfully saved Audit Type And Checkpoints details" };
        }

        private async Task<AuditTypeAndCheckpointParseResult> ParseAuditTypeAndCheckpointExcelAndResolveAuditTypeAsync(Stream stream, int compId, int userId, SqlConnection connection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);

            var sheet = package.Workbook.Worksheets[0];
            int rowCount = sheet.Dimension.Rows;                            

            var result = new AuditTypeAndCheckpointParseResult();
            string[] expectedHeaders = { "Audit Type", "Heading", "Checkpoint", "Assertions" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
                if (!string.Equals(sheet.Cells[1, col].Text?.Trim(), expectedHeaders[col - 1], StringComparison.OrdinalIgnoreCase))
                    result.HeaderErrors.Add($"Expected header '{expectedHeaders[col - 1]}' at column {col}, but found '{sheet.Cells[1, col].Text}'");

            int? lastAuditTypeId = null;

            for (int row = 2; row <= rowCount; row++)
            {
                string typeName = sheet.Cells[row, 1].Text?.Trim();
                int? typeId = null;

                if (!string.IsNullOrWhiteSpace(typeName))
                {
                    typeId = await connection.ExecuteScalarAsync<int?>(
                        "SELECT cmm_ID FROM content_management_master WHERE UPPER(cmm_desc)=UPPER(@type) AND cmm_compID=@cid AND cmm_category='AT'",
                        new { type = typeName, cid = compId });

                    if (!typeId.HasValue)
                        typeId = await InsertNewAuditTypeAsync(typeName, compId, userId, connection, null);

                    lastAuditTypeId = typeId;
                }
                else
                {
                    if (lastAuditTypeId == null)
                        result.HeaderErrors.Add($"Audit Type missing at row {row}.");

                    typeId = lastAuditTypeId;
                }

                result.Rows.Add(new AuditTypeChecklistMasterDTO
                {
                    ACM_AuditTypeID = typeId ?? 0,
                    ACM_Heading = sheet.Cells[row, 2].Text,
                    ACM_Checkpoint = sheet.Cells[row, 3].Text,
                    ACM_Assertions = sheet.Cells[row, 4].Text
                });
            }
            return result;
        }

        private async Task<int> InsertNewAuditTypeAsync(string taskName, int compId, int userId, SqlConnection connection, SqlTransaction tx)
        {
            int maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(cmm_ID), 0) + 1 FROM Content_Management_Master WHERE CMM_CompID = @CompId", new { CompId = compId }, tx);

            string code = $"AT_{maxId}";

            int newId = await connection.ExecuteScalarAsync<int>(
                @"DECLARE @NewId INT = (SELECT ISNULL(MAX(CMM_ID),0) + 1 FROM Content_Management_Master);
                 INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMM_Delflag, CMM_Status, CMM_CrBy, CMM_CrOn, CMM_CompID) 
                 VALUES (@NewId, @CMM_Code, @CMM_Desc, 'AT', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_CompID);
                 SELECT @NewId;",
                new
                {
                    CMM_Code = code,
                    CMM_Desc = taskName,
                    CMM_CompID = compId,
                    CMM_CrBy = userId
                }, tx);

            return newId;
        }

        private List<string> ValidateAuditChecklist(List<AuditTypeChecklistMasterDTO> rows)
        {
            var errors = new List<string>();

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int excelRow = i + 2;

                if (r.ACM_AuditTypeID == 0)
                    errors.Add($"Row {excelRow}: Audit Type missing for checkpoint '{r.ACM_Checkpoint}'");

                if (string.IsNullOrWhiteSpace(r.ACM_Heading))
                    errors.Add($"Row {excelRow}: Heading missing for checkpoint '{r.ACM_Checkpoint}'");

                if (string.IsNullOrWhiteSpace(r.ACM_Checkpoint))
                    errors.Add($"Row {excelRow}: Checkpoint missing for heading '{r.ACM_Heading}'");
            }
            return errors;
        }

        public async Task<bool> SaveOrUpdateAuditTypeAndCheckpointAsync(AuditTypeChecklistMasterDTO dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                int exists = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM AuditType_Checklist_Master WHERE ACM_AuditTypeID = @ACM_AuditTypeID AND ACM_Checkpoint = @ACM_Checkpoint AND ACM_CompId = @ACM_CompId",
                    new
                    {
                        dto.ACM_AuditTypeID,
                        dto.ACM_Checkpoint,
                        dto.ACM_CompId
                    }, tx);

                if (exists > 0)
                {
                    await tx.CommitAsync();
                    return true;
                }

                int maxId = await connection.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(MAX(ACM_ID),0)+1 FROM AuditType_Checklist_Master WHERE ACM_CompId=@CompId",
                    new { CompId = dto.ACM_CompId }, tx);

                dto.ACM_Code = $"ACM_{maxId}";

                dto.ACM_ID = await connection.ExecuteScalarAsync<int>(
                    @"DECLARE @NewId INT = (SELECT ISNULL(MAX(ACM_ID),0)+1 FROM AuditType_Checklist_Master);
                      INSERT INTO AuditType_Checklist_Master (ACM_ID, ACM_Code, ACM_AuditTypeID, ACM_Heading, ACM_Checkpoint, ACM_Assertions, ACM_DELFLG, ACM_STATUS, ACM_CRBY, ACM_CRON, ACM_IPAddress, ACM_CompId)
                      VALUES (@NewId, @ACM_Code, @ACM_AuditTypeID, @ACM_Heading, @ACM_Checkpoint, @ACM_Assertions, 'A', 'A', @ACM_CRBY, GETDATE(), @ACM_IPAddress, @ACM_CompId);
                      SELECT @NewId;",
                    dto, tx);

                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                return false;
            }
        }

        public class TaskAndSubTasksUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }
            public TaskAndSubTasksUploadException(Dictionary<string, List<string>> errors)
            {
                Errors = errors;
            }
        }

        public async Task<List<string>> UploadTaskAndSubTasksAsync(int compId, int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing.");

            var connectionString = _configuration.GetConnectionString(dbName);
            TaskAndSubtasksParseResult parseResult;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var stream = file.OpenReadStream();
                parseResult = await ParseTaskAndSubtasksExcelAndResolveTaskAsync(stream, compId, userId, connection);
            }

            var rows = parseResult.Rows;
            var headerErrors = parseResult.HeaderErrors;
            var missingErrors = ValidateTaskAndSubtasklist(rows);

            var duplicateErrors = rows
                .GroupBy(r => new { r.ACM_AssignmentTaskID, r.ACM_Checkpoint })
                .Where(g => g.Count() > 1)
                .Select(g => $"Duplicate: Task '{g.Key.ACM_AssignmentTaskID}' - Subtask '{g.Key.ACM_Checkpoint}'")
                .ToList();

            var finalErrors = new Dictionary<string, List<string>>();
            if (headerErrors.Any()) finalErrors["Missing column"] = headerErrors;
            if (missingErrors.Any()) finalErrors["Missing values"] = missingErrors;
            if (duplicateErrors.Any()) finalErrors["Duplication"] = duplicateErrors;

            if (finalErrors.Any())
                throw new TaskAndSubTasksUploadException(finalErrors);

            var results = new List<string>();
            var failedCheckpoints = new List<string>();

            foreach (var dto in rows)
            {
                dto.ACM_CompId = compId;
                dto.ACM_CRBY = userId;
                dto.ACM_IPAddress = "127.0.0.1";

                if (!await SaveOrUpdateTaskAndSubTasklistAsync(dto))
                    failedCheckpoints.Add(dto.ACM_Checkpoint);
            }

            if (failedCheckpoints.Any())
            {
                results.Add("Failed to save the following Task And Subtask:");
                results.AddRange(failedCheckpoints);
                return results;
            }

            results.Add("Successfully saved Task And Subtask details");
            return results;
        }

        private async Task<TaskAndSubtasksParseResult> ParseTaskAndSubtasksExcelAndResolveTaskAsync(Stream stream, int compId, int userId, SqlConnection connection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);

            var sheet = package.Workbook.Worksheets[0];
            int rowCount = sheet.Dimension.Rows;

            var result = new TaskAndSubtasksParseResult();

            string[] expectedHeaders = { "Task", "Subtask", "Billing Type" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
                if (!string.Equals(sheet.Cells[1, col].Text?.Trim(), expectedHeaders[col - 1], StringComparison.OrdinalIgnoreCase))
                    result.HeaderErrors.Add($"Expected header '{expectedHeaders[col - 1]}' at column {col}, found '{sheet.Cells[1, col].Text}'");

            int? lastTaskId = null;
            for (int row = 2; row <= rowCount; row++)
            {
                string taskName = sheet.Cells[row, 1].Text?.Trim();
                int? taskId = null;

                if (!string.IsNullOrWhiteSpace(taskName))
                {
                    taskId = await connection.ExecuteScalarAsync<int?>(
                        "SELECT cmm_ID FROM content_management_master WHERE UPPER(cmm_desc)=UPPER(@task) AND cmm_compID=@cid AND cmm_category='ASGT'",
                        new { task = taskName, cid = compId });

                    if (!taskId.HasValue)
                        taskId = await InsertNewTaskAsync(taskName, compId, userId, connection, null);

                    lastTaskId = taskId;
                }
                else
                {
                    if (lastTaskId == null)
                        result.HeaderErrors.Add($"Task missing at row {row}, and no previous task found.");

                    taskId = lastTaskId;
                }

                string billingText = sheet.Cells[row, 3].Text?.Trim().ToLower();
                int billingType = billingText switch
                {
                    "billable" => 1,
                    "non billable" => 0,
                    "" => 0,
                    null => 0,
                    _ => 0
                };

                result.Rows.Add(new AssignmentTaskChecklistMasterDTO
                {
                    ACM_AssignmentTaskID = taskId ?? 0,
                    ACM_Heading = "",
                    ACM_Checkpoint = sheet.Cells[row, 2].Text,
                    ACM_BillingType = billingType
                });
            }
            return result;
        }

        private async Task<int> InsertNewTaskAsync(string taskName, int compId,int userId, SqlConnection connection, SqlTransaction tx)
        {
            int maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(cmm_ID), 0) + 1 FROM Content_Management_Master WHERE CMM_CompID = @CompId", new { CompId = compId }, tx);

            string code = $"ASGT_{maxId}";

            int newId = await connection.ExecuteScalarAsync<int>(
                @"DECLARE @NewId INT = (SELECT ISNULL(MAX(CMM_ID),0) + 1 FROM Content_Management_Master);
                 INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMM_Delflag, CMM_Status, CMM_CrBy, CMM_CrOn, CMM_CompID) 
                 VALUES (@NewId, @CMM_Code, @CMM_Desc, 'ASGT', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_CompID);
                 SELECT @NewId;",
                new
                {
                    CMM_Code = code,
                    CMM_Desc = taskName,
                    CMM_CompID = compId,
                    CMM_CrBy = userId
                }, tx);

            return newId;
        }


        private List<string> ValidateTaskAndSubtasklist(List<AssignmentTaskChecklistMasterDTO> rows)
        {
            var errors = new List<string>();
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int excelRow = i + 2;

                if (r.ACM_AssignmentTaskID == 0)
                    errors.Add($"Row {excelRow}: Task missing for Subtask '{r.ACM_Checkpoint}'");

                if (string.IsNullOrWhiteSpace(r.ACM_Checkpoint))
                    errors.Add($"Row {excelRow}: Subtask missing for the Task");
            }
            return errors;
        }


        public async Task<bool> SaveOrUpdateTaskAndSubTasklistAsync(AssignmentTaskChecklistMasterDTO dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                int exists = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM AssignmentTask_Checklist_Master WHERE ACM_AssignmentTaskID = @ACM_AssignmentTaskID AND ACM_Checkpoint = @ACM_Checkpoint AND ACM_CompId = @ACM_CompId",
                    new
                    {
                        dto.ACM_AssignmentTaskID,
                        dto.ACM_Checkpoint,
                        dto.ACM_CompId
                    }, tx);

                if (exists > 0)
                {
                    await tx.CommitAsync();
                    return true;
                }

                int maxId = await connection.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(MAX(ACM_ID),0) + 1 FROM AssignmentTask_Checklist_Master WHERE ACM_CompId=@CompId",
                    new { CompId = dto.ACM_CompId }, tx);

                dto.ACM_Code = $"ATCM_{maxId}";

                dto.ACM_ID = await connection.ExecuteScalarAsync<int>(
                    @"DECLARE @NewId INT = (SELECT ISNULL(MAX(ACM_ID),0) + 1 FROM AssignmentTask_Checklist_Master);
                    INSERT INTO AssignmentTask_Checklist_Master (ACM_ID, ACM_Code, ACM_AssignmentTaskID, ACM_Heading, ACM_Checkpoint, ACM_BillingType,
                    ACM_DELFLG, ACM_STATUS, ACM_CRBY, ACM_CRON, ACM_IPAddress, ACM_CompId) VALUES(@NewId, @ACM_Code, @ACM_AssignmentTaskID, @ACM_Heading, @ACM_Checkpoint, @ACM_BillingType, 'A', 'A', @ACM_CRBY, GETDATE(), @ACM_IPAddress, @ACM_CompId);
                    SELECT @NewId;",
                    dto, tx);

                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                return false;
            }
        }
    }
}

