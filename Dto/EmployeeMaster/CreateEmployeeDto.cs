namespace TracePca.Dto.EmployeeMaster
{
    public class CreateEmployeeDto
    {
        public string EmployeeCode { get; set; }   // usr_Code
        public string EmployeeName { get; set; }   // usr_FullName
        public string Email { get; set; }          // usr_Email
        public string MobileNo { get; set; }       // usr_MobileNo
        public string LoginName { get; set; }      // usr_LoginName
        public string Password { get; set; }       // usr_Password (hash in real apps!)
       // public int DesignationId { get; set; }     // Usr_Role (FK to SAD_GrpOrLvl_General_Master)
        public int RoleId { get; set; }            // if separate role column exists
       // public string Permission { get; set; }     // usr_Permission
        public int CompId { get; set; }
    }
}
