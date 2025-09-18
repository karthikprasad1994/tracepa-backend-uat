namespace TracePca.Dto.CustomerUserMaster
{
    public class CustomerUsersDetailsDto
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
         public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public string EmpCode { get; set; }
        public string EmployeeName { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }

        public string MobileNo { get; set; }

        public string OfficePhoneNo { get; set; }
        public string LastLoginDate { get; set; }
       // public string Designation { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        

    }
}
