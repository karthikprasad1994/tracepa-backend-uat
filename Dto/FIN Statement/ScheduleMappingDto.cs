using TracePca.Dto.Audit;

namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleMappingDto
    {
        //GetCustomersName
        public class CustDto
        {
            public int Cust_Id { get; set; }
            public string? Cust_Name { get; set; }
        }

        //GetFinancialYear
        public class FinancialYearDto
        {
            public int YMS_YEARID { get; set; }
            public string? YMS_ID { get; set; }
        }

        //GetDuration
        public class CustDurationDto
        {
            public int Branchid { get; set; }
            public string? BranchName { get; set; }
        }

        //GetBranchName
        public class CustBranchDto
        {
            public int Branchid { get; set; }
            public string? BranchName { get; set; }
        }

 
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

        //SaveOrUpdateTrailBalanceUpload
        public class TrailBalanceUploadDto
        {
            public int ATBU_ID { get; set; }
            public string? ATBU_CODE { get; set; }
            public string? ATBU_Description { get; set; }
            public int ATBU_CustId { get; set; }
            public decimal ATBU_Opening_Debit_Amount { get; set; }
            public decimal ATBU_Opening_Credit_Amount { get; set; }
            public decimal ATBU_TR_Debit_Amount { get; set; }
            public decimal ATBU_TR_Credit_Amount { get; set; }
            public decimal ATBU_Closing_Debit_Amount { get; set; }
            public decimal ATBU_Closing_Credit_Amount { get; set; }
            public string ATBU_DELFLG{ get; set; }
            public int ATBU_CRBY { get; set; }
            public string ATBU_STATUS { get; set; }
            public int ATBU_UPDATEDBY { get; set; }
            public string? ATBU_IPAddress { get; set; }
            public int ATBU_CompId { get; set; }
            public int ATBU_YEARId { get; set; }
            public int ATBU_Branchid { get; set; }
            public int ATBU_QuarterId { get; set; }
        }

        //SaveOrUpdateTrialBalanceUploadDetails
        public class TrailBalanceUploadDetailsDto
        {
            public int? ATBUD_ID { get; set; }
            public int? ATBUD_Masid { get; set; }
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
            public string? ATBUD_DELFLG { get; set; }
            public int ATBUD_CRBY { get; set; }
            public int ATBUD_UPDATEDBY { get; set; }
            public string? ATBUD_STATUS { get; set; }
            public string? ATBUD_Progress { get; set; }
            public string? ATBUD_IPAddress { get; set; }
            public int ATBUD_CompId { get; set; }
            public int ATBUD_YEARId { get; set; }
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

        //SaveScheduleTemplate
        public class AccTrailBalanceUploadBatchDto
        {
            // Master table fields (spAcc_TrailBalance_Upload)
            public int ATBU_ID { get; set; }
            public string? ATBU_CODE { get; set; }
            public string? ATBU_Description { get; set; }
            public int ATBU_CustId { get; set; }
            public decimal ATBU_Opening_Debit_Amount { get; set; }
            public decimal ATBU_Opening_Credit_Amount { get; set; }
            public decimal ATBU_TR_Debit_Amount { get; set; }
            public decimal ATBU_TR_Credit_Amount { get; set; }
            public decimal ATBU_Closing_Debit_Amount { get; set; }
            public decimal ATBU_Closing_Credit_Amount { get; set; }
            public int ATBU_CRBY { get; set; }
            public int ATBU_UPDATEDBY { get; set; }
            public string? ATBU_IPAddress { get; set; }
            public int ATBU_CompId { get; set; }
            public int ATBU_YEARId { get; set; }
            public int ATBU_Branchid { get; set; }
            public int ATBU_QuarterId { get; set; }

            // Detail table values (applied to all rows via dto instead of per row — can be adjusted if needed)
            public int ATBUD_ID { get; set; }
            public int ATBUD_Masid { get; set; }
            public string? ATBUD_CODE { get; set; }
            public string? ATBUD_Description { get; set; }
            public int ATBUD_CustId { get; set; }
            public string? ATBUD_SChedule_Type { get; set; }
            public int ATBUD_Branchid { get; set; }
            public int ATBUD_QuarterId { get; set; }
            public int ATBUD_Company_Type { get; set; }
            public int ATBUD_Headingid { get; set; }
            public int ATBUD_Subheading { get; set; }
            public int ATBUD_itemid { get; set; }
            public int ATBUD_SubItemId { get; set; }
            public int ATBUD_CRBY { get; set; }
            public int ATBUD_UPDATEDBY { get; set; }
            public string? ATBUD_IPAddress { get; set; }
            public int ATBUD_CompId { get; set; }

            // Row data list
            public List<AccTrailBalanceUploadRowDto> Rows { get; set; } = new();
            public class AccTrailBalanceUploadRowDto
            {
                public int ATBU_ID { get; set; }
            }
        }

        //UploadExcelFile
        //public class ExcelUploadResultDto
        //{
        //    public List<string> SheetNames { get; set; } = new List<string>();
        //    public bool IsExistingData { get; set; }
        //    public string Message { get; set; }
        //}

        //FreezeForPreviousDuration
        public class FreezePreviousDurationRequestDto
        {
            public int AtbuId { get; set; }
            public string AtbuCode { get; set; }
            public string AtbuDescription { get; set; }
            public int AtbuCustId { get; set; }
            public decimal OpeningDebitAmount { get; set; }
            public decimal OpeningCreditAmount { get; set; }
            public decimal TrDebitAmount { get; set; }
            public decimal TrCreditAmount { get; set; }
            public decimal ClosingDebitAmount { get; set; }
            public decimal ClosingCreditAmount { get; set; }
            public string AtbuDelflg { get; set; }
            public int AtbuCrBy { get; set; }
            public string AtbuStatus { get; set; }
            public int AtbuUpdatedBy { get; set; }
            public string AtbuIpAddress { get; set; }
            public int AtbuCompId { get; set; }
            public int YearId { get; set; }
            public int AtbuBranchId { get; set; }
            public int AtbuQuarterId { get; set; }

            public List<TrailBalanceUploadDetailDto> ScheduleItems { get; set; }
            public class TrailBalanceUploadDetailDto
            {
                public int? AtbudId { get; set; }
                public int AtbudMasid { get; set; }
                public string AtbudCode { get; set; }
                public string AtbudDescription { get; set; }
                public int AtbudCustId { get; set; }
                public int AtbudScheduleType { get; set; }
                public int AtbudBranchId { get; set; }
                public int AtbudQuarterId { get; set; }
                public int AtbudCompanyType { get; set; }
                public int AtbudHeadingId { get; set; }
                public int AtbudSubheadingId { get; set; }
                public int AtbudItemId { get; set; }
                public int AtbudSubItemId { get; set; }
                public string AtbudDelflg { get; set; } = "A";
                public int AtbudCrBy { get; set; }
                public int AtbudUpdatedBy { get; set; }
                public string AtbudStatus { get; set; } = "C";
                public string AtbudProgress { get; set; } = string.Empty;
                public string AtbudIpAddress { get; set; } = string.Empty;
                public int AtbudCompId { get; set; }
                public int YearId { get; set; }
            }
        }

        //FreezeForNextDuration
        public class FreezeNextDurationRequestDto
        {
            public int AtbuId { get; set; }
            public string AtbuCode { get; set; }
            public string AtbuDescription { get; set; }
            public int AtbuCustId { get; set; }
            public decimal OpeningDebitAmount { get; set; }
            public decimal OpeningCreditAmount { get; set; }
            public decimal TrDebitAmount { get; set; }
            public decimal TrCreditAmount { get; set; }
            public decimal ClosingDebitAmount { get; set; }
            public decimal ClosingCreditAmount { get; set; }
            public string AtbuDelflg { get; set; }
            public int AtbuCrBy { get; set; }
            public string AtbuStatus { get; set; }
            public int AtbuUpdatedBy { get; set; }
            public string AtbuIpAddress { get; set; }
            public int AtbuCompId { get; set; }
            public int YearId { get; set; }
            public int AtbuBranchId { get; set; }
            public int AtbuQuarterId { get; set; }
            public List<TrailBalanceUploadDetails1Dto> ScheduleItems { get; set; }
            public class TrailBalanceUploadDetails1Dto
            {
                public int? AtbudId { get; set; }
                public int AtbudMasid { get; set; }
                public string AtbudCode { get; set; }
                public string AtbudDescription { get; set; }
                public int AtbudCustId { get; set; }
                public int AtbudScheduleType { get; set; }
                public int AtbudBranchId { get; set; }
                public int AtbudQuarterId { get; set; }
                public int AtbudCompanyType { get; set; }
                public int AtbudHeadingId { get; set; }
                public int AtbudSubheadingId { get; set; }
                public int AtbudItemId { get; set; }
                public int AtbudSubItemId { get; set; }
                public string AtbudDelflg { get; set; } = "A";
                public int AtbudCrBy { get; set; }
                public int AtbudUpdatedBy { get; set; }
                public string AtbudStatus { get; set; } = "C";
                public string AtbudProgress { get; set; } = string.Empty;
                public string AtbudIpAddress { get; set; } = string.Empty;
                public int AtbudCompId { get; set; }
                public int YearId { get; set; }
            }
        }

        //DownloadExcelFileAndTemplate
        public class FileDownloadResult
         {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }
    }
}

