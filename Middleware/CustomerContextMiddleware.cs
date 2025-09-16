using TracePca.Interface.DatabaseConnection;

namespace TracePca.Middleware
{
    public class CustomerContextMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomerContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, ICustomerContext customerContext)
        {
            // Read X-Customer-Code header if present
            if (context.Request.Headers.TryGetValue("X-Customer-Code", out var customerCode))
            {
                customerContext.CustomerCode = customerCode.ToString()?.Trim();
            }

            // Keep session alive by writing a dummy value
            context.Session.SetString("KeepAlive", DateTime.UtcNow.ToString());

            await _next(context);
        }

    }
}



