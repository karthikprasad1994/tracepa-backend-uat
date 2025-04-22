namespace TracePca.Dto
{
    public class AddLocationDto
    {
        public int? Id { get; set; } // Nullable for insert
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int YearID { get; set; }
        public int ParentID { get; set; } = 0;
    }
}
