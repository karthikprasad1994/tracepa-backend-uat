namespace TracePca.Dto.FIN_Statement
{
    public class FeatchingDataDto
    {
        //Trdm
        public class DatabaseExportResultTrdmDto
        {
            public bool IsSuccess { get; set; }
            public string? Message { get; set; }
            public string? FilePath { get; set; }
            public DateTime ExportedAt { get; set; }
        }

        //Tr25_44
        public class DatabaseExportResultT25_44Dto
        {
            public bool IsSuccess { get; set; }
            public string? Message { get; set; }
            public string? FilePath { get; set; }
            public DateTime ExportedAt { get; set; }
        }

        //Customer Registraction
        public class DatabaseExportResultCustomerRegistrationDto
        {
            public bool IsSuccess { get; set; }
            public string? Message { get; set; }
            public string? FilePath { get; set; }
            public DateTime ExportedAt { get; set; }
        }
    }
}
