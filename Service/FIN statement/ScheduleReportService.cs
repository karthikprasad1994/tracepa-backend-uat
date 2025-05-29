using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;
using static TracePca.Dto.FIN_Statement.ScheduleReportDto;
using static TracePca.Service.FIN_statement.ScheduleReportService;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleReportService: ScheduleReportInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _db;

        public ScheduleReportService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
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

        //GetCompanyName
        public async Task<IEnumerable<CompanyDetailsDto>> GetCompanyNameAsync(int iCompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Company_ID, 
            Company_Name 
        FROM TRACe_CompanyDetails 
        WHERE Company_CompID = @CompID 
        ORDER BY Company_Name";

            await connection.OpenAsync();

            return await connection.QueryAsync<CompanyDetailsDto>(query, new { CompID = iCompId });
        }

        //GetPartners
        public async Task<IEnumerable<PartnersDto>> LoadCustomerPartnersAsync(int compId, int detailsId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query;

            if (detailsId != 0)
            {
                query = @"
            SELECT 
                usr_Id, 
                usr_FullName AS Fullname, 
                usr_PhoneNo, 
                org_name
            FROM Sad_UserDetails
            LEFT JOIN sad_org_structure a ON a.org_node = usr_OrgnId
            WHERE usr_partner = 1 
              AND Usr_CompId = @compId 
              AND usr_Id = @detailsId";
            }
            else
            {
                query = @"
            SELECT 
                usr_Id, 
                usr_Code + '-' + usr_FullName AS Fullname
            FROM Sad_UserDetails
            WHERE usr_partner = 1 
              AND Usr_CompId = @compId";
            }

            await connection.OpenAsync();

            return await connection.QueryAsync<PartnersDto>(query, new { compId, detailsId });
        }

        //GetSubHeading
        public async Task<IEnumerable<SubHeadingDto>> GetSubHeadingAsync(
        int iCompId, int iScheduleId, int iCustId, int iHeadingId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            ASSH_ID AS SubheadingID, 
            ASSH_Name AS SubheadingName, 
            ISNULL(ASSH_Notes, 0) AS Notes
        FROM ACC_ScheduleSubHeading
        WHERE 
            ASSH_scheduletype = @ScheduleId AND
            ASSh_Orgtype = @CustId AND 
            ASSH_HeadingID = @HeadingId
        ORDER BY ASSH_ID";

            await connection.OpenAsync();

            var result = await connection.QueryAsync<SubHeadingDto>(query, new
            {
                CustId = iCustId,
                ScheduleId = iScheduleId,
                HeadingId = iHeadingId
            });

            return result;
        }

        //GetItem
        public async Task<IEnumerable<ItemDto>> GetItemAsync(
        int iCompId, int iScheduleId, int iCustId, int iHeadingId, int iSubHeadId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            ASI_ID AS ItemsId, 
            ASI_Name AS ItemsName
        FROM ACC_ScheduleItems
        WHERE 
            ASI_Orgtype = @CustId AND 
            ASI_scheduletype = @ScheduleId AND 
            ASI_SubHeadingID = @SubHeadId
        ORDER BY ASI_ID";

            await connection.OpenAsync();

            var result = await connection.QueryAsync<ItemDto>(query, new
            {
                CustId = iCustId,
                ScheduleId = iScheduleId,
                SubHeadId = iSubHeadId
            });
            return result;
        }

        //GetDateFormat
        public async Task<string> GetDateFormatSelectionAsync(int companyId, string configKey)
        {
            const string query = @"
        SELECT SAD_Config_Value 
        FROM sad_config_settings 
        WHERE SAD_Config_Key = @Key AND SAD_CompID = @CompanyId";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var configValue = await connection.ExecuteScalarAsync<string>(query, new
            {
                Key = configKey,
                CompanyId = companyId
            });

            return configValue switch
            {
                "1" => "F",
                "2" => "D",
                "3" => "Q",
                "4" => "A",
                "5" => "US",
                "6" => "B",
                "7" => "C",
                "8" => "E",
                _ => string.Empty
            };
        }

        //LoadButton
        public async Task<IEnumerable<ReportDto>> GenerateReportAsync(int reportType, int scheduleTypeId, int accountId, int customerId, int yearId)
        {
            var sql = GetQueryByReportType(reportType, scheduleTypeId);

            var parameters = new
            {
                ScheduleTypeId = scheduleTypeId,
                AccountId = accountId,
                CustomerId = customerId,
                CurrentYearId = yearId,
                PreviousYearId = yearId - 1
            };

            return await _db.QueryAsync<ReportDto>(sql, parameters);
        }

        private string GetQueryByReportType(int reportType, int scheduleTypeId)
        {
            if (reportType == 1 && scheduleTypeId == 3) return SummaryPLQuery();
            if (reportType == 1 && scheduleTypeId == 4) return SummaryBalanceSheetQuery();
            if (reportType == 2 && scheduleTypeId == 3) return DetailedPLQuery();
            if (reportType == 2 && scheduleTypeId == 4) return DetailedBalanceSheetQuery();

            throw new InvalidOperationException("Invalid report or schedule type.");
        }


        private string SummaryPLQuery() => @"
