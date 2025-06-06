using System.Linq.Dynamic.Core.Exceptions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using CTCore.DynamicQuery.Common.Definations;
using CTCore.DynamicQuery.Core.Primitives;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZEN.Controller.Types;

namespace ZEN.Controller.Middlewares;

public class CustomExceptionHandler
(
    RequestDelegate next,
    ILogger<CustomExceptionHandler> logger
)
{
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var (httpStatusCode, resultCode, message, errors) =
            GetHttpStatusCodeAndErrors(exception);

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = (int)httpStatusCode;

        var serializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (errors.Count > 0)
        {
            var re = new ErrorResponse(message, 400)
            {
                Errors = errors
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(re, serializerOptions));
            return;
        }
        var response = JsonSerializer.Serialize(
                  new APIErrorResponse(message, resultCode),
                  serializerOptions);

        await httpContext.Response.WriteAsync(response);
    }

    private static (HttpStatusCode httpStatusCode, int resultCode, string messag, List<ErrorItem> errors)
        GetHttpStatusCodeAndErrors(Exception exception) =>
            exception switch
            {
                ValidationException validationException => (HttpStatusCode.BadRequest, 4000,
                    "One or more validation failures has occurred.",
                    ValidationHelper(validationException)),
                ParseException => (HttpStatusCode.BadRequest, 4000, exception.Message, []),
                BadHttpRequestException => (HttpStatusCode.BadRequest, 4220, exception.Message, []),
                // MySqlException => (HttpStatusCode.InternalServerError, 5000, exception.Message, []),
                // OperationCanceledException => (HttpStatusCode.InternalServerError, 5000, exception.Message, []),
                // DbUpdateException => (HttpStatusCode.InternalServerError, 5000, exception.Message, []),
                _ => (HttpStatusCode.InternalServerError, 5000, exception.GetType().Name + ": " + exception.Message, [])
            };

    private static List<ErrorItem> ValidationHelper(ValidationException validationException)
    {
        var response = new List<ErrorItem>();

        foreach (var er in validationException.Errors)
        {

            if (response.Any(e => e.Key.Equals(er.PropertyName)))
            {
                response.First(e => e.Key == er.PropertyName)
                    .Messages.Add(er.ErrorMessage);
            }
            else
            {
                response.Add(
                    new ErrorItem(er.PropertyName, [er.ErrorMessage]));
            }

        }
        return response;
    }
}