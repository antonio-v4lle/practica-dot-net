using Payments.Core.DTOs;

namespace Payments.Core.Interfaces;

public interface IPaymentService
{
    Task<TransactionResponse> CreateChargeAsync(CreateTransactionRequest request);
    Task<TransactionResponse> CreatePaymentAsync(CreateTransactionRequest request);
    Task<BalanceResponse> GetBalanceAsync();
    Task<IReadOnlyList<TransactionResponse>> GetTransactionsAsync();
}
