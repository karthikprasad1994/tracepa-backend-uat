namespace TracePca.Dto.FIN_Statement
{
    public class ExcelUploadDto
    {
        //DownloadExcelFileAndTemplate
        public class FileDownloadResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

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
            public int Durationid { get; set; }
            public string? DurationName { get; set; }
        }

        //GetBranchName
        public class CustBranchDto
        {
            public int Branchid { get; set; }
            public string? BranchName { get; set; }
        }

        //SaveAllInformation
        public class SaveAllInformationDto
        {

            //ScheduleHeading
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


            //ScheduleSub-Heading
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
            public string ASSH_Notes { get; set; }
            public string ASSH_scheduletype { get; set; }
            public string ASSH_Orgtype { get; set; }

            //ScheduleItem
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

            //ScheduleSub-Item
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
            public string? ASSI_ScheduleType { get; set; }
            public string? ASSI_OrgType { get; set; }

            //SaveTrialBalanceExcelUpload
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
            public string ATBU_DELFLG { get; set; }
            public int ATBU_CRBY { get; set; }
            public string ATBU_STATUS { get; set; }
            public int ATBU_UPDATEDBY { get; set; }
            public string? ATBU_IPAddress { get; set; }
            public int ATBU_CompId { get; set; }
            public int ATBU_YEARId { get; set; }
            public int ATBU_Branchid { get; set; }
            public int ATBU_QuarterId { get; set; }

            //SaveTrialBalanceExcelUploadDetails
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

        //SaveAllInformation
        public class UploadExcelRequestDto
        {
            public int CompId { get; set; }
            public int FinancialYearId { get; set; }
            public int ScheduleType { get; set; }
            public int CustomerId { get; set; }
            public int QuarterId { get; set; }
            public int UserId { get; set; }
            public string IpAddress { get; set; } = null!;
            public int BranchId { get; set; }

            public List<ExcelRowDto> ExcelRows { get; set; } = new();
        }

        public class ExcelRowDto
        {
            public string? AccountHead { get; set; }     
            public string Heading { get; set; } = null!;
            public string SubHeading { get; set; } = null!;
            public string Item { get; set; } = null!;
            public string SubItem { get; set; } = null!;
            public string Description { get; set; } = null!;
            public string DescriptionCode { get; set; } = null!;

            public decimal OpeningDebit { get; set; }
            public decimal OpeningCredit { get; set; }
            public decimal TrDebit { get; set; }
            public decimal TrCredit { get; set; }
            public decimal ClosingDebit { get; set; }
            public decimal ClosingCredit { get; set; }

            // Optional: you may need these if used somewhere else or for saving details
            public int HeadingId { get; set; }
            public int SubHeadingId { get; set; }
            public int ItemId { get; set; }
            public int SubItemId { get; set; }
        }
    }
}
    
