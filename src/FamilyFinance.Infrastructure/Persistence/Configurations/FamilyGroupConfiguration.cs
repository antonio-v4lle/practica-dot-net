using FamilyFinance.Domain.Aggregates.FamilyBudget;
using FamilyFinance.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyFinance.Infrastructure.Persistence.Configurations;

public class FamilyGroupConfiguration : IEntityTypeConfiguration<FamilyGroup>
{
    public void Configure(EntityTypeBuilder<FamilyGroup> builder)
    {
        builder.ToTable("FamilyGroups");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();

        builder.HasMany(x => x.Members)
            .WithOne()
            .HasForeignKey(m => m.FamilyGroupId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Members).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.TotalMonthlyIncome);
        builder.Ignore(x => x.TotalBiweeklyIncome);
    }
}

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.FamilyGroupId).IsRequired();

        builder.OwnsOne(x => x.MonthlyNetIncome, m =>
        {
            m.Property(p => p.Amount).HasColumnName("MonthlyIncomeAmount").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("MonthlyIncomeCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(x => x.Proportion, p =>
        {
            p.Property(v => v.Value).HasColumnName("ProportionValue").HasPrecision(8, 4);
        });
    }
}
