namespace TracePca.Dto
{
    public class AddBayDto
    {
        public int? Id { get; set; }  // Null for new entry, ID for update
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int DepartmentId { get; set; } // Parent ID (Bay belongs to a Department)
        public int CreatedBy { get; set; }
        public int YearID { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public string IPAddress { get; set; }
    }
}
