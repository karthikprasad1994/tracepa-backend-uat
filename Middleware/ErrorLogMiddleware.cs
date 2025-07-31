using System.Diagnostics;
using System.Security.Claims;
using TracePca.Dto.Middleware;
using TracePca.Interface.Middleware;
using TracePca.Service.Miidleware;

namespace TracePca.Middleware
{
    public class ErrorLogMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ErrorLoggerInterface logger)
        {
            var stopwatch = Stopwatch.StartNew(); // Start timing

            try
            {
                await _next(context); // Let the request process
            }
            catch (Exception ex)
            {
                stopwatch.Stop(); // Stop timing on exception

                string controller = context.Request.RouteValues["controller"]?.ToString() ?? "Unknown";
                string action = context.Request.RouteValues["action"]?.ToString() ?? "Unknown";
                string formName = context.Request.Headers["X-Form-Name"].FirstOrDefault() ?? "Unknown";

                string userIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string email = context.User.FindFirst(ClaimTypes.Email)?.Value;

                int.TryParse(userIdStr, out int userId);

                await logger.LogErrorAsync(new ErrorLogDto
                {
                    FormName = formName,
                    Controller = controller,
                    Action = action,
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "",
                    UserId = userId,
                    CustomerId = 0,
                    Description = $"Logged by: {email}",
                    ResponseTime = (int)stopwatch.ElapsedMilliseconds
                });

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An internal error occurred. Please contact support.");
            }
        }
    }
}
