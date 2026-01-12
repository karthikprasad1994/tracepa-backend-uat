using Dapper;
using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Globalization;
using System.Text;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleReportDto;
namespace TracePca.Service.FIN_statement
{
    public class ScheduleReportService : ScheduleReportInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScheduleReportService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetCompanyName
        public async Task<IEnumerable<CompanyDetailsDto>> GetCompanyNameAsync(int CompId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT Company_ID,Company_Name 
        FROM TRACe_CompanyDetails 
        WHERE Company_CompID = @CompID 
        ORDER BY Company_Name";
            return await connection.QueryAsync<CompanyDetailsDto>(query, new { CompID = CompId });
        }

        //GetPartners
        public async Task<IEnumerable<PartnersDto>> LoadCustomerPartnersAsync(int CompId, int DetailsId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
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
            return await connection.QueryAsync<PartnersDto>(query, new { CompId, DetailsId });
        }

        //GetSubHeading
        public async Task<IEnumerable<SubHeadingDto>> GetSubHeadingAsync(int CompId, int ScheduleId, int CustId, int HeadingId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

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

            var result = await connection.QueryAsync<SubHeadingDto>(query, new
            {
                CustId = CustId,
                ScheduleId = ScheduleId,
                HeadingId = HeadingId
            });
            return result;
        }

        //GetItem
        public async Task<IEnumerable<ItemDto>> GetItemAsync(int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

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

            var result = await connection.QueryAsync<ItemDto>(query, new
            {
                CustId = CustId,
                ScheduleId = ScheduleId,
                SubHeadId = SubHeadId
            });
            return result;
        }

        //GetSummaryReportForPandL
        public async Task<IEnumerable<SummaryReportPnLRow>> GetReportSummaryPnLAsync(int CompId, SummaryReportPnL p)
        {
            var results = new List<SummaryReportPnLRow>();
            int ScheduleTypeID = 3;
            int RoundOff = 1;
            decimal fallback = 0; decimal fallbackPrev = 0;
            decimal subNet = 0; decimal subPrevNet = 0;

            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            decimal totalIncome = 0, totalPrevIncome = 0;
            decimal totalExpense = 0, totalPrevExpense = 0;
            decimal GtotalExpense = 0, GtotalPrevExpense = 0;

            var query = @"
select CUST_ORGID as OrgId 
from SAD_CUSTOMER_MASTER 
where CUST_ID = @CompanyId and CUST_DELFLG = 'A'";
            int AuditId = await connection.QueryFirstOrDefaultAsync<int>(query, new
            {
                CompanyId = p.CustID
            });
            if (AuditId == 1 || AuditId == 0)  //ICAI Audit
            {
                //🟩 INCOME HEADINGS
                var incomeHeadingSql = @"
            SELECT AST_HeadingID AS HeadingId, ASH_Name AS Name
            FROM ACC_ScheduleTemplates
            LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
            WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 1
            GROUP BY AST_HeadingID, ASH_Name
            ORDER BY AST_HeadingID";
                var incomeHeadings = await connection.QueryAsync<SummaryReportPnLRow>(incomeHeadingSql, new { ScheduleTypeID, CustID = p.CustID });
                {
                    foreach (var heading in incomeHeadings)
                    {
                        var headingBalanceSql = @"
SELECT ud.ATBUD_Headingid AS HeadingID, h.ASH_Name AS Name,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,    h.ASH_Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 1
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
    AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
    AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE  ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and   ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes ORDER BY ud.ATBUD_Headingid";
                        var headingBalance = await connection.QueryAsync<SummaryReportPnLRow>(headingBalanceSql, new
                        {
                            YearID = p.YearID,
                            PrevYearID = p.YearID - 1,
                            CustID = p.CustID,
                            BranchId = p.BranchId,
                            ScheduleTypeID,
                            HeadingID = heading.HeadingId,
                            CompID = CompId
                        });
                        results.Add(new SummaryReportPnLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Name = heading.Name,
                            HeaderSLNo = "",
                            PrevYearTotal = "",
                            status = "1"
                        });
                        var incomeSubSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 1 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";
                        var incomeSubHeadings = await connection.QueryAsync<SummaryReportPnLRow>(incomeSubSql, new
                        {
                            ScheduleTypeID,
                            CustID = p.CustID,
                            BranchId = p.BranchId,
                            HeadingID = heading.HeadingId
                        });
                        foreach (var sub in incomeSubHeadings)
                        {
                            var subBalSql = @"
SELECT  ud.ATBUD_Subheading AS SubHeadingID,
    sh.ASSH_Name AS Name, sh.ASSH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubHeading sh ON sh.ASSH_ID = ud.ATBUD_Subheading
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_CustId = @CustID
GROUP BY ud.ATBUD_Subheading, sh.ASSH_Name, sh.ASSH_Notes
ORDER BY ud.ATBUD_Subheading";
                            var subBalance = await connection.QueryAsync<SummaryReportPnLRow>(subBalSql, new
                            {
                                YearID = p.YearID,
                                PrevYearID = p.YearID - 1,
                                CustID = p.CustID,
                                BranchId = p.BranchId,
                                ScheduleTypeID,
                                SubHeadingID = sub.SubHeadingID
                            });
                            if (subBalance.Count() > 0)
                            {
                                foreach (var subBalances in subBalance)
                                {
                                    if (subBalances.Name != "")
                                    {
                                        subNet = (subBalances?.CrTotal ?? 0) - (subBalances?.DbTotal ?? 0);
                                        subPrevNet = (subBalances?.PrevCrTotal ?? 0) - (subBalances?.PrevDbTotal ?? 0);
                                        totalIncome += subNet;
                                        totalPrevIncome += subPrevNet;
                                        results.Add(new SummaryReportPnLRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Name = subBalances.Name,
                                            HeaderSLNo = subNet == 0 ? "-" : subNet.ToString($"N{RoundOff}"),
                                            PrevYearTotal = subPrevNet == 0 ? "-" : subPrevNet.ToString($"N{RoundOff}"),
                                            Notes = subBalances.Notes != "0" ? subBalances.Notes.ToString() : "",
                                            status = "2"
                                        });
                                    }
                                }
                            }
                            else
                            {
                                results.Add(new SummaryReportPnLRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Name = sub.Name,
                                    HeaderSLNo = "",
                                    PrevYearTotal = "",
                                    status = "2"
                                });
                            }
                        }
                    }
                    results.Add(new SummaryReportPnLRow
                    {
                        SrNo = (results.Count + 1).ToString(),
                        Name = "III Total Income",
                        status = "1",
                        HeaderSLNo = totalIncome == 0 ? "-" : totalIncome.ToString($"N{RoundOff}"),
                        PrevYearTotal = totalPrevIncome == 0 ? "-" : totalPrevIncome.ToString($"N{RoundOff}")
                    });
                    results.Add(new SummaryReportPnLRow
                    {
                        status = "2"
                    });
                }

                // 🟥 EXPENSE HEADINGS
                subNet = 0; subPrevNet = 0;
                var expenseHeadingSql = @"
SELECT AST_HeadingID AS HeadingID, ASH_Name AS Name
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 2
GROUP BY AST_HeadingID, ASH_Name
ORDER BY AST_HeadingID";
                var expenseHeadings = await connection.QueryAsync<SummaryReportPnLRow>(expenseHeadingSql, new { ScheduleTypeID, CustID = p.CustID });
                foreach (var heading in expenseHeadings)
                {
                    var expenseBalanceSql = @"
SELECT   ud.ATBUD_Headingid AS HeadingID,
    h.ASH_Name AS Name,    h.ASH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 2
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes ORDER BY ud.ATBUD_Headingid";
                    var headingBalance = await connection.QueryAsync<SummaryReportPnLRow>(expenseBalanceSql, new
                    {
                        YearID = p.YearID,
                        PrevYearID = p.YearID - 1,
                        CustID = p.CustID,
                        BranchId = p.BranchId,
                        ScheduleTypeID,
                        HeadingID = heading.HeadingId,
                        CompID = CompId
                    });
                    if (headingBalance.Count() > 0)
                    {
                        foreach (var headingBalances in headingBalance)
                        {
                            if (headingBalances.Name != null)
                            {
                                decimal Net = (headingBalances?.CrTotal ?? 0) - (headingBalances?.DbTotal ?? 0);
                                decimal PrevNet = (headingBalances?.PrevCrTotal ?? 0) - (headingBalances?.PrevDbTotal ?? 0);
                                if (Net != 0 || PrevNet != 0)
                                {
                                    results.Add(new SummaryReportPnLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Name = heading.Name,
                                        HeaderSLNo = "",
                                        PrevYearTotal = "",
                                        status = "1"
                                    });
                                }
                                else
                                {
                                    results.Add(new SummaryReportPnLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Name = heading.Name,
                                        HeaderSLNo = fallback == 0 ? "-" : fallback.ToString($"N{RoundOff}"),
                                        PrevYearTotal = fallbackPrev == 0 ? "-" : fallbackPrev.ToString($"N{RoundOff}"),
                                        status = "1"
                                    });
                                }
                                subNet = 0; subPrevNet = 0;
                                var subSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 2 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";
                                var subHeadings = await connection.QueryAsync<SummaryReportPnLRow>(subSql, new
                                {
                                    ScheduleTypeID,
                                    CustID = p.CustID,
                                    BranchId = p.BranchId,
                                    HeadingID = heading.HeadingId
                                });
                                if (subHeadings != null)
                                {
                                    foreach (var sub in subHeadings)
                                    {
                                        var subBalSql = @"SELECT   ud.ATBUD_Subheading AS SubHeadingID,
    ssh.ASSH_Name AS Name,    ssh.ASSH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubHeading ssh ON ssh.ASSH_ID = ud.ATBUD_Subheading
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_CustId = @CustID
GROUP BY ud.ATBUD_Subheading, ssh.ASSH_Name, ssh.ASSH_Notes
ORDER BY ud.ATBUD_Subheading";
                                        var subBalance = await connection.QueryAsync<SummaryReportPnLRow>(subBalSql, new
                                        {
                                            YearID = p.YearID,
                                            PrevYearID = p.YearID - 1,
                                            CustID = p.CustID,
                                            BranchId = p.BranchId,
                                            ScheduleTypeID,
                                            SubHeadingID = sub.SubHeadingID
                                        });
                                        if (subBalance.Any())
                                        {
                                            foreach (var subBalances in subBalance)
                                            {
                                                if (subBalances.Name != "")
                                                {
                                                    decimal resultsubNet; decimal resultsubPrevNet;
                                                    subNet = (subBalances?.DbTotal ?? 0) - (subBalances?.CrTotal ?? 0);
                                                    resultsubNet = subNet;
                                                    subPrevNet = (subBalances?.PrevDbTotal ?? 0) - (subBalances?.PrevCrTotal ?? 0);
                                                    resultsubPrevNet = subPrevNet;
                                                    totalExpense += subNet;
                                                    totalPrevExpense += subPrevNet;
                                                    results.Add(new SummaryReportPnLRow
                                                    {
                                                        SrNo = (results.Count + 1).ToString(),
                                                        Name = subBalances.Name,
                                                        HeaderSLNo = resultsubNet == 0 ? "-" : resultsubNet.ToString($"N{RoundOff}"),
                                                        PrevYearTotal = resultsubPrevNet == 0 ? "-" : resultsubPrevNet.ToString($"N{RoundOff}"),
                                                        Notes = subBalances.Notes != "0" ? subBalances.Notes.ToString() : "",
                                                        status = "2"
                                                    });
                                                }
                                                else
                                                {
                                                    results.Add(new SummaryReportPnLRow
                                                    {
                                                        SrNo = (results.Count + 1).ToString(),
                                                        Name = sub.Name,
                                                        HeaderSLNo = "0",
                                                        PrevYearTotal = "0",
                                                        Notes = "0",
                                                        status = "1"
                                                    });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            results.Add(new SummaryReportPnLRow
                                            {
                                                SrNo = (results.Count + 1).ToString(),
                                                Name = sub.Name,
                                                HeaderSLNo = "",
                                                PrevYearTotal = "",
                                                status = "2"
                                            });
                                        }
                                    }
                                    results.Add(new SummaryReportPnLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Name = "Total ",
                                        status = "1",
                                        HeaderSLNo = totalExpense == 0 ? "-" : totalExpense.ToString($"N{RoundOff}"),
                                        PrevYearTotal = totalPrevExpense == 0 ? "-" : totalPrevExpense.ToString($"N{RoundOff}")
                                    });
                                }
                                GtotalExpense += totalExpense; GtotalPrevExpense += totalPrevExpense;
                                // end sub head
                            }
                        }
                    }
                    else
                    {
                        fallback = totalIncome - GtotalExpense;
                        fallbackPrev = totalPrevIncome - GtotalPrevExpense;
                        totalExpense = 0; totalPrevExpense = 0;
                        results.Add(new SummaryReportPnLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Name = heading.Name,
                            HeaderSLNo = fallback == 0 ? "-" : fallback.ToString($"N{RoundOff}"),
                            PrevYearTotal = fallbackPrev == 0 ? "-" : fallbackPrev.ToString($"N{RoundOff}"),
                            status = "1"
                        });
                        subNet = 0; subPrevNet = 0;
                        var subSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
