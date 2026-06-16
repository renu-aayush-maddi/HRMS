using System.Net;
using System.Text.Json;
using HRMS.API.Exceptions;
using HRMS.API.Models.Common;

namespace HRMS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionMiddleware> logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            switch (exception)
            {
                case BusinessException:
                case ValidationException:
                case NotFoundException:
                    logger.LogWarning(
                        "{ExceptionType}: {Message}. TraceId: {TraceId}",
                        exception.GetType().Name,
                        exception.Message,
                        context.TraceIdentifier);
                    break;

                default:
                    logger.LogError(
                        exception,
                        "Unhandled exception occurred. TraceId: {TraceId}",
                        context.TraceIdentifier);
                    break;
            }

            await HandleExceptionAsync(
                context,
                exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,

            BusinessException => HttpStatusCode.BadRequest,

            ValidationException => HttpStatusCode.BadRequest,

            ForbiddenException => HttpStatusCode.Forbidden,

            UnauthorizedAccessException => HttpStatusCode.Unauthorized,

            _ => HttpStatusCode.InternalServerError
        };

        var response = new ErrorResponse
        {
            Message = exception.Message,
            StatusCode = (int)statusCode,
            TraceId = context.TraceIdentifier,
            Path = context.Request.Path,
            Timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}


// using System.Net;
// using System.Text.Json;
// using HRMS.API.Exceptions;
// using HRMS.API.Models.Common;

// namespace HRMS.API.Middleware;

// public class ExceptionMiddleware
// {
//     private readonly RequestDelegate next;

//     public ExceptionMiddleware(RequestDelegate next)
//     {
//         this.next = next;
//     }

//     public async Task InvokeAsync(HttpContext context)
//     {
//         try
//         {
//             await next(context);
//         }
//         catch (Exception exception)
//         {
//             await HandleExceptionAsync(context,exception);
//         }
//     }

//     private static async Task HandleExceptionAsync(HttpContext context,Exception exception)
//     {
//         HttpStatusCode statusCode;

//         switch (exception)
//         {
//             case NotFoundException:
//                 statusCode = HttpStatusCode.NotFound;
//                 break;

//             case BusinessException:
//                 statusCode = HttpStatusCode.BadRequest;
//                 break;

//             case ValidationException:
//                 statusCode = HttpStatusCode.BadRequest;
//                 break;

//             default:
//                 statusCode = HttpStatusCode.InternalServerError;
//                 break;
//         }

//         var response = new ErrorResponse
//             {
//                 Message = exception.Message
//             };

//         context.Response.ContentType = "application/json";

//         context.Response.StatusCode = (int)statusCode;

//         await context.Response.WriteAsync(JsonSerializer.Serialize(response));
//     }
// }


