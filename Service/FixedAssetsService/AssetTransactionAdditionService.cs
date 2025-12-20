using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
//using TracePca.Dto;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetTransactionAdditionDto;

namespace TracePca.Service.FixedAssetsService
{
    public class AssetTransactionAdditionService: AssetTransactionAdditionInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public AssetTransactionAdditionService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //LoadCustomer
        public async Task<IEnumerable<CustDto>> LoadCustomerAsync(int CompId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT 
                CUST_ID AS Cust_Id,
                CUST_NAME AS Cust_Name
            FROM SAD_CUSTOMER_MASTER
            WHERE CUST_DELFLG <> 'D' 
              AND CUST_CompID = @CompId";

            return await connection.QueryAsync<CustDto>(query, new { CompId });
        }
        
        //LoadStatus
        public async Task<IEnumerable<StatusDto>> LoadStatusAsync(int compId, string Name)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT 
                CUST_STATUS AS Status
            FROM SAD_CUSTOMER_MASTER
            WHERE CUST_CompID = @CompId
            AND CUST_NAME = @Name";

            return await connection.QueryAsync<StatusDto>(query, new { CompId = compId, Name = Name });

        }

        //FinancialYear
        public async Task<IEnumerable<YearDto>> GetYearsAsync(int compId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
             SELECT 
                 YMS_YEARID,
               YMS_ID
               FROM YEAR_MASTER
               WHERE 
               YMS_FROMDATE < DATEADD(YEAR, 1, GETDATE())
               AND YMS_CompId = @CompId
               ORDER BY LEFT(YMS_ID, 4) DESC";
            return await connection.QueryAsync<YearDto>(query, new { CompId = compId });
        }
    }
}
