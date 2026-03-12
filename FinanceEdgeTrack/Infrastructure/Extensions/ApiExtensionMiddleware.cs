using Microsoft.AspNetCore.Diagnostics;
using FinanceEdgeTrack.Error;

namespace FinanceEdgeTrack.Infrastructure.Extensions;

public static class ApiExtensionMiddleware
{
    public static void ConfigureExceptionHandler(IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandlerFeature != null)
                {
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        MessageError = exceptionHandlerFeature.Error.Message,
                        StatusCode = context.Response.StatusCode,
                        Trace = exceptionHandlerFeature.Error.StackTrace
                    }.ToString());
                }
            });
        });
    }
}
