namespace TracePca.Dto
{
    public class OtpResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string Otp { get; set; }
    }
}
