using FamilyFinance.Domain.Aggregates.Credits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyFinance.Infrastructure.Persistence.Configurations;

public class CreditLineConfiguration : IEntityTypeConfiguration<CreditLine>
{
    public void Configure(EntityTypeBuilder<CreditLine> builder)
    {
        builder.ToTable("CreditLines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PaymentDueDay);
        builder.Property(x => x.IsActive);

        builder.OwnsOne(x => x.CreditLimit, m =>
        {
            m.Property(p => p.Amount).HasColumnName("CreditLimitAmount").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(x => x.CurrentBalance, m =>
        {
            m.Property(p => p.Amount).HasColumnName("CurrentBalanceAmount").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("BalanceCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(x => x.AnnualInterestRate, r =>
        {
            r.Property(p => p.AnnualPercentage).HasColumnName("AnnualInterestRate").HasPrecision(8, 4);
        });

        builder.OwnsOne(x => x.MinimumPayment, m =>
        {
            m.Property(p => p.Amount).HasColumnName("MinimumPaymentAmount").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("MinimumPaymentCurrency").HasMaxLength(3);
        });

        builder.HasMany(x => x.Payments)
            .WithOne()
            .HasForeignKey(p => p.CreditLineId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Payments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.MonthlyInterestCharge);
    }
}

public class CreditPaymentConfiguration : IEntityTypeConfiguration<CreditPayment>
{
    public void Configure(EntityTypeBuilder<CreditPayment> builder)
    {
        builder.ToTable("CreditPayments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PaymentDate);
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.OwnsOne(x => x.TotalAmount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(x => x.InterestPortion, m =>
        {
            m.Property(p => p.Amount).HasColumnName("InterestAmount").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("InterestCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(x => x.PrincipalPortion, m =>
        {
            m.Property(p => p.Amount).HasColumnName("PrincipalAmount").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("PrincipalCurrency").HasMaxLength(3);
        });
    }
}
