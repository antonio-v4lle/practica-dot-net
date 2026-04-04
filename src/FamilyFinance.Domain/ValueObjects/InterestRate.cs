namespace FamilyFinance.Domain.ValueObjects;

/// <summary>Annual interest rate expressed as a percentage (e.g. 10.32 for 10.32%).</summary>
public sealed record InterestRate
{
    public decimal AnnualPercentage { get; }
    public decimal MonthlyRate => AnnualPercentage / 100 / 12;
    public decimal DailyRate => AnnualPercentage / 100 / 365;

    private InterestRate(decimal annualPercentage) => AnnualPercentage = annualPercentage;

    public static InterestRate From(decimal annualPercentage)
    {
        if (annualPercentage < 0)
            throw new ArgumentOutOfRangeException(nameof(annualPercentage), "Interest rate cannot be negative.");
        return new InterestRate(annualPercentage);
    }

    public static InterestRate Zero => new(0);

    public override string ToString() => $"{AnnualPercentage:N2}% anual";
}
