namespace TracePca.Dto.ProfileSetting
{
    public class ProfileSettingDto
    {
        //GetUserProfile
        public class TracePaGetUserProfileDto
        {
            public int UserId { get; set; }
            public string? MobileNo { get; set; }
            public string? Email { get; set; }
            public string? Experience { get; set; }
            public string? LoginName { get; set; }
            public string? SAPCode { get; set; }
            public string? EmpName { get; set; }
            public string? Designation { get; set; }
            public string? Role { get; set; }
            public string? Permission { get; set; }
        }

        //ChangePassword
        //public class TracePaChangePasswordDto
        //{
        //    public string Status { get; set; }
        //    public int UserId { get; set; }
        //    public string LoginName { get; set; }
        //    public string NewPassword { get; set; }
        //}
        public class TracePaChangePasswordDto
        {
            public int UserId { get; set; } 
            public string NewPassword { get; set; }   
        }



        //LicenseInformation
        public class TracePaLicenseInformationDto
        {
            public int CustomerId { get; set; }
            public string EmailId { get; set; }
            public string ModuleId { get; set; }
            public string RegistrationNumber { get; set; }
            public string CustomerName { get; set; }
            public string CustomerCode { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string BillingFrequency { get; set; }
            public string DataSize { get; set; }
            public int? NoOfCustomers { get; set; }
            public int? NoOfUsers { get; set; }

        }

        //UpdateUserProfile
        public class UpdateUserProfileDto
        {
            public int Id { get; set; }     
            public string? Experience { get; set; }
           
        }
    }
}
