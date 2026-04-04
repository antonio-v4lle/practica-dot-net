namespace FamilyFinance.Domain.ValueObjects;

/// <summary>Represents a member's proportional share (0.0 – 1.0).</summary>
public sealed record ProportionRatio
{
    public decimal Value { get; }

    private ProportionRatio(decimal value) => Value = value;

    public static ProportionRatio From(decimal value)
    {
        if (value < 0 || value > 1)
            throw new ArgumentOutOfRangeException(nameof(value), "Proportion must be between 0 and 1.");
        return new ProportionRatio(Math.Round(value, 4));
    }

    public static ProportionRatio Calculate(Money individualIncome, Money totalIncome)
    {
        if (totalIncome.Amount == 0)
            throw new InvalidOperationException("Total income cannot be zero when calculating proportion.");
        return From(Math.Round(individualIncome.Amount / totalIncome.Amount, 4));
    }

    public decimal Percentage => Value * 100;

    public override string ToString() => $"{Percentage:N2}%";
}
