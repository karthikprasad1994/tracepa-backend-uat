namespace TracePca.Dto.Masters
{
    public class UploadExcelDto
    {
        //ValidateClientDetails
        public class CustomerValidationResult
        {
            public int RowNumber { get; set; }
            public string CustomerName { get; set; }
            public string Email { get; set; }
            public List<string> MissingFields { get; set; }
            public bool IsDuplicate { get; set; }
            public string? GeneratedCustomerId { get; set; }
            public Dictionary<string, string> Data { get; set; } = new();
        }
    }
}
