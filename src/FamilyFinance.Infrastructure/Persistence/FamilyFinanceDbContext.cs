using FamilyFinance.Domain.Aggregates.Credits;
using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Domain.Aggregates.FamilyBudget;
using Microsoft.EntityFrameworkCore;

namespace FamilyFinance.Infrastructure.Persistence;

public class FamilyFinanceDbContext(DbContextOptions<FamilyFinanceDbContext> options) : DbContext(options)
{
    public DbSet<FamilyGroup> FamilyGroups => Set<FamilyGroup>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<SharedExpense> SharedExpenses => Set<SharedExpense>();
    public DbSet<CreditLine> CreditLines => Set<CreditLine>();
    public DbSet<CreditPayment> CreditPayments => Set<CreditPayment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FamilyFinanceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
