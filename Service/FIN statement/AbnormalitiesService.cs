using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;

namespace TracePca.Service.FIN_statement
{
    public class AbnormalitiesService : AbnormalitiesInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AbnormalitiesService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetAbnormalTransactions
        //public async Task<IEnumerable<AbnormalTransactionsDto>> GetAbnormalTransactionsAsync(int iCustId, int iBranchId, int iYearID, int iAbnormalType, decimal dAmount)
        //{
        //    // ✅ Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // ✅ Step 2: Get the connection string
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    // ✅ Step 3: Use SqlConnection
        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();
        //    string sql = string.Empty;

        //    //Greater Than Average Amount Ratio
        //    if (iAbnormalType == 1)
        //    {
        //        sql = @"
        //WITH AvgValues AS (
        //    SELECT AJTB_DescName, AVG(ajtb_credit) AS AvgCreditAmt,
        //           AVG(ajtb_debit) AS AvgDebitAmt,
        //           AVG(ajtb_credit * @dAmount) AS AvgCreditAmtRatio,
        //           AVG(ajtb_debit * @dAmount) AS AvgDebitAmtRatio
        //    FROM Acc_JETransactions_Details
        //    WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
        //    GROUP BY AJTB_DescName
        //)
        //SELECT A.AJTB_DescName, A.ajtb_credit AS creditAmt, A.ajtb_debit AS DebitAmt,
        //       V.AvgCreditAmt, V.AvgDebitAmt, V.AvgCreditAmtRatio, V.AvgDebitAmtRatio
        //FROM Acc_JETransactions_Details A
        //JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
        //WHERE A.Ajtb_Custid = @iCustId AND A.AJTB_BranchId = @iBranchId AND A.AJTB_YearID = @iYearID
        //AND (A.ajtb_credit > V.AvgCreditAmtRatio OR A.ajtb_debit > V.AvgDebitAmtRatio)";
        //    }

        //    //Lesser Than Average Amount Ratio
        //    else if (iAbnormalType == 2)
        //    {
        //        sql = @"
        //WITH AvgValues AS (
        //    SELECT AJTB_DescName, AVG(ajtb_credit) AS AvgCreditAmt,
        //           AVG(ajtb_debit) AS AvgDebitAmt,
        //           AVG(ajtb_credit * @dAmount) AS AvgCreditAmtRatio,
        //           AVG(ajtb_debit * @dAmount) AS AvgDebitAmtRatio
        //    FROM Acc_JETransactions_Details
        //    WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
        //    GROUP BY AJTB_DescName
        //)
        //SELECT A.AJTB_DescName, A.ajtb_credit AS creditAmt, A.ajtb_debit AS DebitAmt,
        //       V.AvgCreditAmt, V.AvgDebitAmt, V.AvgCreditAmtRatio, V.AvgDebitAmtRatio
        //FROM Acc_JETransactions_Details A
        //JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
        //WHERE A.Ajtb_Custid = @iCustId AND A.AJTB_BranchId = @iBranchId AND A.AJTB_YearID = @iYearID
        //AND (A.ajtb_credit < V.AvgCreditAmtRatio OR A.ajtb_debit < V.AvgDebitAmtRatio)";
        //    }

        //    //
        //    else if (iAbnormalType == 3)
        //    {
        //        sql = @"
        //SELECT AJTB_DescName, ajtb_credit, ajtb_debit
        //FROM Acc_JETransactions_Details
        //WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
        //AND (AJTB_Credit >= @dAmount OR ajtb_debit >= @dAmount)";
        //    }
        //    else if (iAbnormalType == 4)
        //    {
        //        sql = @"
        //SELECT AJTB_DescName, ajtb_credit AS CreditAmt, ajtb_debit AS DebitAmt
        //FROM Acc_JETransactions_Details
        //WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
        //AND ((AJTB_Credit <= @dAmount AND ajtb_credit <> 0) OR (ajtb_debit <= @dAmount AND ajtb_debit <> 0))";
        //    }

        //    // ✅ Execute query once
        //    var result = await connection.QueryAsync<AbnormalTransactionsDto>(sql, new
        //    {
        //        iCustId,
        //        iBranchId,
        //        iYearID,
        //        dAmount
        //    });

        //    return result;
        //}

