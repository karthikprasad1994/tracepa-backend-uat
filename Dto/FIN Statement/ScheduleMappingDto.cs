using TracePca.Dto.Audit;

namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleMappingDto
    {
 
        //GetScheduleHeading
        public class ScheduleHeadingDto
        {
            public int ASH_ID { get; set; }
            public string? ASH_Name { get; set; }
        }

        //GetScheduleSub-Heading
        public class ScheduleSubHeadingDto
        {
            public int ASSH_ID { get; set; }
            public string ASSH_Name { get; set; }
        }

        //GetScheduleItem
        public class ScheduleItemDto
        {
            public int ASI_ID { get; set; }
            public string ASI_Name { get; set; }
        }

        //GetScheduleSub-Item
        public class ScheduleSubItemDto
        {
            public int ASSI_ID { get; set; }
            public string ASSI_Name { get; set; }
        }

        //GetTotalAmount
        public class CustCOASummaryDto
        {
            public decimal OpeningDebit { get; set; }
            public decimal OpeningCredit { get; set; }
            public decimal TrDebit { get; set; }
            public decimal TrCredit { get; set; }
            public decimal ClosingCredit { get; set; }
            public decimal ClosingDebit { get; set; }
        }

        //GetTrialBalance(Grid)
        public class CustCOADetailsDto
        {
            public int SrNo { get; set; }
            public int DescDetailsID { get; set; }
            public string Status { get; set; }
            public int ScheduleType { get; set; }
            public int DescID { get; set; }
            public string DescriptionCode { get; set; }
            public string Description { get; set; }
            public decimal OpeningDebit { get; set; }
            public decimal OpeningCredit { get; set; }
            public decimal TrDebit { get; set; }
            public decimal TrCredit { get; set; }
            public decimal ClosingDebit { get; set; }
            public decimal ClosingCredit { get; set; }
            public int SubItemID { get; set; }
            public string ASSI_Name { get; set; }
            public int ItemID { get; set; }
            public string ASI_Name { get; set; }
            public int SubHeadingID { get; set; }
            public string ASSH_Name { get; set; }
            public int HeadingID { get; set; }
            public string ASH_Name { get; set; }
            public decimal TrDebittrUploaded { get; set; }
            public decimal TrCredittrUploaded { get; set; }
        }

        //FreezeForPreviousDuration
        public class FreezePreviousYearTrialBalanceDto
        {
            // 🔹 Master Table (Acc_TrailBalance_Upload)
            public int AtbU_ID { get; set; }
            public string? AtbU_CODE { get; set; }
            public string? AtbU_Description { get; set; }
            public int AtbU_CustId { get; set; }
            public int AtbU_YEARId { get; set; }
            public decimal AtbU_Closing_Debit_Amount { get; set; }
            public decimal AtbU_Closing_Credit_Amount { get; set; }
            public string? AtbU_DELFLG { get; set; }
            public int AtbU_CRBY { get; set; }
            public string? AtbU_STATUS { get; set; }
            public int AtbU_UPDATEDBY { get; set; }
            public string? AtbU_IPAddress { get; set; }
            public int AtbU_CompId { get; set; }
            public int AtbU_Branchid { get; set; }
            public int AtbU_QuarterId { get; set; }

            // 🔹 Detail Table (Acc_TrailBalance_Upload_Details)
            public int AtbuD_ID { get; set; }
            public int AtbuD_Masid { get; set; }
            public string? AtbuD_CODE { get; set; }
            public string? AtbuD_Description { get; set; }
            public int AtbuD_CustId { get; set; }
            public int AtbuD_SChedule_Type { get; set; }
            public int AtbuD_Branchid { get; set; }
            public int AtbuD_QuarterId { get; set; }
            public int AtbuD_Company_Type { get; set; }
            public int AtbuD_Headingid { get; set; }
            public int AtbuD_Subheading { get; set; }
            public int AtbuD_itemid { get; set; }
            public int AtbuD_SubItemid { get; set; }
            public string? AtbuD_DELFLG { get; set; }
            public int AtbuD_CRBY { get; set; }
            public int AtbuD_UPDATEDBY { get; set; }
            public string? AtbuD_STATUS { get; set; }
            public string? AtbuD_Progress { get; set; }
            public string? AtbuD_IPAddress { get; set; }
            public int AtbuD_CompId { get; set; }
        }

        //FreezeForNextDuration

        public class FreezeNextYearTrialBalanceDto
        {
            // 🔹 Master Table (Acc_TrailBalance_Upload)
            public int AtbU_ID { get; set; }
            public string? AtbU_CODE { get; set; }
            public string? AtbU_Description { get; set; }
            public int AtbU_CustId { get; set; }
            public int AtbU_YEARId { get; set; }
            public decimal AtbU_Closing_Debit_Amount { get; set; }
            public decimal AtbU_Closing_Credit_Amount { get; set; }
            public string? AtbU_DELFLG { get; set; }
            public int AtbU_CRBY { get; set; }
            public string? AtbU_STATUS { get; set; }
            public int AtbU_UPDATEDBY { get; set; }
            public string? AtbU_IPAddress { get; set; }
            public int AtbU_CompId { get; set; }
            public int AtbU_Branchid { get; set; }
            public int AtbU_QuarterId { get; set; }

            // 🔹 Detail Table (Acc_TrailBalance_Upload_Details)
            public int AtbuD_ID { get; set; }
            public int AtbuD_Masid { get; set; }
            public string? AtbuD_CODE { get; set; }
            public string? AtbuD_Description { get; set; }
            public int AtbuD_CustId { get; set; }
            public int AtbuD_SChedule_Type { get; set; }
            public int AtbuD_Branchid { get; set; }
            public int AtbuD_QuarterId { get; set; }
            public int AtbuD_Company_Type { get; set; }
            public int AtbuD_Headingid { get; set; }
            public int AtbuD_Subheading { get; set; }
            public int AtbuD_itemid { get; set; }
            public int AtbuD_SubItemid { get; set; }
            public string? AtbuD_DELFLG { get; set; }
            public int AtbuD_CRBY { get; set; }
            public int AtbuD_UPDATEDBY { get; set; }
            public string? AtbuD_STATUS { get; set; }
            public string? AtbuD_Progress { get; set; }
            public string? AtbuD_IPAddress { get; set; }
            public int AtbuD_CompId { get; set; }
            public string? HeadingName { get; set; }
        }

        //DownloadExcelFileAndTemplate
        public class FileDownloadResult
         {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //CheckTrailBalanceRecordExists
        public class CheckTrailBalanceRecordExistsDto
        {
            public int CompId { get; set; }
            public int CustId { get; set; }
            public int YearId { get; set; }
            public int BranchId { get; set; }
            public int QuarterId { get; set; }
            public string Description { get; set; }
        }

        //SaveTrailBalanceDetails
        public class TrailBalanceDetailsDto
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
            public string ATBU_Branchid { get; set; }
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

        //UpdateTrailBalance
        public class UpdateTrailBalanceDto
        {

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
        }

        //LoadSubHeadingByHeading
        public class LoadSubHeadingByHeadingDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        //LoadItemBySubHeading
        public class LoadItemBySubHeadingDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        //LoadSubItemByItem
        public class LoadSubItemByItemDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        //GetPreviousLoadId
        public class HierarchyRequestDto
        {
            public int? SubItemId { get; set; }
            public int? ItemId { get; set; }
            public int? SubHeadingId { get; set; }
        }

        public class HierarchyResponseDto
        {
            public int? HeadingId { get; set; }
            public int? SubHeadingId { get; set; }
            public int? ItemId { get; set; }
            public int? SubItemId { get; set; }
        }

        //UploadTrialBalance
        public class UpdateNetIncomeRequestDto
        {
            public int CompId { get; set; }
            public int CustId { get; set; }
            public int UserId { get; set; }
            public int YearId { get; set; }
            public string BranchId { get; set; } = string.Empty;
            public int DurationId { get; set; }
        }

        //SaveMappingTransactionDetails
        public class SaveMappingTransactionDetailsDto
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
            public MappingTransactionDto Transactions { get; set; }
        }

        public class MappingTransactionDto
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
    }
}

