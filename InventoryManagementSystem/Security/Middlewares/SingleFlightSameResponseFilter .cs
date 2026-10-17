namespace SecurityApplicationPresentation.Middlewares
{

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using System.Collections.Concurrent;

    public record StoredResponse(int StatusCode, string ContentType, byte[] Body);

    public static class SingleFlightStore
    {
        // key -> in-flight response task
        public static ConcurrentDictionary<string, Lazy<Task<StoredResponse>>> InFlight = new();
    }

    public class SingleFlightSameResponseFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
        {
            if (ctx.HttpContext.Request.Method != HttpMethods.Post)
            {
                return await next(ctx);
            }

            var http = ctx.HttpContext;

            if (!http.Request.Headers.TryGetValue("Idempotency-Key", out var v) || string.IsNullOrWhiteSpace(v))
                return Results.BadRequest("Missing Idempotency-Key.");

            var key = v.ToString().Trim();

            // Create one shared task for this key (only one request wins)
            var lazy = SingleFlightStore.InFlight.GetOrAdd(
                key,
                _ => new Lazy<Task<StoredResponse>>(() => ExecuteAndCaptureAsync(ctx, next))
            );

            StoredResponse stored;
            try
            {
                stored = await lazy.Value; // everyone awaits the same task
            }
            finally
            {
                // Remove immediately after completion so later requests can run again
                if (lazy.IsValueCreated && lazy.Value.IsCompleted)
                    SingleFlightStore.InFlight.TryRemove(key, out _);
            }

            http.Response.StatusCode = stored.StatusCode;
            http.Response.ContentType = stored.ContentType;
            await http.Response.Body.WriteAsync(stored.Body, http.RequestAborted);

            // We already wrote the response
            return Results.Empty;
        }

        private static async Task<StoredResponse> ExecuteAndCaptureAsync(
            EndpointFilterInvocationContext ctx,
            EndpointFilterDelegate next)
        {
            // Run the real endpoint once
            var resultObj = await next(ctx);

            // Minimal APIs usually return IResult. If not, you can adapt this.
            if (resultObj is not IResult result)
                throw new InvalidOperationException("Endpoint must return IResult to be captured/replayed.");

            // Execute result into a temporary HttpContext to capture bytes
            var temp = new DefaultHttpContext
            {
                RequestServices = ctx.HttpContext.RequestServices
            };

            await using var ms = new MemoryStream();
            temp.Response.Body = ms;

            await result.ExecuteAsync(temp);

            var status = temp.Response.StatusCode;
            var contentType = temp.Response.ContentType ?? "application/json";
            var body = ms.ToArray();

            return new StoredResponse(status, contentType, body);
        }
    }
}
