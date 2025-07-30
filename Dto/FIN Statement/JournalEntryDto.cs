namespace TracePca.Dto.FIN_Statement
{
    public class JournalEntryDto
    {

        //GetJournalEntryInformation
        public class JournalEntryInformationDto
        {
            public int Id { get; set; }
            public string TransactionNo { get; set; }
            public int BranchID { get; set; }
            public string BillNo { get; set; }
            public string BillDate { get; set; }
            public string Party { get; set; }
            public int PartyID { get; set; }
            public string BillType { get; set; }
            public string DebDescription { get; set; }
            public decimal Debit { get; set; }
            public string CredDescription { get; set; }
            public decimal Credit { get; set; }
            public string Status { get; set; }
        }

        //GetJEType 
        public class JETypeDto
        {
            public int cmm_ID { get; set; }
            public string cmm_Desc { get; set; }
        }

        //GetHeadOfAccounts
        public class DescheadDto
        {
            public int ATBU_ID { get; set; }
            public string ATBU_Description { get; set; }
        }

        //GetGeneralLedger
        public class SubGlDto
        {
            public int gl_id { get; set; }
            public string GlDesc { get; set; }
        }

        //SaveTransactionDetails
        public class SaveJournalEntryWithTransactionsDto
        {
            // --- Journal Entry Master Fields ---
            public int Acc_JE_ID { get; set; }
            public string? Acc_JE_TransactionNo { get; set; }
            public int Acc_JE_Party { get; set; }
            public int Acc_JE_Location { get; set; }
            public string? Acc_JE_BillType { get; set; }
            public string? Acc_JE_BillNo { get; set; }
            public DateTime? Acc_JE_BillDate { get; set; }
            public decimal Acc_JE_BillAmount { get; set; }
            public decimal Acc_JE_AdvanceAmount { get; set; }
            public string? Acc_JE_AdvanceNaration { get; set; }
            public decimal Acc_JE_BalanceAmount { get; set; }
            public decimal Acc_JE_NetAmount { get; set; }
            public string? Acc_JE_PaymentNarration { get; set; }
            public string? Acc_JE_ChequeNo { get; set; }
            public DateTime? Acc_JE_ChequeDate { get; set; }
            public string? Acc_JE_IFSCCode { get; set; }
            public string? Acc_JE_BankName { get; set; }
            public string? Acc_JE_BranchName { get; set; }
            public int Acc_JE_CreatedBy { get; set; }
            public int Acc_JE_YearID { get; set; }
            public int Acc_JE_CompID { get; set; }
            public string? Acc_JE_Status { get; set; }
            public int Acc_JE_Operation { get; set; }
            public string? Acc_JE_IPAddress { get; set; }
            public DateTime? Acc_JE_BillCreatedDate { get; set; }
            public int acc_JE_BranchId { get; set; }
            public int Acc_JE_QuarterId { get; set; }
            public string? Acc_JE_Comments { get; set; }

            // --- Transaction Details Fields ---
            public int AJTB_ID { get; set; }
            public int AJTB_MasID { get; set; }
            public string? AJTB_TranscNo { get; set; }
            public int AJTB_CustId { get; set; }
            public int AJTB_ScheduleTypeid { get; set; }
            public int AJTB_Deschead { get; set; }
            public string? AJTB_Desc { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
            public int AJTB_CreatedBy { get; set; }
            public int AJTB_UpdatedBy { get; set; }
            public string? AJTB_Status { get; set; }
            public string? AJTB_IPAddress { get; set; }
            public int AJTB_CompID { get; set; }
            public int AJTB_YearID { get; set; }
            public string? AJTB_BillType { get; set; }
            public string? AJTB_DescName { get; set; }
            public int AJTB_BranchId { get; set; }
            public int AJTB_QuarterId { get; set; }
        }
    }
}
