using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CRM.API.Common.ExceptionHandling
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            var (statusCode, title, actionMessage) = exception switch
            {
                BusinessException be => (be.StatusCode, "Business Error", be.ActionDescription),
                InvalidOperationException => (HttpStatusCode.Conflict, "Logic Error", $"executing {httpContext.Request.Method} {httpContext.Request.Path}"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", $"executing {httpContext.Request.Method} {httpContext.Request.Path}"),
                _ => (HttpStatusCode.InternalServerError, "Internal Server Error", $"executing {httpContext.Request.Method} {httpContext.Request.Path}")
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = $"The error '{exception.Message}' was caused while {actionMessage}",
                Type = exception.GetType().Name
            };

            httpContext.Response.StatusCode = (int)statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
