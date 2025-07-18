using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

internal sealed class GlobalExceptionHandler : IExceptionHandler {
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken) {

        _logger.LogError(exception,
            "Unhandled exception occurred. Path: {Path}, Method: {Method}, Message: {Message} stackTrace: {StackTrace} inner exception: {InnerException}",
            httpContext.Request.Path,
            httpContext.Request.Method,
            exception.Message,
            exception.StackTrace,
            exception.InnerException
            );

        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "Server Error";
        var detail = exception.Message;
        var innerException = exception.InnerException;
        if(exception is UnauthorizedAccessException) {
            statusCode = StatusCodes.Status401Unauthorized;
            title = "Unauthorized";
        }
        else if(exception is KeyNotFoundException) {
            statusCode = StatusCodes.Status404NotFound;
            title = "Resource not found";
        }
        else if(exception is ArgumentException) {
            statusCode = StatusCodes.Status400BadRequest;
            title = "Bad Request";
        }

        var errorDetails = new ErrorDetailsDto {
            StatusCode = statusCode,
            Title = title,
            Detail = detail,
            Path = httpContext.Request.Path,
            TraceId = httpContext.TraceIdentifier
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(errorDetails, cancellationToken);

        return true;
    }
}
