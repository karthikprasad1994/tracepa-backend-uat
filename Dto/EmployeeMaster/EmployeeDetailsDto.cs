namespace TracePca.Dto.EmployeeMaster
{
    public class EmployeeDetailsDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }      // usr_Code
        public string EmployeeName { get; set; }   // usr_FullName
        public string UserName { get; set; }      // usr_LoginName
        public string Role { get; set; }
        public string LastLoginDate { get; set; }
        public string Status { get; set; }
        public string MobileNo { get; set; }
        public int PermissionId { get; set; }

    }
}
