namespace Payments.Core.DTOs;

public record TransactionResponse(
    Guid Id,
    string Type,
    decimal Amount,
    string Description,
    DateTime CreatedAt);