inner JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 2 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";
                        var subHeadings = await connection.QueryAsync<SummaryReportPnLRow>(subSql, new
                        {
                            ScheduleTypeID,
                            CustID = p.CustID,
                            BranchId = p.BranchId,
                            HeadingID = heading.HeadingId
                        });
                        if (subHeadings.Any())
                        {
                            foreach (var sub in subHeadings)
                            {
                                if (sub.Name != "")
                                {
                                    results.Add(new SummaryReportPnLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Name = sub.Name,
                                        HeaderSLNo = "-",
                                        PrevYearTotal = "-",
                                        Notes = sub.Notes != "0" ? sub.Notes.ToString() : "",
                                        status = "2"
                                    });
                                }
                            }
                        }
                    }
                }
            }
            else  //PCAOB Audit
            {
                //🟩 INCOME HEADINGS
                var incomeHeadingSql = @"
            SELECT AST_HeadingID AS HeadingId, ASH_Name AS Name
            FROM ACC_ScheduleTemplates
            LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
            WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 1
            GROUP BY AST_HeadingID, ASH_Name
            ORDER BY AST_HeadingID";
                var incomeHeadings = await connection.QueryAsync<SummaryReportPnLRow>(incomeHeadingSql, new { ScheduleTypeID, CustID = p.CustID });
                {
                    foreach (var heading in incomeHeadings)
                    {
                        var headingBalanceSql = @"
SELECT ud.ATBUD_Headingid AS HeadingID, h.ASH_Name AS Name,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,    h.ASH_Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 1
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
    AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
    AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE  ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and   ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes ORDER BY ud.ATBUD_Headingid";
                        var headingBalance = await connection.QueryAsync<SummaryReportPnLRow>(headingBalanceSql, new
                        {
                            YearID = p.YearID,
                            PrevYearID = p.YearID - 1,
                            CustID = p.CustID,
                            BranchId = p.BranchId,
                            ScheduleTypeID,
                            HeadingID = heading.HeadingId,
                            CompID = CompId
                        });
                        results.Add(new SummaryReportPnLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Name = heading.Name,
                            HeaderSLNo = "",
                            PrevYearTotal = "",
                            status = "1"
                        });
                        var incomeSubSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 1 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";
                        var incomeSubHeadings = await connection.QueryAsync<SummaryReportPnLRow>(incomeSubSql, new
                        {
                            ScheduleTypeID,
                            CustID = p.CustID,
                            BranchId = p.BranchId,
                            HeadingID = heading.HeadingId
                        });
                        foreach (var sub in incomeSubHeadings)
                        {
                            var subBalSql = @"
SELECT  ud.ATBUD_Subheading AS SubHeadingID,
    sh.ASSH_Name AS Name, sh.ASSH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubHeading sh ON sh.ASSH_ID = ud.ATBUD_Subheading
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_CustId = @CustID
GROUP BY ud.ATBUD_Subheading, sh.ASSH_Name, sh.ASSH_Notes
ORDER BY ud.ATBUD_Subheading";
                            var subBalance = await connection.QueryAsync<SummaryReportPnLRow>(subBalSql, new
                            {
                                YearID = p.YearID,
                                PrevYearID = p.YearID - 1,
                                CustID = p.CustID,
                                BranchId = p.BranchId,
                                ScheduleTypeID,
                                SubHeadingID = sub.SubHeadingID
                            });
                            if (subBalance.Count() > 0)
                            {
                                foreach (var subBalances in subBalance)
                                {
                                    if (subBalances.Name != "")
                                    {
                                        subNet = (subBalances?.CrTotal ?? 0) - (subBalances?.DbTotal ?? 0);
                                        subPrevNet = (subBalances?.PrevCrTotal ?? 0) - (subBalances?.PrevDbTotal ?? 0);
                                        totalIncome += subNet;
                                        totalPrevIncome += subPrevNet;
                                        results.Add(new SummaryReportPnLRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Name = subBalances.Name,
                                            HeaderSLNo = subNet == 0 ? "-" : subNet.ToString($"N{RoundOff}"),
                                            PrevYearTotal = subPrevNet == 0 ? "-" : subPrevNet.ToString($"N{RoundOff}"),
                                            Notes = subBalances.Notes != "0" ? subBalances.Notes.ToString() : "",
                                            status = "2"
                                        });
                                    }
                                }
                            }
                            else
                            {
                                results.Add(new SummaryReportPnLRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Name = sub.Name,
                                    HeaderSLNo = "",
                                    PrevYearTotal = "",
                                    status = "2"
                                });
                            }
                        }
                    }
                    results.Add(new SummaryReportPnLRow
                    {
                        SrNo = (results.Count + 1).ToString(),
                        Name = "III Total Income",
                        status = "1",
                        HeaderSLNo = totalIncome == 0 ? "-" : totalIncome.ToString($"N{RoundOff}"),
                        PrevYearTotal = totalPrevIncome == 0 ? "-" : totalPrevIncome.ToString($"N{RoundOff}")
                    });
                    results.Add(new SummaryReportPnLRow
                    {
                        status = "2"
                    });
                }

                // 🟥 EXPENSE HEADINGS
                subNet = 0; subPrevNet = 0;
                var expenseHeadingSql = @"
SELECT AST_HeadingID AS HeadingID, ASH_Name AS Name
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 2
GROUP BY AST_HeadingID, ASH_Name
ORDER BY AST_HeadingID";
                var expenseHeadings = await connection.QueryAsync<SummaryReportPnLRow>(expenseHeadingSql, new { ScheduleTypeID, CustID = p.CustID });
                foreach (var heading in expenseHeadings)
                {
                    var expenseBalanceSql = @"
SELECT   ud.ATBUD_Headingid AS HeadingID,
    h.ASH_Name AS Name,    h.ASH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 2
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes ORDER BY ud.ATBUD_Headingid";
                    var headingBalance = await connection.QueryAsync<SummaryReportPnLRow>(expenseBalanceSql, new
                    {
                        YearID = p.YearID,
                        PrevYearID = p.YearID - 1,
                        CustID = p.CustID,
                        BranchId = p.BranchId,
                        ScheduleTypeID,
                        HeadingID = heading.HeadingId,
                        CompID = CompId
                    });
                    if (headingBalance.Count() > 0)
                    {
                        foreach (var headingBalances in headingBalance)
                        {
                            if (headingBalances.Name != null)
                            {
                                decimal Net = (headingBalances?.CrTotal ?? 0) - (headingBalances?.DbTotal ?? 0);
                                decimal PrevNet = (headingBalances?.PrevCrTotal ?? 0) - (headingBalances?.PrevDbTotal ?? 0);
                                if (Net != 0 || PrevNet != 0)
                                {
                                    results.Add(new SummaryReportPnLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Name = heading.Name,
                                        HeaderSLNo = "",
                                        PrevYearTotal = "",
                                        status = "1"
                                    });
                                }
                                else
                                {
                                    results.Add(new SummaryReportPnLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Name = heading.Name,
                                        HeaderSLNo = fallback == 0 ? "-" : fallback.ToString($"N{RoundOff}"),
                                        PrevYearTotal = fallbackPrev == 0 ? "-" : fallbackPrev.ToString($"N{RoundOff}"),
                                        status = "1"
                                    });
                                }
                                subNet = 0; subPrevNet = 0;
                                var subSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 2 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";
                                var subHeadings = await connection.QueryAsync<SummaryReportPnLRow>(subSql, new
                                {
                                    ScheduleTypeID,
                                    CustID = p.CustID,
                                    BranchId = p.BranchId,
                                    HeadingID = heading.HeadingId
                                });
                                if (subHeadings != null)
                                {
                                    foreach (var sub in subHeadings)
                                    {
                                        var subBalSql = @"SELECT   ud.ATBUD_Subheading AS SubHeadingID,
    ssh.ASSH_Name AS Name,    ssh.ASSH_Notes AS Notes,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubHeading ssh ON ssh.ASSH_ID = ud.ATBUD_Subheading
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_CustId = @CustID
GROUP BY ud.ATBUD_Subheading, ssh.ASSH_Name, ssh.ASSH_Notes
ORDER BY ud.ATBUD_Subheading";
                                        var subBalance = await connection.QueryAsync<SummaryReportPnLRow>(subBalSql, new
                                        {
                                            YearID = p.YearID,
                                            PrevYearID = p.YearID - 1,
                                            CustID = p.CustID,
                                            BranchId = p.BranchId,
                                            ScheduleTypeID,
                                            SubHeadingID = sub.SubHeadingID
                                        });
                                        if (subBalance.Any())
                                        {
                                            foreach (var subBalances in subBalance)
                                            {
                                                if (subBalances.Name != "")
                                                {
                                                    decimal resultsubNet; decimal resultsubPrevNet;
                                                    subNet = (subBalances?.DbTotal ?? 0) - (subBalances?.CrTotal ?? 0);
                                                    resultsubNet = subNet;
                                                    subPrevNet = (subBalances?.PrevDbTotal ?? 0) - (subBalances?.PrevCrTotal ?? 0);
                                                    resultsubPrevNet = subPrevNet;
                                                    totalExpense += subNet;
                                                    totalPrevExpense += subPrevNet;
                                                    results.Add(new SummaryReportPnLRow
                                                    {
                                                        SrNo = (results.Count + 1).ToString(),
                                                        Name = subBalances.Name,
                                                        HeaderSLNo = resultsubNet == 0 ? "-" : resultsubNet.ToString($"N{RoundOff}"),
                                                        PrevYearTotal = resultsubPrevNet == 0 ? "-" : resultsubPrevNet.ToString($"N{RoundOff}"),
                                                        Notes = subBalances.Notes != "0" ? subBalances.Notes.ToString() : "",
                                                        status = "2"
                                                    });
                                                }
                                                else
                                                {
                                                    results.Add(new SummaryReportPnLRow
                                                    {
                                                        SrNo = (results.Count + 1).ToString(),
                                                        Name = sub.Name,
                                                        HeaderSLNo = "0",
                                                        PrevYearTotal = "0",
                                                        Notes = "0",
                                                        status = "1"
                                                    });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            results.Add(new SummaryReportPnLRow
                                            {
                                                SrNo = (results.Count + 1).ToString(),
                                                Name = sub.Name,
                                                HeaderSLNo = "",
                                                PrevYearTotal = "",
                                                status = "2"
                                            });
                                        }
                                    }
                                    results.Add(new SummaryReportPnLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Name = "Total",
                                        status = "1",
                                        HeaderSLNo = totalExpense == 0 ? "-" : totalExpense.ToString($"N{RoundOff}"),
                                        PrevYearTotal = totalPrevExpense == 0 ? "-" : totalPrevExpense.ToString($"N{RoundOff}")
                                    });
                                }
                                GtotalExpense += totalExpense; GtotalPrevExpense += totalPrevExpense;
                                totalExpense = 0; totalPrevExpense = 0;

                                // end sub head
                            }
                        }
                    }
                    else
                    {
                        fallback = totalIncome - GtotalExpense;
                        fallbackPrev = totalPrevIncome - GtotalPrevExpense;
                        totalExpense = 0; totalPrevExpense = 0;
                        results.Add(new SummaryReportPnLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Name = heading.Name,
                            HeaderSLNo = fallback == 0 ? "-" : fallback.ToString($"N{RoundOff}"),
                            PrevYearTotal = fallbackPrev == 0 ? "-" : fallbackPrev.ToString($"N{RoundOff}"),
                            status = "1"
                        });
                        subNet = 0; subPrevNet = 0;
                        var subSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
inner JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 2 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";
                        var subHeadings = await connection.QueryAsync<SummaryReportPnLRow>(subSql, new
                        {
                            ScheduleTypeID,
                            CustID = p.CustID,
                            BranchId = p.BranchId,
                            HeadingID = heading.HeadingId
                        });
                        if (subHeadings.Any())
                        {
                            foreach (var sub in subHeadings)
                            {
                                if (sub.Name != "")
                                {
                                    results.Add(new SummaryReportPnLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Name = sub.Name,
                                        HeaderSLNo = "-",
                                        PrevYearTotal = "-",
                                        Notes = sub.Notes != "0" ? sub.Notes.ToString() : "",
                                        status = "2"
                                    });
                                }
                            }
                        }
                    }                  
                }
                fallback = totalIncome - GtotalExpense;
                fallbackPrev = totalPrevIncome - GtotalPrevExpense;
                totalExpense = 0; totalPrevExpense = 0;
                results.Add(new SummaryReportPnLRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = "Net Income",
                    HeaderSLNo = fallback == 0 ? "-" : fallback.ToString($"N{RoundOff}"),
                    PrevYearTotal = fallbackPrev == 0 ? "-" : fallbackPrev.ToString($"N{RoundOff}"),
                    status = "1"
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
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);
            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            decimal totalIncome = 0, totalPrevIncome = 0;
            decimal totalExpense = 0, totalPrevExpense = 0;

            var query = @"
select CUST_ORGID as OrgId 
from SAD_CUSTOMER_MASTER 
where CUST_ID = @CompanyId and CUST_DELFLG = 'A'";
            int AuditId = await connection.QueryFirstOrDefaultAsync<int>(query, new
            {
                CompanyId = p.CustID
            });

            if (AuditId == 1 || AuditId == 0)  //ICAI Audit
            {

            }

                // 🟩 INCOME HEADINGS
                // Heading
                results.Add(new SummaryReportBalanceSheetRow
            {
                SrNo = "",
                Name = "EQUITY AND LIABILITIES",
                HeaderSLNo = "",
                PrevYearTotal = "",
                status = "1"
            });

            var incomeHeadingSql = @"
