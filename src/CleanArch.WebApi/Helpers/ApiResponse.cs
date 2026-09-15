using CleanArch.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.WebApi.Helpers;

public static class ApiResponse
{
    public static OkObjectResult OkResult<T>(string? route, MessageResponse message, T data) =>
        new(new ApiResult<T>(true, route, message, data));

    public static OkObjectResult OkResult(string? route, MessageResponse message) =>
        new(new ApiResult(true, route, message));

    public static ObjectResult ErrorResult(
        int statusCode,
        MessageResponse message,
        IReadOnlyDictionary<string, string[]>? errors = null,
        string? route = null)
    {
        var response = new ApiResult(false, route, message, errors);

        return statusCode switch
        {
            StatusCodes.Status400BadRequest => new BadRequestObjectResult(response),
            StatusCodes.Status404NotFound => new NotFoundObjectResult(response),
            _ => new ObjectResult(response) { StatusCode = statusCode }
        };
    }

    public static ApiResult Error(
        MessageResponse message,
        IReadOnlyDictionary<string, string[]>? errors = null,
        string? route = null) =>
        new(false, route, message, errors);
}
