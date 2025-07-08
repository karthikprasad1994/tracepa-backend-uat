namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleNoteDto
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
            public int YearId { get; set; }
            public string? Id { get; set; }
        }

        //GetSubHeadingname(Notes For SubHeading)
        public class SubHeadingNoteDto
        {
            public int ASHN_ID { get; set; }
            public string Description { get; set; }
        }

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        public class SubHeadingNotesDto
        {
            public int ASHN_ID { get; set; }
            public int ASHN_SubHeadingId { get; set; }
            public int ASHN_CustomerId { get; set; }
            public string ASHN_Description { get; set; }
            public string ASHN_DelFlag { get; set; }
            public string ASHN_Status { get; set; }
            public string ASHN_Operation { get; set; }
            public int ASHN_CreatedBy { get; set; }
            public DateTime ASHN_CreatedOn { get; set; }
            public int ASHN_CompID { get; set; }
            public int ASHN_YearID { get; set; }
            public string ASHN_IPAddress { get; set; }
        }

        //GetBranch(Notes For Ledger)
        public class CustBranchDto
        {
            public int Branchid { get; set; }
            public string? BranchName { get; set; }
        }

        //GetLedger(Notes For Ledger)
        public class LedgerIndividualDto
        {
            public string Description { get; set; }
            public int ASHL_ID { get; set; }
        }

        //SaveOrUpdateLedger(Notes For Ledger)
        public class SubHeadingLedgerNoteDto
        {
            public int ASHL_ID { get; set; }
            public int ASHL_SubHeadingId { get; set; }
            public int ASHL_CustomerId { get; set; }
            public int ASHL_BranchId { get; set; }
            public string ASHL_Description { get; set; }
            public string ASHL_DelFlag { get; set; }
            public string ASHL_Status { get; set; }
            public string ASHL_Operation { get; set; }
            public int ASHL_CreatedBy { get; set; }
            public DateTime ASHL_CreatedOn { get; set; }
            public int ASHL_CompID { get; set; }
            public int ASHL_YearID { get; set; }
            public string ASHL_IPAddress { get; set; }
        }

        //DownloadNotesExcel
        public class ExcelFileDownloadResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //DownloadNotesPdf
        public class PdfFileDownloadResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //SaveOrUpdate
        public class FirstScheduleNoteDto
        {
            public int SNF_ID { get; set; }
            public int SNF_CustId { get; set; }
            public string SNF_Description { get; set; }
            public string SNF_Category { get; set; }
            public decimal SNF_CYear_Amount { get; set; }
            public decimal SNF_PYear_Amount { get; set; }
            public int SNF_YearID { get; set; }
            public int SNF_CompID { get; set; }
            public string SNF_Status { get; set; }
            public string SNF_DelFlag { get; set; }
            public DateTime SNF_CRON { get; set; }
            public int SNF_CrBy { get; set; }
            public string SNF_IPAddress { get; set; }
        }
    }
}
