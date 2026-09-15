using System.Text.Json;
using CleanArch.WebApi.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace CleanArch.UnitTests.WebApi.Middleware;

public class JsonContentTypeMiddlewareTests
{
    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json; charset=utf-8")]
    public async Task InvokeAsync_WithJsonContentType_CallsNext(string contentType)
    {
        var nextCalled = false;
        var middleware = new JsonContentTypeMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var context = CreateContext(contentType);

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    [Theory]
    [InlineData("text/plain")]
    [InlineData("text/json")]
    [InlineData(null)]
    public async Task InvokeAsync_WithNonJsonContentType_Returns415(string? contentType)
    {
        var nextCalled = false;
        var middleware = new JsonContentTypeMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var context = CreateContext(contentType);

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeFalse();
        context.Response.StatusCode.Should().Be(StatusCodes.Status415UnsupportedMediaType);
        context.Response.ContentType.Should().StartWith("application/json");

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        document.RootElement.GetProperty("status").GetInt32().Should().Be(415);
    }

    [Fact]
    public async Task InvokeAsync_WithoutRequestBody_DoesNotRequireContentType()
    {
        var nextCalled = false;
        var middleware = new JsonContentTypeMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    private static DefaultHttpContext CreateContext(string? contentType)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream("{}"u8.ToArray());
        context.Request.ContentLength = context.Request.Body.Length;
        context.Request.ContentType = contentType;
        context.Response.Body = new MemoryStream();
        return context;
    }
}
