using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class SessionTimeoutMiddleware
{
    private readonly RequestDelegate _next;

    public SessionTimeoutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if session exists
        var customerCode = context.Session.GetString("CustomerCode");
        if (string.IsNullOrWhiteSpace(customerCode))
        {
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"success\": false, \"message\": \"Session timed out. Please login again.\"}");
            return;
        }

        // Session is valid, continue
        await _next(context);
    }
}
