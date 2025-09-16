namespace TracePca.Dto.CustomerUserMaster
{
    public class CreateCustomerUsersDto
    {
        public int ? UserId { get; set; }
        public int? CustomerId { get; set; }

        // 0 for new employee


        public string EmpCode { get; set; } = string.Empty;


        public string UserName { get; set; } = string.Empty;
        public string LoginName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string OfficePhoneNo { get; set; } = string.Empty;

        // public int DesignationId { get; set; }
        public int RoleId { get; set; }
        public int CreatedBy { get; set; }

        public int PermissionId { get; set; }
    }
}
