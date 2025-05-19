using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface;
using TracePca.Interface.Audit;
using TracePca.Interface.FIN_Statement;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
using static TracePca.Service.FIN_statement.ScheduleMappingService;
namespace TracePca.Service.FIN_statement
{
    public class ScheduleMappingService : ScheduleMappingInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        public ScheduleMappingService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        //GetCustomersName
        public async Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Cust_Id,
            Cust_Name 
        FROM SAD_CUSTOMER_MASTER
        WHERE cust_Compid = @CompID";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustDto>(query, new { CompID = icompId });
        }

        //GetFinancialYear
        public async Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            YMS_YEARID,
            YMS_ID 
        FROM YEAR_MASTER 
        WHERE YMS_FROMDATE < DATEADD(year, +1, GETDATE()) 
          AND YMS_CompId = @CompID 
        ORDER BY YMS_ID DESC";

            await connection.OpenAsync();

            return await connection.QueryAsync<FinancialYearDto>(query, new { CompID = icompId });
        }

        //GetDuration
        public async Task<IEnumerable<CustDurationDto>> GetDurationAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            ISNULL(Cust_DurtnId, 0) AS Cust_DurtnId  
        FROM SAD_CUSTOMER_MASTER 
        WHERE Cust_CompID = @compId AND cust_id = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustDurationDto>(query, new { compId, custId });
        }

        //GetBranchName
        public async Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustBranchDto>(query, new { compId, custId });
        }

        // Heading
        public async Task<List<ScheduleHeadingDto>> GetScheduleHeadingsAsync(int compId, int custId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = @"
            SELECT DISTINCT b.ASH_Name as ASH_Name , b.ASH_ID as ASH_ID
            FROM ACC_ScheduleTemplates a
            LEFT JOIN ACC_ScheduleHeading b ON b.ASH_ID = a.AST_HeadingID AND b.ASH_STATUS <> 'D'
            WHERE a.AST_CompId = @compId
              AND a.AST_Companytype = @custId
              AND a.AST_Schedule_type = @ScheduleTypeId
              AND b.ASH_Name IS NOT NULL AND b.ASH_ID IS NOT NULL";
            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompID = compId, custId = custId, ScheduleTypeId = ScheduleTypeId });
            return result.ToList();
        }

        //Sub Heading
        public async Task<List<ScheduleHeadingDto>> GetSchedulesubHeadingsAsync(int compId, int custId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = @"
            select distinct(b.AsSH_Name),ASsH_ID from ACC_ScheduleTemplates 
                    left join ACC_ScheduleSubHeading b on b.AsSH_ID = AST_SubHeadingID and ASsH_STATUS<>'D'
            WHERE a.AST_CompId = @compId
              AND a.AST_Companytype = @custId
              AND a.AST_Schedule_type = @ScheduleTypeId
              AND b.ASSH_Name IS NOT NULL AND b.ASSH_ID IS NOT NULL";
            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompID = compId, custId = custId, ScheduleTypeId = ScheduleTypeId });
            return result.ToList();
        }

        //Item
        public async Task<List<ScheduleHeadingDto>> GetScheduleItemAsync(int compId, int custId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = @"
        select distinct(b.ASI_Name),ASI_ID from ACC_ScheduleTemplates 
                    left join ACC_ScheduleItems b on b.ASI_ID = AST_ItemID and ASI_STATUS<>'D'
            WHERE a.AST_CompId = @compId
              AND a.AST_Companytype = @custId
              AND a.AST_Schedule_type = @ScheduleTypeId
              AND b.ASI_Name IS NOT NULL AND b.ASI_ID IS NOT NULL";
            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompID = compId, custId = custId, ScheduleTypeId = ScheduleTypeId });
            return result.ToList();
        }

        //SubItem
        public async Task<List<ScheduleHeadingDto>> GetScheduleSubItemAsync(int compId, int custId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = @"
            select distinct(b.ASSI_Name),ASSI_ID from ACC_ScheduleTemplates 
                    left join ACC_ScheduleSubItems b on b.ASSI_ID = AST_SubItemID and ASSI_STATUS<>'D'
            WHERE a.AST_CompId = @compId
              AND a.AST_Companytype = @custId
              AND a.AST_Schedule_type = @ScheduleTypeId
              AND b.ASSI_Name IS NOT NULL AND b.ASSI_ID IS NOT NULL";
            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompID = compId, custId = custId, ScheduleTypeId = ScheduleTypeId });
            return result.ToList();
        }
        // Save mapping 



        public async Task<int> SaveTrailBalanceExcelUploadAsync(string sAC, TrailBalanceUploadDto dto, int userId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var builder = new SqlConnectionStringBuilder(connectionString);
            sAC = builder.InitialCatalog;

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add input parameters
                command.Parameters.AddWithValue("@ATBU_ID", (object?)dto.ATBU_ID ?? DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                command.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                command.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                command.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                command.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                command.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                command.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                command.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                command.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                command.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                command.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                command.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                command.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                // Output parameters
                var paramUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var paramOper = new SqlParameter("@iOper", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(paramUpdateOrSave);
                command.Parameters.Add(paramOper);

                await command.ExecuteNonQueryAsync();
                transaction.Commit();




                return (int)(paramOper.Value ?? 0);



            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> SaveTrailBalanceDetailAsync(string dbName, TrailBalanceUploadDetailDto dto, int userId)
        {
            int result = 0;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = dbName
            };

            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add parameters
            command.Parameters.AddWithValue("@ATBUD_ID", (object?)dto.ATBUD_ID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ATBUD_Masid", dto.ATBUD_Masid);
            command.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? "");
            command.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? "");
            command.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
            command.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
            command.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
            command.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
            command.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
            command.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
            command.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
            command.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
            command.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
            command.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "N");
            command.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
            command.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
            command.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "A");
            command.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "");
            command.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? "127.0.0.1");
            command.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
            command.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

            // Output parameters
            var paramUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };
            var paramOper = new SqlParameter("@iOper", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(paramUpdateOrSave);
            command.Parameters.Add(paramOper);

            await command.ExecuteNonQueryAsync();
            result = Convert.ToInt32(paramOper.Value);

            return result;
        }
    }
}


