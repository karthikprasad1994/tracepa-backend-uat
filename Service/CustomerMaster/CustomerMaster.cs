using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Dto.CustomerMaster;
using TracePca.Interface;
using TracePca.Interface.EmployeeMaster;

namespace TracePca.Service.CustomerMaster
{
    public class CustomerMaster: CustomerMasterInterface
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
    CUST_EMAIL AS CustomerEmail,
    CUST_CODE AS CustomerCode,
   CUST_CommitmentDate AS CommitmentDate,
   CUST_INDTYPEID AS IndustrytypeId,
   CUST_ORGTYPEID AS OrganizationtypeId,
    CASE CUST_Delflg
        WHEN 'A' THEN 'Activated'
        WHEN 'D' THEN 'De-Activated'
        WHEN 'W' THEN 'Waiting for Approval'
        ELSE 'Unknown'
    END AS Status
FROM SAD_CUSTOMER_MASTER
WHERE CUST_CompID = @CompanyId
ORDER BY CUST_ID";


            return await connection.QueryAsync<CustomerDetailsDto>(query, new { CompanyId = companyId });
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
            // parameters.Add("@CUST_TASKS", dto.ServiceTypeId);
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
            int customerId = parameters.Get<int>("@iOper");

            return resultType == 2
                ? "Customer updated successfully"
                : "Customer created successfully";
        }


    }
}
