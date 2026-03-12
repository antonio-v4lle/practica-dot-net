using Payments.Core.Enums;

namespace Payments.Core.Entities;

public record Transaction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required TransactionType Type { get; init; }
    public required decimal Amount { get; init; }
    public required string Description { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
