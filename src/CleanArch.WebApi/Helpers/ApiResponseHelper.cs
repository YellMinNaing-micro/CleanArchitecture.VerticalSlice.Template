using CleanArch.WebApi.Models;

namespace CleanArch.WebApi.Helpers;

public static class ApiResponseHelper
{
    public static ApiResponse<T> Success<T>(T data, string message = "Request completed successfully.") =>
        new(true, message, data);

    public static ApiResponse Success(string message = "Request completed successfully.") =>
        new(true, message);

    public static ApiResponse Failure(
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null) =>
        new(false, message, errors);
}
