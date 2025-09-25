using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Dto.CustomerMaster;
using TracePca.Interface;
using TracePca.Interface.EmployeeMaster;

namespace TracePca.Service.CustomerMaster
{
    public class CustomerMaster : CustomerMasterInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerMaster(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<CustomerDetailsDto>> GetCustomersWithStatusAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
SELECT 
    CUST_ID AS CustId,
    CUST_NAME AS CustName,
    CUST_CODE AS CustomerCode,
    CUST_EMAIL AS CustomerEmail,
    CUST_WEBSITE AS CompanyUrl,
    CUST_GROUPNAME AS GroupName,
    CUST_GROUPINDIVIDUAL AS GroupIndividual,
    CUST_ORGTYPEID AS OrganizationTypeId,
    CUST_INDTYPEID AS IndustryTypeId,
    CUST_MGMTTYPEID AS ManagementTypeId,
    CONVERT(VARCHAR(10), CUST_CommitmentDate, 105) AS CommitmentDate, -- ✅ dd-MM-yyyy
    CUST_BranchId AS CINNO,
    CUST_TASKS AS ServiceTypeIdCsv,  -- stored as comma-separated values
    CUST_BOARDOFDIRECTORS AS BoardOfDirectors,
    Cust_FY AS FinancialYearId,
    CASE CUST_Delflg
        WHEN 'A' THEN 'Activated'
        WHEN 'D' THEN 'De-Activated'
        WHEN 'W' THEN 'Waiting for Approval'
        ELSE 'Unknown'
    END AS Status
FROM SAD_CUSTOMER_MASTER
WHERE CUST_CompID = @CompanyId
ORDER BY CUST_ID";


            var customers = await connection.QueryAsync<CustomerDetailsDto>(query, new { CompanyId = companyId });

            // ✅ Step 2: Convert comma-separated ServiceTypeIdCsv to list
            foreach (var customer in customers)
            {
                customer.ServiceTypeId = string.IsNullOrWhiteSpace(customer.ServiceTypeIdCsv)
                    ? new List<int>()
                    : customer.ServiceTypeIdCsv.Split(',').Select(int.Parse).ToList();
            }

            return customers;
        }



        public async Task<IEnumerable<ServicesDto>> GetServicesAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            // ✅ Step 2: SQL Query (converted from VB code)
            string query = @"
        SELECT 
            cmm_ID AS ServiceId,
            cmm_Desc AS  Service
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompanyId
          AND cmm_Category = 'AT'
          AND cmm_Delflag = 'A'
        ORDER BY cmm_Desc ASC";

