using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

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
        public async Task<PagedResult<AbnormalTransactionsDto>> GetAbnormalTransactionsAsync(
       int iCustId,
       int iBranchId,
       int iYearID,
       int iAbnormalType,
       string sAmount,
       string searchTerm = "",
       int pageNumber = 1,
       int pageSize = 10)
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

            string baseSql = string.Empty;
            string countSql = string.Empty;
            string searchCondition = string.Empty;

            // Add search condition if searchTerm is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchCondition = @" AND (A.AJTB_DescName LIKE @SearchPattern 
                               OR A.AJTB_TranscNo LIKE @SearchPattern 
                               OR A.AJTB_CreatedBy LIKE @SearchPattern)";
            }

            //Greater Than Average Amount Ratio
            if (iAbnormalType == 1)
            {
                baseSql = @"
WITH AvgValues AS (
    SELECT 
        AJTB_DescName, 
        AVG(ajtb_credit) AS AvgCreditAmt,
        AVG(ajtb_debit) AS AvgDebitAmt,
        AVG(ajtb_credit * @sAmount) AS AvgCreditAmtRatio,
        AVG(ajtb_debit * @sAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId 
        AND AJTB_BranchId = @iBranchId 
        AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
)
SELECT 
    A.AJTB_DescName,
    A.AJTB_CreatedOn,
    A.AJTB_TranscNo, 
    A.ajtb_credit AS creditAmt, 
    A.ajtb_debit AS DebitAmt,
    V.AvgCreditAmt, 
    V.AvgDebitAmt, 
    V.AvgCreditAmtRatio, 
    V.AvgDebitAmtRatio,
    A.AJTB_AEStatus as status, 
    A.AJTB_BillType,
    A.AJTB_TranscNo,
    A.AJTB_CreatedBy,
    A.ajtb_id
FROM Acc_JETransactions_Details A
JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
WHERE A.Ajtb_Custid = @iCustId 
    AND A.AJTB_BranchId = @iBranchId 
    AND A.AJTB_YearID = @iYearID
    AND (A.ajtb_credit > V.AvgCreditAmtRatio OR A.ajtb_debit > V.AvgDebitAmtRatio)
    " + searchCondition;

                countSql = @"
WITH AvgValues AS (
    SELECT 
        AJTB_DescName, 
        AVG(ajtb_credit) AS AvgCreditAmt,
        AVG(ajtb_debit) AS AvgDebitAmt,
        AVG(ajtb_credit * @sAmount) AS AvgCreditAmtRatio,
        AVG(ajtb_debit * @sAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId 
        AND AJTB_BranchId = @iBranchId 
        AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
)
SELECT COUNT(*)
FROM Acc_JETransactions_Details A
JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
WHERE A.Ajtb_Custid = @iCustId 
    AND A.AJTB_BranchId = @iBranchId 
    AND A.AJTB_YearID = @iYearID
    AND (A.ajtb_credit > V.AvgCreditAmtRatio OR A.ajtb_debit > V.AvgDebitAmtRatio)
    " + searchCondition;
            }

            //Lesser Than Average Amount Ratio
            else if (iAbnormalType == 2)
            {
                baseSql = @"
WITH AvgValues AS (
    SELECT 
        AJTB_DescName, 
        AVG(ajtb_credit) AS AvgCreditAmt,
        AVG(ajtb_debit) AS AvgDebitAmt,
        AVG(ajtb_credit * @sAmount) AS AvgCreditAmtRatio,
        AVG(ajtb_debit * @sAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId 
        AND AJTB_BranchId = @iBranchId 
        AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
)
SELECT 
    A.AJTB_DescName, 
    A.ajtb_credit AS creditAmt, 
    A.ajtb_debit AS DebitAmt,
    V.AvgCreditAmt, 
    V.AvgDebitAmt, 
    V.AvgCreditAmtRatio, 
    V.AvgDebitAmtRatio,
    A.AJTB_status
FROM Acc_JETransactions_Details A
JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
WHERE A.Ajtb_Custid = @iCustId 
    AND A.AJTB_BranchId = @iBranchId 
    AND A.AJTB_YearID = @iYearID
    AND (A.ajtb_credit < V.AvgCreditAmtRatio OR A.ajtb_debit < V.AvgDebitAmtRatio)
    " + searchCondition;

                countSql = @"
WITH AvgValues AS (
    SELECT 
        AJTB_DescName, 
        AVG(ajtb_credit) AS AvgCreditAmt,
        AVG(ajtb_debit) AS AvgDebitAmt,
        AVG(ajtb_credit * @sAmount) AS AvgCreditAmtRatio,
        AVG(ajtb_debit * @sAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId 
        AND AJTB_BranchId = @iBranchId 
        AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
)
SELECT COUNT(*)
FROM Acc_JETransactions_Details A
JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
WHERE A.Ajtb_Custid = @iCustId 
    AND A.AJTB_BranchId = @iBranchId 
    AND A.AJTB_YearID = @iYearID
    AND (A.ajtb_credit < V.AvgCreditAmtRatio OR A.ajtb_debit < V.AvgDebitAmtRatio)
    " + searchCondition;
            }
            else if (iAbnormalType == 3)
            {
                baseSql = @"
SELECT 
    AJTB_DescName, 
    ajtb_credit, 
    ajtb_debit,
    AJTB_status
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId 
    AND AJTB_BranchId = @iBranchId 
    AND AJTB_YearID = @iYearID
    AND (AJTB_Credit >= @sAmount OR ajtb_debit >= @sAmount)
    " + searchCondition;

                countSql = @"
SELECT COUNT(*)
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId 
    AND AJTB_BranchId = @iBranchId 
    AND AJTB_YearID = @iYearID
    AND (AJTB_Credit >= @sAmount OR ajtb_debit >= @sAmount)
    " + searchCondition;
            }
            else if (iAbnormalType == 4)
            {
                baseSql = @"
SELECT 
    AJTB_DescName, 
    ajtb_credit, 
    ajtb_debit
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId 
    AND AJTB_BranchId = @iBranchId 
    AND AJTB_YearID = @iYearID
    AND ((AJTB_Credit <= @sAmount AND ajtb_credit <> 0) 
        OR (ajtb_debit <= @sAmount AND ajtb_debit <> 0))
    " + searchCondition;

                countSql = @"
SELECT COUNT(*)
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId 
    AND AJTB_BranchId = @iBranchId 
    AND AJTB_YearID = @iYearID
    AND ((AJTB_Credit <= @sAmount AND ajtb_credit <> 0) 
        OR (ajtb_debit <= @sAmount AND ajtb_debit <> 0))
    " + searchCondition;
            }

            // Add pagination
            string paginatedSql = $@"
{baseSql}
ORDER BY AJTB_CreatedOn DESC, ajtb_id DESC
OFFSET @Offset ROWS 
FETCH NEXT @PageSize ROWS ONLY";

            // Calculate offset for pagination
            int offset = (pageNumber - 1) * pageSize;

            // Prepare parameters
            var parameters = new
            {
                iCustId,
                iBranchId,
                iYearID,
                sAmount,
                SearchPattern = string.IsNullOrWhiteSpace(searchTerm) ? null : $"%{searchTerm}%",
                Offset = offset,
                PageSize = pageSize
            };

            // ✅ Get total count
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            // ✅ Get paginated data
            var items = await connection.QueryAsync<AbnormalTransactionsDto>(paginatedSql, parameters);

            // ✅ Return paged result
            return new PagedResult<AbnormalTransactionsDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        public async Task<byte[]> DownloadAbnormalTransactionsExcelAsync(
    int iCustId,
    int iBranchId,
    int iYearID,
    int iAbnormalType,
    decimal sAmount,
    string searchTerm = "")
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

            string baseSql = string.Empty;
            string searchCondition = string.Empty;

            // Add search condition if searchTerm is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchCondition = @" AND (A.AJTB_DescName LIKE @SearchPattern 
                           OR A.AJTB_TranscNo LIKE @SearchPattern 
                           OR A.AJTB_CreatedBy LIKE @SearchPattern)";
            }

            // Greater Than Average Amount Ratio
            if (iAbnormalType == 1)
            {
                baseSql = @"
WITH AvgValues AS (
    SELECT 
        AJTB_DescName, 
        AVG(ajtb_credit) AS AvgCreditAmt,
        AVG(ajtb_debit) AS AvgDebitAmt,
        AVG(ajtb_credit * @sAmount) AS AvgCreditAmtRatio,
        AVG(ajtb_debit * @sAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId 
        AND AJTB_BranchId = @iBranchId 
        AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
)
SELECT 
    A.AJTB_DescName as 'Description',
    CONVERT(varchar, A.AJTB_CreatedOn, 120) as 'Created Date',
    A.AJTB_TranscNo as 'Transaction Number', 
    FORMAT(A.ajtb_credit, 'N2') as 'Credit Amount', 
    FORMAT(A.ajtb_debit, 'N2') as 'Debit Amount',
    FORMAT(V.AvgCreditAmt, 'N2') as 'Average Credit', 
    FORMAT(V.AvgDebitAmt, 'N2') as 'Average Debit', 
    FORMAT(V.AvgCreditAmtRatio, 'N2') as 'Average Credit Ratio', 
    FORMAT(V.AvgDebitAmtRatio, 'N2') as 'Average Debit Ratio',
    CASE 
        WHEN A.AJTB_AEStatus = 1 THEN 'Pending'
        WHEN A.AJTB_AEStatus = 2 THEN 'Approved'
        WHEN A.AJTB_AEStatus = 3 THEN 'Rejected'
        ELSE 'N/A'
    END as 'Status',
    A.AJTB_BillType as 'Bill Type',
    A.AJTB_CreatedBy as 'Created By',
    A.ajtb_id as 'Transaction ID'
FROM Acc_JETransactions_Details A
JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
WHERE A.Ajtb_Custid = @iCustId 
    AND A.AJTB_BranchId = @iBranchId 
    AND A.AJTB_YearID = @iYearID
    AND (A.ajtb_credit > V.AvgCreditAmtRatio OR A.ajtb_debit > V.AvgDebitAmtRatio)
    " + searchCondition + @"
ORDER BY A.AJTB_CreatedOn DESC, A.ajtb_id DESC";
            }
            // Lesser Than Average Amount Ratio
            else if (iAbnormalType == 2)
            {
                baseSql = @"
WITH AvgValues AS (
    SELECT 
        AJTB_DescName, 
        AVG(ajtb_credit) AS AvgCreditAmt,
        AVG(ajtb_debit) AS AvgDebitAmt,
        AVG(ajtb_credit * @sAmount) AS AvgCreditAmtRatio,
        AVG(ajtb_debit * @sAmount) AS AvgDebitAmtRatio
    FROM Acc_JETransactions_Details
    WHERE Ajtb_Custid = @iCustId 
        AND AJTB_BranchId = @iBranchId 
        AND AJTB_YearID = @iYearID
    GROUP BY AJTB_DescName
)
SELECT 
    A.AJTB_DescName as 'Description',
    CONVERT(varchar, A.AJTB_CreatedOn, 120) as 'Created Date',
    A.AJTB_TranscNo as 'Transaction Number', 
    FORMAT(A.ajtb_credit, 'N2') as 'Credit Amount', 
    FORMAT(A.ajtb_debit, 'N2') as 'Debit Amount',
    FORMAT(V.AvgCreditAmt, 'N2') as 'Average Credit', 
    FORMAT(V.AvgDebitAmt, 'N2') as 'Average Debit', 
    FORMAT(V.AvgCreditAmtRatio, 'N2') as 'Average Credit Ratio', 
    FORMAT(V.AvgDebitAmtRatio, 'N2') as 'Average Debit Ratio',
    CASE 
        WHEN A.AJTB_AEStatus = 1 THEN 'Pending'
        WHEN A.AJTB_AEStatus = 2 THEN 'Approved'
        WHEN A.AJTB_AEStatus = 3 THEN 'Rejected'
        ELSE 'N/A'
    END as 'Status',
    A.AJTB_BillType as 'Bill Type',
    A.AJTB_CreatedBy as 'Created By',
    A.ajtb_id as 'Transaction ID'
FROM Acc_JETransactions_Details A
JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
WHERE A.Ajtb_Custid = @iCustId 
    AND A.AJTB_BranchId = @iBranchId 
    AND A.AJTB_YearID = @iYearID
    AND (A.ajtb_credit < V.AvgCreditAmtRatio OR A.ajtb_debit < V.AvgDebitAmtRatio)
    " + searchCondition + @"
ORDER BY A.AJTB_CreatedOn DESC, A.ajtb_id DESC";
            }
            // Greater Than or Equal to Amount
            else if (iAbnormalType == 3)
            {
                baseSql = @"
SELECT 
    AJTB_DescName as 'Description',
    CONVERT(varchar, AJTB_CreatedOn, 120) as 'Created Date',
    AJTB_TranscNo as 'Transaction Number', 
    FORMAT(ajtb_credit, 'N2') as 'Credit Amount', 
    FORMAT(ajtb_debit, 'N2') as 'Debit Amount',
    '' as 'Average Credit', 
    '' as 'Average Debit', 
    '' as 'Average Credit Ratio', 
    '' as 'Average Debit Ratio',
    CASE 
        WHEN AJTB_AEStatus = 1 THEN 'Pending'
        WHEN AJTB_AEStatus = 2 THEN 'Approved'
        WHEN AJTB_AEStatus = 3 THEN 'Rejected'
        ELSE 'N/A'
    END as 'Status',
    AJTB_BillType as 'Bill Type',
    AJTB_CreatedBy as 'Created By',
    ajtb_id as 'Transaction ID'
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId 
    AND AJTB_BranchId = @iBranchId 
    AND AJTB_YearID = @iYearID
    AND (AJTB_Credit >= @sAmount OR ajtb_debit >= @sAmount)
    " + searchCondition + @"
ORDER BY AJTB_CreatedOn DESC, ajtb_id DESC";
            }
            // Lesser Than or Equal to Amount
            else if (iAbnormalType == 4)
            {
                baseSql = @"
SELECT 
    AJTB_DescName as 'Description',
    CONVERT(varchar, AJTB_CreatedOn, 120) as 'Created Date',
    AJTB_TranscNo as 'Transaction Number', 
    FORMAT(ajtb_credit, 'N2') as 'Credit Amount', 
    FORMAT(ajtb_debit, 'N2') as 'Debit Amount',
    '' as 'Average Credit', 
    '' as 'Average Debit', 
    '' as 'Average Credit Ratio', 
    '' as 'Average Debit Ratio',
    CASE 
        WHEN AJTB_AEStatus = 1 THEN 'Pending'
        WHEN AJTB_AEStatus = 2 THEN 'Approved'
        WHEN AJTB_AEStatus = 3 THEN 'Rejected'
        ELSE 'N/A'
    END as 'Status',
    AJTB_BillType as 'Bill Type',
    AJTB_CreatedBy as 'Created By',
    ajtb_id as 'Transaction ID'
FROM Acc_JETransactions_Details
WHERE Ajtb_Custid = @iCustId 
    AND AJTB_BranchId = @iBranchId 
    AND AJTB_YearID = @iYearID
    AND ((AJTB_Credit <= @sAmount AND ajtb_credit <> 0) 
        OR (ajtb_debit <= @sAmount AND ajtb_debit <> 0))
    " + searchCondition + @"
ORDER BY AJTB_CreatedOn DESC, ajtb_id DESC";
            }

            // Prepare parameters
            var parameters = new
            {
                iCustId,
                iBranchId,
                iYearID,
                sAmount,
                SearchPattern = string.IsNullOrWhiteSpace(searchTerm) ? null : $"%{searchTerm}%"
            };

            // ✅ Get all data
            var data = await connection.QueryAsync<dynamic>(baseSql, parameters);

            // Convert to DataTable for Excel export
            var dataTable = new DataTable();

            if (data.Any())
            {
                // Get column names from first row
                var firstRow = (IDictionary<string, object>)data.First();
                foreach (var key in firstRow.Keys)
                {
                    dataTable.Columns.Add(key);
                }

                // Add rows
                foreach (var row in data)
                {
                    var dictRow = (IDictionary<string, object>)row;
                    var dataRow = dataTable.NewRow();

                    foreach (var column in dataTable.Columns)
                    {
                        var colName = column.ToString();
                        dataRow[colName] = dictRow.ContainsKey(colName) ? dictRow[colName] : DBNull.Value;
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            // Generate Excel file
            return GenerateExcelFile(dataTable, GetAbnormalTypeName(iAbnormalType), sAmount);
        }

        private byte[] GenerateExcelFile(
        DataTable dataTable,
        string abnormalTypeName,
        decimal? amountFilter
    )
        {
            using var package = new OfficeOpenXml.ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Abnormal Transactions");

            // Title
            worksheet.Cells[1, 1].Value = $"Abnormal Transactions Report - {abnormalTypeName}";

            if (amountFilter.HasValue)
            {
                worksheet.Cells[2, 1].Value = $"Amount Filter: {amountFilter.Value:N2}";
            }

            worksheet.Cells[3, 1].Value = $"Generated On: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            // Merge title rows
            worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Merge = true;

            if (amountFilter.HasValue)
                worksheet.Cells[2, 1, 2, dataTable.Columns.Count].Merge = true;

            worksheet.Cells[3, 1, 3, dataTable.Columns.Count].Merge = true;

            // Styling
            worksheet.Cells[1, 1, 3, dataTable.Columns.Count].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 14;

            // Header row
            int startRow = 5;
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                worksheet.Cells[startRow, i + 1].Value = dataTable.Columns[i].ColumnName;
                worksheet.Cells[startRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[startRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[startRow, i + 1].Style.Fill.BackgroundColor
                    .SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[startRow, i + 1].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            // Data rows
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    worksheet.Cells[startRow + i + 1, j + 1].Value = dataTable.Rows[i][j];
                    worksheet.Cells[startRow + i + 1, j + 1].Style.Border.BorderAround(
                        OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Total row
            if (dataTable.Rows.Count > 0)
            {
                int totalRow = startRow + dataTable.Rows.Count + 1;
                worksheet.Cells[totalRow, 1].Value = "Total Records:";
                worksheet.Cells[totalRow, 2].Value = dataTable.Rows.Count;
                worksheet.Cells[totalRow, 1, totalRow, 2].Style.Font.Bold = true;
            }

            return package.GetAsByteArray();
        }


        private string GetAbnormalTypeName(int iAbnormalType)
        {
            return iAbnormalType switch
            {
                1 => "Greater Than Average Amount Ratio",
                2 => "Lesser Than Average Amount Ratio",
                3 => "Greater Than or Equal to Amount",
                4 => "Lesser Than or Equal to Amount",
                _ => "Unknown Type"
            };
        }
        //UpdateAEStatus
        public async Task<int> UpdateJournalEntrySeqRefAsync(List<UpdateJournalEntrySeqRef1Dto> dtoList)
        {
            if (dtoList == null || !dtoList.Any())
                return 0; // Nothing to update

            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 3: Build dynamic CASE statement
                var caseStatements = dtoList
                    .Select((d, i) => $"WHEN @Id{i} THEN @Status{i}")
                    .ToList();

                var sql = $@"
        UPDATE Acc_JETransactions_Details
        SET AJTB_AEStatus = CASE AJTB_ID
            {string.Join(" ", caseStatements)}
        END
        WHERE AJTB_ID IN ({string.Join(",", dtoList.Select((d, i) => $"@Id{i}"))});";

                // Step 4: Prepare parameters
                var parameters = new DynamicParameters();
                for (int i = 0; i < dtoList.Count; i++)
                {
                    parameters.Add($"Id{i}", dtoList[i].Id);
                    parameters.Add($"Status{i}", dtoList[i].Status);
                }

                // Step 5: Execute query
                var updatedCount = await connection.ExecuteAsync(sql, parameters, transaction);

                transaction.Commit();
                return updatedCount;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

