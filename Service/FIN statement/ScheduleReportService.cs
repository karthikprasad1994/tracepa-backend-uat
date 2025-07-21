using Dapper;
using System.Globalization;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleReportDto;
using Microsoft.AspNetCore.SignalR;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleReportService : ScheduleReportInterface
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

        ////GetSummaryReportForPandL(Income)
        //public async Task<IEnumerable<SummaryPnLRowIncome>> GetSummaryPnLIncomeAsync(int CompId, SummaryPnLIncome p)
        //{
        //    var results = new List<SummaryPnLRowIncome>();
        //    int inAmt = p.InAmt == 0 ? 1 : p.InAmt;

        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection4"));
        //    await connection.OpenAsync();

        //    // 🔹 Fetch main headings
        //    var headingSql = @"
        //SELECT AST_HeadingID, ASH_Name as HeadingName
        //FROM ACC_ScheduleTemplates
        //LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
        //WHERE AST_Schedule_type = @ScheduleTypeID
        //  AND AST_Companytype = @CustID
        //  AND a.ASH_Notes = 1
        //GROUP BY AST_HeadingID, ASH_Name
        //ORDER BY AST_HeadingID";

        //    var headings = await connection.QueryAsync<(int HeadingID, string HeadingName)>(headingSql, new
        //    {
        //        ScheduleTypeID = p.ScheduleTypeID,
        //        CustID = p.CustID
        //    });

        //    foreach (var heading in headings)
        //    {
        //        var balanceSql = @"
        //    SELECT
        //        ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
        //        ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
        //        ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
        //        ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
        //    FROM Acc_TrailBalance_Upload_Details ud
        //    LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
        //        AND d.ATBU_YEARId = @YearID
        //        AND d.ATBU_CustId = @CustID
        //        AND d.ATBU_Branchid = ud.Atbud_Branchnameid
        //    LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
        //        AND e.ATBU_YEARId = @PrevYearID
        //        AND e.ATBU_CustId = @CustID
        //        AND e.ATBU_Branchid = ud.Atbud_Branchnameid
        //    WHERE ud.ATBUD_Schedule_type = @ScheduleTypeID
        //      AND ud.ATBUD_compid = @ACID
        //      AND ud.ATBUD_CustId = @CustID
        //      AND ud.ATBUD_Headingid = @HeadingID";

        //        var balance = await connection.QueryFirstOrDefaultAsync(balanceSql, new
        //        {
        //            YearID = p.YearID,
        //            PrevYearID = p.YearID - 1,
        //            CustID = p.CustID,
        //            ScheduleTypeID = p.ScheduleTypeID,
        //            ACID = p.ACID,
        //            HeadingID = heading.HeadingID
        //        });

        //        decimal crTotal = Convert.ToDecimal(balance?.CrTotal ?? 0);
        //        decimal dbTotal = Convert.ToDecimal(balance?.DbTotal ?? 0);
        //        decimal prevCrTotal = Convert.ToDecimal(balance?.PrevCrTotal ?? 0);
        //        decimal prevDbTotal = Convert.ToDecimal(balance?.PrevDbTotal ?? 0);

        //        decimal netTotal = Math.Abs(crTotal - dbTotal) / inAmt;
        //        decimal prevNetTotal = Math.Abs(prevCrTotal - prevDbTotal) / inAmt;

        //        results.Add(new SummaryPnLRowIncome
        //        {
        //            SrNo = (results.Count + 1).ToString(),
        //            HeadingName = $"<b>{heading.HeadingName}</b>",
        //            HeaderSLNo = netTotal == 0 ? "-" : netTotal.ToString($"N{p.RoundOff}"),
        //            PrevYearTotal = prevNetTotal == 0 ? "-" : prevNetTotal.ToString($"N{p.RoundOff}")
        //        });

        //        var subHeadingSql = @"
        //    SELECT AST_SubHeadingID, ASsH_Name AS SubHeadingName, ASSH_Notes AS Notes
        //    FROM ACC_ScheduleTemplates
        //    LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
        //    WHERE AST_Schedule_type = @ScheduleTypeID
        //      AND AST_Companytype = @CustID
        //      AND AST_AccHeadId = 1
        //      AND AST_HeadingID = @HeadingID
        //    GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";

        //        var subHeadings = await connection.QueryAsync<(int SubHeadingID, string SubHeadingName, int Notes)>(
        //            subHeadingSql,
        //            new { ScheduleTypeID = p.ScheduleTypeID, CustID = p.CustID, HeadingID = heading.HeadingID });

        //        foreach (var sub in subHeadings)
        //        {
        //            var subBalanceSql = @"
        //        SELECT
        //            ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
        //            ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
        //            ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
        //            ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
        //        FROM Acc_TrailBalance_Upload_Details ud
        //        LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
        //            AND d.ATBU_YEARId = @YearID
        //            AND d.ATBU_CustId = @CustID
        //            AND d.ATBU_Branchid = ud.Atbud_Branchnameid
        //        LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
        //            AND e.ATBU_YEARId = @PrevYearID
        //            AND e.ATBU_CustId = @CustID
        //            AND e.ATBU_Branchid = ud.Atbud_Branchnameid
        //        WHERE ud.ATBUD_Schedule_type = @ScheduleTypeID
        //          AND ud.ATBUD_CustId = @CustID
        //          AND ud.ATBUD_Subheading = @SubHeadingID";

        //            var subBalance = await connection.QueryFirstOrDefaultAsync(subBalanceSql, new
        //            {
        //                YearID = p.YearID,
        //                PrevYearID = p.YearID - 1,
        //                CustID = p.CustID,
        //                ScheduleTypeID = p.ScheduleTypeID,
        //                SubHeadingID = sub.SubHeadingID
        //            });

        //            decimal subCr = Convert.ToDecimal(subBalance?.CrTotal ?? 0);
        //            decimal subDb = Convert.ToDecimal(subBalance?.DbTotal ?? 0);
        //            decimal subPrevCr = Convert.ToDecimal(subBalance?.PrevCrTotal ?? 0);
        //            decimal subPrevDb = Convert.ToDecimal(subBalance?.PrevDbTotal ?? 0);

        //            decimal subNet = Math.Abs(subCr - subDb);
        //            decimal subPrevNet = Math.Abs(subPrevCr - subPrevDb);

        //            results.Add(new SummaryPnLRowIncome
        //            {
        //                SrNo = (results.Count + 1).ToString(),
        //                SubHeadingName = sub.SubHeadingName,
        //                HeaderSLNo = subNet == 0 ? "-" : subNet.ToString($"N{p.RoundOff}"),
        //                PrevYearTotal = subPrevNet == 0 ? "-" : subPrevNet.ToString($"N{p.RoundOff}"),
        //                Notes = sub.Notes != 0 ? sub.Notes.ToString() : ""
        //            });
        //        }
        //    }
        //    return results;
        //}

        ////GetSummaryReportForPandL(Expenses)
        //public async Task<IEnumerable<SummaryPnLRowExpenses>> GetSummaryPnLExpensesAsync(int CompId, SummaryPnLExpenses dto)
        //{
        //    var results = new List<SummaryPnLRowExpenses>();
        //    int inAmt = dto.InAmt == 0 ? 1 : dto.InAmt;

        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection4"));
        //    await connection.OpenAsync();

        //    var expenseHeadingSql = @"
        //SELECT AST_HeadingID, ASH_Name as HeadingName
        //FROM ACC_ScheduleTemplates
        //LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
        //WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 2
        //GROUP BY AST_HeadingID, ASH_Name
        //ORDER BY AST_HeadingID";

        //    var expenseHeadings = await connection.QueryAsync<(int HeadingID, string HeadingName)>(expenseHeadingSql, new
        //    {
        //        ScheduleTypeID = dto.ScheduleTypeID,
        //        CustID = dto.CustID
        //    });

        //    foreach (var heading in expenseHeadings)
        //    {
        //        var expenseBalanceSql = @"
        //    SELECT
        //        ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
        //        ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
        //        ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
        //        ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
        //    FROM Acc_TrailBalance_Upload_Details ud
        //    LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
        //        AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
        //    LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
        //        AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
        //    WHERE ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @ACID AND ud.ATBUD_CustId = @CustID
        //      AND ud.ATBUD_Headingid = @HeadingID";

        //        var balance = await connection.QueryFirstOrDefaultAsync(expenseBalanceSql, new
        //        {
        //            YearID = dto.YearID,
        //            PrevYearID = dto.YearID - 1,
        //            CustID = dto.CustID,
        //            ScheduleTypeID = dto.ScheduleTypeID,
        //            ACID = dto.ACID,
        //            HeadingID = heading.HeadingID
        //        });

        //        decimal net = (balance?.DbTotal ?? 0) - (balance?.CrTotal ?? 0);
        //        decimal prevNet = (balance?.PrevDbTotal ?? 0) - (balance?.PrevCrTotal ?? 0);

        //        results.Add(new SummaryPnLRowExpenses
        //        {
        //            SrNo = (results.Count + 1).ToString(),
        //            HeadingName = $"{heading.HeadingName}",
        //            HeaderSLNo = net == 0 ? "-" : net.ToString($"N{dto.RoundOff}"),
        //            PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{dto.RoundOff}")
        //        });

        //        var subHeadingSql = @"
        //    SELECT AST_SubHeadingID, ASsH_Name AS SubHeadingName, ASSH_Notes AS Notes
        //    FROM ACC_ScheduleTemplates
        //    LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
        //    WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID
        //      AND AST_AccHeadId = 2 AND AST_HeadingID = @HeadingID
        //    GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";

        //        var subHeadings = await connection.QueryAsync<(int SubHeadingID, string SubHeadingName, int Notes)>(
        //            subHeadingSql,
        //            new { ScheduleTypeID = dto.ScheduleTypeID, CustID = dto.CustID, HeadingID = heading.HeadingID }
        //        );

        //        foreach (var sub in subHeadings)
        //        {
        //            var subBalanceSql = @"
        //        SELECT
        //            ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
        //            ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
        //            ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
        //            ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
        //        FROM Acc_TrailBalance_Upload_Details ud
        //        LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
        //            AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
        //        LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
        //            AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
        //        WHERE ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_CustId = @CustID
        //          AND ud.ATBUD_Subheading = @SubHeadingID";

        //            var subBalance = await connection.QueryFirstOrDefaultAsync(subBalanceSql, new
        //            {
        //                YearID = dto.YearID,
        //                PrevYearID = dto.YearID - 1,
        //                CustID = dto.CustID,
        //                ScheduleTypeID = dto.ScheduleTypeID,
        //                SubHeadingID = sub.SubHeadingID
        //            });

        //            decimal subNet = (subBalance?.DbTotal ?? 0) - (subBalance?.CrTotal ?? 0);
        //            decimal subPrevNet = (subBalance?.PrevDbTotal ?? 0) - (subBalance?.PrevCrTotal ?? 0);

        //            results.Add(new SummaryPnLRowExpenses
        //            {
        //                SrNo = (results.Count + 1).ToString(),
        //                SubHeadingName = sub.SubHeadingName,
        //                HeaderSLNo = subNet == 0 ? "-" : subNet.ToString($"N{dto.RoundOff}"),
        //                PrevYearTotal = subPrevNet == 0 ? "-" : subPrevNet.ToString($"N{dto.RoundOff}"),
        //                Notes = sub.Notes != 0 ? sub.Notes.ToString() : ""
        //            });
        //        }
        //    }
        //    return results;
        //}

        //GetSummaryReportForPandL
        public async Task<IEnumerable<SummaryReportPnLRow>> GetReportSummaryPnLAsync(int CompId, SummaryReportPnL p)
        {
            var results = new List<SummaryReportPnLRow>();
            int ScheduleTypeID = 3;
            int RoundOff = 1;

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection4"));
            await connection.OpenAsync();

            decimal totalIncome = 0, totalPrevIncome = 0;
            decimal totalExpense = 0, totalPrevExpense = 0;

             //🟩 INCOME HEADINGS
                        var incomeHeadingSql = @"
            SELECT AST_HeadingID AS HeadingID, ASH_Name AS Name
            FROM ACC_ScheduleTemplates
            LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
            WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 1
            GROUP BY AST_HeadingID, ASH_Name
            ORDER BY AST_HeadingID";

            var incomeHeadings = await connection.QueryAsync<(int HeadingID, string Name)>(incomeHeadingSql, new { ScheduleTypeID, CustID = p.CustID });

            foreach (var heading in incomeHeadings)
            {
                var headingBalanceSql = @"
SELECT
    ud.ATBUD_Headingid AS HeadingID,
    h.ASH_Name AS Name,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
    h.ASH_Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 1
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
    AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
    AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
WHERE
    ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes
ORDER BY ud.ATBUD_Headingid";

                var headingBalance = await connection.QueryFirstOrDefaultAsync(headingBalanceSql, new
                {
                    YearID = p.YearID,
                    PrevYearID = p.YearID - 1,
                    CustID = p.CustID,
                    ScheduleTypeID,
                    HeadingID = heading.HeadingID,
                    CompID = CompId
                });

                decimal net = 0;
                decimal prevNet = 0;

                results.Add(new SummaryReportPnLRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = heading.Name,
                    HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
                    PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}")
                });

                var incomeSubSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 1 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";

                var incomeSubHeadings = await connection.QueryAsync<(int SubHeadingID, string Name, int Notes)>(incomeSubSql, new
                {
                    ScheduleTypeID,
                    CustID = p.CustID,
                    HeadingID = heading.HeadingID
                });

                foreach (var sub in incomeSubHeadings)
                {
                    var subBalSql = @"
SELECT
    ud.ATBUD_Subheading AS SubHeadingID,
    sh.ASSH_Name AS Name,
    sh.ASSH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubHeading sh ON sh.ASSH_ID = ud.ATBUD_Subheading
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
WHERE ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_CustId = @CustID
GROUP BY ud.ATBUD_Subheading, sh.ASSH_Name, sh.ASSH_Notes
ORDER BY ud.ATBUD_Subheading";

                    var subBalance = await connection.QueryFirstOrDefaultAsync(subBalSql, new
                    {
                        YearID = p.YearID,
                        PrevYearID = p.YearID - 1,
                        CustID = p.CustID,
                        ScheduleTypeID,
                        SubHeadingID = sub.SubHeadingID
                    });

                    decimal subNet = (subBalance?.CrTotal ?? 0) - (subBalance?.DbTotal ?? 0);
                    decimal subPrevNet = (subBalance?.PrevCrTotal ?? 0) - (subBalance?.PrevDbTotal ?? 0);

                    if (subNet != 0 && subPrevNet != 0)
                    {
                        totalIncome += subNet;
                        totalPrevIncome += subPrevNet;

                        results.Add(new SummaryReportPnLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Name = sub.Name,
                            HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                            PrevYearTotal = subPrevNet.ToString($"N{RoundOff}"),
                            Notes = sub.Notes != 0 ? sub.Notes.ToString() : ""
                        });
                    }
                }

                results.Add(new SummaryReportPnLRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = "III Total Income",
                    HeaderSLNo = totalIncome == 0 ? "-" : totalIncome.ToString($"N{RoundOff}"),
                    PrevYearTotal = totalPrevIncome == 0 ? "-" : totalPrevIncome.ToString($"N{RoundOff}")
                });
            }

            // 🟥 EXPENSE HEADINGS
            var expenseHeadingSql = @"
