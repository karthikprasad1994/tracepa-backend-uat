using System.Text.Json.Serialization;

namespace TracePca.Dto.Middleware
{
    public class ErrorLogDto
    {
             [JsonIgnore]
            public int LogId { get; set; }
            public string FormName { get; set; }
            public string Controller { get; set; }
            public string Action { get; set; }
            public string ErrorMessage { get; set; }
            public string StackTrace { get; set; }
            public int UserId { get; set; }
            public int CustomerId { get; set; }  // Keep this for consistency, even if not used
            public string Description { get; set; }

           public DateTime CreatedDate { get; set; } = DateTime.Now;
           public int? ResponseTime { get; set; }
    }

    }

