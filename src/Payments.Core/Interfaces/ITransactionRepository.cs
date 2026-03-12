using Payments.Core.Entities;

namespace Payments.Core.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<IReadOnlyList<Transaction>> GetAllAsync();
}
