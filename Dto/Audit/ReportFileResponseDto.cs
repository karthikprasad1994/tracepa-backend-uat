namespace TracePca.Dto.Audit
{
    public class ReportFileResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string WordFilePath { get; set; }
        public string PdfFilePath { get; set; }
    }
}
