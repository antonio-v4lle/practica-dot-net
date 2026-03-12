namespace Payments.Core.DTOs;

public record BalanceResponse(
    decimal TotalCharges,
    decimal TotalPayments,
    decimal Balance);
