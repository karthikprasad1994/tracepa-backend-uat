using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleMastersDto;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleMastersService : ScheduleMastersInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;


        public ScheduleMastersService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }
        //GetCustomersName
        public async Task<IEnumerable<CustDto>> GetCustomerNameAsync(int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Cust_Id,
            Cust_Name 
        FROM SAD_CUSTOMER_MASTER
        WHERE cust_Compid = @CompID";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustDto>(query, new { CompID = CompId });
        }

        //GetDuration
        public async Task<int?> GetCustomerDurationIdAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var query = "SELECT Cust_DurtnId FROM SAD_CUSTOMER_MASTER WHERE CUST_CompID = @CompId AND CUST_ID = @CustId";

            var parameters = new { CompId = compId, CustId = custId };
            var result = await connection.QueryFirstOrDefaultAsync<int?>(query, parameters);

            return result;
        }

        //GetFinancialYear
        public async Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            YMS_YEARID AS YearId,
            YMS_ID AS Id
        FROM YEAR_MASTER 
        WHERE YMS_FROMDATE < DATEADD(year, +1, GETDATE()) 
          AND YMS_CompId = @CompID 
        ORDER BY YMS_ID DESC";

            await connection.OpenAsync();

            return await connection.QueryAsync<FinancialYearDto>(query, new { CompID = CompId });
        }

        //GetBranchName
        public async Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int CompId, int CustId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustBranchDto>(query, new { CompId, CustId });
        }

        //GetScheduleHeading
        public async Task<IEnumerable<ScheduleHeadingDto>> GetScheduleHeadingAsync(int CompId, int CustId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT DISTINCT 
            b.ASH_Name AS ASH_Name, 
            b.ASH_ID AS ASH_ID
        FROM ACC_ScheduleTemplates a
        LEFT JOIN ACC_ScheduleHeading b 
            ON b.ASH_ID = a.AST_HeadingID AND b.ASH_STATUS <> 'D'
        WHERE a.AST_CompId = @compId
            AND a.AST_Companytype = @custId
            AND a.AST_Schedule_type = @scheduleTypeId
            AND b.ASH_Name IS NOT NULL 
            AND b.ASH_ID IS NOT NULL";

            await connection.OpenAsync();

            return await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleSubHeading
        public async Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleSubHeadingAsync(int CompId, int CustId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT DISTINCT 
            b.AsSH_Name AS ASSH_Name,
            b.AsSH_ID AS ASSH_ID
        FROM ACC_ScheduleTemplates a
        LEFT JOIN ACC_ScheduleSubHeading b 
            ON b.AsSH_ID = a.AST_SubHeadingID AND b.AsSH_STATUS <> 'D'
        WHERE a.AST_CompId = @compId
          AND a.AST_Companytype = @custId
          AND a.AST_Schedule_type = @scheduleTypeId
          AND b.AsSH_Name IS NOT NULL 
          AND b.AsSH_ID IS NOT NULL";

            await connection.OpenAsync();

            return await connection.QueryAsync<ScheduleSubHeadingDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleItem
        public async Task<IEnumerable<ScheduleItemDto>> GetScheduleItemAsync(int CompId, int CustId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT DISTINCT 
            b.ASI_ID AS ASI_ID,
            b.ASI_Name AS ASI_Name
        FROM ACC_ScheduleTemplates a
        LEFT JOIN ACC_ScheduleItems b 
            ON b.ASI_ID = a.AST_ItemID AND b.ASI_STATUS <> 'D'
        WHERE a.AST_CompId = @compId
            AND a.AST_Companytype = @custId
            AND a.AST_Schedule_type = @scheduleTypeId
            AND b.ASI_Name IS NOT NULL 
            AND b.ASI_ID IS NOT NULL";

            await connection.OpenAsync();

            return await connection.QueryAsync<ScheduleItemDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleSubItem
        public async Task<IEnumerable<ScheduleSubItemDto>> GetScheduleSubItemAsync(int CompId, int CustId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT DISTINCT 
            b.ASSI_ID AS ASSI_ID,
            b.ASSI_Name AS ASSI_Name
        FROM ACC_ScheduleTemplates a
        LEFT JOIN ACC_ScheduleSubItems b 
            ON b.ASSI_ID = a.AST_SubItemID AND b.ASSI_STATUS <> 'D'
        WHERE a.AST_CompId = @compId
            AND a.AST_Companytype = @custId
            AND a.AST_Schedule_type = @scheduleTypeId
            AND b.ASSI_Name IS NOT NULL 
            AND b.ASSI_ID IS NOT NULL";

            await connection.OpenAsync();
            return await connection.QueryAsync<ScheduleSubItemDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetCustomerOrgType
        public async Task<string> GetCustomerOrgTypeAsync(int CustId, int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT ISNULL(cmm_Desc, '') 
        FROM SAD_CUSTOMER_MASTER
        LEFT JOIN Content_Management_Master 
            ON Content_Management_Master.cmm_id = SAD_CUSTOMER_MASTER.CUST_ORGTYPEID
        WHERE SAD_CUSTOMER_MASTER.CUST_ID = @CustId AND SAD_CUSTOMER_MASTER.CUST_CompID = @CompId";

            var result = await connection.QueryFirstOrDefaultAsync<string>(query, new { CustId, CompId });

            return result;
        }

    }
}