            // ✅ Step 3: Execute query with parameters
            return await connection.QueryAsync<ServicesDto>(query, new { CompanyId = companyId });
        }

        public async Task<IEnumerable<OrganizationDto>> GetOrganizationsAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
        SELECT 
            cmm_ID AS OrgId ,
            cmm_Desc AS OrgName
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompanyId
          AND cmm_Category = 'ORG'
          AND cmm_Delflag = 'A'
        ORDER BY cmm_Desc ASC";

            return await connection.QueryAsync<OrganizationDto>(query, new { CompanyId = companyId });
        }

        public async Task<IEnumerable<IndustryTypeDto>> GetIndustryTypesAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
        SELECT 
            cmm_ID AS IndustryId,
            cmm_Desc AS IndustryName
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompanyId
          AND cmm_Category = 'IND'
          AND cmm_Delflag = 'A'
        ORDER BY cmm_Desc ASC";

            return await connection.QueryAsync<IndustryTypeDto>(query, new { CompanyId = companyId });
        }

        public async Task<IEnumerable<ManagementTypeDto>> GetManagementTypesAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
        SELECT 
            cmm_ID AS ManagementId,
            cmm_Desc AS ManagementName
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompanyId
          AND cmm_Category = 'MNG'
          AND cmm_Delflag = 'A'
        ORDER BY cmm_Desc ASC";

            return await connection.QueryAsync<ManagementTypeDto>(query, new { CompanyId = companyId });
        }

        public async Task<string> SaveCustomerMasterAsync(CreateCustomerMasterDto dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            // 🔹 Duplicate Check for Insert and Update
            if (!string.IsNullOrEmpty(dto.CustomerCode) || !string.IsNullOrEmpty(dto.CompanyEmail) || !string.IsNullOrEmpty(dto.CINNO))
            {
                var duplicateQuery = @"
SELECT 
    STUFF(
        CASE WHEN EXISTS (SELECT 1 FROM SAD_CUSTOMER_MASTER WHERE CUST_CODE = @CustomerCode AND (@CustomerId IS NULL OR CUST_ID <> @CustomerId)) THEN ',Customer Code' ELSE '' END +
        CASE WHEN EXISTS (SELECT 1 FROM SAD_CUSTOMER_MASTER WHERE CUST_EMAIL = @CompanyEmail AND (@CustomerId IS NULL OR CUST_ID <> @CustomerId)) THEN ',Company Email' ELSE '' END +
        CASE WHEN EXISTS (SELECT 1 FROM SAD_CUSTOMER_MASTER WHERE CUSt_BranchId = @CINNO AND (@CustomerId IS NULL OR CUST_ID <> @CustomerId)) THEN ',CIN No' ELSE '' END
    , 1, 1, '') AS DuplicateFields
";

                var duplicateFields = await connection.QueryFirstOrDefaultAsync<string>(duplicateQuery, new
                {
                    CustomerCode = dto.CustomerCode,
                    CompanyEmail = dto.CompanyEmail,
                    CINNO = dto.CINNO,
                    CustomerId = dto.CustomerId
                });

                if (!string.IsNullOrEmpty(duplicateFields))
                {
                    var fields = duplicateFields.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    string message;

                    if (fields.Length == 1)
                        message = $"{fields[0]} already exists.";
                    else if (fields.Length == 2)
                        message = $"{fields[0]} and {fields[1]} already exist.";
                    else
                        message = string.Join(", ", fields.Take(fields.Length - 1)) + ", and " + fields.Last() + " already exist.";

                    return message;
                }
            }

            var parameters = new DynamicParameters();

            // Mandatory fields (from DTO)
            parameters.Add("@CUST_ID", dto.CustomerId ?? 0);
            parameters.Add("@CUST_NAME", dto.CustomerName);
            parameters.Add("@CUST_CODE", dto.CustomerCode);
            parameters.Add("@CUST_WEBSITE", dto.CompanyUrl ?? string.Empty);
            parameters.Add("@CUST_EMAIL", dto.CompanyEmail ?? string.Empty);
            parameters.Add("@CUST_GROUPNAME", dto.GroupName ?? string.Empty);
            parameters.Add("@CUST_GROUPINDIVIDUAL", dto.GroupIndividual);
            parameters.Add("@CUST_ORGTYPEID", dto.OrganizationTypeId);
            parameters.Add("@CUST_INDTYPEID", dto.IndustryTypeId);
            parameters.Add("@CUST_MGMTTYPEID", dto.ManagementTypeId);
            parameters.Add("@CUST_CommitmentDate", dto.CommitmentDate);

            // Remaining fields defaulted
            parameters.Add("@CUSt_BranchId", dto.CINNO);
            parameters.Add("@CUST_COMM_ADDRESS", "");
            parameters.Add("@CUST_COMM_CITY", "");
            parameters.Add("@CUST_COMM_PIN", "");
            parameters.Add("@CUST_COMM_STATE", "");
            parameters.Add("@CUST_COMM_COUNTRY", "");
            parameters.Add("@CUST_COMM_FAX", "");
            parameters.Add("@CUST_COMM_TEL", "");
            parameters.Add("@CUST_COMM_Email", "");
            parameters.Add("@CUST_ADDRESS", "");
            parameters.Add("@CUST_CITY", "");
            parameters.Add("@CUST_PIN", "");
            parameters.Add("@CUST_STATE", "");
            parameters.Add("@CUST_COUNTRY", "");
            parameters.Add("@CUST_FAX", "");
            parameters.Add("@CUST_TELPHONE", "");
            parameters.Add("@CUST_ConEmailID", "");
            parameters.Add("@CUST_LOCATIONID", "");
            parameters.Add("@CUST_TASKS", string.Join(",", dto.ServiceTypeId));

            parameters.Add("@CUST_ORGID", 0);
            parameters.Add("@CUST_CRBY", dto.CreatedBy);
            parameters.Add("@CUST_UpdatedBy", dto.CreatedBy);
            parameters.Add("@CUST_BOARDOFDIRECTORS", dto.BoardofDirectors);
            parameters.Add("@CUST_DEPMETHOD", 0);
            parameters.Add("@CUST_IPAddress", "127.0.0.1");
            parameters.Add("@CUST_CompID", dto.CompanyId);
            parameters.Add("@CUST_Amount_Type", 0);
            parameters.Add("@CUST_RoundOff", 0);
            parameters.Add("@Cust_DurtnId", 0);
            parameters.Add("@Cust_FY", dto.FinancialYearId);

            // Output params
            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("spSAD_CUSTOMER_MASTER", parameters, commandType: CommandType.StoredProcedure);

            int resultType = parameters.Get<int>("@iUpdateOrSave");

            return resultType == 2
                ? "Customer updated successfully"
                : "Customer created successfully";
        }


        //public async Task<string> SaveCustomerMasterAsync(CreateCustomerMasterDto dto)
        //{
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

        //    // 🔹 Duplicate Check (only for Insert, not Update)
        //    if (dto.CustomerId == null || dto.CustomerId == 0)
        //    {
        //        var duplicateCheck = await connection.ExecuteScalarAsync<int>(
        //            @"SELECT COUNT(1) 
        //      FROM SAD_CUSTOMER_MASTER 
        //      WHERE (CUST_NAME = @CustomerName OR CUST_CODE = @CustomerCode OR CUST_EMAIL = @CompanyEmail)",
        //            new { dto.CustomerName, dto.CustomerCode, dto.CompanyEmail });

        //        if (duplicateCheck > 0)
        //        {
        //            return "Customer with the same name, code, or email already exists.";
        //        }
        //    }

        //    var parameters = new DynamicParameters();

        //    // Mandatory fields (from DTO)
        //    parameters.Add("@CUST_ID", dto.CustomerId ?? 0);
        //    parameters.Add("@CUST_NAME", dto.CustomerName);
        //    parameters.Add("@CUST_CODE", dto.CustomerCode);
        //    parameters.Add("@CUST_WEBSITE", dto.CompanyUrl ?? string.Empty);
        //    parameters.Add("@CUST_EMAIL", dto.CompanyEmail ?? string.Empty);
        //    parameters.Add("@CUST_GROUPNAME", dto.GroupName ?? string.Empty);
        //    parameters.Add("@CUST_GROUPINDIVIDUAL", dto.GroupIndividual);
        //    parameters.Add("@CUST_ORGTYPEID", dto.OrganizationTypeId);
        //    parameters.Add("@CUST_INDTYPEID", dto.IndustryTypeId);
        //    parameters.Add("@CUST_MGMTTYPEID", dto.ManagementTypeId);
        //    parameters.Add("@CUST_CommitmentDate", dto.CommitmentDate);

        //    // Remaining fields defaulted
        //    parameters.Add("@CUSt_BranchId", dto.CINNO);
        //    parameters.Add("@CUST_COMM_ADDRESS", "");
        //    parameters.Add("@CUST_COMM_CITY", "");
        //    parameters.Add("@CUST_COMM_PIN", "");
        //    parameters.Add("@CUST_COMM_STATE", "");
        //    parameters.Add("@CUST_COMM_COUNTRY", "");
        //    parameters.Add("@CUST_COMM_FAX", "");
        //    parameters.Add("@CUST_COMM_TEL", "");
        //    parameters.Add("@CUST_COMM_Email", "");
        //    parameters.Add("@CUST_ADDRESS", "");
        //    parameters.Add("@CUST_CITY", "");
        //    parameters.Add("@CUST_PIN", "");
        //    parameters.Add("@CUST_STATE", "");
        //    parameters.Add("@CUST_COUNTRY", "");
        //    parameters.Add("@CUST_FAX", "");
        //    parameters.Add("@CUST_TELPHONE", "");
        //    parameters.Add("@CUST_ConEmailID", "");
        //    parameters.Add("@CUST_LOCATIONID", "");
        //    parameters.Add("@CUST_TASKS", string.Join(",", dto.ServiceTypeId));

        //    parameters.Add("@CUST_ORGID", 0);
        //    parameters.Add("@CUST_CRBY", dto.CreatedBy);
        //    parameters.Add("@CUST_UpdatedBy", dto.CreatedBy);
        //    parameters.Add("@CUST_BOARDOFDIRECTORS", dto.BoardofDirectors);
        //    parameters.Add("@CUST_DEPMETHOD", 0);
        //    parameters.Add("@CUST_IPAddress", "127.0.0.1");
        //    parameters.Add("@CUST_CompID", dto.CompanyId);
        //    parameters.Add("@CUST_Amount_Type", 0);
        //    parameters.Add("@CUST_RoundOff", 0);
        //    parameters.Add("@Cust_DurtnId", 0);
        //    parameters.Add("@Cust_FY", dto.FinancialYearId);

        //    // Output params
        //    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //    await connection.ExecuteAsync("spSAD_CUSTOMER_MASTER", parameters, commandType: CommandType.StoredProcedure);

        //    int resultType = parameters.Get<int>("@iUpdateOrSave");
        //    int customerId = parameters.Get<int>("@iOper");

        //    return resultType == 2
        //        ? "Customer updated successfully"
        //        : "Customer created successfully";
        //}

        //public async Task<string> SaveCustomerMasterAsync(CreateCustomerMasterDto dto)
        //{
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

        //    var parameters = new DynamicParameters();

        //    // Mandatory fields (from DTO)
        //    parameters.Add("@CUST_ID", dto.CustomerId ?? 0);
        //    parameters.Add("@CUST_NAME", dto.CustomerName);
        //    parameters.Add("@CUST_CODE", dto.CustomerCode);
        //    parameters.Add("@CUST_WEBSITE", dto.CompanyUrl ?? string.Empty);
        //    parameters.Add("@CUST_EMAIL", dto.CompanyEmail ?? string.Empty);
        //    parameters.Add("@CUST_GROUPNAME", dto.GroupName ?? string.Empty);
        //    parameters.Add("@CUST_GROUPINDIVIDUAL", dto.GroupIndividual);
        //    parameters.Add("@CUST_ORGTYPEID", dto.OrganizationTypeId);
        //    parameters.Add("@CUST_INDTYPEID", dto.IndustryTypeId);
        //    parameters.Add("@CUST_MGMTTYPEID", dto.ManagementTypeId);
        //    parameters.Add("@CUST_CommitmentDate", dto.CommitmentDate);

        //    // Remaining fields defaulted
        //    parameters.Add("@CUSt_BranchId", dto.CINNO);
        //    parameters.Add("@CUST_COMM_ADDRESS", "");
        //    parameters.Add("@CUST_COMM_CITY", "");
        //    parameters.Add("@CUST_COMM_PIN", "");
        //    parameters.Add("@CUST_COMM_STATE", "");
        //    parameters.Add("@CUST_COMM_COUNTRY", "");
        //    parameters.Add("@CUST_COMM_FAX", "");
        //    parameters.Add("@CUST_COMM_TEL", "");
        //    parameters.Add("@CUST_COMM_Email", "");
        //    parameters.Add("@CUST_ADDRESS", "");
        //    parameters.Add("@CUST_CITY", "");
        //    parameters.Add("@CUST_PIN", "");
        //    parameters.Add("@CUST_STATE", "");
        //    parameters.Add("@CUST_COUNTRY", "");
        //    parameters.Add("@CUST_FAX", "");
        //    parameters.Add("@CUST_TELPHONE", "");
        //    parameters.Add("@CUST_ConEmailID", "");
        //    parameters.Add("@CUST_LOCATIONID", "");
        //    // parameters.Add("@CUST_TASKS", dto.ServiceTypeId);
        //    parameters.Add("@CUST_TASKS", string.Join(",", dto.ServiceTypeId));

        //    parameters.Add("@CUST_ORGID", 0);
        //    parameters.Add("@CUST_CRBY", dto.CreatedBy);
        //    parameters.Add("@CUST_UpdatedBy", dto.CreatedBy);
        //    parameters.Add("@CUST_BOARDOFDIRECTORS", dto.BoardofDirectors);
        //    parameters.Add("@CUST_DEPMETHOD", 0);
        //    parameters.Add("@CUST_IPAddress", "127.0.0.1");
        //    parameters.Add("@CUST_CompID", dto.CompanyId);
        //    parameters.Add("@CUST_Amount_Type", 0);
        //    parameters.Add("@CUST_RoundOff", 0);
        //    parameters.Add("@Cust_DurtnId", 0);
        //    parameters.Add("@Cust_FY", dto.FinancialYearId);

        //    // Output params
        //    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //    await connection.ExecuteAsync("spSAD_CUSTOMER_MASTER", parameters, commandType: CommandType.StoredProcedure);

        //    int resultType = parameters.Get<int>("@iUpdateOrSave");
        //    int customerId = parameters.Get<int>("@iOper");

        //    return resultType == 2
        //        ? "Customer updated successfully"
        //        : "Customer created successfully";
        //}

        public async Task<(bool IsSuccess, string Message)> ToggleCustomerStatusAsync(int custId)
        {
            try
            {
                // ✅ Step 1: Get DB name from session
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
                if (string.IsNullOrEmpty(dbName))
                    return (false, "CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Open connection
                await using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
                await connection.OpenAsync();

                // ✅ Step 3: Toggle status
                const string query = @"
UPDATE SAD_CUSTOMER_MASTER
SET CUST_Delflg = 
    CASE 
        WHEN CUST_Delflg = 'W' THEN 'A'  -- Waiting → Activate
        WHEN CUST_Delflg = 'A' THEN 'D'  -- Activate → Deactivate
        WHEN CUST_Delflg = 'D' THEN 'A'  -- Deactivate → Activate
        ELSE CUST_Delflg                 -- No change
    END
WHERE CUST_ID = @CustId";

                // ✅ Pass correct param name
                var rowsAffected = await connection.ExecuteAsync(query, new { CustId = custId });

                if (rowsAffected > 0)
                    return (true, "Customer status updated successfully");

                return (false, "Customer not found");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating customer status: {ex.Message}");
            }
        }
    }
}


    
