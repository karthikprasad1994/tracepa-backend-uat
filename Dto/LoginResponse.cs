namespace TracePca.Dto
{
    public class LoginResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }

        public int UsrId { get; set; }

        public string? MCR_emails { get; set; }

    }
}
