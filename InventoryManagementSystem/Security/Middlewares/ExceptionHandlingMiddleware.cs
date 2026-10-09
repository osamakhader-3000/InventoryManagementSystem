namespace SecurityApplicationPresentation.Middlewares
{
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Text.Json;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Components.Web;
    using SecurityApplicationPresentation.DataTransferObjects;
    using SecurityApplicationPresentation.Exceptions;

    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(FailedLoginAttemptException fle)
            {
                await WriteProblemAsync(context, fle, (HttpStatusCode)401, "Failed Login Attempted");
            }
            catch (OperationCanceledException oce) when (context.RequestAborted.IsCancellationRequested)
            {
                // Client disconnected / request aborted
                await WriteProblemAsync(context, oce, (HttpStatusCode)499, "Request canceled");
            }
            catch (Exception ex)
            {
                await WriteProblemAsync(context, ex, MapStatusCode(ex), GetTitle(ex));
            }
        }

        private static HttpStatusCode MapStatusCode(Exception ex) =>
            ex switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized, // 401
                KeyNotFoundException => HttpStatusCode.NotFound,     // 404
                ArgumentException => HttpStatusCode.BadRequest,    // 400
                ValidationException => HttpStatusCode.BadRequest,    // 400
                NotSupportedException => HttpStatusCode.NotImplemented,// 501
                _ => HttpStatusCode.InternalServerError // 500
            };

        private static string GetTitle(Exception ex) =>
            ex switch
            {
                UnauthorizedAccessException => "Unauthorized",
                KeyNotFoundException => "Not Found",
                ArgumentException => "Bad Request",
                ValidationException => "Validation Failed",
                NotSupportedException => "Not Implemented",
                _ => "An unexpected error occurred"
            };

        private async Task WriteProblemAsync(
            HttpContext context,
            Exception ex,
            HttpStatusCode statusCode,
            string title)
        {
            // Try to use existing correlation id or create one
            var correlationId = context.TraceIdentifier;
            if (!context.Request.Headers.TryGetValue("X-Correlation-Id", out var fromHeader) || string.IsNullOrWhiteSpace(fromHeader))
            {
                context.Response.Headers["X-Correlation-Id"] = correlationId;
            }
            else
            {
                correlationId = fromHeader!;
                context.Response.Headers["X-Correlation-Id"] = correlationId;
            }

            // Log with context
            _logger.LogError(ex, "Unhandled exception. Path: {Path}, Method: {Method}, CorrelationId: {CorrelationId}",
                context.Request.Path, context.Request.Method, correlationId);
                       

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            ExceptionTransfer exceptionTransfer = new ExceptionTransfer
            {
                Message = ex.Message,
                HelpLink = ex.HelpLink,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
            };

            
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Message));
        }
    }

}