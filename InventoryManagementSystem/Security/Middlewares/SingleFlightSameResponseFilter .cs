namespace SecurityApplicationPresentation.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Caching.Distributed;
    using System.Text.Json;

    public record StoredResponse(int StatusCode, string ContentType, byte[] Body);

    public class SingleFlightSameResponseFilter : IEndpointFilter
    {
        private readonly IDistributedCache _cache;

        public SingleFlightSameResponseFilter(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
        {
            if (ctx.HttpContext.Request.Method != HttpMethods.Post)
            {
                return await next(ctx);
            }

            var http = ctx.HttpContext;

            if (!http.Request.Headers.TryGetValue("Idempotency-Key", out var v) || string.IsNullOrWhiteSpace(v))
                return Results.BadRequest("Missing Idempotency-Key.");

            var idempotencyKey = v.ToString().Trim();
            var cacheKey = $"idempotency:{idempotencyKey}";
            var lockKey = $"idempotency_lock:{idempotencyKey}";

            // Check if a response is already cached
            var cachedBytes = await _cache.GetAsync(cacheKey, http.RequestAborted);
            if (cachedBytes is not null)
            {
                var cached = JsonSerializer.Deserialize<StoredResponseDto>(cachedBytes)!;
                http.Response.StatusCode = cached.StatusCode;
                http.Response.ContentType = cached.ContentType;
                await http.Response.Body.WriteAsync(cached.Body, http.RequestAborted);
                return Results.Empty;
            }

            // Try to acquire a distributed lock so only one instance executes
            var lockAcquired = await TryAcquireLockAsync(lockKey, TimeSpan.FromSeconds(30));

            if (!lockAcquired)
            {
                // Another instance is processing this key — poll until the result appears
                var result = await WaitForCachedResponseAsync(cacheKey, timeout: TimeSpan.FromSeconds(30), http.RequestAborted);

                if (result is null)
                    return Results.StatusCode(StatusCodes.Status409Conflict);

                http.Response.StatusCode = result.StatusCode;
                http.Response.ContentType = result.ContentType;
                await http.Response.Body.WriteAsync(result.Body, http.RequestAborted);
                return Results.Empty;
            }

            try
            {
                // Execute the real endpoint
                var stored = await ExecuteAndCaptureAsync(ctx, next);

                // Cache the response
                var dto = new StoredResponseDto(stored.StatusCode, stored.ContentType, stored.Body);
                var serialized = JsonSerializer.SerializeToUtf8Bytes(dto);

                await _cache.SetAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, http.RequestAborted);

                http.Response.StatusCode = stored.StatusCode;
                http.Response.ContentType = stored.ContentType;
                await http.Response.Body.WriteAsync(stored.Body, http.RequestAborted);
                return Results.Empty;
            }
            finally
            {
                await ReleaseLockAsync(lockKey);
            }
        }

        private async Task<bool> TryAcquireLockAsync(string lockKey, TimeSpan expiry)
        {
            // Uses SET NX (set if not exists) via the cache
            // Since IDistributedCache doesn't support this natively,
            // we check-then-set. For true atomicity, see the note below.
            var existing = await _cache.GetAsync(lockKey);
            if (existing is not null)
                return false;

            await _cache.SetAsync(lockKey, new byte[] { 1 }, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            });

            return true;
        }

        private async Task ReleaseLockAsync(string lockKey)
        {
            await _cache.RemoveAsync(lockKey);
        }

        private async Task<StoredResponseDto?> WaitForCachedResponseAsync(
            string cacheKey, TimeSpan timeout, CancellationToken ct)
        {
            var deadline = DateTime.UtcNow + timeout;

            while (DateTime.UtcNow < deadline)
            {
                ct.ThrowIfCancellationRequested();

                var bytes = await _cache.GetAsync(cacheKey, ct);
                if (bytes is not null)
                    return JsonSerializer.Deserialize<StoredResponseDto>(bytes);

                await Task.Delay(100, ct);
            }

            return null;
        }

        private static async Task<StoredResponse> ExecuteAndCaptureAsync(
            EndpointFilterInvocationContext ctx,
            EndpointFilterDelegate next)
        {
            var resultObj = await next(ctx);

            if (resultObj is not IResult result)
                throw new InvalidOperationException("Endpoint must return IResult to be captured/replayed.");

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

    // Separate DTO for serialization (the record with byte[] serializes fine with System.Text.Json)
    public record StoredResponseDto(int StatusCode, string ContentType, byte[] Body);
}