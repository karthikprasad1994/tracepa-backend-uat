namespace TracePca.Models
{
    public class JournalEntryMaster
    {
        public int Acc_JE_ID { get; set; }
        public string AJTB_TranscNo { get; set; }
        public int Acc_JE_Party { get; set; }
        public int Acc_JE_BillType { get; set; }
        public string Acc_JE_BillNo { get; set; }
        public DateTime Acc_JE_BillDate { get; set; }
        public decimal Acc_JE_BillAmount { get; set; }
        public int Acc_JE_YearID { get; set; }
        public int Acc_JE_CompID { get; set; }
        public int Acc_JE_CreatedBy { get; set; }
        public int acc_JE_BranchId { get; set; }
        public int Acc_JE_Quarterly { get; set; }
    }

    public class JournalEntryDetail
    {
        public int AJTB_ID { get; set; }
        public string AJTB_TranscNo { get; set; }
        public int AJTB_CustId { get; set; }
        public int AJTB_Desc { get; set; }
        public string AJTB_DescName { get; set; }
        public decimal AJTB_Debit { get; set; }
        public decimal AJTB_Credit { get; set; }
        public int AJTB_YearID { get; set; }
        public int AJTB_CompID { get; set; }
        public int AJTB_BranchId { get; set; }
        public int AJTB_QuarterId { get; set; }
        public DateTime AJTB_CreatedOn { get; set; }
    }
}