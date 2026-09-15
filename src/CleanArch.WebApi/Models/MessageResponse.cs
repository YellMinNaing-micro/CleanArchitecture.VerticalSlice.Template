namespace CleanArch.WebApi.Models;

public sealed record MessageResponse
{
    public required string EN { get; init; }
    public required string MM { get; init; }
}
