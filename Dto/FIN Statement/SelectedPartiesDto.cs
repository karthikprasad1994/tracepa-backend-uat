namespace TracePca.Dto.FIN_Statement
{
    public class SelectedPartiesDto
    {

        //GetSelectedParties
        public class LoadTrailBalanceDto
        {
            public string ATBU_Description { get; set; }
            public int ATBU_Id { get; set; }
            public decimal ATBU_Closing_TotalDebit_Amount { get; set; }
            public decimal ATBU_Closing_TotalCredit_Amount { get; set; }
            public string ATBU_Delflg { get; set; }
        }

        //UpdateSelectedPartiesStatus
        public class UpdateTrailBalanceStatusDto
        {
            public int Id { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        //GetJETransactionDetails
        public class JournalEntryWithTrailBalanceDto
        {
            public int AJTB_ID { get; set; }
            public string AJTB_DescName { get; set; }
            public string AJTB_TranscNo { get; set; }
            public string AJTB_CreatedOn { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
            public string ATBU_Description { get; set; }
            public int ATBU_ID { get; set; }
            public int Status { get; set; }
            public string JeType { get; set; }

        }

        //UpdateJESeqReferenceNum
        public class UpdateJournalEntrySeqRefDto
        {
            public int Id { get; set; }
            public int SeqReferenceNum { get; set; }
        }
    }
}
