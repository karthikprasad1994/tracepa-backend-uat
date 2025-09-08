namespace TracePca.Dto.EmployeeMaster
{
    public class EmployeeBasicDetailsDto
    {
        public int? UserId { get; set; }

        // 0 for new employee

        public string EmpCode { get; set; } = string.Empty;


        public string EmployeeName { get; set; } = string.Empty;
        public string LoginName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        // public int DesignationId { get; set; }
        public int RoleId { get; set; }
        public int CreatedBy { get; set; }

        public int PermissionId { get; set; }
        // public bool SendMail { get; set; }
        //  public bool Suggestions { get; set; }
        // public bool IsPartner { get; set; }
    }

}

