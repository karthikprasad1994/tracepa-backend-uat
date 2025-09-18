using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.LedgerMaterialityDto;
using static TracePca.Dto.FIN_Statement.ScheduleMastersDto;

namespace TracePca.Service.FIN_statement
{
    public class LedgerMaterialityService : LedgerMaterialityInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LedgerMaterialityService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetContentManagement
        public async Task<IEnumerable<ContentManagementDto>> GetContentManagementAsync(int CompId, string cmmCategory)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT
              cmm_ID,
              cmm_Desc
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompID
          AND cmm_Category = 'MT'
        ORDER by cmm_ID";

            return await connection.QueryAsync<ContentManagementDto>(
                query, new { CompID = CompId, Category = cmmCategory }
            );
        }

    }

}