        //Type1
        public async Task<IEnumerable<AbnormalTransactions1Dto>> GetAllAbnormalTransactions1Async(
            int iCustId, int iBranchId, int iYearID, decimal dAmount)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"
        WITH AvgValues AS (
            SELECT 
                AJTB_DescName,
                AVG(ajtb_credit) AS AvgCreditAmt,
                AVG(ajtb_debit) AS AvgDebitAmt
            FROM Acc_JETransactions_Details
            WHERE Ajtb_Custid = @iCustId
              AND AJTB_BranchId = @iBranchId
              AND AJTB_YearID = @iYearID
            GROUP BY AJTB_DescName
        ),
        DuplicateEntries AS (
            SELECT AJTB_DescName, ajtb_credit, ajtb_debit
            FROM Acc_JETransactions_Details
            WHERE Ajtb_Custid = @iCustId
              AND AJTB_BranchId = @iBranchId
              AND AJTB_YearID = @iYearID
            GROUP BY AJTB_DescName, ajtb_credit, ajtb_debit
            HAVING COUNT(*) > 1
        )

        -- 1. Greater than Average Amount Ratio
        SELECT A.AJTB_ID, A.AJTB_DescName, A.ajtb_credit AS CreditAmt, A.ajtb_debit AS DebitAmt,
               V.AvgCreditAmt, V.AvgDebitAmt, 'Greater than Average Amount Ratio' AS AbnormalType
        FROM Acc_JETransactions_Details A
        JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
        WHERE (A.ajtb_credit > V.AvgCreditAmt OR A.ajtb_debit > V.AvgDebitAmt)

        UNION ALL

        -- 2. Lesser than Average Amount Ratio
        SELECT A.AJTB_ID, A.AJTB_DescName, A.ajtb_credit, A.ajtb_debit,
               V.AvgCreditAmt, V.AvgDebitAmt, 'Lesser than Average Amount Ratio'
        FROM Acc_JETransactions_Details A
        JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
        WHERE (A.ajtb_credit < V.AvgCreditAmt OR A.ajtb_debit < V.AvgDebitAmt)

        UNION ALL

        -- 3. Greater than Transaction Amount
        SELECT AJTB_ID, AJTB_DescName, ajtb_credit, ajtb_debit,
               0 AS AvgCreditAmt, 0 AS AvgDebitAmt, 'Greater than Transaction Amount'
        FROM Acc_JETransactions_Details
        WHERE ajtb_credit >= @dAmount OR ajtb_debit >= @dAmount

        UNION ALL

        -- 4. Lesser than Transaction Amount
        SELECT AJTB_ID, AJTB_DescName, ajtb_credit, ajtb_debit,
               0, 0, 'Lesser than Transaction Amount'
        FROM Acc_JETransactions_Details
        WHERE (ajtb_credit <= @dAmount AND ajtb_credit <> 0) OR (ajtb_debit <= @dAmount AND ajtb_debit <> 0)

        UNION ALL

        -- 5. Duplicate Entries
        SELECT A.AJTB_ID, A.AJTB_DescName, A.ajtb_credit, A.ajtb_debit,
               0, 0, 'Duplicate Entries'
        FROM Acc_JETransactions_Details A
        JOIN DuplicateEntries D ON A.AJTB_DescName = D.AJTB_DescName
                                AND A.ajtb_credit = D.ajtb_credit
                                AND A.ajtb_debit = D.ajtb_debit

        UNION ALL

        -- 6. Average Amount Ratio
        SELECT A.AJTB_ID, A.AJTB_DescName, A.ajtb_credit, A.ajtb_debit,
               V.AvgCreditAmt, V.AvgDebitAmt, 'Average Amount Ratio'
        FROM Acc_JETransactions_Details A
        JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
        WHERE (A.ajtb_credit >= V.AvgCreditAmt OR A.ajtb_debit >= V.AvgDebitAmt)

        UNION ALL

        -- 7. Percentage of Amount Value (>=50% of total Avg)
        SELECT A.AJTB_ID, A.AJTB_DescName, A.ajtb_credit, A.ajtb_debit,
               V.AvgCreditAmt, V.AvgDebitAmt, 'Percentage of Amount Value'
        FROM Acc_JETransactions_Details A
        JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
        WHERE ((A.ajtb_credit + A.ajtb_debit) / NULLIF((V.AvgCreditAmt + V.AvgDebitAmt),0) * 100) >= 50
        ORDER BY AJTB_DescName, AJTB_ID;
        ";

            var result = await connection.QueryAsync<AbnormalTransactions1Dto>(sql, new
            {
                iCustId,
                iBranchId,
                iYearID,
                dAmount
            });
            return result;
        }

        //Type2
        public async Task<IEnumerable<AbnormalTransactions2Dto>> GetAbnormalTransactions2Async(
    int iCustId, int iBranchId, int iYearID, decimal dAmount)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"
