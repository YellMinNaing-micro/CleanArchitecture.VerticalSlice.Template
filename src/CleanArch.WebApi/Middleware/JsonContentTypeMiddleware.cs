using CleanArch.WebApi.Helpers;
using CleanArch.WebApi.Models;

namespace CleanArch.WebApi.Middleware;

public sealed class JsonContentTypeMiddleware
{
    private const string JsonContentType = "application/json";
    private readonly RequestDelegate _next;

    public JsonContentTypeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (HasRequestBody(context.Request) && !HasApplicationJsonContentType(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            context.Response.ContentType = JsonContentType;

            await context.Response.WriteAsJsonAsync(
                ApiResponse.Error(new MessageResponse
                {
                    EN = "Request Content-Type must be application/json.",
                    MM = "Request Content-Type သည် application/json ဖြစ်ရပါမည်။"
                }));
            return;
        }

        await _next(context);
    }

    private static bool HasRequestBody(HttpRequest request) =>
        request.ContentLength is > 0 || request.Headers.TransferEncoding.Count > 0;

    private static bool HasApplicationJsonContentType(HttpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ContentType))
        {
            return false;
        }

        var mediaType = request.ContentType.Split(';', 2)[0].Trim();
        return string.Equals(mediaType, JsonContentType, StringComparison.OrdinalIgnoreCase);
    }
}