SELECT AST_HeadingID AS HeadingID, ASH_Name AS HeadingName
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 2
GROUP BY AST_HeadingID, ASH_Name
ORDER BY AST_HeadingID";

            var expenseHeadings = await connection.QueryAsync<(int HeadingID, string Name)>(expenseHeadingSql, new { ScheduleTypeID, CustID = p.CustID });

            foreach (var heading in expenseHeadings)
            {
                var expenseBalanceSql = @"
SELECT
    ud.ATBUD_Headingid AS HeadingID,
    h.ASH_Name AS Name,
    h.ASH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 2
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
WHERE
    ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes
ORDER BY ud.ATBUD_Headingid";

                var headingBalance = await connection.QueryFirstOrDefaultAsync(expenseBalanceSql, new
                {
                    YearID = p.YearID,
                    PrevYearID = p.YearID - 1,
                    CustID = p.CustID,
                    ScheduleTypeID,
                    HeadingID = heading.HeadingID,
                    CompID = CompId
                });
                
                decimal net = 0;
                decimal prevNet = 0;
                if (net != 0 || prevNet != 0)
                {
                    results.Add(new SummaryReportPnLRow
                    {
                        SrNo = (results.Count + 1).ToString(),
                        Name = heading.Name,
                        HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
                        PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}")
                    });
                }
                else
                {
                    decimal fallback = totalIncome - totalExpense;
                    decimal fallbackPrev = totalPrevIncome - totalPrevExpense;

                    results.Add(new SummaryReportPnLRow
                    {
                        SrNo = (results.Count + 1).ToString(),
                        Name = heading.Name,
                        HeaderSLNo = fallback == 0 ? "-" : fallback.ToString($"N{RoundOff}"),
                        PrevYearTotal = fallbackPrev == 0 ? "-" : fallbackPrev.ToString($"N{RoundOff}")
                    });
                }
                results.Add(new SummaryReportPnLRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = heading.Name,
                    HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
                    PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}")
                });

                // 🟥 EXPENSE SUBHEADINGS
                var subSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS SubHeadingName, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 2 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";

                var subHeadings = await connection.QueryAsync<(int SubHeadingID, string Name, int Notes)>(subSql, new
                {
                    ScheduleTypeID,
                    CustID = p.CustID,
                    HeadingID = heading.HeadingID
                });

                foreach (var sub in subHeadings)
                {
                    var subBalSql = @"
SELECT
    ud.ATBUD_Subheading AS SubHeadingID,
    ssh.ASSH_Name AS Name,
    ssh.ASSH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubHeading ssh ON ssh.ASSH_ID = ud.ATBUD_Subheading
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
WHERE ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_CustId = @CustID
GROUP BY ud.ATBUD_Subheading, ssh.ASSH_Name, ssh.ASSH_Notes
ORDER BY ud.ATBUD_Subheading";

                    var subBalance = await connection.QueryFirstOrDefaultAsync(subBalSql, new
                    {
                        YearID = p.YearID,
                        PrevYearID = p.YearID - 1,
                        CustID = p.CustID,
                        ScheduleTypeID,
                        SubHeadingID = sub.SubHeadingID
                    });

                    decimal subNet = (subBalance?.DbTotal ?? 0) - (subBalance?.CrTotal ?? 0);
                    decimal subPrevNet = (subBalance?.PrevDbTotal ?? 0) - (subBalance?.PrevCrTotal ?? 0);

                    if (subNet != 0 && subPrevNet != 0)
                    {
                        totalExpense += subNet;
                        totalPrevExpense += subPrevNet;

                        results.Add(new SummaryReportPnLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Name = sub.Name,
                            HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                            PrevYearTotal = subPrevNet.ToString($"N{RoundOff}"),
                            Notes = sub.Notes != 0 ? sub.Notes.ToString() : ""
                        });
                    }
                }
                results.Add(new SummaryReportPnLRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = "Total Expenses",
                    HeaderSLNo = totalExpense == 0 ? "-" : totalExpense.ToString($"N{RoundOff}"),
                    PrevYearTotal = totalPrevExpense == 0 ? "-" : totalPrevExpense.ToString($"N{RoundOff}")
                });

            } 
            return results;
        }

        //GetSummaryReportForBalanceSheet
        public async Task<IEnumerable<SummaryReportBalanceSheetRow>> GetReportSummaryBalanceSheetAsync(int CompId, SummaryReportBalanceSheet p)
        {
            var results = new List<SummaryReportBalanceSheetRow>();
            int ScheduleTypeID = 4;
            int RoundOff = 1;


            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection4"));
            await connection.OpenAsync();

            decimal totalIncome = 0, totalPrevIncome = 0;
            decimal totalExpense = 0, totalPrevExpense = 0;

            // 🟩 INCOME HEADINGS
            var incomeHeadingSql = @"
                SELECT AST_HeadingID AS HeadingID, ASH_Name AS Name
                FROM ACC_ScheduleTemplates
                LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
                WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 1
                GROUP BY AST_HeadingID, ASH_Name
                ORDER BY AST_HeadingID";

            var incomeHeadings = await connection.QueryAsync<(int HeadingID, string Name)>(incomeHeadingSql, new { ScheduleTypeID, CustID = p.CustID });

            foreach (var heading in incomeHeadings)
            {
                var headingBalanceSql = @"
                SELECT
                    ud.ATBUD_Headingid AS HeadingID,
                    h.ASH_Name AS Name,
                    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
                    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
                    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
                    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
                    h.ASH_Notes
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 1
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
                    AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
                    AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
                WHERE
                    ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
                    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
                GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes
                ORDER BY ud.ATBUD_Headingid";

                var headingBalance = await connection.QueryFirstOrDefaultAsync(headingBalanceSql, new
                {
                    YearID = p.YearID,
                    PrevYearID = p.YearID - 1,
                    CustID = p.CustID,
                    ScheduleTypeID,
                    HeadingID = heading.HeadingID,
                    CompID = CompId
                });

                decimal net = 0;
                decimal prevNet = 0;

                results.Add(new SummaryReportBalanceSheetRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = heading.Name,
                    HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
                    PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}")
                });

                var incomeSubSql = @"
                SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS SubHeadingName, ASSH_Notes AS Notes
                FROM ACC_ScheduleTemplates
                LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
                WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 1 AND AST_HeadingID = @HeadingID
                GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";

                var incomeSubHeadings = await connection.QueryAsync<(int SubHeadingID, string Name, int Notes)>(incomeSubSql, new
                {
                    ScheduleTypeID,
                    CustID = p.CustID,
                    HeadingID = heading.HeadingID
                });

                foreach (var sub in incomeSubHeadings)
                {
                    var subBalSql = @"
                SELECT
                    ud.ATBUD_Subheading AS SubHeadingID,
                    sh.ASSH_Name AS Name,
                    sh.ASSH_Notes AS Notes,
                    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
                    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
                    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
                    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN ACC_ScheduleSubHeading sh ON sh.ASSH_ID = ud.ATBUD_Subheading
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
                WHERE ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_CustId = @CustID
                GROUP BY ud.ATBUD_Subheading, sh.ASSH_Name, sh.ASSH_Notes
                ORDER BY ud.ATBUD_Subheading";

                    var subBalance = await connection.QueryFirstOrDefaultAsync(subBalSql, new
                    {
                        YearID = p.YearID,
                        PrevYearID = p.YearID - 1,
                        CustID = p.CustID,
                        ScheduleTypeID,
                        SubHeadingID = sub.SubHeadingID
                    });

                    decimal subNet = (subBalance?.CrTotal ?? 0) - (subBalance?.DbTotal ?? 0);
                    decimal subPrevNet = (subBalance?.PrevCrTotal ?? 0) - (subBalance?.PrevDbTotal ?? 0);

                    totalIncome += subNet;
                    totalPrevIncome += subPrevNet;

                    results.Add(new SummaryReportBalanceSheetRow
                    {
                        SrNo = (results.Count + 1).ToString(),
                        Name = sub.Name,
                        HeaderSLNo = subNet == 0 ? "-" : subNet.ToString($"N{RoundOff}"),
                        PrevYearTotal = subPrevNet == 0 ? "-" : subPrevNet.ToString($"N{RoundOff}"),
                        Notes = sub.Notes != 0 ? sub.Notes.ToString() : ""
                    });
                }
                results.Add(new SummaryReportBalanceSheetRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = "III Total Income",
                    HeaderSLNo = totalIncome == 0 ? "-" : totalIncome.ToString($"N{RoundOff}"),
                    PrevYearTotal = totalPrevIncome == 0 ? "-" : totalPrevIncome.ToString($"N{RoundOff}")
                });
            }

            // 🟥 EXPENSE HEADINGS
            var expenseHeadingSql = @"
                SELECT AST_HeadingID AS HeadingID, ASH_Name AS Name
                FROM ACC_ScheduleTemplates
                LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
                WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 2
                GROUP BY AST_HeadingID, ASH_Name
                ORDER BY AST_HeadingID";

            var expenseHeadings = await connection.QueryAsync<(int HeadingID, string Name)>(expenseHeadingSql, new { ScheduleTypeID, CustID = p.CustID });

            foreach (var heading in expenseHeadings)
            {
                var expenseBalanceSql = @"
                SELECT
                    ud.ATBUD_Headingid AS HeadingID,
                    h.ASH_Name AS Name,
                    h.ASH_Notes AS Notes,
                    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
                    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
                    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
                    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 2
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
                WHERE
                    ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
                    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
                GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes
                ORDER BY ud.ATBUD_Headingid";

                var headingBalance = await connection.QueryFirstOrDefaultAsync(expenseBalanceSql, new
                {
                    YearID = p.YearID,
                    PrevYearID = p.YearID - 1,
                    CustID = p.CustID,
                    ScheduleTypeID,
                    HeadingID = heading.HeadingID,
                    CompID = CompId
                });

                decimal net = 0;
                decimal prevNet = 0;

                results.Add(new SummaryReportBalanceSheetRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = heading.Name,
                    HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
                    PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}")
                });

                var subSql = @"
                SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
                FROM ACC_ScheduleTemplates
                LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
                WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 2 AND AST_HeadingID = @HeadingID
                GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";

                var subHeadings = await connection.QueryAsync<(int SubHeadingID, string Name, int Notes)>(subSql, new
                {
                    ScheduleTypeID,
                    CustID = p.CustID,
                    HeadingID = heading.HeadingID
                });

                foreach (var sub in subHeadings)
                {
                    var subBalSql = @"
                SELECT
                    ud.ATBUD_Subheading AS SubHeadingID,
                    ssh.ASSH_Name AS Name,
                    ssh.ASSH_Notes AS Notes,
                    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
                    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
                    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
                    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN ACC_ScheduleSubHeading ssh ON ssh.ASSH_ID = ud.ATBUD_Subheading
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
                WHERE ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_CustId = @CustID
                GROUP BY ud.ATBUD_Subheading, ssh.ASSH_Name, ssh.ASSH_Notes
                ORDER BY ud.ATBUD_Subheading";

                    var subBalance = await connection.QueryFirstOrDefaultAsync(subBalSql, new
                    {
                        YearID = p.YearID,
                        PrevYearID = p.YearID - 1,
                        CustID = p.CustID,
                        ScheduleTypeID,
                        SubHeadingID = sub.SubHeadingID
                    });

                    decimal subNet = (subBalance?.DbTotal ?? 0) - (subBalance?.CrTotal ?? 0);
                    decimal subPrevNet = (subBalance?.PrevDbTotal ?? 0) - (subBalance?.PrevCrTotal ?? 0);

                    totalExpense += subNet;
                    totalPrevExpense += subPrevNet;

                    results.Add(new SummaryReportBalanceSheetRow
                    {
                        SrNo = (results.Count + 1).ToString(),
                        Name = sub.Name,
                        HeaderSLNo = subNet == 0 ? "-" : subNet.ToString($"N{RoundOff}"),
                        PrevYearTotal = subPrevNet == 0 ? "-" : subPrevNet.ToString($"N{RoundOff}"),
                        Notes = sub.Notes != 0 ? sub.Notes.ToString() : ""
                    });
                }
                results.Add(new SummaryReportBalanceSheetRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = "Total Expenses",
                    HeaderSLNo = totalExpense == 0 ? "-" : totalExpense.ToString($"N{RoundOff}"),
                    PrevYearTotal = totalPrevExpense == 0 ? "-" : totalPrevExpense.ToString($"N{RoundOff}")
                });
            }
            return results;
        }

        //GetDetailedReportPandL
        public async Task<IEnumerable<DetailedReportRow>> GetDetailedReportAsync(DetailedReportParams p)
        {
            var results = new List<DetailedReportRow>();
            int inAmt = p.InAmt == 0 ? 1 : p.InAmt;

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection4"));
            await connection.OpenAsync();

            // 🟩 INCOME HEADINGS
            var incomeSql = @"
            SELECT AST_HeadingID, ASH_Name
            FROM ACC_ScheduleTemplates
            LEFT JOIN ACC_ScheduleHeading ON ASH_ID = AST_HeadingID
            WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND ASH_Notes = 1
            GROUP BY AST_HeadingID, ASH_Name
            ORDER BY AST_HeadingID";

            var incomeHeadings = await connection.QueryAsync<(int HeadingId, string HeadingName)>(incomeSql, new
            {
                ScheduleTypeID = p.ScheduleTypeID,
                CustID = p.CustID
            });

            foreach (var heading in incomeHeadings)
            {
                results.Add(new DetailedReportRow
                {
                    Status = "Heading",
                    Name = heading.HeadingName
                });

                var subSql = @"
                SELECT AST_SubHeadingID, ASSH_Name
                FROM ACC_ScheduleTemplates
                LEFT JOIN ACC_ScheduleSubHeading ON ASSH_ID = AST_SubHeadingID
                WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID
                      AND AST_HeadingID = @HeadingId AND AST_AccHeadId = 1
                GROUP BY AST_SubHeadingID, ASSH_Name";

                var subHeadings = await connection.QueryAsync<(int SubHeadingId, string SubHeadingName)>(
                    subSql, new
                    {
                        ScheduleTypeID = p.ScheduleTypeID,
                        CustID = p.CustID,
                        HeadingId = heading.HeadingId
                    });

                foreach (var sub in subHeadings)
                {
                    results.Add(new DetailedReportRow
                    {
                        Status = "SubHeading",
                        Name = sub.SubHeadingName
                    });
                }
            }

            // 🟥 EXPENSE HEADINGS
            var expenseSql = @"
            SELECT AST_HeadingID, ASH_Name
            FROM ACC_ScheduleTemplates
            LEFT JOIN ACC_ScheduleHeading ON ASH_ID = AST_HeadingID
            WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND ASH_Notes = 2
            GROUP BY AST_HeadingID, ASH_Name
            ORDER BY AST_HeadingID";

            var expenseHeadings = await connection.QueryAsync<(int HeadingId, string HeadingName)>(expenseSql, new
            {
                ScheduleTypeID = p.ScheduleTypeID,
                CustID = p.CustID
            });

            foreach (var heading in expenseHeadings)
            {
                results.Add(new DetailedReportRow
                {
                    Status = "Heading",
                    Name = heading.HeadingName
                });

                var subSql = @"
                SELECT AST_SubHeadingID, ASSH_Name
                FROM ACC_ScheduleTemplates
                LEFT JOIN ACC_ScheduleSubHeading ON ASSH_ID = AST_SubHeadingID
                WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID
                      AND AST_HeadingID = @HeadingId AND AST_AccHeadId = 2
                GROUP BY AST_SubHeadingID, ASSH_Name";

                var subHeadings = await connection.QueryAsync<(int SubHeadingId, string SubHeadingName)>(
                    subSql, new
                    {
                        ScheduleTypeID = p.ScheduleTypeID,
                        CustID = p.CustID,
                        HeadingId = heading.HeadingId
                    });
                foreach (var sub in subHeadings)
                {
                    results.Add(new DetailedReportRow
                    {
                        Status = "SubHeading",
                        Name = sub.SubHeadingName
                    });
                }
            }
            return results;
        }
    }
}




