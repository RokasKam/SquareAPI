using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using SquareDomain.Exeptions;

namespace SquareAPI.ExceptionHandling;

public class ExceptionFilter: ExceptionFilterAttribute
{
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        await HandleExceptionAsync(context);
    }

    private Task HandleExceptionAsync(ExceptionContext context)
    {
        var exceptionResponse = HandleException(context.Exception);
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = exceptionResponse.StatusCode;
        context.ExceptionHandled = true;

        return context.HttpContext.Response.WriteAsync(exceptionResponse.ToString());
    }

    private static ExceptionResponse HandleException(Exception exception)
    {
        var httpStatusCode = HttpStatusCode.InternalServerError;
        if (exception.GetType() == typeof(NotFoundException))
        {
            httpStatusCode = HttpStatusCode.NotFound;
        }
        if (exception.GetType() == typeof(BadRequestException))
        {
            httpStatusCode = HttpStatusCode.BadRequest;
        }

        var exceptionResponse = new ExceptionResponse((int)httpStatusCode, exception.Message);
        return exceptionResponse;
    }
}