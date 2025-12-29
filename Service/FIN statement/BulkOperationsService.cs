using System.Data;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using TracePca.Interface;
using TracePca.Interface.FIN_Statement;
using TracePca.Models;


namespace TracePca.Service.FIN_statement
{
    public class BulkOperationsService 
    {
        private readonly ILogger<BulkOperationsService> _logger;


        public BulkOperationsService(ILogger<BulkOperationsService> logger)
        {
            _logger = logger;
        }

      
        public async Task BulkInsertJournalEntriesAsync(SqlConnection connection, SqlTransaction transaction, List<JournalEntryMaster> entries)
        {
            if (!entries.Any()) return;

            var dt = CreateJournalEntryDataTable();
            foreach (var entry in entries)
            {
                dt.Rows.Add(
                    entry.AJTB_TranscNo,
                    entry.Acc_JE_Party,
                    entry.Acc_JE_BillType,
                    entry.Acc_JE_BillNo,
                    entry.Acc_JE_BillDate,
                    entry.Acc_JE_BillAmount,
                    entry.Acc_JE_YearID,
                    entry.Acc_JE_CompID,
                    entry.Acc_JE_CreatedBy,
                    entry.acc_JE_BranchId,
                    entry.Acc_JE_Quarterly
                );
            }

            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopy.DestinationTableName = "Acc_JE_Master";
            bulkCopy.BatchSize = 1000;
            bulkCopy.BulkCopyTimeout = 300;

            // Map columns
            bulkCopy.ColumnMappings.Add("AJTB_TranscNo", "Acc_JE_TransactionNo");
            bulkCopy.ColumnMappings.Add("Acc_JE_Party", "Acc_JE_Party");
            // Add other mappings...

            await bulkCopy.WriteToServerAsync(dt);
        }

        public async Task BulkInsertTransactionDetailsAsync(SqlConnection connection, SqlTransaction transaction, List<JournalEntryDetail> details)
        {
            if (!details.Any()) return;

            var dt = CreateTransactionDetailsDataTable();
            foreach (var detail in details)
            {
                dt.Rows.Add(
                    detail.AJTB_TranscNo,
                    detail.AJTB_CustId,
                    detail.AJTB_Desc,
                    detail.AJTB_DescName,
                    detail.AJTB_Debit,
                    detail.AJTB_Credit,
                    detail.AJTB_YearID,
                    detail.AJTB_CompID,
                    detail.AJTB_BranchId,
                    detail.AJTB_QuarterId,
                    detail.AJTB_CreatedOn
                );
            }

            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopy.DestinationTableName = "Acc_JETransactions_Details";
            bulkCopy.BatchSize = 1000;

            // Map columns
            foreach (DataColumn column in dt.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dt);
        }

        private DataTable CreateJournalEntryDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("AJTB_TranscNo", typeof(string));
            dt.Columns.Add("Acc_JE_Party", typeof(int));
            dt.Columns.Add("Acc_JE_BillType", typeof(int));
            dt.Columns.Add("Acc_JE_BillNo", typeof(string));
            dt.Columns.Add("Acc_JE_BillDate", typeof(DateTime));
            dt.Columns.Add("Acc_JE_BillAmount", typeof(decimal));
            dt.Columns.Add("Acc_JE_YearID", typeof(int));
            dt.Columns.Add("Acc_JE_CompID", typeof(int));
            dt.Columns.Add("Acc_JE_CreatedBy", typeof(int));
            dt.Columns.Add("acc_JE_BranchId", typeof(int));
            dt.Columns.Add("Acc_JE_Quarterly", typeof(int));
            return dt;
        }

        private DataTable CreateTransactionDetailsDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("AJTB_TranscNo", typeof(string));
            dt.Columns.Add("AJTB_CustId", typeof(int));
            dt.Columns.Add("AJTB_Desc", typeof(int));
            dt.Columns.Add("AJTB_DescName", typeof(string));
            dt.Columns.Add("AJTB_Debit", typeof(decimal));
            dt.Columns.Add("AJTB_Credit", typeof(decimal));
            dt.Columns.Add("AJTB_YearID", typeof(int));
            dt.Columns.Add("AJTB_CompID", typeof(int));
            dt.Columns.Add("AJTB_BranchId", typeof(int));
            dt.Columns.Add("AJTB_QuarterId", typeof(int));
            dt.Columns.Add("AJTB_CreatedOn", typeof(DateTime));
            return dt;
        }
    }
}