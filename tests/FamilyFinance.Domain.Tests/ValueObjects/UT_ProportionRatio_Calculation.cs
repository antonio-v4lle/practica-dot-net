using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.ValueObjects;

/// <summary>UT_ProportionRatio_Calculation — validación de cálculo proporcional por ingreso.</summary>
public class UT_ProportionRatio_Calculation
{
    [Fact]
    public void Calculate_AntonioAndAdriana_Returns70And30Percent()
    {
        var antonio = Money.Of(42_000m);
        var adriana = Money.Of(17_600m);
        var total = antonio.Add(adriana); // 59 600

        var antonioProp = ProportionRatio.Calculate(antonio, total);
        var adrianaProp = ProportionRatio.Calculate(adriana, total);

        antonioProp.Percentage.Should().BeApproximately(70.47m, precision: 0.01m);
        adrianaProp.Percentage.Should().BeApproximately(29.53m, precision: 0.01m);
        (antonioProp.Value + adrianaProp.Value).Should().BeApproximately(1m, precision: 0.0001m);
    }

    [Fact]
    public void From_ValueBetweenZeroAndOne_CreatesRatio()
    {
        var ratio = ProportionRatio.From(0.7m);

        ratio.Value.Should().Be(0.7m);
        ratio.Percentage.Should().Be(70m);
    }

    [Fact]
    public void From_ValueGreaterThanOne_ThrowsArgumentOutOfRangeException()
    {
        var act = () => ProportionRatio.From(1.1m);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void From_NegativeValue_ThrowsArgumentOutOfRangeException()
    {
        var act = () => ProportionRatio.From(-0.1m);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Calculate_WithZeroTotalIncome_ThrowsInvalidOperationException()
    {
        var act = () => ProportionRatio.Calculate(Money.Of(1000m), Money.Zero);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*zero*");
    }

    [Fact]
    public void Calculate_SingleMember_ReturnsFull100Percent()
    {
        var income = Money.Of(42_000m);
        var ratio = ProportionRatio.Calculate(income, income);

        ratio.Value.Should().Be(1m);
        ratio.Percentage.Should().Be(100m);
    }
}
