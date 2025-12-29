using Serilog.Context;

namespace TKH.Presentation.Middlewares
{
    public class LogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public LogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //var username = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Anonymous";
            string traceId = context.TraceIdentifier;

            //using (LogContext.PushProperty("Username", username))
            using (LogContext.PushProperty("TraceId", traceId))
            {
                await _next(context);
            }
        }
    }
}
