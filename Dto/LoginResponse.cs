using System.ComponentModel.DataAnnotations;

namespace TracePca.Dto
{
    public class LoginResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }

        public int UsrId { get; set; }
        public string usertype { get; set; }

        public string? MCR_emails { get; set; }

        public string? YmsId { get; set; }           // should be int? ✅
        public int? YmsYearId { get; set; }

        public string ClientIpAddress { get; set; }

        public string CustomerCode { get; set; }

        public string? SystemIpAddress { get; set; }

        public string RoleName { get; set; }


        public string UserEmail { get; set; }

        public List<int> ModuleIds { get; set; } = new();

        public List<int> PkIds { get; set; } = new();

        public int CustomerId { get; set; }

        public bool ActivePlan { get; set; }


    }
    public class UpdatePasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [MaxLength(50)]
        public string Password { get; set; }

    }
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }
    public class VerifyOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

}
