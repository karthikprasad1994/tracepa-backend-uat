using System.Diagnostics;
using System.Security.Claims;
using TracePca.Dto.Middleware;
using TracePca.Interface.Middleware;

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
                await _next(context); // Process the request normally
            }
            catch (Exception ex)
            {
                stopwatch.Stop(); // Stop timing on exception

                // Use controller/action from route, default "Login" for login attempts
                string controller = context.Request.RouteValues["controller"]?.ToString() ?? "Unknown";
                string action = context.Request.RouteValues["action"]?.ToString() ?? "Unknown";
                string formName = context.Request.Headers["X-Form-Name"].FirstOrDefault() ?? "Unknown";

                string userIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                int.TryParse(userIdStr, out int userId);

                // Log the error to DB
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

                // Respond to client
                if (ex is UnauthorizedAccessException) // Invalid credentials
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        StatusCode = 401,
                        Message = ex.Message
                    });
                }
                else
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        StatusCode = 500,
                        Message = "An internal error occurred. Please contact support."
                    });
                }
            }
        }
    }
}
