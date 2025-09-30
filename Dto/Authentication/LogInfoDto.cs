namespace TracePca.Dto.Authentication
{
    public class LogInfoDto
    {
        public int UserId { get; set; }
        public string? RoleName { get; set; }
        //public string ClientUserId { get; set; }
        public string UserEmail { get; set; }

        // Current login
        public string LoginDate { get; set; }    // dd/MM/yyyy
        public string LoginTime { get; set; }    // hh:mm tt

        // Logout info
        public string LogoutDate { get; set; }   // dd/MM/yyyy
        public string LogoutTime { get; set; }   // hh:mm tt

        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string Browser { get; set; }
       // public bool IsRevoked { get; set; }
        public string Status { get; set; }
        public string UserTimeLine { get; set; }
    }
}
