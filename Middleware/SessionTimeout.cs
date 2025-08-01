namespace TracePca.Middleware
{
    public class SessionTimeout
    {
        private readonly RequestDelegate _next;

        public SessionTimeout(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Skip static files and login endpoints
            if (!path.Contains("/login") && !path.Contains("/static"))
            {
                var isLoggedIn = context.Session.GetString("IsLoggedIn");

                if (string.IsNullOrEmpty(isLoggedIn))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync("{\"message\": \"Session timed out, please login again.\"}");
                    return;
                }
            }

            await _next(context);
        }
    }
}

