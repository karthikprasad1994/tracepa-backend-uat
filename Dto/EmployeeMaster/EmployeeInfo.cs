namespace TracePca.Dto.EmployeeMaster
{
    public class EmployeeInfo
    {
        public int UserId { get; set; }
        public string EmpCode { get; set; }
        public string EmployeeName { get; set; }
        public string LoginName { get; set; }
      //  public string Password { get; set; }
      //  public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }  // <-- comes from join
       
    }
}
