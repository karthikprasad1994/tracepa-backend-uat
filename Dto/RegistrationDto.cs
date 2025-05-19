namespace TracePca.Dto
{
    public class RegistrationDto
    {
        public int McrId { get; set; }

        public int? McrMpId { get; set; }

        public string? McrCustomerName { get; set; }
        public string? McrCustomerCode { get; set; }

        public string? McrCustomerEmail { get; set; }
        public string? McrCustomerTelephoneNo { get; set; }
        public string? McrProductKey { get; private set; }

        public string? McrTstatus { get; set; }

        //  public string? McrEmailsJson { get; set; }

        //  public string? McrEmails { get; set; }

        public string? McrStatus { get; set; }

        public int StatusCode { get; set; }  // Default to 200 (OK)
        public string? Message { get; set; }

        public int UsrId { get; set; }

        public int? UsrNode { get; set; }

        public string? UsrCode { get; set; }

        public string? UsrFullName { get; set; }

        public string? UsrLoginName { get; set; }

        public string? UsrPassWord { get; set; }

        public string? UsrEmail { get; set; }

        public string? UsrMobileNo { get; set; }

        public string? Otp { get; set; }
        public string? MCR_emails { get; set; }

    }
}

