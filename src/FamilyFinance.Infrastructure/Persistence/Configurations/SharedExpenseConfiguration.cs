using FamilyFinance.Domain.Aggregates.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyFinance.Infrastructure.Persistence.Configurations;

public class SharedExpenseConfiguration : IEntityTypeConfiguration<SharedExpense>
{
    public void Configure(EntityTypeBuilder<SharedExpense> builder)
    {
        builder.ToTable("SharedExpenses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.IsRecurring);
        builder.Property(x => x.IsActive);
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.OwnsOne(x => x.MonthlyAmount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("MonthlyAmount").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(x => x.Schedule, s =>
        {
            s.Property(p => p.PaymentDay).HasColumnName("PaymentDay");
        });

        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.FirstFortnightAmount);
        builder.Ignore(x => x.SecondFortnightAmount);
    }
}
