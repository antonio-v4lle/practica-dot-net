namespace Payments.Core.DTOs;

public record CreateTransactionRequest(decimal Amount, string Description);
