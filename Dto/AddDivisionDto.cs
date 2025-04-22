namespace TracePca.Dto
{
    public class AddDivisionDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
       // public int LevelCode { get; set; } // This will come from the request body
        public int CreatedBy { get; set; }
        public int YearID { get; set; }
        public int CompanyId { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int CustomerId { get; set; }
    }
}
