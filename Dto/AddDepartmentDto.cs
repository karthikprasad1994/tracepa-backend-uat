namespace TracePca.Dto
{
    public class AddDepartmentDto
    {
        public int? Id { get; set; }  // Nullable for insert/update
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int YearID { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int DivisionId { get; set; } // Parent ID for Department
    }
}
