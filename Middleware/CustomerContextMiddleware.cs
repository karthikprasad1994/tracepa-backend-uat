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
            if (context.Request.Headers.TryGetValue("X-Customer-Code", out var customerCode))
            {
                customerContext.CustomerCode = customerCode.ToString()?.Trim();
            }

            await _next(context);
        }
    }
}



