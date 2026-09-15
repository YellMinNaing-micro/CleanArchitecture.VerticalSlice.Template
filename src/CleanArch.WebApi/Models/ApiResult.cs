using System.Text.Json.Serialization;

namespace CleanArch.WebApi.Models;

public record ApiResult(
    [property: JsonPropertyOrder(0)]
    bool Success,
    [property: JsonPropertyOrder(1)]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Route,
    [property: JsonPropertyOrder(2)]
    MessageResponse Message,
    [property: JsonPropertyOrder(4)]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyDictionary<string, string[]>? Errors = null);

public sealed record ApiResult<T>(
    bool Success,
    string? Route,
    MessageResponse Message,
    [property: JsonPropertyOrder(3)]
    T Data) : ApiResult(Success, Route, Message);
