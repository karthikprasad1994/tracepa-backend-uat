namespace TracePca.Dto.Dashboard
{
    public class DashboardDto
    {
        public class RemarksCountDto
        {
            public string? RemarkStatus { get; set; }
            public int RemarkCount { get; set; }
        }
        public class RemarksSummaryDto
        {
            public int Sent { get; set; }
            public int Received { get; set; }
        }
        public class StandardAuditF1DTO
        {
            public string CustName { get; set; }
            public string SA_AuditNo { get; set; }
            public string AuditType { get; set; }
            public DateTime? SA_AuditOpinionDate { get; set; }
            public DateTime? SA_MRLDate { get; set; }
            public DateTime? SA_FilingDatePCAOB { get; set; }
            public DateTime? SA_BinderCompletedDate { get; set; }
        }

        public class StandardAuditF2DTO
        {
            public string CustName { get; set; }
            public string SA_AuditNo { get; set; }
            public string AuditType { get; set; }
            public DateTime? SA_RptFilDate { get; set; }
            public DateTime? SA_RptRvDate { get; set; }
            public DateTime? SA_MRSDate { get; set; }
        }



        public class ApiResponse<T>
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }


    }
}
