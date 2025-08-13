namespace TracePca.Dto.Middleware
{
    public class ApiAlertDto
    {

        public string FormName { get; set; }
        public string ApiName { get; set; }
        public int LatestResponseTimeMs { get; set; }
        public double AvgResponseTimeMs { get; set; }
        public string Message { get; set; }  
    }
}
