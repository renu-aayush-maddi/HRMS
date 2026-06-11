using System.Net;
using System.Text.Json;
using HRMS.API.Exceptions;
using HRMS.API.Models.Common;

namespace HRMS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context,exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context,Exception exception)
    {
        HttpStatusCode statusCode;

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;

            case BusinessException:
                statusCode = HttpStatusCode.BadRequest;
                break;

            case ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                break;
        }

        var response = new ErrorResponse
            {
                Message = exception.Message
            };

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}