SELECT AST_HeadingID AS HeadingID, ASH_Name AS Name
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 2
GROUP BY AST_HeadingID, ASH_Name
ORDER BY AST_HeadingID";

            var incomeHeadings = await connection.QueryAsync<(int HeadingID, string Name)>(incomeHeadingSql, new { ScheduleTypeID, CustID = p.CustID });

            foreach (var heading in incomeHeadings)
            {
                var headingBalanceSql = @"
SELECT  ud.ATBUD_Headingid AS HeadingID,
    h.ASH_Name AS Name,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,
    h.ASH_Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 2
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
    AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND d.ATBU_Branchid = ud.Atbud_Branchnameid  AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description
    AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE  ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and   ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes
ORDER BY ud.ATBUD_Headingid";

                var headingBalance = await connection.QueryFirstOrDefaultAsync(headingBalanceSql, new
                {
                    YearID = p.YearID,
                    PrevYearID = p.YearID - 1,
                    CustID = p.CustID,
                    BranchId = p.BranchId,
                    ScheduleTypeID,
                    HeadingID = heading.HeadingID,
                    CompID = CompId
                });

                results.Add(new SummaryReportBalanceSheetRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = heading.Name,
                    HeaderSLNo = "",
                    PrevYearTotal = "",
                    status = "1"
                });

                var incomeSubSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 2 AND AST_HeadingID = @HeadingID
GROUP BY AST_SubHeadingID, ASsH_Name, ASSH_Notes";

                var incomeSubHeadings = await connection.QueryAsync<(int SubHeadingID, string Name, int Notes)>(incomeSubSql, new
                {
                    ScheduleTypeID,
                    CustID = p.CustID,
                    HeadingID = heading.HeadingID
                });
                if (incomeSubHeadings != null)
                {
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
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_CustId = @CustID
GROUP BY ud.ATBUD_Subheading, sh.ASSH_Name, sh.ASSH_Notes
ORDER BY ud.ATBUD_Subheading";
                        var subBalance = await connection.QueryFirstOrDefaultAsync(subBalSql, new
                        {
                            YearID = p.YearID,
                            PrevYearID = p.YearID - 1,
                            CustID = p.CustID,
                            BranchId = p.BranchId,
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
                            Notes = sub.Notes != 0 ? sub.Notes.ToString() : "",
                            status = "2"
                        });
                    }
                }
            }
            results.Add(new SummaryReportBalanceSheetRow
            {
                SrNo = (results.Count + 1).ToString(),
                Name = "Total ",
                HeaderSLNo = totalIncome == 0 ? "-" : totalIncome.ToString($"N{RoundOff}"),
                PrevYearTotal = totalPrevIncome == 0 ? "-" : totalPrevIncome.ToString($"N{RoundOff}"),
                status = "1"
            });
            results.Add(new SummaryReportBalanceSheetRow
            {
                SrNo = "",
                status = "2"
            });

            // 🟥 EXPENSE HEADINGS

            // Heading
            results.Add(new SummaryReportBalanceSheetRow
            {
                SrNo = "",
                Name = "Assets",
                HeaderSLNo = "",
                PrevYearTotal = "",
                status = "1"
            });
            var expenseHeadingSql = @"
SELECT AST_HeadingID AS HeadingID, ASH_Name AS Name
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleHeading a ON a.ash_id = AST_HeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND a.ASH_Notes = 1
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
LEFT JOIN ACC_ScheduleHeading h ON h.ASH_ID = ud.ATBUD_Headingid AND h.ASH_Notes = 1
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_compid = @CompID AND ud.ATBUD_CustId = @CustID AND ud.ATBUD_Headingid = @HeadingID
    AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s WHERE s.AST_HeadingID = ud.ATBUD_Headingid AND s.AST_AccHeadId IN (1, 2))
GROUP BY ud.ATBUD_Headingid, h.ASH_Name, h.ASH_Notes
ORDER BY ud.ATBUD_Headingid";
                var headingBalance = await connection.QueryFirstOrDefaultAsync(expenseBalanceSql, new
                {
                    YearID = p.YearID,
                    PrevYearID = p.YearID - 1,
                    CustID = p.CustID,
                    BranchId = p.BranchId,
                    ScheduleTypeID,
                    HeadingID = heading.HeadingID,
                    CompID = CompId
                });
                results.Add(new SummaryReportBalanceSheetRow
                {
                    SrNo = (results.Count + 1).ToString(),
                    Name = heading.Name,
                    HeaderSLNo = "",
                    PrevYearTotal = "",
                    status = "1"
                });

                var subSql = @"
SELECT AST_SubHeadingID AS SubHeadingID, ASsH_Name AS Name, ASSH_Notes AS Notes
FROM ACC_ScheduleTemplates
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = AST_SubHeadingID
WHERE AST_Schedule_type = @ScheduleTypeID AND AST_Companytype = @CustID AND AST_AccHeadId = 1 AND AST_HeadingID = @HeadingID
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
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearID AND d.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @YearID AND d.ATBU_Branchid = ud.Atbud_Branchnameid AND d.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YEARId = @PrevYearID AND e.ATBU_CustId = @CustID AND ud.ATBUD_YEARId = @PrevYearID AND e.ATBU_Branchid = ud.Atbud_Branchnameid AND e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and ud.ATBUD_Subheading = @SubHeadingID AND ud.ATBUD_Schedule_type = @ScheduleTypeID AND ud.ATBUD_CustId = @CustID
GROUP BY ud.ATBUD_Subheading, ssh.ASSH_Name, ssh.ASSH_Notes
ORDER BY ud.ATBUD_Subheading";

                    var subBalance = await connection.QueryFirstOrDefaultAsync(subBalSql, new
                    {
                        YearID = p.YearID,
                        PrevYearID = p.YearID - 1,
                        CustID = p.CustID,
                        BranchId = p.BranchId,
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
                        Notes = sub.Notes != 0 ? sub.Notes.ToString() : "",
                        status = "2"
                    });
                }
            }
            results.Add(new SummaryReportBalanceSheetRow
            {
                SrNo = (results.Count + 1).ToString(),
                Name = "Total",
                HeaderSLNo = totalExpense == 0 ? "-" : totalExpense.ToString($"N{RoundOff}"),
                PrevYearTotal = totalPrevExpense == 0 ? "-" : totalPrevExpense.ToString($"N{RoundOff}"),
                status = "1"
            });
            return results;
        }

        //GetDetailedReportPandL
        public async Task<IEnumerable<DetailedReportPandLRow>> GetDetailedReportPandLAsync(int CompId, DetailedReportPandL p)
        {
            var results = new List<DetailedReportPandLRow>();
            int ScheduleTypeID = 3;
            int RoundOff = 1;
            int YearId = p.YearID;
            int CustId = p.CustID;
            string selectedBranches = p.BranchId;
            decimal totalIncome = 0, totalPrevIncome = 0;

            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // 1. Fetch Account Head -1 Heading
            var headingSql = $@"
SELECT DISTINCT ATBUD_Headingid AS HeadingID,ASH_Name AS HeadingName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,    a.ASH_Notes
FROM Acc_TrailBalance_Upload_Details
LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_Headingid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description
WHERE ATBUD_Schedule_type = @ScheduleTypeId AND ATBUD_compid = @CompanyId AND ATBUD_CustId = @CustomerId
  AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s 
      WHERE s.AST_HeadingID = ATBUD_Headingid AND s.AST_AccHeadId = 1 )
GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes ORDER BY ATBUD_Headingid";
            var headings = await connection.QueryAsync<DetailedReportPandLRow>(headingSql, new
            {
                ScheduleTypeId = ScheduleTypeID,
                CompanyId = CompId,
                CustomerId = CustId
            });
            if (headings != null)
            {
                foreach (var heading in headings)
                {
                    // 1. Fetch SubHeading
                    var subHeadingTotalsSql = $@"
                        select distinct(ATBUD_Subheading) as SubHeadingID,a.ASSH_Name as SubHeadingName,a.AsSh_Notes as Notes,
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0  And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid  And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 where ATBUD_Headingid = @HeadingId And ATBUD_Subheading<>0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId  And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description order by ATBUD_Subheading";
                    var subHeadingTotals = await connection.QueryAsync<DetailedReportPandLRow>(subHeadingTotalsSql, new
                    {
                        YearId,
                        PrevYearId = YearId - 1,
                        CustomerId = CustId,
                        HeadingId = heading.HeadingId,
                        BranchId = selectedBranches,
                        ScheduleTypeID
                    });
                    // 3. Descriptions under Subheading
                    foreach (var subBalance in subHeadingTotals)
                    {
                        if (subBalance.CrTotal1 != 0 && subBalance.CrTotal1 != 0)
                        {
                            results.Add(new DetailedReportPandLRow
                            {
                                SrNo = (results.Count + 1).ToString(),
                                Status = "1",
                                Name = subBalance.SubHeadingName,
                                HeaderSLNo = "",
                                PrevYearTotal = "",
                                Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
                            });
                            var descriptionSql = @"
                            select distinct(ATBUD_Subheading),a.ASSH_Name as headingname,a.AsSh_Notes as Notes,ATBUD_Description as Name, 
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0  And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid  And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 where ATBUD_Subheading= @subHeadingId  And ATBUD_Subheading<>0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId  And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description,ATBUD_Description order by ATBUD_Subheading";
                            var descs = await connection.QueryAsync<DetailedReportPandLRow>(descriptionSql, new
                            {
                                YearId,
                                PrevYearId = YearId - 1,
                                CustomerId = CustId,
                                HeadingId = heading.HeadingId,
                                subHeadingId = subBalance.SubHeadingID,
                                BranchId = selectedBranches,
                                ScheduleTypeID
                            });
                            foreach (var desc in descs)
                            {
                                if (desc.Name != null)
                                {
                                    decimal subNet = (desc.CrTotal1 ?? 0) - (desc.DbTotal1 ?? 0);
                                    decimal subPrevNet = (desc.CrTotalPrev ?? 0) - (desc.DbTotalPrev ?? 0);
                                    totalIncome += subNet;
                                    totalPrevIncome += subPrevNet;
                                    results.Add(new DetailedReportPandLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Status = "2",
                                        Name = desc.Name,
                                        HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                                        PrevYearTotal = subPrevNet.ToString($"N{RoundOff}")
                                    });
                                }
                            }
                            // 1. Fetch Item
                            var itemSql = @" SELECT ud.ATBUD_itemid AS ItemId,i.ASI_Name AS ItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
    ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleItems i ON i.ASI_ID = ud.ATBUD_itemid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0 
LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ud.ATBUD_Description
    AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
WHERE    ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId and ATBUD_Headingid=@HeadingId And ATBUD_Subheading= @subHeadingId  And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
GROUP BY ud.ATBUD_itemid, i.ASI_Name";
                            var items = await connection.QueryAsync<DetailedReportPandLRow>(itemSql, new
                            {
                                YearId = YearId,
                                PrevYearId = YearId - 1,
                                ScheduleTypeId = ScheduleTypeID,
                                HeadingId = heading.HeadingId,
                                subHeadingId = subBalance.SubHeadingID,
                                CompanyId = CompId,
                                BranchId = selectedBranches,
                                CustomerId = CustId
                            });
                            // 2. Description under Item
                            foreach (var item in items)
                            {
                                if (item.ItemName != null)
                                {
                                    results.Add(new DetailedReportPandLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Status = "1",
                                        Name = item.ItemName,
                                        HeaderSLNo = "",
                                        PrevYearTotal = ""
                                    });
                                    {
                                        var itemDescSql = @"
SELECT ud.ATBUD_Description as Name,ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description   And b.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description  And e.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
WHERE   ud.ATBUD_Subheading=@subHeadingId and ud.ATBUD_itemid=@ItemId and ud.ATBUD_SubItemId=0  and
    ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_compid = @CompanyId  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";
                                        var itemDescriptions = await connection.QueryAsync<DetailedReportPandLRow>(itemDescSql, new
                                        {
                                            YearId = YearId,
                                            PrevYearId = YearId - 1,
                                            ScheduleTypeId = ScheduleTypeID,
                                            subHeadingId = subBalance.SubHeadingID,
                                            ItemId = item.ItemID,
                                            CompanyId = CompId,
                                            BranchId = selectedBranches,
                                            CustomerId = CustId
                                        });
                                        foreach (var itemDescription in itemDescriptions)
                                        {
                                            decimal itemNet = (item.CrTotal ?? 0) - (item.DbTotal ?? 0);
                                            decimal itemPrevNet = (item.CrTotalPrev ?? 0) - (item.DbTotalPrev ?? 0);
                                            if (itemNet != 0 || itemPrevNet != 0)
                                            {
                                                totalIncome += itemNet;
                                                totalPrevIncome += itemPrevNet;
                                                results.Add(new DetailedReportPandLRow
                                                {
                                                    SrNo = (results.Count + 1).ToString(),
                                                    Status = "2",
                                                    Name = itemDescription.Name,
                                                    HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
                                                    PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}")
                                                });
                                            }
                                        }
                                        // 1. Fetch  SubItems
                                        var subItemSql = $@"
SELECT  ud.ATBUD_SubItemId AS SubItemId,
    si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
    AND d.ATBU_Branchid = ud.Atbud_Branchnameid  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
    AND f.ATBU_Branchid = ud.Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.ATBUD_Headingid=@HeadingId And ud.ATBUD_Subheading=@subHeadingId  And ud.ATBUD_SubItemId<>0 And ud.ATBUD_itemid =@ItemId 
   and  ud.ATBUD_Schedule_type = @ScheduleTypeId AND  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) and
    ud.ATBUD_CustId = @CustId GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";
                                        var subItems = (await connection.QueryAsync<DetailedReportPandLRow>(subItemSql, new
                                        {
                                            YearId = YearId,
                                            PrevYearId = YearId - 1,
                                            ScheduleTypeId = ScheduleTypeID,
                                            HeadingId = heading.HeadingId,
                                            subHeadingId = subBalance.SubHeadingID,
                                            ItemId = item.ItemID,
                                            CompId = CompId,
                                            BranchId = selectedBranches,
                                            CustId = CustId
                                        })).ToList();
                                        // 2. Descriptions under subitem
                                        foreach (var subItem in subItems)
                                        {
                                            var subDescSql = $@"
SELECT ud.ATBUD_ID,ud.ATBUD_Description  as Name,si.ASSI_ID,si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) AS Total,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
    AND d.ATBU_Branchid = ud.Atbud_Branchnameid  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
    AND f.ATBU_Branchid = ud.Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustId AND ldg.ASHL_YearID = @YearId
WHERE  ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustId AND 
    ud.ATBUD_compid = @CompId AND     ud.ATBUD_YEARId = @YearId  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
GROUP BY ud.ATBUD_ID, ud.ATBUD_Description, si.ASSI_ID, si.ASSI_Name, ldg.ASHL_Description";
                                            var subItemDescriptions = (await connection.QueryAsync<DetailedReportPandLRow>(subDescSql, new
                                            {
                                                YearId = YearId,
                                                PrevYearId = YearId - 1,
                                                ScheduleTypeId = ScheduleTypeID,
                                                CompId = CompId,
                                                BranchId = selectedBranches,
                                                CustId = CustId
                                            })).ToList();
                                            decimal subitemNet = (subItem.CrTotal ?? 0) - (subItem.DbTotal ?? 0);
                                            decimal subitemPrevNet = (subItem.CrTotalPrev ?? 0) - (subItem.DbTotalPrev ?? 0);
                                            if (subitemNet != 0 || subitemPrevNet != 0)
                                            {
                                                totalIncome += subitemNet;
                                                totalPrevIncome += subitemPrevNet;
                                                results.Add(new DetailedReportPandLRow
                                                {
                                                    SrNo = (results.Count + 1).ToString(),
                                                    Status = "1",
                                                    Name = item.Name,
                                                    HeaderSLNo = subitemNet.ToString($"N{RoundOff}"),
                                                    PrevYearTotal = subitemPrevNet.ToString($"N{RoundOff}"),
                                                    Notes = !string.IsNullOrEmpty(subItem.Notes) ? subItem.Notes.ToString() : ""
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            results.Add(new DetailedReportPandLRow
                            {
                                SrNo = (results.Count + 1).ToString(),
                                Status = "1",
                                Name = subBalance.SubHeadingName,
                                HeaderSLNo = "",
                                PrevYearTotal = "",
                                Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
                            });

                            var descriptionSql = @"
                                select distinct(ATBUD_Subheading),ATBUD_Description as Name,a.ASSH_Name as headingname,a.AsSh_Notes as Notes, 
  ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal, 
  ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
  e.ASHN_Description as ASHN_Description,  
  ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0) as PrevCrTotal, 
  ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0) as PrevDbTotal,g.ASHL_Description
  from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
  left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description  And d.ATBU_YEARId = @YearId
  and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
  left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
  left join ACC_SubHeadingLedgerDesc g on g.ASHL_SubHeadingId = Atbud_ID and g.ASHL_CustomerId = @CustomerId  and g.ASHL_YearID = @YearId
  And g.ASHL_BranchId IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
  left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description And f.ATBU_YEARId = @PrevYearId and f.ATBU_CustId = @CustomerId
  and ATBUD_YEARId = @PrevYearId and f.ATBU_Branchid = Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
  where ATBUD_Subheading = @subHeadingId And ATBUD_Subheading<>0 and ATBUD_Schedule_type = @ScheduleTypeID and ATBUD_CustId = @CustomerId
  and ATBUD_SubItemId = 0 and ATBUD_itemid = 0   And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description,ATBUD_Description,
  g.ASHL_Description order by ATBUD_Subheading";
                            var descs = await connection.QueryAsync<DetailedReportPandLRow>(descriptionSql, new
                            {
                                YearId,
                                PrevYearId = YearId - 1,
                                CustomerId = CustId,
                                HeadingId = heading.HeadingId,
                                subHeadingId = subBalance.SubHeadingID,
                                BranchId = selectedBranches,
                                ScheduleTypeID
                            });
                            foreach (var desc in descs)
                            {
                                decimal subNet = (desc.CrTotal ?? 0) - (desc.DbTotal ?? 0);
                                decimal subPrevNet = (desc.PrevCrTotal ?? 0) - (desc.PrevDbTotal ?? 0);
                                totalIncome += subNet;
                                totalPrevIncome += subPrevNet;
                                results.Add(new DetailedReportPandLRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Status = "2",
                                    Name = desc.Name,
                                    HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                                    PrevYearTotal = subPrevNet.ToString($"N{RoundOff}"),
                                });

                            }
                            // 1. Fetch Item
                            var itemSql = @"
SELECT ud.ATBUD_itemid AS ItemId,i.ASI_Name AS ItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
    ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleItems i ON i.ASI_ID = ud.ATBUD_itemid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = ud.Atbud_Branchnameid 
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ud.ATBUD_Description
    AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
WHERE    ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId and ATBUD_Headingid=@HeadingId And ATBUD_Subheading= @subHeadingId 
 And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
GROUP BY ud.ATBUD_itemid, i.ASI_Name";
                            var items = await connection.QueryAsync<DetailedReportPandLRow>(itemSql, new
                            {
                                YearId = YearId,
                                PrevYearId = YearId - 1,
                                ScheduleTypeId = ScheduleTypeID,
                                HeadingId = heading.HeadingId,
                                subHeadingId = subBalance.SubHeadingID,
                                CompanyId = CompId,
                                BranchId = selectedBranches,
                                CustomerId = CustId
                            });
                            // 2. Description under Item
                            foreach (var item in items)
                            {
                                if (item.ItemName != null)
                                {
                                    results.Add(new DetailedReportPandLRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Status = "1",
                                        Name = item.ItemName,
                                        HeaderSLNo = "",
                                        PrevYearTotal = ""
                                    });
                                    {
                                        var itemDescSql = @"
SELECT ud.ATBUD_Description as Name,ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description   And b.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description  And e.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
WHERE   ud.ATBUD_Subheading=@subHeadingId and ud.ATBUD_itemid=@ItemId and ud.ATBUD_SubItemId=0  and
    ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_compid = @CompanyId  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";
                                        var itemDescriptions = await connection.QueryAsync<DetailedReportPandLRow>(itemDescSql, new
                                        {
                                            YearId = YearId,
                                            PrevYearId = YearId - 1,
                                            ScheduleTypeId = ScheduleTypeID,
                                            subHeadingId = subBalance.SubHeadingID,
                                            ItemId = item.ItemID,
                                            CompanyId = CompId,
                                            BranchId = selectedBranches,
                                            CustomerId = CustId
                                        });
                                        foreach (var itemDescription in itemDescriptions)
                                        {
                                            decimal itemNet = (item.CrTotal ?? 0) - (item.DbTotal ?? 0);
                                            decimal itemPrevNet = (item.CrTotalPrev ?? 0) - (item.DbTotalPrev ?? 0);
                                            if (itemNet != 0 || itemPrevNet != 0)
                                            {
                                                totalIncome += itemNet;
                                                totalPrevIncome += itemPrevNet;
                                                results.Add(new DetailedReportPandLRow
                                                {
                                                    SrNo = (results.Count + 1).ToString(),
                                                    Status = "2",
                                                    Name = itemDescription.Name,
                                                    HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
                                                    PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}"),
                                                });
                                            }
                                        }
                                        // 1. Fetch  SubItems
                                        var subItemSql = $@" SELECT   ud.ATBUD_SubItemId AS SubItemId,
    si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
    AND d.ATBU_Branchid = ud.Atbud_Branchnameid  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
    AND f.ATBU_Branchid = ud.Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.ATBUD_Headingid=@HeadingId And ud.ATBUD_Subheading=@subHeadingId  And ud.ATBUD_SubItemId<>0 And ud.ATBUD_itemid =@ItemId 
   and  ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
    ud.ATBUD_CustId = @CustId  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";
                                        var subItems = (await connection.QueryAsync<DetailedReportPandLRow>(subItemSql, new
                                        {
                                            YearId = YearId,
                                            PrevYearId = YearId - 1,
                                            ScheduleTypeId = ScheduleTypeID,
                                            HeadingId = heading.HeadingId,
                                            subHeadingId = subBalance.SubHeadingID,
                                            ItemId = item.ItemID,
                                            CompId = CompId,
                                            BranchId = selectedBranches,
                                            CustId = CustId
                                        })).ToList();
                                        // 2. Descriptions under subitem
                                        foreach (var subItem in subItems)
                                        {
                                            var subDescSql = $@"
SELECT ud.ATBUD_ID,ud.ATBUD_Description  as Name,si.ASSI_ID,si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) AS Total,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
    AND f.ATBU_Branchid = ud.Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustId AND ldg.ASHL_YearID = @YearId
WHERE  ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustId AND 
    ud.ATBUD_compid = @CompId AND     ud.ATBUD_YEARId = @YearId  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
GROUP BY ud.ATBUD_ID, ud.ATBUD_Description, si.ASSI_ID, si.ASSI_Name, ldg.ASHL_Description";
                                            var subItemDescriptions = (await connection.QueryAsync<DetailedReportPandLRow>(subDescSql, new
                                            {
                                                YearId = YearId,
                                                PrevYearId = YearId - 1,
                                                ScheduleTypeId = ScheduleTypeID,
                                                CompId = CompId,
                                                BranchId = selectedBranches,
                                                CustId = CustId
                                            })).ToList();
                                            decimal subitemNet = (subItem.CrTotal ?? 0) - (subItem.DbTotal ?? 0);
                                            decimal subitemPrevNet = (subItem.CrTotalPrev ?? 0) - (subItem.DbTotalPrev ?? 0);
                                            if (subitemNet != 0 || subitemPrevNet != 0)
                                            {
                                                totalIncome += subitemNet;
                                                totalPrevIncome += subitemPrevNet;
                                                results.Add(new DetailedReportPandLRow
                                                {
                                                    SrNo = (results.Count + 1).ToString(),
                                                    Status = "1",
                                                    Name = item.Name,
                                                    HeaderSLNo = subitemNet.ToString($"N{RoundOff}"),
                                                    PrevYearTotal = subitemPrevNet.ToString($"N{RoundOff}"),
                                                    Notes = !string.IsNullOrEmpty(subItem.Notes) ? subItem.Notes.ToString() : ""
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        results.Add(new DetailedReportPandLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Status = "1",
                            Name = "Total",
                            HeaderSLNo = totalIncome.ToString($"N{RoundOff}"),
                            PrevYearTotal = totalPrevIncome.ToString($"N{RoundOff}")
                        });
                        results.Add(new DetailedReportPandLRow
                        {
                        });
                        totalIncome = 0; totalPrevIncome = 0;
                    }
                }
            }
            // 1. Fetch Account Head -2 Heading
            headingSql = $@"
SELECT DISTINCT ATBUD_Headingid AS HeadingID,ASH_Name AS HeadingName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,    a.ASH_Notes
FROM Acc_TrailBalance_Upload_Details
LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_Headingid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description
WHERE ATBUD_Schedule_type = @ScheduleTypeId AND ATBUD_compid = @CompanyId AND ATBUD_CustId = @CustomerId
  AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s 
      WHERE s.AST_HeadingID = ATBUD_Headingid AND s.AST_AccHeadId = 2 )
GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes ORDER BY ATBUD_Headingid";
            headings = await connection.QueryAsync<DetailedReportPandLRow>(headingSql, new
            {
                ScheduleTypeId = ScheduleTypeID,
                CompanyId = CompId,
                CustomerId = CustId
            });
            if (headings != null)
            {
                foreach (var heading in headings)
                {
                    // 1. Fetch SubHeading
                    var subHeadingTotalsSql = $@"
                        select distinct(ATBUD_Subheading) as SubHeadingID,a.ASSH_Name as SubHeadingName,a.AsSh_Notes as Notes, 
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0  And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid  And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 where ATBUD_Headingid = @HeadingId And ATBUD_Subheading<>0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId  And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description order by ATBUD_Subheading";
                    var subHeadingTotals = await connection.QueryAsync<DetailedReportPandLRow>(subHeadingTotalsSql, new
                    {
                        YearId,
                        PrevYearId = YearId - 1,
                        CustomerId = CustId,
                        HeadingId = heading.HeadingId,
                        BranchId = selectedBranches,
                        ScheduleTypeID
                    });
                    // 3. Descriptions under Subheading
                    foreach (var subBalance in subHeadingTotals)
                    {
                        results.Add(new DetailedReportPandLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Status = "1",
                            Name = subBalance.SubHeadingName,
                            HeaderSLNo = "",
                            PrevYearTotal = "",
                            Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
                        });
                        var descriptionSql = @"
                            select distinct(ATBUD_Subheading),a.ASSH_Name as headingname,a.AsSh_Notes as Notes,ATBUD_Description as Name, 
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0  And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid  And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 where ATBUD_Subheading= @subHeadingId  And ATBUD_Subheading<>0 And ATBUD_itemid=0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId  And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description,ATBUD_Description order by ATBUD_Subheading";
                        var descs = await connection.QueryAsync<DetailedReportPandLRow>(descriptionSql, new
                        {
                            YearId,
                            PrevYearId = YearId - 1,
                            CustomerId = CustId,
                            HeadingId = heading.HeadingId,
                            subHeadingId = subBalance.SubHeadingID,
                            BranchId = selectedBranches,
                            ScheduleTypeID
                        });
                        foreach (var desc in descs)
                        {
                            if (desc.Name != null)
                            {
                                decimal subNet = (desc.DbTotal1 ?? 0) - (desc.CrTotal1 ?? 0);
                                decimal subPrevNet = (desc.DbTotalPrev ?? 0) - (desc.CrTotalPrev ?? 0);

                                totalIncome += subNet;
                                totalPrevIncome += subPrevNet;
                                results.Add(new DetailedReportPandLRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Status = "2",
                                    Name = desc.Name,
                                    HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                                    PrevYearTotal = subPrevNet.ToString($"N{RoundOff}")
                                });
                            }
                        }
                        // 1. Fetch Item
                        var itemSql = @"
SELECT ud.ATBUD_itemid AS ItemId,i.ASI_Name AS ItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
    ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleItems i ON i.ASI_ID = ud.ATBUD_itemid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = ud.Atbud_Branchnameid and d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description
    AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0 and f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ud.ATBUD_Description and g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0
WHERE    ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId and ATBUD_Headingid=@HeadingId And ATBUD_Subheading= @subHeadingId  And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 GROUP BY ud.ATBUD_itemid, i.ASI_Name";
                        var items = await connection.QueryAsync<DetailedReportPandLRow>(itemSql, new
                        {
                            YearId = YearId,
                            PrevYearId = YearId - 1,
                            ScheduleTypeId = ScheduleTypeID,
                            HeadingId = heading.HeadingId,
                            subHeadingId = subBalance.SubHeadingID,
                            BranchId = selectedBranches,
                            CompanyId = CompId,
                            CustomerId = CustId
                        });
                        // 2. Description under Item
                        foreach (var item in items)
                        {
                            if (item.ItemName != null)
                            {
                                results.Add(new DetailedReportPandLRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Status = "1",
                                    Name = item.ItemName,
                                    HeaderSLNo = "",
                                    PrevYearTotal = ""
                                });
                                {
                                    var itemDescSql = @"
SELECT ud.ATBUD_Description as Name,ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description  And b.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description  And e.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
WHERE   ud.ATBUD_Subheading=@subHeadingId and ud.ATBUD_itemid=@ItemId and ud.ATBUD_SubItemId=0  and
    ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_compid = @CompanyId  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";
                                    var itemDescriptions = await connection.QueryAsync<DetailedReportPandLRow>(itemDescSql, new
                                    {
                                        YearId = YearId,
                                        PrevYearId = YearId - 1,
                                        ScheduleTypeId = ScheduleTypeID,
                                        subHeadingId = subBalance.SubHeadingID,
                                        ItemId = item.ItemID,
                                        CompanyId = CompId,
                                        BranchId = selectedBranches,
                                        CustomerId = CustId
                                    });
                                    foreach (var itemDescription in itemDescriptions)
                                    {
                                        decimal itemNet = (itemDescription.DbTotal ?? 0) - (itemDescription.CrTotal ?? 0);
                                        decimal itemPrevNet = (itemDescription.DbTotalPrev ?? 0) - (itemDescription.CrTotalPrev ?? 0);
                                        totalIncome += itemNet;
                                        totalPrevIncome += itemPrevNet;
                                        results.Add(new DetailedReportPandLRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Status = "2",
                                            Name = itemDescription.Name,
                                            HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
                                            PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}"),
                                        });
                                    }
                                    // 1. Fetch  SubItems
                                    var subItemSql = $@"
SELECT  ud.ATBUD_SubItemId AS SubItemId,
    si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearId 
AND d.ATBU_CustId = @CustId AND d.ATBU_Branchid = ud.Atbud_Branchnameid  and d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description AND f.ATBU_YEARId = @PrevYearId
AND f.ATBU_CustId = @CustId AND f.ATBU_Branchid = ud.Atbud_Branchnameid and f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.ATBUD_Headingid=@HeadingId And ud.ATBUD_Subheading=@subHeadingId  And ud.ATBUD_SubItemId<>0 And ud.ATBUD_itemid =@ItemId 
   and  ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustId  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";
                                    var subItems = (await connection.QueryAsync<DetailedReportPandLRow>(subItemSql, new
                                    {
                                        YearId = YearId,
                                        PrevYearId = YearId - 1,
                                        ScheduleTypeId = ScheduleTypeID,
                                        HeadingId = heading.HeadingId,
                                        subHeadingId = subBalance.SubHeadingID,
                                        ItemId = item.ItemID,
                                        BranchId = selectedBranches,
                                        CompId = CompId,
                                        CustId = CustId
                                    })).ToList();
                                    // 2. Descriptions under subitem
                                    foreach (var subItem in subItems)
                                    {
                                        results.Add(new DetailedReportPandLRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Status = "1",
                                            Name = subItem.SubItemName,
                                            HeaderSLNo = "",
                                            PrevYearTotal = ""
                                        });
                                        var subDescSql = $@"  select distinct(ATBUD_ID),ATBUD_Description,a.ASSI_ID, a.ASSI_Name as headingname, ATBUD_Description as Name,
ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) as Total,
ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal, 
ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) as PrevTotal, 
ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0) as PrevCrTotal, 
ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0) as PrevDbTotal,g.ASHL_Description
from Acc_TrailBalance_Upload_Details
left join ACC_ScheduleSubItems a on a.ASSI_ID = ATBUD_SubItemId
left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description  And d.ATBU_YEARId = @YearId
and d.ATBU_CustId = @CustId and ATBUD_YEARId = @YearId  and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description And f.ATBU_YEARId = @PrevYearId and f.ATBU_CustId = @CustId
and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
left join ACC_SubHeadingLedgerDesc g on g.ASHL_SubHeadingId = Atbud_ID and g.ASHL_CustomerId = @CustId  and g.ASHL_YearID = @YearId
And g.ASHL_BranchId IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
where ATBUD_SChedule_Type = @ScheduleTypeId and ATBUD_CustId = @CustId  and ATBUD_Headingid = @HeadingId
And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) And ATBUD_Subheading = @subHeadingId And ATBUD_SubItemId<>0 And ATBUD_SubItemId = @subItemId
And ATBUD_itemid = @ItemId and atbud_yearid = @YearId 
group by ATBUD_ID,ATBUD_Description,a.ASSI_ID, a.ASSI_Name,g.ASHL_Description order by ATBUD_ID";
                                        var subItemDescriptions = (await connection.QueryAsync<DetailedReportPandLRow>(subDescSql, new
                                        {
                                            YearId = YearId,
                                            PrevYearId = YearId - 1,
                                            ScheduleTypeId = ScheduleTypeID,
                                            HeadingId = heading.HeadingId,
                                            subHeadingId = subBalance.SubHeadingID,
                                            ItemId = item.ItemID,
                                            subItemId = subItem.subItemID,
                                            CompId = CompId,
                                            BranchId = selectedBranches,
                                            CustId = CustId
                                        })).ToList();
                                        foreach (var subItemDescription in subItemDescriptions)
                                        {
                                            decimal subitemNet = (subItemDescription.DbTotal ?? 0) - (subItemDescription.CrTotal ?? 0);
                                            decimal subitemPrevNet = (subItemDescription.DbTotalPrev ?? 0) - (subItemDescription.CrTotalPrev ?? 0);
                                            totalIncome += subitemNet;
                                            totalPrevIncome += subitemPrevNet;
                                            results.Add(new DetailedReportPandLRow
                                            {
                                                SrNo = (results.Count + 1).ToString(),
                                                Status = "2",
                                                Name = subItemDescription.Name,
                                                HeaderSLNo = subitemNet.ToString($"N{RoundOff}"),
                                                PrevYearTotal = subitemPrevNet.ToString($"N{RoundOff}"),
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        results.Add(new DetailedReportPandLRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Status = "1",
                            Name = "Total",
                            HeaderSLNo = totalIncome.ToString($"N{RoundOff}"),
                            PrevYearTotal = totalPrevIncome.ToString($"N{RoundOff}")
                        });
                        results.Add(new DetailedReportPandLRow
                        {                         
                        });
                        totalIncome = 0; totalPrevIncome = 0;
                    }
                }
            }
            return results;
        }

        //GetDetailedReportBalanceSheet
        public async Task<IEnumerable<DetailedReportBalanceSheetRow>> GetDetailedReportBalanceSheetAsync(int CompId, DetailedReportBalanceSheet p)
        {
            var results = new List<DetailedReportBalanceSheetRow>();
            int ScheduleTypeID = 4;
            int RoundOff = 1;
            int YearId = p.YearID;
            int CustId = p.CustID;
            string selectedBranches = p.BranchId;
            decimal totalIncome = 0, totalPrevIncome = 0;

            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
select CUST_ORGTYPEID as OrgId 
from SAD_CUSTOMER_MASTER 
where CUST_ID = @CompanyId and CUST_DELFLG = 'A'";
            int OrgId = await connection.QueryFirstOrDefaultAsync<int>(query, new
            {
                CompanyId = CustId
            });

            // 1. Fetch Account Head -1 Heading
            var headingSql = $@"
SELECT DISTINCT ATBUD_Headingid AS HeadingID,ASH_Name AS HeadingName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,    a.ASH_Notes
FROM Acc_TrailBalance_Upload_Details
LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_Headingid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description
WHERE ATBUD_Schedule_type = @ScheduleTypeId AND ATBUD_compid = @CompanyId AND ATBUD_CustId = @CustomerId
  AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s 
      WHERE s.AST_HeadingID = ATBUD_Headingid AND s.AST_AccHeadId = 2 )
GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes ORDER BY ATBUD_Headingid";
            var headings = await connection.QueryAsync<DetailedReportBalanceSheetRow>(headingSql, new
            {
                ScheduleTypeId = ScheduleTypeID,
                CompanyId = CompId,
                CustomerId = CustId
            });
            if (headings != null)
            {
                foreach (var heading in headings)
                {
                    // 1. Fetch SubHeading
                    var subHeadingTotalsSql = $@"
                        select distinct(ATBUD_Subheading) as SubHeadingID,a.ASSH_Name as SubHeadingName,a.AsSh_Notes as Notes, 
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0  And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid  And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 where ATBUD_Headingid = @HeadingId And ATBUD_Subheading<>0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description order by ATBUD_Subheading";
                    var subHeadingTotals = await connection.QueryAsync<DetailedReportBalanceSheetRow>(subHeadingTotalsSql, new
                    {
                        YearId,
                        PrevYearId = YearId - 1,
                        CustomerId = CustId,
                        HeadingId = heading.HeadingId,
                        BranchId = selectedBranches,
                        ScheduleTypeID
                    });
                    // 3. Descriptions under Subheading
                    foreach (var subBalance in subHeadingTotals)
                    {
                        if (subBalance.CrTotal1 != 0 || subBalance.DbTotal1 != 0)
                        {

                            var descriptionSql = @"
                            select distinct(ATBUD_Subheading),a.ASSH_Name as headingname,a.AsSh_Notes as Notes,ATBUD_Description as Name, 
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0 And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) 
 where ATBUD_Subheading= @subHeadingId  And ATBUD_Subheading<>0 and ATBUD_itemid=0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description,ATBUD_Description order by ATBUD_Subheading";
                            var descs = await connection.QueryAsync<DetailedReportBalanceSheetRow>(descriptionSql, new
                            {
                                YearId,
                                PrevYearId = YearId - 1,
                                CustomerId = CustId,
                                HeadingId = heading.HeadingId,
                                subHeadingId = subBalance.SubHeadingID,
                                BranchId = selectedBranches,
                                ScheduleTypeID
                            });
                            results.Add(new DetailedReportBalanceSheetRow
                            {
                                SrNo = (results.Count + 1).ToString(),
                                Status = "1",
                                Name = subBalance.SubHeadingName,
                                HeaderSLNo = "",
                                PrevYearTotal = "",
                                Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
                            });
                            foreach (var desc in descs)
                            {
                                if (desc.Name != null)
                                {

                                    //Check if it has partners
                                    //if (subBalance.SubHeadingName == "(a)   Proprietor capital" && (OrgId == 68))
                                    //{

                                    //}
                                    //else
                                    //{ }
                                    decimal subNet = (desc.CrTotal ?? 0) - (desc.DbTotal ?? 0);
                                    decimal subPrevNet = (desc.CrTotalPrev ?? 0) - (desc.DbTotalPrev ?? 0);
                                    totalIncome += subNet;
                                    totalPrevIncome += subPrevNet;
                                    results.Add(new DetailedReportBalanceSheetRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Status = "2",
                                        Name = desc.Name,
                                        HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                                        PrevYearTotal = subPrevNet.ToString($"N{RoundOff}")
                                    });
                                }
                            }
                        }
                        else
                        {
                            var descriptionSql = @"
                            select distinct(ATBUD_Subheading),a.ASSH_Name as headingname,a.AsSh_Notes as Notes,ATBUD_Description as Name, 
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0  And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid  And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 where ATBUD_Subheading= @subHeadingId  And ATBUD_Subheading<>0 and ATBUD_ItemId = 0 and ATBUD_SubItemId = 0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description,ATBUD_Description order by ATBUD_Subheading";
                            var descs = await connection.QueryAsync<DetailedReportBalanceSheetRow>(descriptionSql, new
                            {
                                YearId,
                                PrevYearId = YearId - 1,
                                CustomerId = CustId,
                                HeadingId = heading.HeadingId,
                                subHeadingId = subBalance.SubHeadingID,
                                BranchId = selectedBranches,
                                ScheduleTypeID
                            });
                            results.Add(new DetailedReportBalanceSheetRow
                            {
                                SrNo = (results.Count + 1).ToString(),
                                Status = "1",
                                Name = subBalance.SubHeadingName,
                                HeaderSLNo = "",
                                PrevYearTotal = "",
                                Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
                            });
                            foreach (var desc in descs)
                            {
                                if (desc.Name != null)
                                {

                                    //Check if it has partners
                                    //if (subBalance.SubHeadingName == "(a)   Proprietor capital" && (OrgId == 68))
                                    //{

                                    //}
                                    //else
                                    //{ }
                                    decimal subNet = (desc.CrTotal ?? 0) - (desc.DbTotal ?? 0);
                                    decimal subPrevNet = (desc.CrTotalPrev ?? 0) - (desc.DbTotalPrev ?? 0);

                                    totalIncome += subNet;
                                    totalPrevIncome += subPrevNet;
                                    results.Add(new DetailedReportBalanceSheetRow
                                    {
                                        SrNo = (results.Count + 1).ToString(),
                                        Status = "2",
                                        Name = desc.Name,
                                        HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                                        PrevYearTotal = subPrevNet.ToString($"N{RoundOff}")
                                    });

                                }
                            }
                        }

                        // 1. Fetch Item
                        var itemSql = @"
SELECT ud.ATBUD_itemid AS ItemId,i.ASI_Name AS ItemName,
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
WHERE    ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId and ATBUD_Headingid=@HeadingId And ATBUD_Subheading= @subHeadingId 
GROUP BY ud.ATBUD_itemid, i.ASI_Name";
                        var items = await connection.QueryAsync<DetailedReportBalanceSheetRow>(itemSql, new
                        {
                            YearId = YearId,
                            PrevYearId = YearId - 1,
                            ScheduleTypeId = ScheduleTypeID,
                            HeadingId = heading.HeadingId,
                            subHeadingId = subBalance.SubHeadingID,
                            CompanyId = CompId,
                            CustomerId = CustId
                        });
                        // 2. Description under Item
                        foreach (var item in items)
                        {
                            if (item.ItemName != null)
                            {
                                results.Add(new DetailedReportBalanceSheetRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Status = "1",
                                    Name = item.ItemName,
                                    HeaderSLNo = "",
                                    PrevYearTotal = ""
                                });
                                {
                                    var itemDescSql = @"
SELECT ud.ATBUD_Description as Name,ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description  And b.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid 
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description  And e.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
WHERE  ud.ATBUD_YEARId=@YearId and   ud.ATBUD_Subheading=@subHeadingId and ud.ATBUD_itemid=@ItemId and ud.ATBUD_SubItemId=0  and
    ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_compid = @CompanyId  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";
                                    var itemDescriptions = await connection.QueryAsync<DetailedReportBalanceSheetRow>(itemDescSql, new
                                    {
                                        YearId = YearId,
                                        PrevYearId = YearId - 1,
                                        ScheduleTypeId = ScheduleTypeID,
                                        subHeadingId = subBalance.SubHeadingID,
                                        ItemId = item.ItemID,
                                        CompanyId = CompId,
                                        BranchId = selectedBranches,
                                        CustomerId = CustId
                                    });
                                    foreach (var itemDescription in itemDescriptions)
                                    {
                                        decimal itemNet;
                                        decimal itemPrevNet;

                                        itemNet = (itemDescription.CrTotal ?? 0) - (itemDescription.DbTotal ?? 0);
                                        itemPrevNet = (itemDescription.CrTotalPrev ?? 0) - (itemDescription.DbTotalPrev ?? 0);

                                        totalIncome += itemNet;
                                        totalPrevIncome += itemPrevNet;
                                        results.Add(new DetailedReportBalanceSheetRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Status = "2",
                                            Name = itemDescription.Name,
                                            HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
                                            PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}")
                                        });

                                    }
                                    // 1. Fetch  SubItems
                                    var subItemSql = $@"
SELECT  ud.ATBUD_SubItemId AS subItemID,
    si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND d.ATBU_Branchid = ud.Atbud_Branchnameid
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND f.ATBU_Branchid = ud.Atbud_Branchnameid
WHERE ud.ATBUD_Headingid=@HeadingId And ud.ATBUD_Subheading=@subHeadingId  And ud.ATBUD_SubItemId<>0 And ud.ATBUD_itemid =@ItemId 
   and  ud.ATBUD_Schedule_type = @ScheduleTypeId AND 
    ud.ATBUD_CustId = @CustId GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";
                                    var subItems = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subItemSql, new
                                    {
                                        YearId = YearId,
                                        PrevYearId = YearId - 1,
                                        ScheduleTypeId = ScheduleTypeID,
                                        HeadingId = heading.HeadingId,
                                        subHeadingId = subBalance.SubHeadingID,
                                        ItemId = item.ItemID,
                                        CompId = CompId,
                                        BranchId = selectedBranches,
                                        CustId = CustId
                                    })).ToList();
                                    // 2. Descriptions under subitem
                                    foreach (var subItem in subItems)
                                    {
                                        results.Add(new DetailedReportBalanceSheetRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Status = "1",
                                            Name = subItem.SubItemName,
                                            HeaderSLNo = "",
                                            PrevYearTotal = ""
                                        });
                                        var subDescSql = $@"
