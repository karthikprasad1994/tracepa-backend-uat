namespace TracePca.Dto.ClientPortal
{
    public class ClientPortalDto
    {
        public class GetUserCustomerIdRequest
        {
            public string CompanyId { get; set; }

            public string Cust_Name { get; set; }
            public string UserId { get; set; }
        }
        public class UserCustomerResponse
        {
            public string CompanyId { get; set; }
            public string CustomerName { get; set; }
        }
        public class DRLLogDto
        {
            public int DRLID { get; set; }
            public int CheckPointID { get; set; }
            public string CheckPoint { get; set; }
            public string DocumentRequestedList { get; set; }
            public int DocumentRequestedListID { get; set; }
            public string DocumentRequestedType { get; set; }
            public int DocumentRequestedTypeID { get; set; }
            public string EmailID { get; set; }
            public DateTime? RequestedOn { get; set; }
            public DateTime? TimlinetoResOn { get; set; }
            public string Comments { get; set; }
            public string Status { get; set; }
            public string ReceivedComments { get; set; }
            public DateTime? ReceivedOn { get; set; }
            public int AttachID { get; set; }
            public int DocID { get; set; }
            public int ReportType { get; set; }

            public int Count { get; set; }

            public string AllAtchIds { get; set; }
        }
        public class LoadDRLLogRequest
        {
            public int CompanyId { get; set; }
            public int AuditNo { get; set; }
            public int CustId { get; set; }
            public int YearId { get; set; }
        }
        public class LoadDRLLogResponse
        {
            public bool Success { get; set; }
            public List<DRLLogDto> Data { get; set; }
            public string Message { get; set; }
        }
        public class AttachmentRequestDto
        {
            public int CompanyId { get; set; }
            public int CustomerId { get; set; }
            public int AuditId { get; set; }
            public int AttachId { get; set; }
            public int Type { get; set; }
            public string DuringAttachIds { get; set; }  // comma-separated
        }
        public class AttachmentResponseDto
        {
            public int SrNo { get; set; }
            public int AtchDocID { get; set; }
            public int AtchID { get; set; }
            public int DrlId { get; set; }
            public string FileName { get; set; }
            public string Remarks { get; set; }
            public string DRLName { get; set; }
            public string Description { get; set; }
            public string CreatedBy { get; set; }
            public string CreatedOn { get; set; }
            public string FileSize { get; set; }
            public string Extension { get; set; }
            public string ReportType { get; set; }
        }


    }
}
