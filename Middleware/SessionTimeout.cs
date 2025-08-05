//namespace TracePca.Middleware
//{
//    public class SessionTimeout
//    {
//        private readonly RequestDelegate _next;

//        public SessionTimeout(RequestDelegate next)
//        {
//            _next = next;
//        }

<<<<<<< HEAD
=======
 
//        public async Task InvokeAsync(HttpContext context)
//        {
//            var path = context.Request.Path.Value?.ToLower();

//            // Skip static files, login, Swagger UI and Swagger JSON endpoints
//            if (path != null &&
//                (path.Contains("/login")
//                 || path.Contains("/static")
//                 || path.Contains("/swagger")
//                 || path.Contains("/favicon.ico")))
//            {
//                await _next(context);
//                return;
//            }

//            var isLoggedIn = context.Session.GetString("IsLoggedIn");

//            if (string.IsNullOrEmpty(isLoggedIn))
//            {
//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                context.Response.ContentType = "application/json";

//                await context.Response.WriteAsync("{\"message\": \"Session timed out, please login again.\"}");
//                return;
//            }

//            await _next(context);
//        }

>>>>>>> b010d62 (dafney code updated 2)
//        //public async Task InvokeAsync(HttpContext context)
//        //{
//        //    var path = context.Request.Path.Value?.ToLower();

//        //    // Skip static files, login, Swagger UI and Swagger JSON endpoints
//        //    if (path != null &&
//        //        (path.Contains("/login")
//        //         || path.Contains("/static")
//        //         || path.Contains("/swagger")
//        //         || path.Contains("/favicon.ico")))
//        //    {
//        //        await _next(context);
//        //        return;
//        //    }

//        //    var isLoggedIn = context.Session.GetString("IsLoggedIn");

//        //    if (string.IsNullOrEmpty(isLoggedIn))
//        //    {
//        //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//        //        context.Response.ContentType = "application/json";

//        //        await context.Response.WriteAsync("{\"message\": \"Session timed out, please login again.\"}");
//        //        return;
//        //    }

//        //    await _next(context);
//        //}
<<<<<<< HEAD

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var path = context.Request.Path.Value?.ToLower();

//            // Skip static files, login, Swagger UI and Swagger JSON endpoints
//            if (path != null &&
//                (path.Contains("/login")
//                 || path.Contains("/static")
//                 || path.Contains("/swagger")
//                 || path.Contains("/favicon.ico")))
//            {
//                await _next(context);
//                return;
//            }

//            var isLoggedIn = context.Session.GetString("IsLoggedIn");

//            if (string.IsNullOrEmpty(isLoggedIn))
//            {
//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                context.Response.ContentType = "application/json";

//                await context.Response.WriteAsync("{\"message\": \"Session timed out, please login again.\"}");
//                return;
//            }

//            await _next(context);
//        }
=======
 
>>>>>>> b010d62 (dafney code updated 2)
//    }
//}


