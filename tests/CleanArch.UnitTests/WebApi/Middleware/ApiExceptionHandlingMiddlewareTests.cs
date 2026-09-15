using CleanArch.WebApi.Middleware;
using CleanArch.WebApi.Models;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.Json;

namespace CleanArch.UnitTests.WebApi.Middleware;

public class ApiExceptionHandlingMiddlewareTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger = Substitute.For<ILogger<ApiExceptionHandlingMiddleware>>();

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldInvokeNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var wasNextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            wasNextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ApiExceptionHandlingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        wasNextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_ShouldReturn400WithValidationProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required."),
            new("Price", "Price must be greater than 0.")
        };
        RequestDelegate next = (ctx) => throw new ValidationException(failures);

        var middleware = new ApiExceptionHandlingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ApiResult>(responseBody, JsonOptions);

        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.EN.Should().Be("One or more validation errors occurred.");
        response.Errors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task InvokeAsync_WhenKeyNotFoundException_ShouldReturn404WithProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (ctx) => throw new KeyNotFoundException("Product with ID 42 was not found.");

        var middleware = new ApiExceptionHandlingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ApiResult>(responseBody, JsonOptions);

        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.EN.Should().Be("Product with ID 42 was not found.");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_ShouldReturn500WithProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (ctx) => throw new InvalidOperationException("Something unexpected happened.");

        var middleware = new ApiExceptionHandlingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ApiResult>(responseBody, JsonOptions);

        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.EN.Should().Be("An unexpected error occurred.");
    }
}
