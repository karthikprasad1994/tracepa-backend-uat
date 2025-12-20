namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleExcelUploadDto
    {
        //DownloadExcelFileAndTemplate
        public class FileDownloadResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //SaveScheduleTemplate(P and L)
        public class ScheduleTemplatePandLDto
        {
            // === Schedule Heading ===
            public int ASH_ID { get; set; }
            public string ASH_Name { get; set; }
            public string ASH_DELFLG { get; set; }
            public int ASH_CRBY { get; set; }
            public string ASH_STATUS { get; set; }
            public int ASH_UPDATEDBY { get; set; }
            public string ASH_IPAddress { get; set; }
            public int ASH_CompId { get; set; }
            public int ASH_YEARId { get; set; }
            public int Ash_scheduletype { get; set; }
            public int Ash_Orgtype { get; set; }
            public int ASH_Notes { get; set; }

            // === Schedule SubHeading ===
            public int ASSH_ID { get; set; }
            public string ASSH_Name { get; set; }
            public int ASSH_HeadingID { get; set; }
            public string ASSH_DELFLG { get; set; }
            public int ASSH_CRBY { get; set; }
            public string ASSH_STATUS { get; set; }
            public int ASSH_UPDATEDBY { get; set; }
            public string ASSH_IPAddress { get; set; }
            public int ASSH_CompId { get; set; }
            public int ASSH_YEARId { get; set; }
            public int ASSH_Notes { get; set; }
            public int ASSH_scheduletype { get; set; }
            public int ASSH_Orgtype { get; set; }

            // === Schedule Item ===
            public int ASI_ID { get; set; }
            public string? ASI_Name { get; set; }
            public int ASI_HeadingID { get; set; }
            public int ASI_SubHeadingID { get; set; }
            public string? ASI_DELFLG { get; set; }
            public int ASI_CRBY { get; set; }
            public string? ASI_STATUS { get; set; }
            public string? ASI_IPAddress { get; set; }
            public int ASI_CompId { get; set; }
            public int ASI_YEARId { get; set; }
            public int ASI_scheduletype { get; set; }
            public int ASI_Orgtype { get; set; }

            // === Schedule SubItem ===
            public int ASSI_ID { get; set; }
            public string? ASSI_Name { get; set; }
            public int ASSI_HeadingID { get; set; }
            public int ASSI_SubHeadingID { get; set; }
            public int ASSI_ItemsID { get; set; }
            public string? ASSI_DELFLG { get; set; }
            public int ASSI_CRBY { get; set; }
            public string? ASSI_STATUS { get; set; }
            public int ASSI_UPDATEDBY { get; set; }
            public string? ASSI_IPAddress { get; set; }
            public int ASSI_CompId { get; set; }
            public int ASSI_YEARId { get; set; }
            public int ASSI_ScheduleType { get; set; }
            public int ASSI_OrgType { get; set; }

            // === Schedule Template ===
            public int AST_ID { get; set; }
            public string? AST_Name { get; set; }
            public int AST_HeadingID { get; set; }
            public int AST_SubHeadingID { get; set; }
            public int AST_ItemID { get; set; }
            public int AST_SubItemID { get; set; }
            public int AST_AccHeadId { get; set; }
            public string? AST_DELFLG { get; set; }
            public int AST_CRBY { get; set; }
            public string? AST_STATUS { get; set; }
            public int AST_UPDATEDBY { get; set; }
            public string? AST_IPAddress { get; set; }
            public int AST_CompId { get; set; }
            public int AST_YEARId { get; set; }
            public int AST_Schedule_type { get; set; }
            public int AST_Companytype { get; set; }
            public int AST_Company_limit { get; set; }

            // === Extra / Derived Property ===
            public string AST_AccountHeadName { get; set; }
        }

        //SaveScheduleTemplate(Balance Sheet)
        public class ScheduleTemplateBalanceSheetDto
        {
            // === Schedule Heading ===
            public int ASH_ID { get; set; }
            public string ASH_Name { get; set; }
            public string ASH_DELFLG { get; set; }
            public int ASH_CRBY { get; set; }
            public string ASH_STATUS { get; set; }
            public int ASH_UPDATEDBY { get; set; }
            public string ASH_IPAddress { get; set; }
            public int ASH_CompId { get; set; }
            public int ASH_YEARId { get; set; }
            public int Ash_scheduletype { get; set; }
            public int Ash_Orgtype { get; set; }
            public int ASH_Notes { get; set; }

            // === Schedule SubHeading ===
            public int ASSH_ID { get; set; }
            public string ASSH_Name { get; set; }
            public int ASSH_HeadingID { get; set; }
            public string ASSH_DELFLG { get; set; }
            public int ASSH_CRBY { get; set; }
            public string ASSH_STATUS { get; set; }
            public int ASSH_UPDATEDBY { get; set; }
            public string ASSH_IPAddress { get; set; }
            public int ASSH_CompId { get; set; }
            public int ASSH_YEARId { get; set; }
            public int ASSH_Notes { get; set; }
            public int ASSH_scheduletype { get; set; }
            public int ASSH_Orgtype { get; set; }

            // === Schedule Item ===
            public int ASI_ID { get; set; }
            public string? ASI_Name { get; set; }
            public int ASI_HeadingID { get; set; }
            public int ASI_SubHeadingID { get; set; }
            public string? ASI_DELFLG { get; set; }
            public int ASI_CRBY { get; set; }
            public string? ASI_STATUS { get; set; }
            public string? ASI_IPAddress { get; set; }
            public int ASI_CompId { get; set; }
            public int ASI_YEARId { get; set; }
            public int ASI_scheduletype { get; set; }
            public int ASI_Orgtype { get; set; }

            // === Schedule SubItem ===
            public int ASSI_ID { get; set; }
            public string? ASSI_Name { get; set; }
            public int ASSI_HeadingID { get; set; }
            public int ASSI_SubHeadingID { get; set; }
            public int ASSI_ItemsID { get; set; }
            public string? ASSI_DELFLG { get; set; }
            public int ASSI_CRBY { get; set; }
            public string? ASSI_STATUS { get; set; }
            public int ASSI_UPDATEDBY { get; set; }
            public string? ASSI_IPAddress { get; set; }
            public int ASSI_CompId { get; set; }
            public int ASSI_YEARId { get; set; }
            public int ASSI_ScheduleType { get; set; }
            public int ASSI_OrgType { get; set; }

            // === Schedule Template ===
            public int AST_ID { get; set; }
            public string? AST_Name { get; set; }
            public int AST_HeadingID { get; set; }
            public int AST_SubHeadingID { get; set; }
            public int AST_ItemID { get; set; }
            public int AST_SubItemID { get; set; }
            public int AST_AccHeadId { get; set; }
            public string? AST_DELFLG { get; set; }
            public int AST_CRBY { get; set; }
            public string? AST_STATUS { get; set; }
            public int AST_UPDATEDBY { get; set; }
            public string? AST_IPAddress { get; set; }
            public int AST_CompId { get; set; }
            public int AST_YEARId { get; set; }
            public int AST_Schedule_type { get; set; }
            public int AST_Companytype { get; set; }
            public int AST_Company_limit { get; set; }

            // === Extra / Derived Property ===
            public string AST_AccountHeadName { get; set; }
        }

        //SaveScheduleTemplate
        public class ScheduleTemplateDto
        {
            // === Schedule Heading ===
            public int ASH_ID { get; set; }
            public string ASH_Name { get; set; }
            public string ASH_DELFLG { get; set; }
            public int ASH_CRBY { get; set; }
            public string ASH_STATUS { get; set; }
            public int ASH_UPDATEDBY { get; set; }
            public string ASH_IPAddress { get; set; }
            public int ASH_CompId { get; set; }
            public int ASH_YEARId { get; set; }
            public int Ash_scheduletype { get; set; }
            public int Ash_Orgtype { get; set; }
            public int ASH_Notes { get; set; }

            // === Schedule SubHeading ===
            public int ASSH_ID { get; set; }
            public string ASSH_Name { get; set; }
            public int ASSH_HeadingID { get; set; }
            public string ASSH_DELFLG { get; set; }
            public int ASSH_CRBY { get; set; }
            public string ASSH_STATUS { get; set; }
            public int ASSH_UPDATEDBY { get; set; }
            public string ASSH_IPAddress { get; set; }
            public int ASSH_CompId { get; set; }
            public int ASSH_YEARId { get; set; }
            public int ASSH_Notes { get; set; }
            public int ASSH_scheduletype { get; set; }
            public int ASSH_Orgtype { get; set; }

            // === Schedule Item ===
            public int ASI_ID { get; set; }
            public string? ASI_Name { get; set; }
            public int ASI_HeadingID { get; set; }
            public int ASI_SubHeadingID { get; set; }
            public string? ASI_DELFLG { get; set; }
            public int ASI_CRBY { get; set; }
            public string? ASI_STATUS { get; set; }
            public string? ASI_IPAddress { get; set; }
            public int ASI_CompId { get; set; }
            public int ASI_YEARId { get; set; }
            public int ASI_scheduletype { get; set; }
            public int ASI_Orgtype { get; set; }

            // === Schedule SubItem ===
            public int ASSI_ID { get; set; }
            public string? ASSI_Name { get; set; }
            public int ASSI_HeadingID { get; set; }
            public int ASSI_SubHeadingID { get; set; }
            public int ASSI_ItemsID { get; set; }
            public string? ASSI_DELFLG { get; set; }
            public int ASSI_CRBY { get; set; }
            public string? ASSI_STATUS { get; set; }
            public int ASSI_UPDATEDBY { get; set; }
            public string? ASSI_IPAddress { get; set; }
            public int ASSI_CompId { get; set; }
            public int ASSI_YEARId { get; set; }
            public int ASSI_ScheduleType { get; set; }
            public int ASSI_OrgType { get; set; }

            // === Schedule Template ===
            public int AST_ID { get; set; }
            public string? AST_Name { get; set; }
            public int AST_HeadingID { get; set; }
            public int AST_SubHeadingID { get; set; }
            public int AST_ItemID { get; set; }
            public int AST_SubItemID { get; set; }
            public int AST_AccHeadId { get; set; }
            public string? AST_DELFLG { get; set; }
            public int AST_CRBY { get; set; }
            public string? AST_STATUS { get; set; }
            public int AST_UPDATEDBY { get; set; }
            public string? AST_IPAddress { get; set; }
            public int AST_CompId { get; set; }
            public int AST_YEARId { get; set; }
            public int AST_Schedule_type { get; set; }
            public int AST_Companytype { get; set; }
            public int AST_Company_limit { get; set; }

            // === Extra / Derived Property ===
            public string AST_AccountHeadName { get; set; }
        }

        //SaveOpeningBalance
        public class OpeningBalanceDto
        {
            // Upload properties (from TrailBalanceUploadDto)
            public int ATBU_ID { get; set; }
            public string ATBU_CODE { get; set; }
            public string ATBU_Description { get; set; }
            public int ATBU_CustId { get; set; }
            public decimal ATBU_Opening_Debit_Amount { get; set; }
            public decimal ATBU_Opening_Credit_Amount { get; set; }
            public decimal ATBU_TR_Debit_Amount { get; set; }
            public decimal ATBU_TR_Credit_Amount { get; set; }
            public decimal ATBU_Closing_Debit_Amount { get; set; }
            public decimal ATBU_Closing_Credit_Amount { get; set; }
            public string ATBU_DELFLG { get; set; }
            public int ATBU_CRBY { get; set; }
            public string ATBU_STATUS { get; set; }
            public int ATBU_UPDATEDBY { get; set; }
            public string ATBU_IPAddress { get; set; }
            public int ATBU_CompId { get; set; }
            public int ATBU_YEARId { get; set; }
            public int ATBU_Branchid { get; set; }
            public int ATBU_QuarterId { get; set; }
            // UploadDetails properties (from TrailBalanceUploadDetailsDto)
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
            public int FlagUpdate { get; set; }
        }

        //SaveTrailBalance
        public class TrailBalanceDto
        {
            // Upload properties (from TrailBalanceUploadDto)
            public int ATBU_ID { get; set; }
            public string ATBU_CODE { get; set; }
            public string ATBU_Description { get; set; }
            public int ATBU_CustId { get; set; }
            public decimal ATBU_Opening_Debit_Amount { get; set; }
            public decimal ATBU_Opening_Credit_Amount { get; set; }
            public decimal ATBU_TR_Debit_Amount { get; set; }
            public decimal ATBU_TR_Credit_Amount { get; set; }
            public decimal ATBU_Closing_Debit_Amount { get; set; }
            public decimal ATBU_Closing_Credit_Amount { get; set; }
            public string ATBU_DELFLG { get; set; }
            public int ATBU_CRBY { get; set; }
            public string ATBU_STATUS { get; set; }
            public int ATBU_UPDATEDBY { get; set; }
            public string ATBU_IPAddress { get; set; }
            public int ATBU_CompId { get; set; }
            public int ATBU_YEARId { get; set; }
            public int ATBU_Branchid { get; set; }
            public int ATBU_QuarterId { get; set; }

            // UploadDetails properties (from TrailBalanceUploadDetailsDto)
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
            public int ATBUD_SubItemid { get; set; }
            public string ATBUD_DELFLG { get; set; }
            public int ATBUD_CRBY { get; set; }
            public int ATBUD_UPDATEDBY { get; set; }
            public string ATBUD_STATUS { get; set; }
            public string ATBUD_Progress { get; set; }
            public string ATBUD_IPAddress { get; set; }
            public int ATBUD_CompId { get; set; }
            public int ATBUD_YEARId { get; set; }

            public string? ItemName { get; set; }
            public string? HeadingName { get; set; }
            public string? SubItemName { get; set; }
            public string? SubHeadingName { get; set; }

            public string Excel_SubItem { get; set; }
            public string Excel_Item { get; set; }
            public string Excel_SubHeading { get; set; }
            public string Excel_Heading { get; set; }

            public int FlagUpdate { get; set; }

        }

        //SaveClientTrailBalance
        public class ClientTrailBalance
        {
            public int ATBCU_ID { get; set; }
            public string ATBCU_CODE { get; set; }
            public string ATBCU_Description { get; set; }
            public int ATBCU_CustId { get; set; }
            public double ATBCU_Opening_Debit_Amount { get; set; }
            public double ATBCU_Opening_Credit_Amount { get; set; }
            public double ATBCU_TR_Debit_Amount { get; set; }
            public double ATBCU_TR_Credit_Amount { get; set; }
            public double ATBCU_Closing_Debit_Amount { get; set; }
            public double ATBCU_Closing_Credit_Amount { get; set; }
            public string ATBCU_DELFLG { get; set; }
            public int ATBCU_CRBY { get; set; }
            public string ATBCU_STATUS { get; set; }
            public int ATBCU_UPDATEDBY { get; set; }
            public string ATBCU_IPAddress { get; set; }
            public int ATBCU_CompId { get; set; }
            public int ATBCU_YEARId { get; set; }
            public int ATBCU_Branchid { get; set; }  
            public int ATBCU_Masid { get; set; }
            public int ATBCU_QuarterId { get; set; }
        }

        //SaveJournalEntry
        public class TrailBalanceCompositeModel
        {
            public TrailBalanceUploadDto Upload { get; set; }
            public TrailBalanceUploadDetailsDto UploadDetails { get; set; }
            public JournalEntryDto JournalEntry { get; set; }
            public List<TransactionDetailsDto> TransactionDetailsList { get; set; }
        }

        public class TrailBalanceUploadDto
        {
            public int ATBU_ID { get; set; }
            public string ATBU_CODE { get; set; }
            public string ATBU_Description { get; set; }
            public int ATBU_CustId { get; set; }
            public decimal ATBU_Opening_Debit_Amount { get; set; }
            public decimal ATBU_Opening_Credit_Amount { get; set; }
            public decimal ATBU_TR_Debit_Amount { get; set; }
            public decimal ATBU_TR_Credit_Amount { get; set; }
            public decimal ATBU_Closing_Debit_Amount { get; set; }
            public decimal ATBU_Closing_Credit_Amount { get; set; }
            public string ATBU_DELFLG { get; set; }
            public string ATBU_STATUS { get; set; }
            public string ATBU_UPDATEDBY { get; set; }
            public string ATBU_IPAddress { get; set; }
            public int ATBU_CompId { get; set; }
            public int ATBU_YEARId { get; set; }
            public int ATBU_Branchid { get; set; }
            public int ATBU_QuarterId { get; set; }
            public string ATBU_CRBY { get; set; }
        }

        public class TrailBalanceUploadDetailsDto
        {
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
            public string ATBUD_CRBY { get; set; }
            public string ATBUD_UPDATEDBY { get; set; }
            public string ATBUD_STATUS { get; set; }
            public string ATBUD_Progress { get; set; }
            public string ATBUD_IPAddress { get; set; }
            public int ATBUD_CompId { get; set; }
            public int ATBUD_YEARId { get; set; }
        }

        public class JournalEntryDto
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
            public string Acc_JE_CreatedBy { get; set; }

            public string Acc_JE_UPDATEDBY { get; set; }
            public int Acc_JE_YearID { get; set; }
            public int Acc_JE_CompID { get; set; }
            public string Acc_JE_Status { get; set; }
            public string Acc_JE_Operation { get; set; }
            public string Acc_JE_IPAddress { get; set; }
            public DateTime? Acc_JE_BillCreatedDate { get; set; }
            public int Acc_JE_BranchId { get; set; }
            public int Acc_JE_QuarterId { get; set; }
            public string Acc_JE_Comnments { get; set; }
        }

        public class TransactionDetailsDto
        {
            public int AJTB_ID { get; set; }
            public int AJTB_MasID { get; set; }
            public string AJTB_TranscNo { get; set; }
            public int AJTB_CustId { get; set; }
            public int AJTB_ScheduleTypeid { get; set; }
            public string AJTB_Deschead { get; set; }
            public string AJTB_Desc { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
            public string AJTB_CreatedBy { get; set; }
            public string AJTB_UpdatedBy { get; set; }
            public string AJTB_Status { get; set; }
            public string AJTB_IPAddress { get; set; }
            public int AJTB_CompID { get; set; }
            public int AJTB_YearID { get; set; }
            public int AJTB_BillType { get; set; }
            public string AJTB_DescName { get; set; }
            public int AJTB_BranchId { get; set; }
            public int AJTB_QuarterId { get; set; }
        }

        //UploadCustomerTrialBalance
        public class TrialBalanceExcelUploadRequestDto
        {
            public int CustomerId { get; set; }
            public int YearId { get; set; }
            public int BranchId { get; set; }
            public int QuarterId { get; set; }
            public int CompanyId { get; set; }
            public int CompId { get; set; }
            public int UserId { get; set; }
        }
        public class TrialBalanceExcelUploadDto
        {
            public IFormFile ExcelFile { get; set; }
            public TrialBalanceExcelUploadRequestDto Data { get; set; }
        }
        public class BadDto
        {
            public IFormFile ExcelFile { get; set; }
            public int CustomerId { get; set; }
        }
    }
}

    
