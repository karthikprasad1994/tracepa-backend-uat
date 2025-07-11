namespace TracePca.Dto
{
    public class UserPermissionRequestDto
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
       // public string ModuleCode { get; set; }
        public int CheckModuleId { get; set; }
    }
}
