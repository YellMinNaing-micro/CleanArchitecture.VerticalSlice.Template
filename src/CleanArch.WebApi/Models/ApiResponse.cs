using System.Text.Json.Serialization;

namespace CleanArch.WebApi.Models;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T Data);

public sealed record ApiResponse(
    bool Success,
    string Message,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyDictionary<string, string[]>? Errors = null);
