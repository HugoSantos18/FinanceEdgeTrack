using FinanceEdgeTrack.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinanceEdgeTrack.Extensions
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var error = new ErrorDetails
            {
                MessageError = context.Exception.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Trace = context.Exception.StackTrace // talvez tirar por segurança (verificar)
            };

            context.Result = new ObjectResult(error)
            {
                StatusCode = error.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
