namespace UltraPlatform.Worker.Models;

public record DataRecord
{
    public string Id { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
}
