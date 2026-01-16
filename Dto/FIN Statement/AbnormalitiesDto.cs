namespace TracePca.Dto.FIN_Statement
{
    public class AbnormalitiesDto
    {
        //GetAbnormalTransactions
        public class AbnormalTransactionsDto
        {
            public string AJTB_DescName { get; set; }
            public decimal creditAmt { get; set; }
            public decimal DebitAmt { get; set; }
            public decimal AvgCreditAmt { get; set; }
            public decimal AvgDebitAmt { get; set; }
            public decimal AvgCreditAmtRatio { get; set; }
            public decimal AvgDebitAmtRatio { get; set; }
            public string AJTB_status { get; set; }
            public int ajtb_id { get; set; }
            public string AJTB_CreatedOn { get; set; }
            public string AJTB_TranscNo{ get; set; }
        }

        public class PagedResult<T>
        {
            public List<T> Items { get; set; } = new List<T>();
            public int TotalCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
            public bool HasPreviousPage => PageNumber > 1;
            public bool HasNextPage => PageNumber < TotalPages;
        }
        //UpdateAEStatus
        public class UpdateJournalEntrySeqRef1Dto
        {
            public int Id { get; set; }
            public string Status { get; set; }
        }
    }
}
