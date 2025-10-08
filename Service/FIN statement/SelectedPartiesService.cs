using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static TracePca.Dto.FIN_Statement.SchedulePartnerFundsDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

namespace TracePca.Service.FIN_statement
{
    public class SelectedPartiesService : SelectedPartiesInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SelectedPartiesService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetSelectedParties
        public async Task<IEnumerable<LoadTrailBalanceDto>> GetTrailBalanceAsync(int custId, int financialYearId, int branchId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            ATBU_Description, 
            ATBU_ID
        FROM Acc_TrailBalance_Upload
        WHERE ATBU_CustId = @CustId
          AND ATBU_YEARId = @FinancialYearId
          AND ATBU_Branchid = @BranchId
        ORDER BY ATBU_Id DESC";

            return await connection.QueryAsync<LoadTrailBalanceDto>(query, new
            {
                CustId = custId,
                FinancialYearId = financialYearId,
                BranchId = branchId
            });
        }

        //UpdateSelectedPartiesStatus
        public async Task<int> UpdateTrailBalanceStatusAsync(UpdateTrailBalanceStatusDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        UPDATE Acc_TrailBalance_Upload
        SET ATBU_STATUS = @Status
        WHERE ATBU_ID = @Id
          AND ATBU_CustId = @CustId
          AND ATBU_YEARId = @FinancialYearId
          AND ATBU_Branchid = @BranchId";

            await connection.ExecuteAsync(query, new
            {
                Id = dto. Id,
                Status = dto.Status,
                CustId = dto.CustId,
                FinancialYearId = dto.FinancialYearId,
                BranchId = dto.BranchId
            });

            return dto.Id;
        }

    }
}