namespace TracePca.DTOs
{
    public class CreateOrderDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Receipt { get; set; }
        public Dictionary<string, string>? Notes { get; set; }
    }
}
