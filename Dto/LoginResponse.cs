namespace TracePca.Dto
{
    public class LoginResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }

        public int UsrId { get; set; }

        public string? MCR_emails { get; set; }

        public string? YmsId { get; set; }           // should be int? ✅
        public int? YmsYearId { get; set; }

        public string ClientIpAddress { get; set; }

        public string CustomerCode { get; set; }

        public string? SystemIpAddress { get; set; }

        public string RoleName { get; set; }


        public string UserEmail { get; set; }

        public List<int> ModuleIds { get; set; } = new();

        public List<int> PkIds { get; set; } = new();

        public int CustomerId { get; set; }


        

    }
}
