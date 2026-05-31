using FinanceEdgeTrack.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinanceEdgeTrack.Errors;

public class ApiExceptionFilter : ExceptionFilterAttribute
{
    private readonly IWebHostEnvironment _env;

    public ApiExceptionFilter(IWebHostEnvironment env)
    {
        _env = env;
    }

    public override void OnException(ExceptionContext context)
    {
        var error = new ErrorDetails
        {
            MessageError = context.Exception.Message,
            StatusCode = StatusCodes.Status500InternalServerError,
            Trace = _env.IsDevelopment() ? context.Exception.StackTrace : null
        };

        context.Result = new ObjectResult(error)
        {
            StatusCode = error.StatusCode
        };

        context.ExceptionHandled = true;
    }
}
