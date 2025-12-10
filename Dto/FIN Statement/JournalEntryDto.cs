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

            public string acc_JE_QuarterId { get; set; }
            public string comments { get; set; }
        }

        //GetExistingJournalVouchers
        public class JournalEntryVoucherDto
        {
            public int Acc_JE_ID { get; set; }
            public string Acc_JE_TransactionNo { get; set; }
        }

        //GetJEType 
        public class GeneralMasterJETypeDto
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

        //SaveOrUpdateTransactionDetails
        public class SaveJournalEntryWithTransactionsDto
        {
            public int Acc_JE_ID { get; set; }
            public string Acc_JE_TransactionNo { get; set; }
            public int Acc_JE_Party { get; set; }
            public int Acc_JE_Location { get; set; }
            public int Acc_JE_BillType { get; set; }
            public string Acc_JE_BillNo { get; set; }
            public DateTime? Acc_JE_BillDate { get; set; }
            public decimal Acc_JE_BillAmount { get; set; }
            public decimal Acc_JE_AdvanceAmount { get; set; }
            public string Acc_JE_AdvanceNaration { get; set; }
            public decimal Acc_JE_BalanceAmount { get; set; }
            public decimal Acc_JE_NetAmount { get; set; }
            public string Acc_JE_PaymentNarration { get; set; }
            public string Acc_JE_ChequeNo { get; set; }
            public DateTime? Acc_JE_ChequeDate { get; set; }
            public string Acc_JE_IFSCCode { get; set; }
            public string Acc_JE_BankName { get; set; }
            public string Acc_JE_BranchName { get; set; }
            public int Acc_JE_CreatedBy { get; set; }
            public int Acc_JE_YearID { get; set; }
            public int Acc_JE_CompID { get; set; }
            public string Acc_JE_Status { get; set; }
            public string Acc_JE_Operation { get; set; }
            public string Acc_JE_IPAddress { get; set; }
            public DateTime? Acc_JE_BillCreatedDate { get; set; }
            public int acc_JE_BranchId { get; set; }
            public int Acc_JE_QuarterId { get; set; }
            public string Acc_JE_Comments { get; set; }

            // ✅ Add this property:
            public List<JournalEntryTransactionDto> Transactions { get; set; }
        }

        public class JournalEntryTransactionDto
        {
            public int AJTB_ID { get; set; }
            public int AJTB_MasID { get; set; }
            public string AJTB_TranscNo { get; set; }
            public int AJTB_CustId { get; set; }
            public int AJTB_ScheduleTypeid { get; set; }
            public int AJTB_Deschead { get; set; }
            public int AJTB_Desc { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
            public int AJTB_CreatedBy { get; set; }
            public int AJTB_UpdatedBy { get; set; }
            public string AJTB_Status { get; set; }
            public string AJTB_IPAddress { get; set; }
            public int AJTB_CompID { get; set; }
            public int AJTB_YearID { get; set; }
            public int AJTB_BillType { get; set; }
            public string AJTB_DescName { get; set; }
            public int AJTB_BranchId { get; set; }
            public int AJTB_QuarterId { get; set; }
        }

        // //SaveGeneralLedger
        public class GeneralLedgerDto
        {
            // Master
            public int ATBU_ID { get; set; }
            public string ATBU_CODE { get; set; }
            public string ATBU_Description { get; set; }
            public int ATBU_CustId { get; set; }
            public double ATBU_Opening_Debit_Amount { get; set; }
            public double ATBU_Opening_Credit_Amount { get; set; }
            public double ATBU_TR_Debit_Amount { get; set; }
            public double ATBU_TR_Credit_Amount { get; set; }
            public double ATBU_Closing_Debit_Amount { get; set; }
            public double ATBU_Closing_Credit_Amount { get; set; }
            public string ATBU_DELFLG { get; set; }
            public int ATBU_CRBY { get; set; }
            public string ATBU_STATUS { get; set; }
            public int ATBU_UPDATEDBY { get; set; }
            public string ATBU_IPAddress { get; set; }
            public int ATBU_CompId { get; set; }
            public int ATBU_YEARId { get; set; }
            public int ATBU_Branchid { get; set; }
            public int ATBU_QuarterId { get; set; }

            // Detail
            public int ATBUD_ID { get; set; }
            public int ATBUD_Masid { get; set; }
            public string ATBUD_CODE { get; set; }
            public string ATBUD_Description { get; set; }
            public int ATBUD_CustId { get; set; }
            public int ATBUD_SChedule_Type { get; set; }
            public int ATBUD_Branchid { get; set; }
            public int ATBUD_QuarterId { get; set; }
            public int ATBUD_Company_Type { get; set; }
            public int ATBUD_Headingid { get; set; }
            public int ATBUD_Subheading { get; set; }
            public int ATBUD_itemid { get; set; }
            public int ATBUD_Subitemid { get; set; }
            public string ATBUD_DELFLG { get; set; }
            public int ATBUD_CRBY { get; set; }
            public int ATBUD_UPDATEDBY { get; set; }
            public string ATBUD_STATUS { get; set; }
            public string ATBUD_Progress { get; set; }
            public string ATBUD_IPAddress { get; set; }
            public int ATBUD_CompId { get; set; }
            public int ATBUD_YEARId { get; set; }
        }

        //ActivateJE
        public class ActivateRequestDto
        {
            public string Status { get; set; }
            public List<int> DescriptionIds { get; set; } = new(); 
            public int CompId { get; set; }                       
            public int UserId { get; set; }                
            public string IpAddress { get; set; } = "127.0.0.1";
        }

        //DeActiveteJE
        public class ApproveRequestDto
        {
            public List<int> DescriptionIds { get; set; } = new();
            public int CompId { get; set; }
            public string IpAddress { get; set; }
            public int BranchId { get; set; }
        }
        public class JERecordDto
        {
            public int Acc_JE_ID { get; set; }
            public int Acc_JE_CompID { get; set; }
            public string JE_Number { get; set; }
            public DateTime JE_Date { get; set; }
            public string Description { get; set; }
            // Add other columns as needed
        }
        public class TransactionDetailDto
        {
            public int SrNo { get; set; }
            public int? DetID { get; set; }
            public int? HeadID { get; set; }
            public int? GLID { get; set; }
            public int? SubGLID { get; set; }
            public int? PaymentID { get; set; }
            public string Type { get; set; }
            public string GLCode { get; set; }
            public string GLDescription { get; set; }
            public string SubGL { get; set; }
            public string SubGLDescription { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public decimal Balance { get; set; }
        }
        public class GenerateTransactionNoRequest
        {
            public int AccessCodeID { get; set; }
            public int YearID { get; set; }
            public int PartyID { get; set; }
            public int DurationID { get; set; }
            public int BranchID { get; set; }
        }
        public class AdminMasterDto
        {
            public int Id { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Desc { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Remarks { get; set; } = string.Empty;
            public int KeyComponent { get; set; }
            public string Module { get; set; } = string.Empty;
            public int RiskCategory { get; set; }
            public string Status { get; set; } = string.Empty;
            public double Rate { get; set; }
            public string CMMAct { get; set; } = string.Empty;
            public string CMMHSNSAC { get; set; } = string.Empty;
            public string Delflag { get; set; } = string.Empty;
            public int CreatedBy { get; set; }
            public int UpdatedBy { get; set; }
            public string IpAddress { get; set; } = string.Empty;
            public int CompId { get; set; }
            public string MasterName { get; set; } = string.Empty; // to generate Code if new
        }

        //GetJETypeDropDown
        public class JeTypeDto
        {
            public int JeTypeId { get; set; }
            public string JeTypeName { get; set; }
        }

        //GetJETypeDropDownDetails
        public class JETypeDropDownDetailsDto
        {
            public int Acc_JE_ID { get; set; }
            public string? Acc_JE_TransactionNo { get; set; }
            public int Acc_JE_BranchId { get; set; }
            public string? BillDate { get; set; }
            public string BillType { get; set; }
            public int Acc_JE_Party { get; set; }
            public string? Acc_JE_Status { get; set; }
            public string? Acc_JE_Comnments { get; set; }
            public int Acc_JE_QuarterId { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal AJTB_Credit { get; set; }
            public decimal AJTB_Debit { get; set; }
            public string CUST_NAME { get; set; }

        }

        //SaveJEType
        public class CreateJEContentRequestDto
        {
            public int? cmm_ID { get; set; }
            public int CMM_CompID { get; set; }
            public string? cmm_Desc { get; set; }
            public string? cms_Remarks { get; set; }
            public string? cmm_Category { get; set; }
        }
    }
}
