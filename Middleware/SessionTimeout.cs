using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public class SessionTimeoutMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TimeSpan _idleTimeout = TimeSpan.FromMinutes(90);

    public SessionTimeoutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var lastActivityStr = context.Session.GetString("LastActivity");
        DateTime lastActivity;

        if (!string.IsNullOrWhiteSpace(lastActivityStr) &&
            DateTime.TryParse(lastActivityStr, out lastActivity))
        {
            if (DateTime.UtcNow - lastActivity > _idleTimeout)
            {
                context.Session.Clear();
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"success\": false, \"message\": \"Session timed out. Please login again.\"}");
                return;
            }
        }

        // Update last activity timestamp
        context.Session.SetString("LastActivity", DateTime.UtcNow.ToString());

        await _next(context);
    }
}
