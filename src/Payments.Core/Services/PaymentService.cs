using Payments.Core.DTOs;
using Payments.Core.Entities;
using Payments.Core.Enums;
using Payments.Core.Interfaces;

namespace Payments.Core.Services;

public class PaymentService(ITransactionRepository repository) : IPaymentService
{
    public async Task<TransactionResponse> CreateChargeAsync(CreateTransactionRequest request)
    {
        ValidateRequest(request);

        var transaction = new Transaction
        {
            Type = TransactionType.Charge,
            Amount = request.Amount,
            Description = request.Description
        };

        await repository.AddAsync(transaction);
        return ToResponse(transaction);
    }

    public async Task<TransactionResponse> CreatePaymentAsync(CreateTransactionRequest request)
    {
        ValidateRequest(request);

        var transaction = new Transaction
        {
            Type = TransactionType.Payment,
            Amount = request.Amount,
            Description = request.Description
        };

        await repository.AddAsync(transaction);
        return ToResponse(transaction);
    }

    public async Task<BalanceResponse> GetBalanceAsync()
    {
        var transactions = await repository.GetAllAsync();

        var totalCharges = transactions
            .Where(t => t.Type == TransactionType.Charge)
            .Sum(t => t.Amount);

        var totalPayments = transactions
            .Where(t => t.Type == TransactionType.Payment)
            .Sum(t => t.Amount);

        return new BalanceResponse(totalCharges, totalPayments, totalCharges - totalPayments);
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetTransactionsAsync()
    {
        var transactions = await repository.GetAllAsync();
        return transactions.Select(ToResponse).ToList().AsReadOnly();
    }

    private static void ValidateRequest(CreateTransactionRequest request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description is required.");
    }

    private static TransactionResponse ToResponse(Transaction t) =>
        new(t.Id, t.Type.ToString(), t.Amount, t.Description, t.CreatedAt);
}
