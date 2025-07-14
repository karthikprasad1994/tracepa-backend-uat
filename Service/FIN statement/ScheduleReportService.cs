using Dapper;
using System.Globalization;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleReportDto;

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

        //GetCompanyName
        public async Task<IEnumerable<CompanyDetailsDto>> GetCompanyNameAsync(int CompId)
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

            return await connection.QueryAsync<CompanyDetailsDto>(query, new { CompID = CompId });
        }

        //GetPartners
        public async Task<IEnumerable<PartnersDto>> LoadCustomerPartnersAsync(int CompId, int DetailsId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query;

            if (DetailsId != 0)
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

            return await connection.QueryAsync<PartnersDto>(query, new { CompId, DetailsId });
        }

        //GetSubHeading
        public async Task<IEnumerable<SubHeadingDto>> GetSubHeadingAsync(
        int CompId, int ScheduleId, int CustId, int HeadingId)
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
                CustId = CustId,
                ScheduleId = ScheduleId,
                HeadingId = HeadingId
            });

            return result;
        }

        //GetItem
        public async Task<IEnumerable<ItemDto>> GetItemAsync(
        int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId)
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
                CustId = CustId,
                ScheduleId = ScheduleId,
                SubHeadId = SubHeadId
            });
            return result;
        }

        //GetDateFormat
        public async Task<string> GetDateFormatSelectionAsync(int CompanyId, string ConfigKey)
        {
            const string query = @"
        SELECT SAD_Config_Value 
        FROM sad_config_settings 
        WHERE SAD_Config_Key = @Key AND SAD_CompID = @CompanyId";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var configValue = await connection.ExecuteScalarAsync<string>(query, new
            {
                Key = ConfigKey,
                CompanyId = CompanyId
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

        //LoadButtonForSummaryReportPandL
        //public async Task<List<BalanceSheetRow>> GetBalanceSheetAsync(int iACID, int yearID, int custID, int scheduleTypeID, int repType, int chkPt, int inAmt, string selectedBranches, int roundOff, string selectedSHeading, string selectedItems, int stat)
        //{
        //    var rows = new List<BalanceSheetRow>();
        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    await conn.OpenAsync();

        //    double totalDebit = 0, prevTotalDebit = 0;
        //    double dPandLamt = 0, dPrevPandLamt = 0;

        //    int orgTypeId = await conn.QueryFirstOrDefaultAsync<int>("SELECT OrgTypeId FROM OrgTable WHERE CompanyID = @iACID AND CustID = @custID", new { iACID, custID });

        //    foreach (var noteType in new[] { 2, 1 })
        //    {
        //        if (noteType == 2 && (repType != 1 || scheduleTypeID != 4))
        //        {
        //            rows.Add(new BalanceSheetRow { SrNo = "", HeadingName = orgTypeId == 28 ? "<B>EQUITY AND LIABILITIES</B>" : "<B>PARTNERS FUND AND LIABILITIES</B>" });
        //        }
        //        else if (noteType == 1)
        //        {
        //            rows.Add(new BalanceSheetRow { SrNo = "", HeadingName = "<B>ASSETS</B>" });
        //        }

        //        var headings = await conn.QueryAsync<dynamic>(@"
        //        SELECT AST_HeadingID, ASH_Name AS HeadingName FROM ACC_ScheduleTemplates
        //        LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
        //        WHERE AST_Schedule_type = @scheduleTypeID AND AST_Companytype = @custID AND a.ASH_Notes = @noteType
        //        GROUP BY AST_HeadingID, ASH_Name
        //        ORDER BY AST_HeadingID",
        //            new { scheduleTypeID, custID, noteType });

        //        int srIndex = 1;
        //        foreach (var heading in headings)
        //        {
        //            string headingName = heading.HeadingName;
        //            int headingId = (int)heading.AST_HeadingID;

        //            double crTotal = await GetAmountAsync(conn, headingId, yearID, iACID, custID, selectedBranches, "CrTotal", scheduleTypeID);
        //            double dbTotal = await GetAmountAsync(conn, headingId, yearID, iACID, custID, selectedBranches, "DbTotal", scheduleTypeID);
        //            double prevCrTotal = await GetAmountAsync(conn, headingId, yearID - 1, iACID, custID, selectedBranches, "CrTotal", scheduleTypeID);
        //            double prevDbTotal = await GetAmountAsync(conn, headingId, yearID - 1, iACID, custID, selectedBranches, "DbTotal", scheduleTypeID);

        //            double net = dbTotal - crTotal;
        //            double prevNet = prevDbTotal - prevCrTotal;

        //            totalDebit += net;
        //            prevTotalDebit += prevNet;

        //            rows.Add(new BalanceSheetRow
        //            {
        //                SrNo = srIndex.ToString(),
        //                HeadingName = $"<b>{headingName}</b>",
        //                HeaderSLNo = FormatAmount(net / inAmt, roundOff),
        //                PrevYearTotal = FormatAmount(prevNet / inAmt, roundOff)
        //            });

        //            if (custID == 28 && headingName?.Trim() == "(b) Reserves and surplus")
        //            {
        //                dPandLamt = await GetPandLFinalAmtAsync(conn, yearID, custID, scheduleTypeID, selectedBranches);
        //                dPrevPandLamt = await GetPandLFinalAmtAsync(conn, yearID - 1, custID, 3, selectedBranches);

        //                rows.Add(new BalanceSheetRow
        //                {
        //                    SrNo = "",
        //                    HeadingName = "&nbsp;&nbsp;&nbsp;&nbsp;(b) Reserves and surplus",
        //                    HeaderSLNo = FormatAmount(dPandLamt / inAmt, roundOff),
        //                    PrevYearTotal = FormatAmount(dPrevPandLamt / inAmt, roundOff),
        //                    Notes = "1"
        //                });

        //                totalDebit += dPandLamt;
        //                prevTotalDebit += dPrevPandLamt;
        //            }

        //            srIndex++;
        //        }
        //    }

        //    if (scheduleTypeID == 3)
        //    {
        //        rows.Add(new BalanceSheetRow
        //        {
        //            SrNo = "",
        //            HeadingName = "<B>Net Ordinary Income</B>",
        //            HeaderSLNo = FormatAmount(totalDebit / inAmt, roundOff),
        //            PrevYearTotal = FormatAmount(prevTotalDebit / inAmt, roundOff)
        //        });
        //    }

        //    rows.Add(new BalanceSheetRow
        //    {
        //        SrNo = "",
        //        HeadingName = "<B>Total</B>",
        //        HeaderSLNo = FormatAmount(totalDebit / inAmt, roundOff),
        //        PrevYearTotal = FormatAmount(prevTotalDebit / inAmt, roundOff)
        //    });

        //    return rows;
        //}

        //private string FormatAmount(double amount, int roundOff)
        //{
        //    if (amount == 0) return "-";
        //    return amount < 0
        //        ? $"({Math.Abs(amount).ToString($"N{roundOff}", CultureInfo.InvariantCulture)})"
        //        : amount.ToString($"N{roundOff}", CultureInfo.InvariantCulture);
        //}

        //private async Task<double> GetAmountAsync(SqlConnection conn, int headingId, int yearId, int compId, int custId, string branches, string columnType, int scheduleTypeId)
        //{
        //    string columnName = columnType == "CrTotal"
        //        ? "ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0)"
        //        : "ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0)";

        //    string sql = $@"
        //    SELECT {columnName}
        //    FROM Acc_TrailBalance_Upload_Details bud
        //    LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = bud.ATBUD_Description
        //        AND d.ATBU_YEARId = @YearID
        //        AND d.ATBU_CustId = @CustID
        //        AND d.ATBU_Branchid = bud.Atbud_Branchnameid
        //    WHERE bud.ATBUD_Schedule_type = @ScheduleTypeID
        //        AND bud.ATBUD_compid = @CompID
        //        AND bud.ATBUD_CustId = @CustID
        //        AND bud.ATBUD_Headingid = @HeadingID
        //        {(branches != "0" ? "AND d.Atbu_Branchid IN (" + branches + ")" : string.Empty)}";

        //    return await conn.ExecuteScalarAsync<double>(sql, new
        //    {
        //        YearID = yearId,
        //        CompID = compId,
        //        CustID = custId,
        //        ScheduleTypeID = scheduleTypeId,
        //        HeadingID = headingId
        //    });
        //}

        //private async Task<double> GetPandLFinalAmtAsync(SqlConnection conn, int yearId, int custId, int scheduleTypeId, string branches)
        //{
        //    string sql = $@"
        //    SELECT ISNULL(SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount), 0)
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_YEARId = @YearID
        //      AND ATBU_CustId = @CustID
        //      AND ATBU_Schedule_type = @ScheduleTypeID
        //      {(branches != "0" ? "AND ATBU_Branchid IN (" + branches + ")" : string.Empty)}";

        //    return await conn.ExecuteScalarAsync<double>(sql, new
        //    {
        //        YearID = yearId,
        //        CustID = custId,
        //        ScheduleTypeID = scheduleTypeId
        //    });
        //}
    }
}


