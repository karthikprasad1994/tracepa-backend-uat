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
        public string? Address { get; set; }
        public string? McrProductKey { get; private set; }

        public string? MCR_IPAddress { get; private set; }
        public string? MCR_Location { get; set; }

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

        public List<int>? ModuleIds { get; set; }

    }


    public class DashboardCounts
    {
        public int TotalClients { get; set; }
        public int NewSignup30Days { get; set; }
        public int TrialUsers { get; set; }
        public int PendingIssues { get; set; }
        public int ResolvedIssues { get; set; }
        public int ApprovalStatus { get; set; }
        public string PendingStatus { get; set; }
        public string RejectedStatus { get; set; }
    }


    public class ClientDetails
    {
        public int FirmID { get; set; }
        public string FirmName { get; set; }
        public string Email { get; set; }
        public string ModuleNames { get; set; }
        public string NumberOfUsers { get; set; }
        public string SignedDate { get; set; }
        public string Types { get; set; }
        public string IssueIDentified { get; set; }
       
    }


    public class ClientViewDetails
    {
        public int FirmID { get; set; }
        public string FirmName { get; set; }
        public string Email { get; set; }
        public string SignedDate { get; set; }
        public string AccessCode { get; set; }
        public string FirstLogin { get; set; }
        public string LastLogin { get; set; }
        public string TimeSpent { get; set; }
        public string TimeLoges { get; set; }

    }
}

