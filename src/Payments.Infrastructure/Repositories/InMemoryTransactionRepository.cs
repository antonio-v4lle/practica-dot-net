using System.Collections.Concurrent;
using Payments.Core.Entities;
using Payments.Core.Interfaces;

namespace Payments.Infrastructure.Repositories;

public class InMemoryTransactionRepository : ITransactionRepository
{
    private readonly ConcurrentBag<Transaction> _transactions = new();

    public Task AddAsync(Transaction transaction)
    {
        _transactions.Add(transaction);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Transaction>> GetAllAsync()
    {
        IReadOnlyList<Transaction> result = _transactions
            .OrderByDescending(t => t.CreatedAt)
            .ToList()
            .AsReadOnly();
        return Task.FromResult(result);
    }
}
