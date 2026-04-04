namespace FamilyFinance.Domain.ValueObjects;

/// <summary>Represents the day-of-month when a payment is due.</summary>
public sealed record BiweeklySchedule
{
    public int PaymentDay { get; }

    private BiweeklySchedule(int paymentDay) => PaymentDay = paymentDay;

    public static BiweeklySchedule OnDay(int day)
    {
        if (day < 1 || day > 31)
            throw new ArgumentOutOfRangeException(nameof(day), "Payment day must be between 1 and 31.");
        return new BiweeklySchedule(day);
    }

    /// <summary>Returns the first and second fortnight payment amounts.</summary>
    public (Money FirstFortnight, Money SecondFortnight) SplitMonthly(Money monthlyAmount)
    {
        var half = monthlyAmount.Divide(2);
        return (half, half);
    }

    public override string ToString() => $"Día {PaymentDay} de cada mes";
}