SELECT ud.ATBUD_ID,ud.ATBUD_Description  as Name,si.ASSI_ID as subItemID,si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) AS Total,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal,    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description 
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustId 
    AND d.ATBU_Branchid = ud.Atbud_Branchnameid  And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description 
    AND f.ATBU_YEARId = @PrevYearId AND f.ATBU_CustId = @CustId 
    AND f.ATBU_Branchid = ud.Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustId AND ldg.ASHL_YearID = @YearId
WHERE  ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustId AND 
ud.ATBUD_Headingid=@HeadingId And ud.ATBUD_Subheading=@subHeadingId  And ud.ATBUD_SubItemId<>0 And ud.ATBUD_itemid =@ItemId
and ud.ATBUD_SubItemId=@subItemId and
    ud.ATBUD_compid = @CompId AND     ud.ATBUD_YEARId = @YearId
GROUP BY ud.ATBUD_ID, ud.ATBUD_Description, si.ASSI_ID, si.ASSI_Name, ldg.ASHL_Description";
                                        var subItemDescriptions = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subDescSql, new
                                        {
                                            YearId = YearId,
                                            PrevYearId = YearId - 1,
                                            ScheduleTypeId = ScheduleTypeID,
                                            HeadingId = heading.HeadingId,
                                            subHeadingId = subBalance.SubHeadingID,
                                            ItemId = item.ItemID,
                                            subItemId = subItem.subItemID,
                                            CompId = CompId,
                                            BranchId = selectedBranches,
                                            CustId = CustId
                                        })).ToList();
                                        foreach (var subItemDescription in subItemDescriptions)
                                        {
                                            decimal subitemNet = (subItemDescription.CrTotal ?? 0) - (subItemDescription.DbTotal ?? 0);
                                            decimal subitemPrevNet = (subItemDescription.CrTotalPrev ?? 0) - (subItemDescription.DbTotalPrev ?? 0);
                                            if (subitemNet != 0 || subitemPrevNet != 0)
                                            {
                                                totalIncome += subitemNet;
                                                totalPrevIncome += subitemPrevNet;
                                                results.Add(new DetailedReportBalanceSheetRow
                                                {
                                                    SrNo = (results.Count + 1).ToString(),
                                                    Status = "2",
                                                    Name = subItemDescription.Name,
                                                    HeaderSLNo = subitemNet.ToString($"N{RoundOff}"),
                                                    PrevYearTotal = subitemPrevNet.ToString($"N{RoundOff}")
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        results.Add(new DetailedReportBalanceSheetRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Status = "1",
                            Name = "Total",
                            HeaderSLNo = totalIncome.ToString($"N{RoundOff}"),
                            PrevYearTotal = totalPrevIncome.ToString($"N{RoundOff}")
                        });
                        results.Add(new DetailedReportBalanceSheetRow
                        {

                        });
                        totalIncome = 0; totalPrevIncome = 0;

                    }

                }
            }

            // 1. Fetch Account Head -2 Heading
            headingSql = $@"
SELECT DISTINCT ATBUD_Headingid AS HeadingID,ASH_Name AS HeadingName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,    a.ASH_Notes
FROM Acc_TrailBalance_Upload_Details
LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_Headingid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description
WHERE ATBUD_Schedule_type = @ScheduleTypeId AND ATBUD_compid = @CompanyId AND ATBUD_CustId = @CustomerId
  AND EXISTS (SELECT 1 FROM ACC_ScheduleTemplates s 
      WHERE s.AST_HeadingID = ATBUD_Headingid AND s.AST_AccHeadId = 1 )
GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_Notes ORDER BY ATBUD_Headingid";
            headings = await connection.QueryAsync<DetailedReportBalanceSheetRow>(headingSql, new
            {
                ScheduleTypeId = ScheduleTypeID,
                CompanyId = CompId,
                CustomerId = CustId
            });
            if (headings != null)
            {
                foreach (var heading in headings)
                {
                    // 1. Fetch SubHeading
                    var subHeadingTotalsSql = $@"
                        select distinct(ATBUD_Subheading) as SubHeadingID,a.ASSH_Name as SubHeadingName,a.AsSh_Notes as Notes, 
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0  And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid  And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 where ATBUD_Headingid = @HeadingId And ATBUD_Subheading<>0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description order by ATBUD_Subheading";
                    var subHeadingTotals = await connection.QueryAsync<DetailedReportBalanceSheetRow>(subHeadingTotalsSql, new
                    {
                        YearId,
                        PrevYearId = YearId - 1,
                        CustomerId = CustId,
                        HeadingId = heading.HeadingId,
                        BranchId = selectedBranches,
                        ScheduleTypeID
                    });
                    // 3. Descriptions under Subheading
                    foreach (var subBalance in subHeadingTotals)
                    {
                        results.Add(new DetailedReportBalanceSheetRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Status = "1",
                            Name = subBalance.SubHeadingName,
                            HeaderSLNo = "",
                            PrevYearTotal = "",
                            Notes = !string.IsNullOrEmpty(subBalance.Notes) ? subBalance.Notes.ToString() : ""
                        });
                        var descriptionSql = @"
                            select distinct(ATBUD_Subheading),a.ASSH_Name as headingname,a.AsSh_Notes as Notes,ATBUD_Description as Name, 
 ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
 ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal ,
 e.ASHN_Description as ASHN_Description,
 ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotal1,
 ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotal1,
 ISNULL(Sum(g.ATBU_Closing_TotalCredit_Amount + 0), 0)  As CrTotalPrev,
 ISNULL(Sum(g.ATBU_Closing_TotalDebit_Amount + 0), 0)  As DbTotalPrev
 from Acc_TrailBalance_Upload_Details left join ACC_ScheduleSubHeading a on a.ASSH_ID = ATBUD_Subheading
 left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description And d.ATBU_YEARId = @YearId
 and d.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 left join ACC_SubHeadingNoteDesc e on e.ASHN_SubHeadingId = ASSH_ID and e.ASHN_CustomerId = @CustomerId and e.ASHN_YearID = @YearId
 left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0 and ATBUD_itemid = 0
 And f.ATBU_YEARId = @YearId  and f.ATBU_CustId = @CustomerId and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid
 left join Acc_TrailBalance_Upload g on g.ATBU_Description = ATBUD_Description and ATBUD_SubItemId = 0  And g.ATBU_YEARId = @PrevYearId
 and g.ATBU_CustId = @CustomerId and ATBUD_YEARId = @PrevYearId and g.ATBU_Branchid = Atbud_Branchnameid  And g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 where ATBUD_Subheading= @subHeadingId  And ATBUD_Subheading<>0 And ATBUD_itemid=0 and ATBUD_SubItemId=0 and ATBUD_Schedule_type = @ScheduleTypeID   And ATBUD_CustId = @CustomerId  And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
 group by ATBUD_Subheading,ASSH_Name,AsSh_Notes,ASHN_Description,ATBUD_Description order by ATBUD_Subheading";
                        var descs = await connection.QueryAsync<DetailedReportBalanceSheetRow>(descriptionSql, new
                        {
                            YearId,
                            PrevYearId = YearId - 1,
                            CustomerId = CustId,
                            HeadingId = heading.HeadingId,
                            subHeadingId = subBalance.SubHeadingID,
                            BranchId = selectedBranches,
                            ScheduleTypeID
                        });
                        foreach (var desc in descs)
                        {
                            if (desc.Name != null)
                            {
                                decimal subNet = (desc.DbTotal1 ?? 0) - (desc.CrTotal1 ?? 0);
                                decimal subPrevNet = (desc.DbTotalPrev ?? 0) - (desc.CrTotalPrev ?? 0);
                                totalIncome += subNet;
                                totalPrevIncome += subPrevNet;
                                results.Add(new DetailedReportBalanceSheetRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Status = "2",
                                    Name = desc.Name,
                                    HeaderSLNo = subNet.ToString($"N{RoundOff}"),
                                    PrevYearTotal = subPrevNet.ToString($"N{RoundOff}")
                                });
                            }
                        }
                        // 1. Fetch Item
                        var itemSql = @" SELECT ud.ATBUD_itemid AS ItemId,i.ASI_Name AS ItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,
    ISNULL(SUM(g.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(g.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleItems i ON i.ASI_ID = ud.ATBUD_itemid
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description
    AND d.ATBU_YEARId = @YearId AND d.ATBU_CustId = @CustomerId AND d.ATBU_Branchid = ud.Atbud_Branchnameid and d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description
    AND f.ATBU_YEARId = @YearId AND f.ATBU_CustId = @CustomerId AND f.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0 and f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload g ON g.ATBU_Description = ud.ATBUD_Description and g.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND g.ATBU_YEARId = @PrevYearId AND g.ATBU_CustId = @CustomerId AND g.ATBU_Branchid = ud.Atbud_Branchnameid AND ud.ATBUD_SubItemId = 0  And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE    ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_Schedule_type = @ScheduleTypeId and ATBUD_Headingid=@HeadingId And ATBUD_Subheading= @subHeadingId 
 GROUP BY ud.ATBUD_itemid, i.ASI_Name";
                        var items = await connection.QueryAsync<DetailedReportBalanceSheetRow>(itemSql, new
                        {
                            YearId = YearId,
                            PrevYearId = YearId - 1,
                            ScheduleTypeId = ScheduleTypeID,
                            HeadingId = heading.HeadingId,
                            subHeadingId = subBalance.SubHeadingID,
                            BranchId = selectedBranches,
                            CompanyId = CompId,
                            CustomerId = CustId
                        });
                        // 2. Description under Item
                        foreach (var item in items)
                        {
                            if (item.ItemName != null)
                            {
                                results.Add(new DetailedReportBalanceSheetRow
                                {
                                    SrNo = (results.Count + 1).ToString(),
                                    Status = "1",
                                    Name = item.ItemName,
                                    HeaderSLNo = "",
                                    PrevYearTotal = ""
                                });
                                {
                                    var itemDescSql = @"
SELECT ud.ATBUD_Description as Name,ISNULL(SUM(b.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(b.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal1,
    ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal1,    ldg.ASHL_Description AS Notes
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_Description = ud.ATBUD_Description and b.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND b.ATBU_YEARId = @YearId AND b.ATBU_CustId = @CustomerId AND b.ATBU_Branchid = ud.Atbud_Branchnameid and ud.atbud_yearid=@YearId
LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description and e.ATBU_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
    AND e.ATBU_YEARId = @PrevYearId AND e.ATBU_CustId = @CustomerId AND e.ATBU_Branchid = ud.Atbud_Branchnameid and ud.atbud_yearid=@PrevYearId
LEFT JOIN ACC_SubHeadingLedgerDesc ldg ON ldg.ASHL_SubHeadingId = ud.Atbud_ID 
    AND ldg.ASHL_CustomerId = @CustomerId AND ldg.ASHL_YearID = @YearId
WHERE   ud.ATBUD_Subheading=@subHeadingId and ud.ATBUD_itemid=@ItemId and ud.ATBUD_SubItemId=0    And ud.Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
   and ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_compid = @CompanyId
GROUP BY ud.ATBUD_Description, ldg.ASHL_Description";
                                    var itemDescriptions = await connection.QueryAsync<DetailedReportBalanceSheetRow>(itemDescSql, new
                                    {
                                        YearId = YearId,
                                        PrevYearId = YearId - 1,
                                        ScheduleTypeId = ScheduleTypeID,
                                        subHeadingId = subBalance.SubHeadingID,
                                        ItemId = item.ItemID,
                                        CompanyId = CompId,
                                        BranchId = selectedBranches,
                                        CustomerId = CustId
                                    });
                                    foreach (var itemDescription in itemDescriptions)
                                    {
                                        decimal itemNet;
                                        decimal itemPrevNet;

                                        if (itemDescription.DbTotal > itemDescription.CrTotal)
                                        {
                                            itemNet = (itemDescription.DbTotal ?? 0) - (itemDescription.CrTotal ?? 0);
                                            itemPrevNet = (itemDescription.DbTotal1 ?? 0) - (itemDescription.CrTotal1 ?? 0);
                                    }
                                        else
                                    {
                                        itemNet =  (itemDescription.DbTotal ?? 0) - (itemDescription.CrTotal ?? 0);
                                        itemPrevNet =  (itemDescription.DbTotal1 ?? 0)- (itemDescription.CrTotal1 ?? 0);
                                        }


                                    totalIncome += itemNet;
                                        totalPrevIncome += itemPrevNet;
                                        results.Add(new DetailedReportBalanceSheetRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Status = "2",
                                            Name = itemDescription.Name,
                                            HeaderSLNo = itemNet.ToString($"N{RoundOff}"),
                                            PrevYearTotal = itemPrevNet.ToString($"N{RoundOff}"),
                                        });
                                    }
                                    // 1. Fetch  SubItems
                                    var subItemSql = $@"
SELECT  ud.ATBUD_SubItemId AS SubItemId,
    si.ASSI_Name AS SubItemName,
    ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount), 0) AS CrTotal,
    ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount), 0) AS DbTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalCredit_Amount), 0) AS PrevCrTotal,
    ISNULL(SUM(f.ATBU_Closing_TotalDebit_Amount), 0) AS PrevDbTotal
FROM Acc_TrailBalance_Upload_Details ud
LEFT JOIN ACC_ScheduleSubItems si ON si.ASSI_ID = ud.ATBUD_SubItemId
LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YEARId = @YearId 
AND d.ATBU_CustId = @CustId AND d.ATBU_Branchid = ud.Atbud_Branchnameid  and d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
LEFT JOIN Acc_TrailBalance_Upload f ON f.ATBU_Description = ud.ATBUD_Description AND f.ATBU_YEARId = @PrevYearId
AND f.ATBU_CustId = @CustId AND f.ATBU_Branchid = ud.Atbud_Branchnameid and f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
WHERE ud.ATBUD_Headingid=@HeadingId And ud.ATBUD_Subheading=@subHeadingId  And ud.ATBUD_SubItemId<>0 And ud.ATBUD_itemid =@ItemId 
   and  ud.ATBUD_Schedule_type = @ScheduleTypeId AND ud.ATBUD_CustId = @CustId GROUP BY ud.ATBUD_SubItemId, si.ASSI_Name";
                                    var subItems = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subItemSql, new
                                    {
                                        YearId = YearId,
                                        PrevYearId = YearId - 1,
                                        ScheduleTypeId = ScheduleTypeID,
                                        HeadingId = heading.HeadingId,
                                        subHeadingId = subBalance.SubHeadingID,
                                        ItemId = item.ItemID,
                                        BranchId = selectedBranches,
                                        CompId = CompId,
                                        CustId = CustId
                                    })).ToList();
                                    // 2. Descriptions under subitem
                                    foreach (var subItem in subItems)
                                    {
                                        results.Add(new DetailedReportBalanceSheetRow
                                        {
                                            SrNo = (results.Count + 1).ToString(),
                                            Status = "1",
                                            Name = subItem.SubItemName,
                                            HeaderSLNo = "",
                                            PrevYearTotal = ""
                                        });
                                        var subDescSql =
$@"  select distinct(ATBUD_ID),ATBUD_Description,a.ASSI_ID, a.ASSI_Name as headingname, ATBUD_Description as Name,
ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + d.ATBU_Closing_TotalDebit_Amount), 0) as Total,
ISNULL(Sum(d.ATBU_Closing_TotalCredit_Amount + 0), 0) as CrTotal,
ISNULL(Sum(d.ATBU_Closing_TotalDebit_Amount + 0), 0) as DbTotal, 
ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + f.ATBU_Closing_TotalDebit_Amount), 0) as PrevTotal, 
ISNULL(Sum(f.ATBU_Closing_TotalCredit_Amount + 0), 0) as PrevCrTotal, 
ISNULL(Sum(f.ATBU_Closing_TotalDebit_Amount + 0), 0) as PrevDbTotal,g.ASHL_Description
from Acc_TrailBalance_Upload_Details
left join ACC_ScheduleSubItems a on a.ASSI_ID = ATBUD_SubItemId
left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description  And d.ATBU_YEARId = @YearId
and d.ATBU_CustId = @CustId and ATBUD_YEARId = @YearId  and d.ATBU_Branchid = Atbud_Branchnameid   And d.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
left join Acc_TrailBalance_Upload f on f.ATBU_Description = ATBUD_Description And f.ATBU_YEARId = @PrevYearId and f.ATBU_CustId = @CustId
and ATBUD_YEARId = @YearId  and f.ATBU_Branchid = Atbud_Branchnameid  And f.Atbu_Branchid IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
left join ACC_SubHeadingLedgerDesc g on g.ASHL_SubHeadingId = Atbud_ID and g.ASHL_CustomerId = @CustId  and g.ASHL_YearID = @YearId
And g.ASHL_BranchId IN (SELECT value FROM STRING_SPLIT(@BranchId, ','))
where ATBUD_SChedule_Type = @ScheduleTypeId and ATBUD_CustId = @CustId  and ATBUD_Headingid = @HeadingId
And Atbud_Branchnameid IN (SELECT value FROM STRING_SPLIT(@BranchId, ',')) And ATBUD_Subheading = @subHeadingId And ATBUD_SubItemId<>0 And ATBUD_SubItemId = @subItemId
And ATBUD_itemid = @ItemId and atbud_yearid = @YearId 
group by ATBUD_ID,ATBUD_Description,a.ASSI_ID, a.ASSI_Name,g.ASHL_Description order by ATBUD_ID";
                                        var subItemDescriptions = (await connection.QueryAsync<DetailedReportBalanceSheetRow>(subDescSql, new
                                        {
                                            YearId = YearId,
                                            PrevYearId = YearId - 1,
                                            ScheduleTypeId = ScheduleTypeID,
                                            HeadingId = heading.HeadingId,
                                            subHeadingId = subBalance.SubHeadingID,
                                            ItemId = item.ItemID,
                                            subItemId = subItem.subItemID,
                                            CompId = CompId,
                                            BranchId = selectedBranches,
                                            CustId = CustId
                                        })).ToList();
                                        foreach (var subItemDescription in subItemDescriptions)
                                        {
                                            decimal subitemNet = (subItemDescription.CrTotal ?? 0) - (subItemDescription.DbTotal ?? 0);
                                            decimal subitemPrevNet = (subItemDescription.CrTotalPrev ?? 0) - (subItemDescription.DbTotalPrev ?? 0);

                                            totalIncome += subitemNet;
                                            totalPrevIncome += subitemPrevNet;
                                            results.Add(new DetailedReportBalanceSheetRow
                                            {
                                                SrNo = (results.Count + 1).ToString(),
                                                Status = "2",
                                                Name = subItemDescription.Name,
                                                HeaderSLNo = subitemNet.ToString($"N{RoundOff}"),
                                                PrevYearTotal = subitemPrevNet.ToString($"N{RoundOff}"),
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        results.Add(new DetailedReportBalanceSheetRow
                        {
                            SrNo = (results.Count + 1).ToString(),
                            Status = "1",
                            Name = "Total",
                            HeaderSLNo = totalIncome.ToString($"N{RoundOff}"),
                            PrevYearTotal = totalPrevIncome.ToString($"N{RoundOff}")
                        });
                        results.Add(new DetailedReportBalanceSheetRow
                        {

                        });
                        totalIncome = 0; totalPrevIncome = 0;
                    }
                }
            }
            return results;
        }

        //GetScheduleReportDetails
        public async Task<ScheduleReportResponseDto> GetScheduleReportDetailsAsync(ScheduleReportRequestDto request)
        {
            var response = new ScheduleReportResponseDto();
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // 1. Customer Details
                var custQuery = @"SELECT CUST_NAME, CUST_ConEmailID, 
                                     CUST_COMM_ADDRESS + ' ' + CUST_COMM_PIN AS Address 
                              FROM SAD_CUSTOMER_MASTER 
                              WHERE CUST_ID = @CustId AND cust_Compid = @CompanyId";

                using (var cmd = new SqlCommand(custQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CustId", request.CustomerId);
                    cmd.Parameters.AddWithValue("@CompanyId", request.CompanyId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.CustomerName = reader["CUST_NAME"]?.ToString();
                            response.CustomerEmail = reader["CUST_ConEmailID"]?.ToString();
                            response.CustomerAddress = reader["Address"]?.ToString();
                        }
                    }
                }

                // 2. CIN Number
                var cinQuery = "SELECT SMD_RegistrationNo FROM sad_Membership_Details WHERE SMD_EmployeeID = @EmployeeID";

                using (var cmd = new SqlCommand(cinQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", "3");
                    var result = await cmd.ExecuteScalarAsync();
                    response.CINNumber = result != null ? "CIN No: " + result.ToString() : "CIN No: Not Found";
                }

                // 3. Partners
                string partnerQuery;
                if (request.PartnerId > 0)
                {
                    partnerQuery = @"SELECT usr_Id, usr_FullName AS FullName, usr_PhoneNo, org_name 
                                 FROM Sad_UserDetails 
                                 LEFT JOIN sad_org_structure a ON a.org_node = usr_OrgnId 
                                 WHERE usr_partner = 1 AND Usr_CompId = @CompanyId AND usr_Id = @PartnerId";
                }
                else
                {
                    partnerQuery = @"SELECT usr_Id, usr_Code + '-' + usr_FullName AS FullName, usr_PhoneNo, '' AS org_name 
                                 FROM Sad_UserDetails 
                                 WHERE usr_partner = 1 AND Usr_CompId = @CompanyId";
                }

                using (var cmd = new SqlCommand(partnerQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", request.CompanyId);
                    if (request.PartnerId > 0)
                        cmd.Parameters.AddWithValue("@PartnerId", request.PartnerId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Partners.Add(new PartnerDto
                            {
                                Id = Convert.ToInt32(reader["usr_Id"]),
                                FullName = reader["FullName"].ToString(),
                                PhoneNo = reader["usr_PhoneNo"].ToString(),
                                OrgName = reader["org_name"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }

        //GetOrgTypeAndMembers
        public async Task<OrgTypeResponseDto> GetOrgTypeAndMembersAsync(int customerId, int companyId)
        {
            var response = new OrgTypeResponseDto();

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Step 1: Get Org Type
            var orgTypeQuery = @"
        SELECT cmm_Desc
        FROM SAD_CUSTOMER_MASTER 
        LEFT JOIN Content_Management_Master 
            ON Content_Management_Master.cmm_id = SAD_CUSTOMER_MASTER.CUST_ORGTYPEID 
        WHERE SAD_CUSTOMER_MASTER.CUST_ID = @CustomerId";

            await using (var cmd = new SqlCommand(orgTypeQuery, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                var orgTypeResult = await cmd.ExecuteScalarAsync();
                response.OrgType = orgTypeResult?.ToString() ?? "Unknown";
            }

            // Step 2: Load People Based on Org Type
            string peopleQuery;
            string label;

            if (response.OrgType.Equals("Partnership firms", StringComparison.OrdinalIgnoreCase))
            {
                label = "Partners";
                peopleQuery = @"
            SELECT SSP_Id AS Id, SSP_PartnerName AS Name, '' as DIN
            FROM SAD_Statutory_PartnerDetails
            WHERE SSP_CustID = @CustomerId AND SSP_CompID = @CompanyId";
            }
            else
            {
                label = "Directors";
                peopleQuery = @"
            SELECT SSD_Id AS Id, SSD_DirectorName AS Name, SSD_DIN as DIN
            FROM SAD_Statutory_DirectorDetails
            WHERE SSD_CustID = @CustomerId AND SSD_CompID = @CompanyId";
            }

            var persons = new List<PersonDto>();

            await using (var peopleCmd = new SqlCommand(peopleQuery, conn))
            {
                peopleCmd.Parameters.AddWithValue("@CustomerId", customerId);
                peopleCmd.Parameters.AddWithValue("@CompanyId", companyId);

                await using var reader = await peopleCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    persons.Add(new PersonDto
                    {
                        Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        DIN = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    });
                }
            }

            response.Label = label;
            response.Persons = persons;

            return response;
        }

        //GetLoadCompanyDetails
        public async Task<List<CompanyDto>> LoadCompanyDetailsAsync(int compId)
        {
            var companies = new List<CompanyDto>();

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);


            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var query = "SELECT Company_ID, Company_Name FROM TRACe_CompanyDetails WHERE Company_ID = @CompId ORDER BY Company_Name";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompId", compId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            companies.Add(new CompanyDto
                            {
                                Company_ID = Convert.ToInt32(reader["Company_ID"]),
                                Company_Name = reader["Company_Name"].ToString()
                            });
                        }
                    }
                }
            }

            return companies;
        }

        //UpdatePnL
        public async Task<bool> UpdatePnLAsync(string pnlAmount, int compId, int custId, int userId, int yearId, string branchId, int durationId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (string.IsNullOrWhiteSpace(pnlAmount))
                return false;

            var selectQuery = @"
  SELECT * 
  FROM Acc_TrailBalance_Upload
  WHERE ATBU_Description = 'Net income'
    AND ATBU_CustId = @CustId
    AND ATBU_YearId = @YearId
    AND ATBU_Branchid = @BranchId
    AND ATBU_QuarterId = @DurationId";

            var record = await connection.QueryFirstOrDefaultAsync<dynamic>(selectQuery, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId,
                DurationId = durationId
            });

            if (record == null)
                return false;
       
            decimal TrAmount;
            string updateQuery;

            if (decimal.TryParse(pnlAmount, out var pnlDecimal))
            {
                TrAmount = pnlDecimal;
                if (pnlDecimal < 0)
                {
                    pnlDecimal = Math.Abs(pnlDecimal);
                    updateQuery = @"
          UPDATE Acc_TrailBalance_Upload 
          SET 
              ATBU_Closing_Debit_Amount = @PnlAmount,
              ATBU_Closing_TotalDebit_Amount = @PnlAmount,
              ATBU_Closing_Credit_Amount = '0.00',
              ATBU_Closing_TotalCredit_Amount = '0.00',
              ATBU_TR_Debit_Amount = @PnlAmount
          WHERE ATBU_ID = @Id";
                }
                else
                {
          updateQuery = @"
          UPDATE Acc_TrailBalance_Upload 
          SET 
              ATBU_Closing_Credit_Amount = @PnlAmount,
              ATBU_Closing_TotalCredit_Amount = @PnlAmount,
              ATBU_Closing_Debit_Amount = '0.00',
              ATBU_Closing_TotalDebit_Amount = '0.00',
              ATBU_TR_Credit_Amount = @PnlAmount
          WHERE ATBU_ID = @Id";                }
                var affected = await connection.ExecuteAsync(updateQuery, new
                {
                    PnlAmount = pnlDecimal.ToString("F2"),
                    Id = (int)record.ATBU_ID
                });
                return affected > 0;
            }
            return false;
        }
        public async Task<(int iUpdateOrSave, int iOper)> SaveCustomerStatutoryPartnerAsync(StatutoryPartnerDto partnerDto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("spSAD_Statutory_PartnerDetails", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.AddWithValue("@SSP_Id", partnerDto.SSP_Id);
                cmd.Parameters.AddWithValue("@SSP_CustID", partnerDto.SSP_CustID);
                cmd.Parameters.AddWithValue("@SSP_PartnerName", partnerDto.SSP_PartnerName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSP_DOJ", partnerDto.SSP_DOJ);
                cmd.Parameters.AddWithValue("@SSP_PAN", partnerDto.SSP_PAN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSP_ShareOfProfit", partnerDto.SSP_ShareOfProfit);
                cmd.Parameters.AddWithValue("@SSP_CapitalAmount", partnerDto.SSP_CapitalAmount);
                cmd.Parameters.AddWithValue("@SSP_CRON", partnerDto.SSP_CRON);
                cmd.Parameters.AddWithValue("@SSP_CRBY", partnerDto.SSP_CRBY);
                cmd.Parameters.AddWithValue("@SSP_UpdatedOn", partnerDto.SSP_UpdatedOn);
                cmd.Parameters.AddWithValue("@SSP_UpdatedBy", partnerDto.SSP_UpdatedBy);
                cmd.Parameters.AddWithValue("@SSP_DelFlag", partnerDto.SSP_DelFlag ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSP_STATUS", partnerDto.SSP_STATUS ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSP_IPAddress", partnerDto.SSP_IPAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSP_CompID", partnerDto.SSP_CompID);

                // Output parameters
                var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(updateOrSaveParam);
                cmd.Parameters.Add(operParam);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                int iUpdateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                int iOper = (int)(operParam.Value ?? 0);

                return (iUpdateOrSave, iOper);
            }
        }
        public async Task<(int iUpdateOrSave, int iOper)> SaveCustomerStatutoryDirectorAsync(StatutoryDirectorDto directorDto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("spSAD_Statutory_DirectorDetails", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SSD_Id", directorDto.SSD_Id);
                cmd.Parameters.AddWithValue("@SSD_CustID", directorDto.SSD_CustID);
                cmd.Parameters.AddWithValue("@SSD_DirectorName", (object)directorDto.SSD_DirectorName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_DOB", (object)directorDto.SSD_DOB ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_DIN", (object)directorDto.SSD_DIN ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_MobileNo", (object)directorDto.SSD_MobileNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_Email", (object)directorDto.SSD_Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_Remarks", (object)directorDto.SSD_Remarks ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_CRON", (object)directorDto.SSD_CRON ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_CRBY", directorDto.SSD_CRBY);
                cmd.Parameters.AddWithValue("@SSD_UpdatedOn", (object)directorDto.SSD_UpdatedOn ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_UpdatedBy", directorDto.SSD_UpdatedBy);
                cmd.Parameters.AddWithValue("@SSD_DelFlag", (object)directorDto.SSD_DelFlag ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_STATUS", (object)directorDto.SSD_STATUS ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_IPAddress", (object)directorDto.SSD_IPAddress ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SSD_CompID", directorDto.SSD_CompID);

                var outputUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outputOper = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(outputUpdateOrSave);
                cmd.Parameters.Add(outputOper);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                int iUpdateOrSave = (int)(outputUpdateOrSave.Value ?? 0);
                int iOper = (int)(outputOper.Value ?? 0);

                return (iUpdateOrSave, iOper);
            }
        }

        public async Task<DirectorDto> GetDirectorByIdAsync(int directorId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using (var con = new SqlConnection(_configuration.GetConnectionString(dbName)))
            {
                var query = @"
            SELECT 
                SSD_Id,
                SSD_DirectorName,
                SSD_DOB,
                SSD_DIN,
                SSD_MobileNo,
                SSD_Email,
                SSD_Remarks
            FROM SAD_Statutory_DirectorDetails
            WHERE SSD_Id = @Id";

                var result = await con.QueryFirstOrDefaultAsync<DirectorDto>(query, new { Id = directorId });

                return result ?? new DirectorDto();
            }
        }
        public async Task<CustomerAmountSettingsDto> GetCustomerAmountSettingsAsync(int customerId)
        {
            // 1. Get DB Name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // 2. Build connection string dynamically using the DB name
            string connectionString = _configuration.GetConnectionString(dbName);

            // 3. Open connection
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = @"
            SELECT CUST_Amount_Type, CUST_RoundOff
            FROM SAD_Customer_Master
            WHERE CUST_ID = @CUST_ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CUST_ID", customerId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new CustomerAmountSettingsDto
                            {
                                CUST_Amount_Type = reader["CUST_Amount_Type"]?.ToString(),
                                CUST_RoundOff = reader["CUST_RoundOff"] == DBNull.Value
                                                ? null
                                                : Convert.ToDecimal(reader["CUST_RoundOff"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        //SaveFinancialStatement
        public async Task<bool> SaveLoeTemplatesAsync( int loeId, int reportTypeId, int compId, int createdBy, string ipAddress)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Fetch templates for the selected report type
                string fetchQuery = @"
            SELECT
                RCM_ID,
                RCM_Heading,
                RCM_Description
            FROM SAD_ReportContentMaster
            WHERE RCM_ReportId = @ReportTypeId
              AND RCM_DelFlag = 'A'";

                var templates = await connection.QueryAsync(fetchQuery,
                    new { ReportTypeId = reportTypeId },
                    transaction);

                if (!templates.Any())
                    throw new Exception("No templates found for the selected report type.");

                // 2. Insert into LOE_Template_details
                string insertQuery = @"
            INSERT INTO LOE_Template_details
            (
                LTD_LOE_ID,
                LTD_ReportTypeID,
                LTD_HeadingID,
                LTD_Heading,
                LTD_Decription,
                LTD_FormName,
                LTD_CrBy,
                LTD_CrOn,
                LTD_IPAddress,
                LTD_CompID,
                LTD_UpdatedBy,
                LTD_UpdatedOn
            )
            VALUES
            (
                @LoeId,
                @ReportTypeId,
                @HeadingId,
                @Heading,
                @Description,
                @FormName,
                @CreatedBy,
                GETDATE(),
                @IPAddress,
                @CompId
                @UpdatedBy
                @UpdateOn
            )";

                foreach (var item in templates)
                {
                    await connection.ExecuteAsync(insertQuery, new
                    {
                        LoeId = loeId,
                        ReportTypeId = reportTypeId,
                        HeadingId = item.RCM_Id,
                        Heading = item.RCM_Heading,
                        Description = item.RCM_Description,
                        CreatedBy = createdBy,
                        IPAddress = ipAddress,
                        CompId = compId
                    }, transaction);
                }
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}