SELECT DISTINCT ATBUD_Headingid,
       ASH_Name AS HeadingName,
       FORMAT(SUM(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 'N2') AS HeaderSLNo,
       NULL AS SubHeaderSLNo,
       CAST(a.ASH_Notes AS VARCHAR) AS Notes,
       FORMAT(SUM(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 'N2') AS PrevYearTotal,
       '1' AS SrNo
FROM Acc_TrailBalance_Upload_Details bud
LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = bud.ATBUD_Headingid AND a.ASH_Notes = 1
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = bud.ATBUD_Description
    AND d.ATBU_YEARId = @CurrentYearId AND d.ATBU_CustId = @CustomerId AND bud.ATBUD_YEARId = @CurrentYearId
    AND d.ATBU_Branchid = bud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = bud.ATBUD_Description
    AND e.ATBU_YEARId = @PreviousYearId AND e.ATBU_CustId = @CustomerId AND bud.ATBUD_YEARId = @PreviousYearId
    AND e.ATBU_Branchid = bud.Atbud_Branchnameid
WHERE bud.ATBUD_Schedule_type = @ScheduleTypeId
  AND bud.ATBUD_compid = @AccountId
  AND bud.ATBUD_CustId = @CustomerId
  AND EXISTS (
      SELECT 1 FROM ACC_ScheduleTemplates s
      WHERE s.AST_HeadingID = bud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2)
  )
GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes";

        private string SummaryBalanceSheetQuery() => @"
SELECT AST_HeadingID AS SrNo,
       ASH_Name AS HeadingName,
       NULL AS HeaderSLNo,
       NULL AS SubHeaderSLNo,
       NULL AS Notes,
       NULL AS PrevYearTotal
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
WHERE AST_Schedule_type = @ScheduleTypeId
  AND AST_Companytype = @CustomerId
  AND a.ASH_Notes = 2
GROUP BY AST_HeadingID, ASH_Name
ORDER BY AST_HeadingID";

        private string DetailedPLQuery() => @"
SELECT DISTINCT ATBUD_Headingid,
       ASH_Name AS HeadingName,
       FORMAT(SUM(d.ATBU_Closing_TotalCredit_Amount), 'N2') AS HeaderSLNo,
       NULL AS SubHeaderSLNo,
       CAST(a.ASH_Notes AS VARCHAR) AS Notes,
       '-' AS PrevYearTotal,
       '1' AS SrNo
FROM Acc_TrailBalance_Upload_Details bud
LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = bud.ATBUD_Headingid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = bud.ATBUD_Description
WHERE bud.ATBUD_Schedule_type = @ScheduleTypeId
  AND bud.ATBUD_compid = @AccountId
  AND bud.ATBUD_CustId = @CustomerId
  AND EXISTS (
      SELECT 1 FROM ACC_ScheduleTemplates s
      WHERE s.AST_HeadingID = bud.ATBUD_Headingid AND s.AST_AccHeadId = 1
  )
GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes";

        private string DetailedBalanceSheetQuery() => @"
SELECT DISTINCT ATBUD_Headingid,
       ASH_Name AS HeadingName,
       FORMAT(SUM(d.ATBU_Closing_TotalDebit_Amount), 'N2') AS HeaderSLNo,
       NULL AS SubHeaderSLNo,
       CAST(a.ASH_Notes AS VARCHAR) AS Notes,
       '-' AS PrevYearTotal,
       '1' AS SrNo
FROM Acc_TrailBalance_Upload_Details bud
LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = bud.ATBUD_Headingid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = bud.ATBUD_Description
WHERE bud.ATBUD_Schedule_type = @ScheduleTypeId
  AND bud.ATBUD_compid = @AccountId
  AND bud.ATBUD_CustId = @CustomerId
  AND EXISTS (
      SELECT 1 FROM ACC_ScheduleTemplates s
      WHERE s.AST_HeadingID = bud.ATBUD_Headingid AND s.AST_AccHeadId = 2
  )
GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes";
    }
}


