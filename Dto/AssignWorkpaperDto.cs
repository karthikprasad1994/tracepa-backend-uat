
namespace TracePca.Dto
{
    public class AssignWorkpaperDto
    {
        public List<int> SACId { get; set; } = new();
        public int CompanyId { get; set; }
        public int AuditId { get; set; }
        public string CheckPointId { get; set; }
        public int WorkpaperId { get; set; }
        public int UserId { get; set; }
    }
}

