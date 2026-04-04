using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.ValueObjects;

/// <summary>UT_InterestRate_Conversions — tasas de interés anual a mensual/diaria.</summary>
public class UT_InterestRate_Conversions
{
    [Fact]
    public void MonthlyRate_For10_32Percent_IsCorrect()
    {
        var rate = InterestRate.From(10.32m);

        rate.MonthlyRate.Should().BeApproximately(0.0086m, precision: 0.0001m);
    }

    [Fact]
    public void MonthlyRate_For36Percent_IsThreePercentMonthly()
    {
        var rate = InterestRate.From(36m);

        rate.MonthlyRate.Should().BeApproximately(0.03m, precision: 0.0001m);
    }

    [Fact]
    public void From_NegativeRate_ThrowsArgumentOutOfRangeException()
    {
        var act = () => InterestRate.From(-1m);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Zero_HasZeroMonthlyAndDailyRates()
    {
        InterestRate.Zero.MonthlyRate.Should().Be(0m);
        InterestRate.Zero.DailyRate.Should().Be(0m);
    }
}