;WITH AvgValues AS (
    SELECT AJTB_DescName,
           AVG(ajtb_credit) AS AvgCreditAmt,
           AVG(ajtb_debit) AS AvgDebitAmt,
           AVG(ajtb_credit * @dAmount) AS AvgCreditAmtRatio,
           AVG(ajtb_debit * @dAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
),
Totals AS (
    SELECT SUM(ajtb_credit) AS TotalCredit, SUM(ajtb_debit) AS TotalDebit
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
),
Dupes AS (
    SELECT AJTB_DescName, ajtb_credit, ajtb_debit, COUNT(*) AS DuplicateCount
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName, ajtb_credit, ajtb_debit
    HAVING COUNT(*) > 1
)
SELECT 
    A.AJTB_ID,
    A.AJTB_DescName,
    A.ajtb_credit AS CreditAmt,
    A.ajtb_debit AS DebitAmt,
    ISNULL(V.AvgCreditAmt, 0) AS AvgCreditAmt,
    ISNULL(V.AvgDebitAmt, 0) AS AvgDebitAmt,
    CASE
        WHEN (A.ajtb_creGreater than dit >= @dAmount OR A.ajtb_debit >= @dAmount)
            THEN 'Transaction Amount'
        WHEN ((A.ajtb_credit <= @dAmount AND A.ajtb_credit <> 0)
           OR (A.ajtb_debit <= @dAmount AND A.ajtb_debit <> 0))
            THEN 'Lesser than Transaction Amount'
        WHEN (A.ajtb_credit > V.AvgCreditAmtRatio OR A.ajtb_debit > V.AvgDebitAmtRatio)
            THEN 'Greater than Average Amount Ratio'
        WHEN (A.ajtb_credit < V.AvgCreditAmtRatio OR A.ajtb_debit < V.AvgDebitAmtRatio)
            THEN 'Lesser than Average Amount Ratio'
        WHEN ((A.ajtb_credit / NULLIF(T.TotalCredit, 0)) * 100 >= @dAmount
           OR (A.ajtb_debit / NULLIF(T.TotalDebit, 0)) * 100 >= @dAmount)
            THEN 'Percentage of Amount Value'
        WHEN D.AJTB_DescName IS NOT NULL
            THEN 'Duplicate Entries'
        ELSE NULL
    END AS AbnormalType
FROM Acc_JETransactions_Details A
LEFT JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
CROSS JOIN Totals T
LEFT JOIN Dupes D ON A.AJTB_DescName = D.AJTB_DescName
                  AND A.ajtb_credit = D.ajtb_credit
                  AND A.ajtb_debit = D.ajtb_debit
WHERE A.Ajtb_Custid = @iCustId
  AND A.AJTB_BranchId = @iBranchId
  AND A.AJTB_YearID = @iYearID
  AND (
        (A.ajtb_credit >= @dAmount OR A.ajtb_debit >= @dAmount)
     OR ((A.ajtb_credit <= @dAmount AND A.ajtb_credit <> 0)
      OR (A.ajtb_debit <= @dAmount AND A.ajtb_debit <> 0))
     OR (A.ajtb_credit > V.AvgCreditAmtRatio OR A.ajtb_debit > V.AvgDebitAmtRatio)
     OR (A.ajtb_credit < V.AvgCreditAmtRatio OR A.ajtb_debit < V.AvgDebitAmtRatio)
     OR ((A.ajtb_credit / NULLIF(T.TotalCredit, 0)) * 100 >= @dAmount
      OR (A.ajtb_debit / NULLIF(T.TotalDebit, 0)) * 100 >= @dAmount)
     OR D.AJTB_DescName IS NOT NULL
    );";

            var result = await connection.QueryAsync<AbnormalTransactions2Dto>(sql, new
            {
                iCustId,
                iBranchId,
                iYearID,
                dAmount
            });

            return result;
        }

        //Type3
        public async Task<IEnumerable<AbnormalTransactions3Dto>> GetAbnormalTransactions3Async(
    int iCustId, int iBranchId, int iYearID, int iAbnormalType, decimal dAmount)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string sql = string.Empty;

            // (1) Greater Than Average Amount Ratio
            if (iAbnormalType == 1)
            {
                sql = @"
WITH AvgValues AS (
    SELECT AJTB_DescName,
           AVG(ajtb_credit) AS AvgCreditAmt,
           AVG(ajtb_debit) AS AvgDebitAmt,
           AVG(ajtb_credit * @dAmount) AS AvgCreditAmtRatio,
           AVG(ajtb_debit * @dAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
)
SELECT A.AJTB_DescName, A.ajtb_credit AS CreditAmt, A.ajtb_debit AS DebitAmt,
       V.AvgCreditAmt, V.AvgDebitAmt, V.AvgCreditAmtRatio, V.AvgDebitAmtRatio
FROM Acc_JETransactions_Details A
JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
WHERE A.Ajtb_Custid = @iCustId AND A.AJTB_BranchId = @iBranchId AND A.AJTB_YearID = @iYearID
AND (A.ajtb_credit > V.AvgCreditAmtRatio OR A.ajtb_debit > V.AvgDebitAmtRatio)";
            }

            // (2) Lesser Than Average Amount Ratio
            else if (iAbnormalType == 2)
            {
                sql = @"
WITH AvgValues AS (
    SELECT AJTB_DescName,
           AVG(ajtb_credit) AS AvgCreditAmt,
           AVG(ajtb_debit) AS AvgDebitAmt,
           AVG(ajtb_credit * @dAmount) AS AvgCreditAmtRatio,
           AVG(ajtb_debit * @dAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
)
SELECT A.AJTB_DescName, A.ajtb_credit AS CreditAmt, A.ajtb_debit AS DebitAmt,
       V.AvgCreditAmt, V.AvgDebitAmt, V.AvgCreditAmtRatio, V.AvgDebitAmtRatio
FROM Acc_JETransactions_Details A
JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
WHERE A.Ajtb_Custid = @iCustId AND A.AJTB_BranchId = @iBranchId AND A.AJTB_YearID = @iYearID
AND (A.ajtb_credit < V.AvgCreditAmtRatio OR A.ajtb_debit < V.AvgDebitAmtRatio)";
            }

            // (3) Greater Than Transaction Amount
            else if (iAbnormalType == 3)
            {
                sql = @"
SELECT AJTB_DescName, ajtb_credit AS CreditAmt, ajtb_debit AS DebitAmt
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
AND (AJTB_Credit >= @dAmount OR ajtb_debit >= @dAmount)";
            }

            // (4) Lesser Than Transaction Amount
            else if (iAbnormalType == 4)
            {
                sql = @"
SELECT AJTB_DescName, ajtb_credit AS CreditAmt, ajtb_debit AS DebitAmt
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
AND ((AJTB_Credit <= @dAmount AND ajtb_credit <> 0) OR (ajtb_debit <= @dAmount AND ajtb_debit <> 0))";
            }

            // (5) Duplicate Entries
            else if (iAbnormalType == 5)
            {
                sql = @"
SELECT AJTB_DescName, ajtb_credit AS CreditAmt, ajtb_debit AS DebitAmt, COUNT(*) AS DuplicateCount
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
GROUP BY AJTB_DescName, ajtb_credit, ajtb_debit
HAVING COUNT(*) > 1";
            }

            // (6) Average Amount Ratio (just show averages)
            else if (iAbnormalType == 6)
            {
                sql = @"
SELECT AJTB_DescName,
       AVG(ajtb_credit) AS AvgCreditAmt,
       AVG(ajtb_debit) AS AvgDebitAmt,
       AVG(ajtb_credit * @dAmount) AS AvgCreditAmtRatio,
       AVG(ajtb_debit * @dAmount) AS AvgDebitAmtRatio
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
GROUP BY AJTB_DescName";
            }

            // (7) Percentage of Amount Value
            else if (iAbnormalType == 7)
            {
                sql = @"
WITH Totals AS (
    SELECT 
        SUM(ajtb_credit) AS TotalCredit,
        SUM(ajtb_debit) AS TotalDebit
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
)
SELECT 
    A.AJTB_DescName,
    A.ajtb_credit AS CreditAmt,
    A.ajtb_debit AS DebitAmt,
    (A.ajtb_credit / NULLIF(T.TotalCredit, 0)) * 100 AS CreditPercentage,
    (A.ajtb_debit / NULLIF(T.TotalDebit, 0)) * 100 AS DebitPercentage
FROM Acc_JETransactions_Details A
CROSS JOIN Totals T
WHERE A.Ajtb_Custid = @iCustId AND A.AJTB_BranchId = @iBranchId AND A.AJTB_YearID = @iYearID";
            }

            // ✅ Execute query once
            var result = await connection.QueryAsync<AbnormalTransactions3Dto>(sql, new
            {
                iCustId,
                iBranchId,
                iYearID,
                dAmount
            });

            return result;
        }
    }
}

