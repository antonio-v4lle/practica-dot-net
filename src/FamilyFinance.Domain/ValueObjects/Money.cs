namespace FamilyFinance.Domain.ValueObjects;

public sealed record Money(decimal Amount, string Currency = "MXN")
{
    public static Money Zero => new(0);
    public static Money Of(decimal amount) => new(amount);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor) => new(Math.Round(Amount * factor, 2), Currency);

    public Money Divide(decimal divisor)
    {
        if (divisor == 0) throw new DivideByZeroException("Cannot divide money by zero.");
        return new(Math.Round(Amount / divisor, 2), Currency);
    }

    public bool IsPositive => Amount > 0;
    public bool IsNegative => Amount < 0;
    public bool IsZero => Amount == 0;

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot operate on different currencies: {Currency} and {other.Currency}");
    }

    public override string ToString() => $"${Amount:N2} {Currency}";
}
