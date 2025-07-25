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
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.IdentityModel.Tokens;
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
        public async Task<IEnumerable<CompanyDetailsDto>> GetCompanyNameAsync(string DBName, int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(DBName));

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
        public async Task<IEnumerable<PartnersDto>> LoadCustomerPartnersAsync(string DBName, int CompId, int DetailsId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(DBName));

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
              AND usr_CompanyId = @compId 
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
              AND usr_CompanyId = @compId";
            }

            await connection.OpenAsync();

            return await connection.QueryAsync<PartnersDto>(query, new { CompId, DetailsId });
        }

        //GetSubHeading
        public async Task<IEnumerable<SubHeadingDto>> GetSubHeadingAsync(string DBName, int CompId, int ScheduleId, int CustId, int HeadingId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(DBName));

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
        public async Task<IEnumerable<ItemDto>> GetItemAsync(string DBName, int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(DBName));

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

        //GetSummaryReportForPandL
        public async Task<IEnumerable<SummaryReportPnLRow>> GetReportSummaryPnLAsync(string DBName, int CompId, SummaryReportPnL p)
        {
            var results = new List<SummaryReportPnLRow>();
            int ScheduleTypeID = 3;
            int RoundOff = 1;

            using var connection = new SqlConnection(_configuration.GetConnectionString(DBName));
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
        public async Task<IEnumerable<SummaryReportBalanceSheetRow>> GetReportSummaryBalanceSheetAsync(string DBName, int CompId, SummaryReportBalanceSheet p)
        {
            var results = new List<SummaryReportBalanceSheetRow>();
            int ScheduleTypeID = 4;
            int RoundOff = 1;


            using var connection = new SqlConnection(_configuration.GetConnectionString(DBName));
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
        public async Task<IEnumerable<DetailedReportPandLRow>> GetDetailedReportPandLAsync(string DBName, int CompId, DetailedReportPandL p)
        {
            var results = new List<DetailedReportPandLRow>();
            int ScheduleTypeID = 3;
            int RoundOff = 1;
            int YearId = p.YearID;
            int CustId = p.CustID;
            int BranchId = p.BranchID;

            using var connection = new SqlConnection(_configuration.GetConnectionString(DBName));
            await connection.OpenAsync();

            decimal totalIncome = 0, totalPrevIncome = 0;
            decimal totalExpense = 0, totalPrevExpense = 0;

            // 1. Fetch Headings
            var headingSql = $@"
 SELECT DISTINCT ATBUD_Headingid AS HeadingID, ASH_Name AS HeadingName, ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal, ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal, a.ASH_Notes
 FROM Acc_TrailBalance_Upload_Details
 LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_Headingid
 LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description
 WHERE ATBUD_Schedule_type = @ScheduleTypeId AND ATBUD_compid = @CompanyId AND ATBUD_CustId = @CustomerId AND Atbud_Branchnameid = @BranchId AND EXISTS (
       SELECT 1 
       FROM ACC_ScheduleTemplates s 
       WHERE s.AST_HeadingID = ATBUD_Headingid AND s.AST_AccHeadId = 1)
 GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes
 ORDER BY ATBUD_Headingid";

            var headings = await connection.QueryAsync<DetailedReportPandLRow>(headingSql, new
            {
                ScheduleTypeId = ScheduleTypeID,
                CompanyId = CompId,
                CustomerId = CustId,
                BranchID = BranchId
            });

            //Description under heading
            foreach (var heading in headings)
            {
                var descSql = $@"
     SELECT DISTINCT b.ATBUD_Headingid AS HeadingID, b.ATBUD_ID, b.ATBUD_Description AS Name, a.ASH_ID, a.ASH_Name AS HeadingName, ISNULL(SUM(ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal, ISNULL(SUM(ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal
     FROM Acc_TrailBalance_Upload
     LEFT JOIN Acc_TrailBalance_Upload_Details b ON b.ATBUD_Description = ATBU_Description
     LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = b.ATBUD_Headingid
     WHERE b.ATBUD_Schedule_Type = @ScheduleTypeId AND b.ATBUD_compid = @CompanyId AND b.ATBUD_CustId = @CustomerId AND b.ATBUD_Headingid = @HeadingId AND b.ATBUD_YEARId = @YearId AND b.Atbud_Branchnameid = @BranchId AND ATBU_YEARId = @YearId 
     GROUP BY b.ATBUD_ID, b.ATBUD_Description, a.ASH_ID, a.ASH_Name, ATBU_Branchid, b.ATBUD_Headingid
     ORDER BY b.ATBUD_ID, b.ATBUD_Headingid";

                var descriptions = await connection.QueryAsync<DetailedReportPandLRow>(descSql, new
                {
                    YearId = YearId,
                    ScheduleTypeId = ScheduleTypeID,
                    CompanyId = CompId,
                    CustomerId = CustId,
                    HeadingId = heading.HeadingID,
                    BranchID = BranchId
                });

                decimal net = 0;
                decimal prevNet = 0;

                results.Add(new DetailedReportPandLRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Status = "1",
                    Name = heading.HeadingName,
                    HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
                    PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}"),
                    Notes = !string.IsNullOrEmpty(heading.Notes) ? heading.Notes.ToString() : ""
                });
               
                // 1. Fetch Income SubHeading
                var subHeadingSql = @"
                SELECT DISTINCT AST_SubHeadingID AS SubHeadingID, a.ASSH_Name AS SubHeadingName, a.ASSH_Notes AS Notes
                FROM ACC_ScheduleTemplates
                LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
                WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustomerId AND AST_AccHeadId = 1 AND AST_HeadingID = @HeadingID ";

                var subHeadings = (await connection.QueryAsync<DetailedReportPandLRow>(subHeadingSql, new
                {
                    ScheduleTypeId = ScheduleTypeID,
                    CustomerId = CustId,
                    HeadingId = heading.HeadingID
                })).ToList();

                 if (!subHeadings.Any())
                {
                    Console.WriteLine("No subheadings returned for HeadingID: " + heading.HeadingID);
                }

                // Step 2: Get SubHeading Totals
                foreach (var sub in subHeadings)
                {
                    var subHeadingTotalsSql = $@"
        SELECT 
            ATBUD_Subheading AS SubHeadingID, a.ASSH_Name AS SubHeadingName, a.ASSH_Notes AS Notes,
            ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
            ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
            e.ASHN_Description AS ASHN_Description,
            ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
            ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
            ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotalPrev,
            ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotalPrev
        FROM Acc_TrailBalance_Upload_Details
        LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
        LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = Atbud_Branchnameid
        LEFT JOIN ACC_SubHeadingNoteDesc e ON e.ASHN_SubHeadingId = a.ASSH_ID AND e.ASHN_CustomerId = @CustomerId AND e.ASHN_YearID = @YearId
        LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ATBUD_Description AND ATBUD_SubItemId = 0 AND ATBUD_itemid = 0 AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = Atbud_Branchnameid
        LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ATBUD_Description AND ATBUD_SubItemId = 0 AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = Atbud_Branchnameid
        WHERE ATBUD_Headingid = @HeadingID AND ATBUD_Subheading <> 0 AND ATBUD_Schedule_type = @ScheduleTypeID AND ATBUD_CustId = @CustomerId AND Atbud_Branchnameid = @BranchId
        GROUP BY ATBUD_Subheading, a.ASSH_Name, a.ASSH_Notes, e.ASHN_Description
        ORDER BY ATBUD_Subheading";

                    var subHeadingTotals = await connection.QueryAsync<DetailedReportPandLRow>(subHeadingTotalsSql, new
                    {
                        YearId,
                        PrevYearId = YearId - 1,
                        CustomerId = CustId,
                        ScheduleTypeID,
                        HeadingId = heading.HeadingID,
                        BranchID = BranchId
                    });

                    // Step 3: Descriptions under subheading
                    foreach (var subHeading in subHeadingTotals)
                    {
                        //decimal subNet = (subBalance.CrTotal ?? 0) - (subBalance.DbTotal ?? 0);
                        //decimal subPrevNet = (subBalance.CrTotalPrev ?? 0) - (subBalance.DbTotalPrev ?? 0);

                        //if (subNet != 0 || subPrevNet != 0)
                        //{
                        //    totalIncome += subNet;
                        //    totalPrevIncome += subPrevNet;

                        //    results.Add(new DetailedReportPandLRow
                        //    {
                        //        SrNo = (results.Count + 1).ToString(),
                        //        Name = subBalance.HeadingName,
                        //        HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                        //        PrevYearTotal = subPrevNet.ToString($"N{RoundOff}"),
                        //        Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
                        //    });

                        var descriptionSql = @"
            SELECT DISTINCT 
                ATBUD_Subheading AS SubHeadingID,
                ATBUD_Description,
                a.ASSH_Name AS SubHeadingName,
                a.ASSH_Notes AS Notes,
                ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
                ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
                e.ASHN_Description AS ASHN_Description,
                ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
                ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
                ISNULL(SUM(d.ATBU_Opening_Credit_Amount), 0) AS CrOPB,
                ISNULL(SUM(d.ATBU_Opening_Debit_Amount), 0) AS DbOPB,
                ISNULL(SUM(d.ATBU_TR_Credit_Amount), 0) AS CrTRn,
                ISNULL(SUM(d.ATBU_TR_Debit_Amount), 0) AS DbTRn,
                ISNULL(SUM(f.ATBU_Opening_Credit_Amount), 0) AS CrPrevOPB,
                ISNULL(SUM(f.ATBU_Opening_Debit_Amount), 0) AS DbPrevOPB,
                ISNULL(SUM(f.ATBU_TR_Credit_Amount), 0) AS CrPrevTRn,
                ISNULL(SUM(f.ATBU_TR_Debit_Amount), 0) AS DbPrevTRn,
                g.ASHL_Description
            FROM Acc_TrailBalance_Upload_Details
            LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
            LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = Atbud_Branchnameid
            LEFT JOIN ACC_SubHeadingNoteDesc e ON e.ASHN_SubHeadingId = a.ASSH_ID AND e.ASHN_CustomerId = @CustomerId AND e.ASHN_YearID = @YearId
            LEFT JOIN ACC_SubHeadingLedgerDesc g ON g.ASHL_SubHeadingId = ATBUD_ID AND g.ASHL_CustomerId = @CustomerId AND g.ASHL_YearID = @YearId
            LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ATBUD_Description AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = Atbud_Branchnameid
            WHERE ATBUD_Subheading = @SubHeadingID AND ATBUD_SubItemId = 0 AND ATBUD_itemid = 0 AND ATBUD_Schedule_type = @ScheduleTypeID AND ATBUD_CustId = @CustomerId AND Atbud_Branchnameid = @BranchId
            GROUP BY ATBUD_Subheading, ATBUD_Description, a.ASSH_Name, a.ASSH_Notes, e.ASHN_Description, g.ASHL_Description
            ORDER BY ATBUD_Subheading";

                        var subDescriptions = await connection.QueryAsync<DetailedReportPandLRow>(descriptionSql, new
                        {
                            YearId = YearId,
                            PrevYearId = YearId - 1,
                            ScheduleTypeId = ScheduleTypeID,
                            CompanyId = CompId,
                            CustomerId = CustId,
                            HeadingId = heading.HeadingID,
                            SubHeadingId = subHeading.SubHeadingID,
                            BranchID = BranchId
                        });

                        decimal subheadingNet = (subHeading.CrTotal ?? 0) - (subHeading.DbTotal ?? 0);
                        decimal subheadingPrevNet = (subHeading.CrTotalPrev ?? 0) - (subHeading.DbTotalPrev ?? 0);

                        if (subheadingNet != 0 && subheadingPrevNet != 0)
                        {
                            totalIncome += subheadingNet;
                            totalPrevIncome += subheadingPrevNet;

                            foreach (var description in subDescriptions)
                            {
                                results.Add(new DetailedReportPandLRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Status = "2",
                                    Name = subHeading.SubHeadingName,
                                    HeaderSLNo = subheadingNet.ToString($"N{RoundOff}"),
                                    PrevYearTotal = subheadingPrevNet.ToString($"N{RoundOff}"),
                                    Notes = !string.IsNullOrEmpty(subHeading.Notes) ? subHeading.Notes.ToString() : ""
                                });
                            }
                        }

                        // Fetch Items
                        var itemSql = @"
SELECT 
    ud.ATBUD_itemid AS ItemID,
    i.ASI_Name AS ItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
    ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleItems i ON i.ASI_ID = ud.ATBUD_itemid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description
    AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ud.ATBUD_Description
    AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
WHERE 
    ud.ATBUD_Headingid = @HeadingId AND ud.ATBUD_Subheading = @SubHeadingId
    AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.Atbud_Branchnameid = @BranchId
    AND ud.ATBUD_itemid <> 0
GROUP BY ud.ATBUD_itemid, i.ASI_Name";

                        var items = await connection.QueryAsync<DetailedReportPandLRow>(itemSql, new
                        {
                            YearID = YearId,
                            PrevYearId = YearId - 1,
                            ScheduleTypeId = ScheduleTypeID,
                            CompanyId = CompId,
                            CustomerId = CustId,
                            HeadingId = heading.HeadingID,
                            SubHeadingId = subHeading.SubHeadingID,
                            BranchID = BranchId
                        });

                        // For each item, fetch and add their descriptions
                        foreach (var item in items)
                        {

                            var itemDescSql = @"
SELECT 
    ud.ATBUD_Description,
    ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description 
    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description 
    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
WHERE 
    ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_compid = @CompanyId AND ud.Atbud_Branchnameid = @BranchId
    AND ud.ATBUD_Headingid = @HeadingId AND ud.ATBUD_Subheading = @SubHeadingId AND ud.ATBUD_itemid = @ItemId AND ud.ATBUD_SubItemId = 0
GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";

                            var itemDescriptions = await connection.QueryAsync<DetailedReportPandLRow>(itemDescSql, new
                            {
                                YearId = YearId,
                                PrevYearId = YearId - 1,
                                ScheduleTypeID = ScheduleTypeID,
                                CompanyId = CompId,
                                CustomerId = CustId,
                                HeadingId = heading.HeadingID,
                                SubHeadingId = subHeading.SubHeadingID,
                                ItemId = item.ItemID,
                                BranchID = BranchId
                            });

                            decimal itemNet = (item.CrTotal ?? 0) - (item.DbTotal ?? 0);
                            decimal itemPrevNet = (item.CrTotalPrev ?? 0) - (item.DbTotalPrev ?? 0);

                            if (itemNet != 0 && itemPrevNet != 0)
                            {
                                totalIncome += subheadingNet;
                                totalPrevIncome += subheadingPrevNet;

                                foreach (var description in itemDescriptions)
                                {
                                    results.Add(new DetailedReportPandLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Status = "3",
                                        Name = item.ItemName,
                                        HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
                                        PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}"),
                                        Notes = !string.IsNullOrEmpty(item.Notes) ? item.Notes.ToString() : ""
                                    });
                                }
                            }

                            // 🔹 Query 1: Fetch SubItems
                            var subItemSql = $@"
SELECT 
    ud.ATBUD_SubItemId AS SubItemId,
    si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
WHERE 
    ud.ATBUD_Headingid = @HeadingId AND 
    ud.ATBUD_Subheading = @SubHeadingId AND
    ud.ATBUD_itemid = @ItemId AND
    ISNULL(ud.ATBUD_SubItemId, 0) <> 0 AND 
    ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
    ud.ATBUD_CustId = @CustId AND ud.Atbud_Branchnameid = @BranchId
GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";

                            var subItems = (await connection.QueryAsync<DetailedReportPandLRow>(subItemSql, new
                            {
                                YearId = YearId,
                                PrevYearId = YearId - 1,
                                ScheduleTypeId = ScheduleTypeID,
                                CompId = CompId,
                                CustId = CustId,
                                HeadingId = heading.HeadingID,
                                SubHeadingId = subHeading.SubHeadingID,
                                ItemId = item.ItemID,
                                BranchID = BranchId
                            })).ToList();

                            // 🔹 Fetch descriptions for each subitem
                            foreach (var subItem in subItems)
                            {
                                var subDescSql = $@"
SELECT 
    ud.ATBUD_ID,
    ud.ATBUD_Description,
    si.ASSI_ID,
    si.ASSI_Name AS HeadingName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) AS Total,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustId AND ldg.ASHL_YearID = @YearId
WHERE 
    ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
    ud.ATBUD_CustId = @CustId AND 
    ud.ATBUD_compid = @CompId AND
    ud.ATBUD_Headingid = @HeadingId AND 
    ud.ATBUD_Subheading = @SubHeadingId AND 
    ud.ATBUD_itemid = @ItemId AND 
    ud.ATBUD_SubItemId = @SubItemId AND 
    ud.ATBUD_YEARId = @YearId AND ud.Atbud_Branchnameid = @BranchId
GROUP BY ud.ATBUD_ID, ud.ATBUD_Description, si.ASSI_ID, si.ASSI_Name, ldg.ASHL_Description";

                                var subItemDescriptions = (await connection.QueryAsync<DetailedReportPandLRow>(subDescSql, new
                                {
                                    YearId = YearId,
                                    PrevYearId = YearId - 1,
                                    ScheduleTypeId = ScheduleTypeID,
                                    CompId = CompId,
                                    CustId = CustId,
                                    HeadingId = heading.HeadingID,
                                    SubHeadingId = subHeading.SubHeadingID,
                                    ItemId = item.ItemID,
                                    SubItemId = subItem.SubItemID,
                                    BranchID = BranchId
                                })).ToList();

                                decimal subItemNet = (subItem.CrTotal ?? 0) - (subItem.DbTotal ?? 0);
                                decimal subItemPrevNet = (subItem.CrTotalPrev ?? 0) - (subItem.DbTotalPrev ?? 0);

                                if (subheadingNet != 0 && subheadingPrevNet != 0)
                                {
                                    totalIncome += subheadingNet;
                                    totalPrevIncome += subheadingPrevNet;

                                    foreach (var description in subItemDescriptions)
                                    {
                                        results.Add(new DetailedReportPandLRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Status = "4",
                                            Name = subItem.SubItemName,
                                            HeaderSLNo = subItemNet.ToString($"N{RoundOff}"),
                                            PrevYearTotal = subItemPrevNet.ToString($"N{RoundOff}"),
                                            Notes = !string.IsNullOrEmpty(subItem.Notes) ? subItem.Notes.ToString() : ""
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            results.Add(new DetailedReportPandLRow
            {
                SrNo = (results.Count + 1).ToString(),
                Name = "III Total Income",
                HeaderSLNo = totalIncome == 0 ? "-" : totalIncome.ToString($"N{RoundOff}"),
                PrevYearTotal = totalPrevIncome == 0 ? "-" : totalPrevIncome.ToString($"N{RoundOff}")
            });
        }
    }
}




//            //Expenses
//            // 1. Fetch Expense Headings
//            var expenseheadingSql = $@"
//SELECT DISTINCT 
//    ATBUD_Headingid AS HeadingID,
//    ASH_Name AS HeadingName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    a.ASH_Notes AS Notes
//FROM Acc_TrailBalance_Upload_Details
//LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_Headingid
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_Description = ATBUD_Description
//WHERE ATBUD_Schedule_type = @ScheduleTypeId
//  AND ATBUD_compid = @CompanyId
//  AND ATBUD_CustId = @CustomerId
//  AND EXISTS (
//      SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ATBUD_Headingid AND AST_AccHeadId = 2
//)";


//            var headingRows = await connection.QueryAsync<DetailedReportPandLRow>(headingSql, new
//            {
//                ScheduleTypeId = ScheduleTypeID,
//                CompanyId = CompId,
//                CustomerId = CustId
//            });

//            // 2. Fetch Descriptions under Heading
//            foreach (var heading in headingRows)
//            {
//                var descSql = $@"
//SELECT DISTINCT 
//    b.ATBUD_ID,
//    b.ATBUD_Description AS Name,
//    a.ASH_ID,
//    a.ASH_Name AS HeadingName,
//    ISNULL(SUM(ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal
//FROM Acc_TrailBalance_Upload
//LEFT JOIN Acc_TrailBalance_Upload_Details b ON b.ATBUD_Description = ATBU_Description
//LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = b.ATBUD_Headingid
//WHERE b.ATBUD_Schedule_Type = @ScheduleTypeId
//  AND b.ATBUD_compid = @CompanyId
//  AND b.ATBUD_CustId = @CustomerId
//  AND b.ATBUD_YEARId = @YearId
//  AND ATBU_YEARId = @YearId
//GROUP BY b.ATBUD_ID, b.ATBUD_Description, a.ASH_ID, a.ASH_Name, ATBU_Branchid
//            ORDER BY b.ATBUD_ID";


//                var descriptions = await connection.QueryAsync<DetailedReportPandLRow>(descSql, new
//                {
//                    ScheduleTypeId = ScheduleTypeID,
//                    CompanyId = CompId,
//                    CustomerId = CustId,
//                    YearId = YearId
//                });

//                decimal net = 0;
//                decimal prevNet = 0;

//                results.Add(new DetailedReportPandLRow
//                {
//                    SrNo = (results.Count + 1).ToString(),
//                    Status = "1",
//                    Name = "Heading",
//                    HeadingName = heading.HeadingName,
//                    HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
//                    PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}")
//                });

//                // 1. Fetch Expense SubHeadings
//                var subHeadingSql = $@"
//        SELECT DISTINCT AST_SubHeadingID AS SubHeadingID, a.ASSH_Name AS SubHeadingName, a.ASSH_Notes AS Notes
//        FROM ACC_ScheduleTemplates
//        LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
//        WHERE AST_Schedule_type = @ScheduleTypeId
//          AND AST_Companytype = @CustomerId
//          AND AST_AccHeadId = 2
//        GROUP BY AST_SubHeadingID, a.ASSH_Name, a.ASSH_Notes";

//                var subHeadings = (await connection.QueryAsync<DetailedReportPandLRow>(subHeadingSql, new
//                {
//                    ScheduleTypeId = ScheduleTypeID,
//                    CustomerId = CustId
//                })).ToList();

//                // 2. Get SubHeading Totals
//                foreach (var sub in subHeadings)
//                {
//                    var subTotalsSql = $@"
//        SELECT 
//            ATBUD_Subheading AS SubHeadingID,
//            a.ASSH_Name AS SubHeadingName,
//            a.ASSH_Notes AS Notes,
//            ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//            ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//            e.ASHN_Description AS ASHN_Description,
//            ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//            ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//            ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotalPrev,
//            ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotalPrev
//        FROM Acc_TrailBalance_Upload_Details
//        LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
//        LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = Atbud_Branchnameid
//        LEFT JOIN ACC_SubHeadingNoteDesc e ON e.ASHN_SubHeadingId = a.ASSH_ID AND e.ASHN_CustomerId = @CustomerId AND e.ASHN_YearID = @YearId
//        LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ATBUD_Description AND ATBUD_SubItemId = 0 AND ATBUD_itemid = 0 AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = Atbud_Branchnameid
//        LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ATBUD_Description AND ATBUD_SubItemId = 0 AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = Atbud_Branchnameid
//        WHERE ATBUD_Subheading <> 0
//          AND ATBUD_Schedule_type = @ScheduleTypeId
//          AND ATBUD_CustId = @CustomerId
//        GROUP BY ATBUD_Subheading, a.ASSH_Name, a.ASSH_Notes, e.ASHN_Description
//        ORDER BY ATBUD_Subheading";

//                    var subHeadingTotals = await connection.QueryAsync<DetailedReportPandLRow>(subTotalsSql, new
//                    {
//                        YearId = YearId,
//                        PrevYearId = YearId - 1,
//                        ScheduleTypeId = ScheduleTypeID,
//                        CustomerId = CustId
//                    });

//                    // 3. Descriptions under SubHeading
//                    foreach (var subBalance in subHeadingTotals)
//                    {
//                        var descriptionSql = $@"
//            SELECT 
//                ATBUD_Subheading AS SubHeadingID,
//                ATBUD_Description,
//                a.ASSH_Name AS SubHeadingName,
//                a.ASSH_Notes AS Notes,
//                ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//                ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//                e.ASHN_Description AS ASHN_Description,
//                ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//                ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
//                ISNULL(SUM(d.ATBU_Opening_Credit_Amount), 0) AS CrOPB,
//                ISNULL(SUM(d.ATBU_Opening_Debit_Amount), 0) AS DbOPB,
//                ISNULL(SUM(d.ATBU_TR_Credit_Amount), 0) AS CrTRn,
//                ISNULL(SUM(d.ATBU_TR_Debit_Amount), 0) AS DbTRn,
//                ISNULL(SUM(f.ATBU_Opening_Credit_Amount), 0) AS CrPrevOPB,
//                ISNULL(SUM(f.ATBU_Opening_Debit_Amount), 0) AS DbPrevOPB,
//                ISNULL(SUM(f.ATBU_TR_Credit_Amount), 0) AS CrPrevTRn,
//                ISNULL(SUM(f.ATBU_TR_Debit_Amount), 0) AS DbPrevTRn,
//                g.ASHL_Description AS Notes
//            FROM Acc_TrailBalance_Upload_Details
//            LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
//            LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = Atbud_Branchnameid
//            LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ATBUD_Description AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = Atbud_Branchnameid
//            LEFT JOIN ACC_SubHeadingNoteDesc e ON e.ASHN_SubHeadingId = a.ASSH_ID AND e.ASHN_CustomerId = @CustomerId AND e.ASHN_YearID = @YearId
//            LEFT JOIN ACC_SubHeadingLedgerDesc g ON g.ASHL_SubHeadingId = ATBUD_ID AND g.ASHL_CustomerId = @CustomerId AND g.ASHL_YearID = @YearId
//            WHERE ATBUD_Schedule_type = @ScheduleTypeId
//              AND ATBUD_CustId = @CustomerId AND ATBUD_CompId = @CompanyId
//            GROUP BY ATBUD_Subheading, ATBUD_Description, a.ASSH_Name, a.ASSH_Notes, e.ASHN_Description, g.ASHL_Description
//            ORDER BY ATBUD_Subheading";

//                        var descs = await connection.QueryAsync<DetailedReportPandLRow>(descSql, new
//                        {
//                            YearId = YearId,
//                            PrevYearId = YearId - 1,
//                            ScheduleTypeId = ScheduleTypeID,
//                            CustomerId = CustId,
//                            CompanyId = CompId
//                        });

//                        decimal subNet = (subBalance.CrTotal ?? 0) - (subBalance.DbTotal ?? 0);
//                        decimal subPrevNet = (subBalance.CrTotalPrev ?? 0) - (subBalance.DbTotalPrev ?? 0);

//                        if (subNet != 0 || subPrevNet != 0)
//                        {
//                            totalIncome += subNet;
//                            totalPrevIncome += subPrevNet;

//                            results.Add(new DetailedReportPandLRow
//                            {
//                                SrNo = (results.Count + 1).ToString(),
//                                Status = "2",
//                                Name = "SubHeading",
//                                SubHeadingName = subBalance.SubHeadingName,
//                                HeaderSLNo = subNet.ToString($"N{RoundOff}"),
//                                PrevYearTotal = subPrevNet.ToString($"N{RoundOff}"),
//                                Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
//                            });
//                        }

//                        // 1. Fetch Expense Items
//                        var itemSql = $@"
//        SELECT 
//    ud.ATBUD_itemid AS ItemId,
//    i.ASI_Name AS ItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//    ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleItems i ON i.ASI_ID = ud.ATBUD_itemid
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description
//    AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
//LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ud.ATBUD_Description
//    AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
//WHERE
//    ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId
//GROUP BY ud.ATBUD_itemid, i.ASI_Name";

//                        var items = (await connection.QueryAsync<DetailedReportPandLRow>(itemSql, new
//                        {
//                            YearId = YearId,
//                            PrevYearId = YearId - 1,
//                            ScheduleTypeId = ScheduleTypeID,
//                            CustomerId = CustId
//                        })).ToList();

//                        // 3. Description under Item
//                        foreach (var item in items)
//                        {
//                            var itemDescSql = $@"
//                SELECT 
//    ud.ATBUD_Description,
//    ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//    ldg.ASHL_Description AS Notes
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description 
//    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description 
//    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
//    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
//WHERE 
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_CompId = @CompanyId
//GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";

//                            var itemDescriptions = await connection.QueryAsync<DetailedReportPandLRow>(descSql, new
//                            {
//                                YearId = YearId,
//                                PrevYearId = YearId - 1,
//                                ScheduleTypeId = ScheduleTypeID,
//                                CustomerId = CustId,
//                                CompanyId = CompId
//                            });

//                            decimal itemNet = (item.CrTotal ?? 0) - (item.DbTotal ?? 0);
//                            decimal itemPrevNet = (item.CrTotalPrev ?? 0) - (item.DbTotalPrev ?? 0);

//                            if (itemNet != 0 || itemPrevNet != 0)
//                            {
//                                totalIncome += itemNet;
//                                totalPrevIncome += itemPrevNet;

//                                results.Add(new DetailedReportPandLRow
//                                {
//                                    SrNo = (results.Count + 1).ToString(),
//                                    Status = "3",
//                                    Name = "Item",
//                                    ItemName = item.ItemName,
//                                    HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
//                                    PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}"),
//                                    Notes = !string.IsNullOrEmpty(item.Notes) ? item.Notes.ToString() : ""
//                                });
//                            }

//                            // 1. Fetch Expense SubItems
//                            var subItemSql = $@"
//     SELECT 
//    ud.ATBUD_SubItemId AS SubItemId,
//    si.ASSI_Name AS SubItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
//    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
//    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
//    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
//WHERE
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
//    ud.ATBUD_CustId = @CustId
//GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";

//                            var subItems = (await connection.QueryAsync<DetailedReportPandLRow>(subItemSql, new
//                            {
//                                YearId = YearId,
//                                PrevYearId = YearId - 1,
//                                ScheduleTypeId = ScheduleTypeID,
//                                CompId = CompId,
//                                CustId = CustId
//                            })).ToList();

//                            // 2. Description underSubItems
//                            foreach (var subItem in subItems)
//                            {
//                                var subitemDescSql = $@"
//             SELECT 
//    ud.ATBUD_ID,
//    ud.ATBUD_Description,
//    si.ASSI_ID,
//    si.ASSI_Name AS SubItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) AS Total,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
//    ldg.ASHL_Description AS Notes
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
//    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
//    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
//    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
//    AND ldg.ASHL_CustomerId = @CustId AND ldg.ASHL_YearID = @YearId
//WHERE 
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
//    ud.ATBUD_CustId = @CustId AND 
//    ud.ATBUD_compid = @CompId AND 
//    ud.ATBUD_YEARId = @YearId
//GROUP BY ud.ATBUD_ID, ud.ATBUD_Description, si.ASSI_ID, si.ASSI_Name, ldg.ASHL_Description";

//                                var subItemDescriptions = (await connection.QueryAsync<DetailedReportPandLRow>(subitemDescSql, new
//                                {
//                                    YearId = YearId,
//                                    PrevYearId = YearId - 1,
//                                    ScheduleTypeId = ScheduleTypeID,
//                                    CompId = CompId,
//                                    CustId = CustId
//                                })).ToList();

//                                decimal subitemNet = (subItem.CrTotal ?? 0) - (subItem.DbTotal ?? 0);
//                                decimal subitemPrevNet = (subItem.CrTotalPrev ?? 0) - (subItem.DbTotalPrev ?? 0);

//                                if (subitemNet != 0 || subitemPrevNet != 0)
//                                {
//                                    totalIncome += subitemNet;
//                                    totalPrevIncome += subitemPrevNet;

//                                    results.Add(new DetailedReportPandLRow
//                                    {
//                                        SrNo = (results.Count + 1).ToString(),
//                                        Status = "4",
//                                        Name = "SubItem",
//                                        SubItemName = item.SubItemName,
//                                        HeaderSLNo = subitemNet.ToString($"N{RoundOff}"),
//                                        PrevYearTotal = subitemPrevNet.ToString($"N{RoundOff}"),
//                                        Notes = !string.IsNullOrEmpty(subItem.Notes) ? subItem.Notes.ToString() : ""
//                                    });
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//             return results;
//        }
//}


//        //GetDetailedReportBalanceSheet
//        public async Task<IEnumerable<DetailedReportBalanceSheetRow>> GetDetailedReportBalanceSheetAsync(int CompId, DetailedReportBalanceSheet p)
//        {
//            var results = new List<DetailedReportBalanceSheetRow>();
//            int ScheduleTypeID = 4;
//            int RoundOff = 1;
//            int YearId = p.YearID;
//            int CustId = p.CustID;
//            int selectedBranches = 1;
//            decimal totalIncome = 0, totalPrevIncome = 0;
//            decimal totalExpense = 0, totalPrevExpense = 0;

//            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection4"));
//            await connection.OpenAsync();

//            // 1. Fetch Income Heading
//            var headingSql = $@"
//SELECT DISTINCT 
//    ATBUD_Headingid AS HeadingID,
//    ASH_Name AS HeadingName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    a.ASH_Notes
//FROM Acc_TrailBalance_Upload_Details
//LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_Headingid
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description
//WHERE ATBUD_Schedule_type = @ScheduleTypeId
//  AND ATBUD_compid = @CompanyId
//  AND ATBUD_CustId = @CustomerId
//  AND EXISTS (
//      SELECT 1 
//      FROM ACC_ScheduleTemplates s 
//      WHERE s.AST_HeadingID = ATBUD_Headingid AND s.AST_AccHeadId = 1
//  )
//GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes
//ORDER BY ATBUD_Headingid";


//            var headings = await connection.QueryAsync<DetailedReportBalanceSheetRow>(headingSql, new
//            {
//                ScheduleTypeId = ScheduleTypeID,
//                CompanyId = CompId,
//                CustomerId = CustId
//            });

//            foreach (var heading in headings)
//            {
//                // 2. Descriptions under Heading
//                var descSql = $@"
//            SELECT DISTINCT 
//                b.ATBUD_ID,
//                b.ATBUD_Description AS Name,
//                a.ASH_ID,
//                a.ASH_Name AS HeadingName,
//                ISNULL(SUM(ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//                ISNULL(SUM(ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal
//            FROM Acc_TrailBalance_Upload
//            LEFT JOIN Acc_TrailBalance_Upload_Details b ON b.ATBUD_Description = ATBU_Description
//            LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = b.ATBUD_Headingid
//            WHERE b.ATBUD_Schedule_Type = @ScheduleTypeId
//              AND b.ATBUD_compid = @CompanyId
//              AND b.ATBUD_CustId = @CustomerId
//              AND b.ATBUD_YEARId = @YearId
//              AND ATBU_YEARId = @YearId
//            GROUP BY b.ATBUD_ID, b.ATBUD_Description, a.ASH_ID, a.ASH_Name, ATBU_Branchid
//            ORDER BY b.ATBUD_ID";

//                var descriptions = await connection.QueryAsync<DetailedReportBalanceSheetRow>(descSql, new
//                {
//                    ScheduleTypeId = ScheduleTypeID,
//                    CompanyId = CompId,
//                    CustomerId = CustId,
//                    YearId = YearId
//                });

//                decimal net = 0;
//                decimal prevNet = 0;

//                results.Add(new DetailedReportBalanceSheetRow
//                {
//                    SrNo = (results.Count + 1).ToString(),
//                    Status = "1",
//                    Name = "Heading",
//                    HeadingName = heading.HeadingName,
//                    HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
//                    PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}")
//                });

//                // 1. Fetch Income SubHeading

//                var subHeadingSql = @"
//                SELECT DISTINCT AST_SubHeadingID AS SubHeadingID, a.ASSH_Name AS SubHeadingName, a.ASSH_Notes AS Notes
//                FROM ACC_ScheduleTemplates
//                LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
//                WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustomerId AND AST_AccHeadId = 1";

//                var subHeadings = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subHeadingSql, new
//                {
//                    ScheduleTypeID = ScheduleTypeID,
//                    CustomerId = CustId,
//                })).ToList();


//                // 2. Get SubHeading Totals
//                foreach (var sub in subHeadings)
//                {

//                    var subHeadingTotalsSql = $@"
//        SELECT 
//            ATBUD_Subheading AS SubHeadingID, a.ASSH_Name AS SubHeadingName, a.ASSH_Notes AS Notes,
//            ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//            ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//            e.ASHN_Description AS ASHN_Description,
//            ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//            ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//            ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotalPrev,
//            ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotalPrev
//        FROM Acc_TrailBalance_Upload_Details
//        LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
//        LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = Atbud_Branchnameid
//        LEFT JOIN ACC_SubHeadingNoteDesc e ON e.ASHN_SubHeadingId = a.ASSH_ID AND e.ASHN_CustomerId = @CustomerId AND e.ASHN_YearID = @YearId
//        LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ATBUD_Description AND ATBUD_SubItemId = 0 AND ATBUD_itemid = 0 AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = Atbud_Branchnameid
//        LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ATBUD_Description AND ATBUD_SubItemId = 0 AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = Atbud_Branchnameid
//        WHERE  ATBUD_Subheading <> 0 AND ATBUD_Schedule_type = @ScheduleTypeID AND ATBUD_CustId = @CustomerId
//        GROUP BY ATBUD_Subheading, a.ASSH_Name, a.ASSH_Notes, e.ASHN_Description
//        ORDER BY ATBUD_Subheading";

//                    var subHeadingTotals = await connection.QueryAsync<DetailedReportBalanceSheetRow>(subHeadingTotalsSql, new
//                    {
//                        YearId,
//                        PrevYearId = YearId - 1,
//                        CustomerId = CustId,
//                        ScheduleTypeID
//                    });

//                    // 3. Descriptions under Subheading
//                    foreach (var subBalance in subHeadingTotals)
//                    {
//                        var descriptionSql = @"
//            SELECT DISTINCT 
//                ATBUD_Subheading AS SubHeadingID,
//                ATBUD_Description,
//                a.ASSH_Name AS SubHeadingName,
//                a.ASSH_Notes AS Notes,
//                ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//                ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//                e.ASHN_Description AS ASHN_Description,
//                ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//                ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
//                ISNULL(SUM(d.ATBU_Opening_Credit_Amount), 0) AS CrOPB,
//                ISNULL(SUM(d.ATBU_Opening_Debit_Amount), 0) AS DbOPB,
//                ISNULL(SUM(d.ATBU_TR_Credit_Amount), 0) AS CrTRn,
//                ISNULL(SUM(d.ATBU_TR_Debit_Amount), 0) AS DbTRn,
//                ISNULL(SUM(f.ATBU_Opening_Credit_Amount), 0) AS CrPrevOPB,
//                ISNULL(SUM(f.ATBU_Opening_Debit_Amount), 0) AS DbPrevOPB,
//                ISNULL(SUM(f.ATBU_TR_Credit_Amount), 0) AS CrPrevTRn,
//                ISNULL(SUM(f.ATBU_TR_Debit_Amount), 0) AS DbPrevTRn,
//                g.ASHL_Description
//            FROM Acc_TrailBalance_Upload_Details
//            LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
//            LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = Atbud_Branchnameid
//            LEFT JOIN ACC_SubHeadingNoteDesc e ON e.ASHN_SubHeadingId = a.ASSH_ID AND e.ASHN_CustomerId = @CustomerId AND e.ASHN_YearID = @YearId
//            LEFT JOIN ACC_SubHeadingLedgerDesc g ON g.ASHL_SubHeadingId = ATBUD_ID AND g.ASHL_CustomerId = @CustomerId AND g.ASHL_YearID = @YearId
//            LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ATBUD_Description AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = Atbud_Branchnameid
//            WHERE ATBUD_Schedule_type = @ScheduleTypeID AND ATBUD_CustId = @CustomerId
//            GROUP BY ATBUD_Subheading, ATBUD_Description, a.ASSH_Name, a.ASSH_Notes, e.ASHN_Description, g.ASHL_Description
//            ORDER BY ATBUD_Subheading";

//                        var descs = await connection.QueryAsync<DetailedReportBalanceSheetRow>(descriptionSql, new
//                        {
//                            YearId,
//                            PrevYearId = YearId - 1,
//                            CustomerId = CustId,
//                            ScheduleTypeID
//                        });

//                        decimal subNet = (subBalance.CrTotal ?? 0) - (subBalance.DbTotal ?? 0);
//                        decimal subPrevNet = (subBalance.CrTotalPrev ?? 0) - (subBalance.DbTotalPrev ?? 0);

//                        if (subNet != 0 || subPrevNet != 0)
//                        {
//                            totalIncome += subNet;
//                            totalPrevIncome += subPrevNet;

//                            results.Add(new DetailedReportBalanceSheetRow
//                            {
//                                SrNo = (results.Count + 1).ToString(),
//                                Status = "2",
//                                Name = "SubHeading",
//                                SubHeadingName = subBalance.SubHeadingName,
//                                HeaderSLNo = subNet.ToString($"N{RoundOff}"),
//                                PrevYearTotal = subPrevNet.ToString($"N{RoundOff}"),
//                                Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
//                            });
//                        }
//                        else
//                        {
//                            //Prev Year Total
//                        }

//                        // 1. Fetch Income Item
//                        var itemSql = @"
//SELECT 
//    ud.ATBUD_itemid AS ItemId,
//    i.ASI_Name AS ItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//    ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleItems i ON i.ASI_ID = ud.ATBUD_itemid
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description
//    AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
//LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ud.ATBUD_Description
//    AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
//WHERE
//    ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId
//GROUP BY ud.ATBUD_itemid, i.ASI_Name";

//                        var items = await connection.QueryAsync<DetailedReportBalanceSheetRow>(itemSql, new
//                        {
//                            YearId = YearId,
//                            PrevYearId = YearId - 1,
//                            ScheduleTypeId = ScheduleTypeID,
//                            CompanyId = CompId,
//                            CustomerId = CustId
//                        });

//                        // 2. Description under Item
//                        foreach (var item in items)
//                        {
//                            var itemDescSql = @"
//SELECT 
//    ud.ATBUD_Description,
//    ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//    ldg.ASHL_Description AS Notes
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description 
//    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description 
//    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
//    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
//WHERE 
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_compid = @CompanyId
//GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";

//                            var itemDescriptions = await connection.QueryAsync<DetailedReportBalanceSheetRow>(itemDescSql, new
//                            {
//                                YearId = YearId,
//                                PrevYearId = YearId - 1,
//                                ScheduleTypeId = ScheduleTypeID,
//                                CompanyId = CompId,
//                                CustomerId = CustId
//                            });

//                            decimal itemNet = (item.CrTotal ?? 0) - (item.DbTotal ?? 0);
//                            decimal itemPrevNet = (item.CrTotalPrev ?? 0) - (item.DbTotalPrev ?? 0);

//                            if (itemNet != 0 || itemPrevNet != 0)
//                            {
//                                totalIncome += itemNet;
//                                totalPrevIncome += itemPrevNet;

//                                results.Add(new DetailedReportBalanceSheetRow
//                                {
//                                    SrNo = (results.Count + 1).ToString(),
//                                    Status = "3",
//                                    Name = "Item",
//                                    ItemName = item.ItemName,
//                                    HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
//                                    PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}"),
//                                    Notes = !string.IsNullOrEmpty(item.Notes) ? item.Notes.ToString() : ""
//                                });
//                            }

//                            // 1. Fetch Income SubItems
//                            var subItemSql = $@"
//SELECT 
//    ud.ATBUD_SubItemId AS SubItemId,
//    si.ASSI_Name AS SubItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
//    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
//    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
//    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
//WHERE
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
//    ud.ATBUD_CustId = @CustId
//GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";

//                            var subItems = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subItemSql, new
//                            {
//                                YearId = YearId,
//                                PrevYearId = YearId - 1,
//                                ScheduleTypeId = ScheduleTypeID,
//                                CompId = CompId,
//                                CustId = CustId
//                            })).ToList();

//                            // 2. Descriptions under subitem
//                            foreach (var subItem in subItems)
//                            {
//                                var subDescSql = $@"
//SELECT 
//    ud.ATBUD_ID,
//    ud.ATBUD_Description,
//    si.ASSI_ID,
//    si.ASSI_Name AS SubItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) AS Total,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
//    ldg.ASHL_Description AS Notes
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
//    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
//    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
//    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
//    AND ldg.ASHL_CustomerId = @CustId AND ldg.ASHL_YearID = @YearId
//WHERE 
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
//    ud.ATBUD_CustId = @CustId AND 
//    ud.ATBUD_compid = @CompId AND 
//    ud.ATBUD_YEARId = @YearId
//GROUP BY ud.ATBUD_ID, ud.ATBUD_Description, si.ASSI_ID, si.ASSI_Name, ldg.ASHL_Description";

//                                var subItemDescriptions = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subDescSql, new
//                                {
//                                    YearId = YearId,
//                                    PrevYearId = YearId - 1,
//                                    ScheduleTypeId = ScheduleTypeID,
//                                    CompId = CompId,
//                                    CustId = CustId
//                                })).ToList();

//                                decimal subitemNet = (subItem.CrTotal ?? 0) - (subItem.DbTotal ?? 0);
//                                decimal subitemPrevNet = (subItem.CrTotalPrev ?? 0) - (subItem.DbTotalPrev ?? 0);

//                                if (subitemNet != 0 || subitemPrevNet != 0)
//                                {
//                                    totalIncome += subitemNet;
//                                    totalPrevIncome += subitemPrevNet;

//                                    results.Add(new DetailedReportBalanceSheetRow
//                                    {
//                                        SrNo = (results.Count + 1).ToString(),
//                                        Status = "4",
//                                        Name = "SubItem",
//                                        SubItemName = item.SubItemName,
//                                        HeaderSLNo = subitemNet.ToString($"N{RoundOff}"),
//                                        PrevYearTotal = subitemPrevNet.ToString($"N{RoundOff}"),
//                                        Notes = !string.IsNullOrEmpty(subItem.Notes) ? subItem.Notes.ToString() : ""
//                                    });
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            //Expenses
//            // 1. Fetch Expense Headings
//            var expenseheadingSql = $@"
//SELECT DISTINCT 
//    ATBUD_Headingid AS HeadingID,
//    ASH_Name AS HeadingName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    a.ASH_Notes AS Notes
//FROM Acc_TrailBalance_Upload_Details
//LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_Headingid
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_Description = ATBUD_Description
//WHERE ATBUD_Schedule_type = @ScheduleTypeId
//  AND ATBUD_compid = @CompanyId
//  AND ATBUD_CustId = @CustomerId
//  AND EXISTS (
//      SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ATBUD_Headingid AND AST_AccHeadId = 2
//)";


//            var headingRows = await connection.QueryAsync<DetailedReportBalanceSheetRow>(headingSql, new
//            {
//                ScheduleTypeId = ScheduleTypeID,
//                CompanyId = CompId,
//                CustomerId = CustId
//            });

//            // 2. Fetch Descriptions under Heading
//            foreach (var heading in headingRows)
//            {
//                var descSql = $@"
//SELECT DISTINCT 
//    b.ATBUD_ID,
//    b.ATBUD_Description AS Name,
//    a.ASH_ID,
//    a.ASH_Name AS HeadingName,
//    ISNULL(SUM(ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal
//FROM Acc_TrailBalance_Upload
//LEFT JOIN Acc_TrailBalance_Upload_Details b ON b.ATBUD_Description = ATBU_Description
//LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = b.ATBUD_Headingid
//WHERE b.ATBUD_Schedule_Type = @ScheduleTypeId
//  AND b.ATBUD_compid = @CompanyId
//  AND b.ATBUD_CustId = @CustomerId
//  AND b.ATBUD_YEARId = @YearId
//  AND ATBU_YEARId = @YearId
//GROUP BY b.ATBUD_ID, b.ATBUD_Description, a.ASH_ID, a.ASH_Name, ATBU_Branchid
//            ORDER BY b.ATBUD_ID";


//                var descriptions = await connection.QueryAsync<DetailedReportBalanceSheetRow>(descSql, new
//                {
//                    ScheduleTypeId = ScheduleTypeID,
//                    CompanyId = CompId,
//                    CustomerId = CustId,
//                    YearId = YearId
//                });

//                decimal net = 0;
//                decimal prevNet = 0;

//                results.Add(new DetailedReportBalanceSheetRow
//                {
//                    SrNo = (results.Count + 1).ToString(),
//                    Status = "1",
//                    Name = "Heading",
//                    HeadingName = heading.HeadingName,
//                    HeaderSLNo = net == 0 ? "-" : net.ToString($"N{RoundOff}"),
//                    PrevYearTotal = prevNet == 0 ? "-" : prevNet.ToString($"N{RoundOff}")
//                });

//                // 1. Fetch Expense SubHeadings
//                var subHeadingSql = $@"
//        SELECT DISTINCT AST_SubHeadingID AS SubHeadingID, a.ASSH_Name AS SubHeadingName, a.ASSH_Notes AS Notes
//        FROM ACC_ScheduleTemplates
//        LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
//        WHERE AST_Schedule_type = @ScheduleTypeId
//          AND AST_Companytype = @CustomerId
//          AND AST_AccHeadId = 2
//        GROUP BY AST_SubHeadingID, a.ASSH_Name, a.ASSH_Notes";

//                var subHeadings = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subHeadingSql, new
//                {
//                    ScheduleTypeId = ScheduleTypeID,
//                    CustomerId = CustId
//                })).ToList();

//                // 2. Get SubHeading Totals
//                foreach (var sub in subHeadings)
//                {
//                    var subTotalsSql = $@"
//        SELECT 
//            ATBUD_Subheading AS SubHeadingID,
//            a.ASSH_Name AS SubHeadingName,
//            a.ASSH_Notes AS Notes,
//            ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//            ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//            e.ASHN_Description AS ASHN_Description,
//            ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//            ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//            ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotalPrev,
//            ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotalPrev
//        FROM Acc_TrailBalance_Upload_Details
//        LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
//        LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = Atbud_Branchnameid
//        LEFT JOIN ACC_SubHeadingNoteDesc e ON e.ASHN_SubHeadingId = a.ASSH_ID AND e.ASHN_CustomerId = @CustomerId AND e.ASHN_YearID = @YearId
//        LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ATBUD_Description AND ATBUD_SubItemId = 0 AND ATBUD_itemid = 0 AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = Atbud_Branchnameid
//        LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ATBUD_Description AND ATBUD_SubItemId = 0 AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = Atbud_Branchnameid
//        WHERE ATBUD_Subheading <> 0
//          AND ATBUD_Schedule_type = @ScheduleTypeId
//          AND ATBUD_CustId = @CustomerId
//        GROUP BY ATBUD_Subheading, a.ASSH_Name, a.ASSH_Notes, e.ASHN_Description
//        ORDER BY ATBUD_Subheading";

//                    var subHeadingTotals = await connection.QueryAsync<DetailedReportBalanceSheetRow>(subTotalsSql, new
//                    {
//                        YearId = YearId,
//                        PrevYearId = YearId - 1,
//                        ScheduleTypeId = ScheduleTypeID,
//                        CustomerId = CustId
//                    });

//                    // 3. Descriptions under SubHeading
//                    foreach (var subBalance in subHeadingTotals)
//                    {
//                        var descriptionSql = $@"
//            SELECT 
//                ATBUD_Subheading AS SubHeadingID,
//                ATBUD_Description,
//                a.ASSH_Name AS SubHeadingName,
//                a.ASSH_Notes AS Notes,
//                ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//                ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//                e.ASHN_Description AS ASHN_Description,
//                ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//                ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
//                ISNULL(SUM(d.ATBU_Opening_Credit_Amount), 0) AS CrOPB,
//                ISNULL(SUM(d.ATBU_Opening_Debit_Amount), 0) AS DbOPB,
//                ISNULL(SUM(d.ATBU_TR_Credit_Amount), 0) AS CrTRn,
//                ISNULL(SUM(d.ATBU_TR_Debit_Amount), 0) AS DbTRn,
//                ISNULL(SUM(f.ATBU_Opening_Credit_Amount), 0) AS CrPrevOPB,
//                ISNULL(SUM(f.ATBU_Opening_Debit_Amount), 0) AS DbPrevOPB,
//                ISNULL(SUM(f.ATBU_TR_Credit_Amount), 0) AS CrPrevTRn,
//                ISNULL(SUM(f.ATBU_TR_Debit_Amount), 0) AS DbPrevTRn,
//                g.ASHL_Description AS Notes
//            FROM Acc_TrailBalance_Upload_Details
//            LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
//            LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = Atbud_Branchnameid
//            LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ATBUD_Description AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = Atbud_Branchnameid
//            LEFT JOIN ACC_SubHeadingNoteDesc e ON e.ASHN_SubHeadingId = a.ASSH_ID AND e.ASHN_CustomerId = @CustomerId AND e.ASHN_YearID = @YearId
//            LEFT JOIN ACC_SubHeadingLedgerDesc g ON g.ASHL_SubHeadingId = ATBUD_ID AND g.ASHL_CustomerId = @CustomerId AND g.ASHL_YearID = @YearId
//            WHERE ATBUD_Schedule_type = @ScheduleTypeId
//              AND ATBUD_CustId = @CustomerId AND ATBUD_CompId = @CompanyId
//            GROUP BY ATBUD_Subheading, ATBUD_Description, a.ASSH_Name, a.ASSH_Notes, e.ASHN_Description, g.ASHL_Description
//            ORDER BY ATBUD_Subheading";

//                        var descs = await connection.QueryAsync<DetailedReportBalanceSheetRow>(descSql, new
//                        {
//                            YearId = YearId,
//                            PrevYearId = YearId - 1,
//                            ScheduleTypeId = ScheduleTypeID,
//                            CustomerId = CustId,
//                            CompanyId = CompId
//                        });

//                        decimal subNet = (subBalance.CrTotal ?? 0) - (subBalance.DbTotal ?? 0);
//                        decimal subPrevNet = (subBalance.CrTotalPrev ?? 0) - (subBalance.DbTotalPrev ?? 0);

//                        if (subNet != 0 || subPrevNet != 0)
//                        {
//                            totalIncome += subNet;
//                            totalPrevIncome += subPrevNet;

//                            results.Add(new DetailedReportBalanceSheetRow
//                            {
//                                SrNo = (results.Count + 1).ToString(),
//                                Status = "2",
//                                Name = "SubHeading",
//                                SubHeadingName = subBalance.SubHeadingName,
//                                HeaderSLNo = subNet.ToString($"N{RoundOff}"),
//                                PrevYearTotal = subPrevNet.ToString($"N{RoundOff}"),
//                                Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
//                            });
//                        }

//                        // 1. Fetch Expense Items
//                        var itemSql = $@"
//        SELECT 
//    ud.ATBUD_itemid AS ItemId,
//    i.ASI_Name AS ItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//    ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleItems i ON i.ASI_ID = ud.ATBUD_itemid
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description
//    AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
//LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ud.ATBUD_Description
//    AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
//WHERE
//    ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId
//GROUP BY ud.ATBUD_itemid, i.ASI_Name";

//                        var items = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(itemSql, new
//                        {
//                            YearId = YearId,
//                            PrevYearId = YearId - 1,
//                            ScheduleTypeId = ScheduleTypeID,
//                            CustomerId = CustId
//                        })).ToList();

//                        // 3. Description under Item
//                        foreach (var item in items)
//                        {
//                            var itemDescSql = $@"
//                SELECT 
//    ud.ATBUD_Description,
//    ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
//    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
//    ldg.ASHL_Description AS Notes
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description 
//    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description 
//    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
//    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
//WHERE 
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_CompId = @CompanyId
//GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";

//                            var itemDescriptions = await connection.QueryAsync<DetailedReportBalanceSheetRow>(descSql, new
//                            {
//                                YearId = YearId,
//                                PrevYearId = YearId - 1,
//                                ScheduleTypeId = ScheduleTypeID,
//                                CustomerId = CustId,
//                                CompanyId = CompId
//                            });

//                            decimal itemNet = (item.CrTotal ?? 0) - (item.DbTotal ?? 0);
//                            decimal itemPrevNet = (item.CrTotalPrev ?? 0) - (item.DbTotalPrev ?? 0);

//                            if (itemNet != 0 || itemPrevNet != 0)
//                            {
//                                totalIncome += itemNet;
//                                totalPrevIncome += itemPrevNet;

//                                results.Add(new DetailedReportBalanceSheetRow
//                                {
//                                    SrNo = (results.Count + 1).ToString(),
//                                    Status = "3",
//                                    Name = "Item",
//                                    ItemName = item.ItemName,
//                                    HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
//                                    PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}"),
//                                    Notes = !string.IsNullOrEmpty(item.Notes) ? item.Notes.ToString() : ""
//                                });
//                            }

//                            // 1. Fetch Expense SubItems
//                            var subItemSql = $@"
//     SELECT 
//    ud.ATBUD_SubItemId AS SubItemId,
//    si.ASSI_Name AS SubItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
//    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
//    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
//    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
//WHERE
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
//    ud.ATBUD_CustId = @CustId
//GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";

//                            var subItems = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subItemSql, new
//                            {
//                                YearId = YearId,
//                                PrevYearId = YearId - 1,
//                                ScheduleTypeId = ScheduleTypeID,
//                                CompId = CompId,
//                                CustId = CustId
//                            })).ToList();

//                            // 2. Description underSubItems
//                            foreach (var subItem in subItems)
//                            {
//                                var subitemDescSql = $@"
//             SELECT 
//    ud.ATBUD_ID,
//    ud.ATBUD_Description,
//    si.ASSI_ID,
//    si.ASSI_Name AS SubItemName,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) AS Total,
//    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
//    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
//    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
//    ldg.ASHL_Description AS Notes
//FROM Acc_TrailBalance_Upload_Details ud
//LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
//LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
//    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
//    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
//    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
//    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
//LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
//    AND ldg.ASHL_CustomerId = @CustId AND ldg.ASHL_YearID = @YearId
//WHERE 
//    ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
//    ud.ATBUD_CustId = @CustId AND 
//    ud.ATBUD_compid = @CompId AND 
//    ud.ATBUD_YEARId = @YearId
//GROUP BY ud.ATBUD_ID, ud.ATBUD_Description, si.ASSI_ID, si.ASSI_Name, ldg.ASHL_Description";

//                                var subItemDescriptions = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subitemDescSql, new
//                                {
//                                    YearId = YearId,
//                                    PrevYearId = YearId - 1,
//                                    ScheduleTypeId = ScheduleTypeID,
//                                    CompId = CompId,
//                                    CustId = CustId
//                                })).ToList();

//                                decimal subitemNet = (subItem.CrTotal ?? 0) - (subItem.DbTotal ?? 0);
//                                decimal subitemPrevNet = (subItem.CrTotalPrev ?? 0) - (subItem.DbTotalPrev ?? 0);

//                                if (subitemNet != 0 || subitemPrevNet != 0)
//                                {
//                                    totalIncome += subitemNet;
//                                    totalPrevIncome += subitemPrevNet;

//                                    results.Add(new DetailedReportBalanceSheetRow
//                                    {
//                                        SrNo = (results.Count + 1).ToString(),
//                                        Status = "4",
//                                        Name = "SubItem",
//                                        SubItemName = item.SubItemName,
//                                        HeaderSLNo = subitemNet.ToString($"N{RoundOff}"),
//                                        PrevYearTotal = subitemPrevNet.ToString($"N{RoundOff}"),
//                                        Notes = !string.IsNullOrEmpty(subItem.Notes) ? subItem.Notes.ToString() : ""
//                                    });
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return results;
//        }














