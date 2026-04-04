using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.ValueObjects;

/// <summary>
/// UT = Unit Test | Sujeto: Money | Escenario: operación aritmética
/// Nomenclatura: UT_[Sujeto]_[Escenario].cs → métodos: [Método]_[Condición]_[Resultado]
/// </summary>
public class UT_Money_Arithmetic
{
    [Fact]
    public void Add_TwoPositiveAmounts_ReturnsSumWithSameCurrency()
    {
        var a = Money.Of(1000m);
        var b = Money.Of(500m);

        var result = a.Add(b);

        result.Amount.Should().Be(1500m);
        result.Currency.Should().Be("MXN");
    }

    [Fact]
    public void Subtract_LargerFromSmaller_ReturnsNegativeMoney()
    {
        var result = Money.Of(500m).Subtract(Money.Of(1000m));

        result.Amount.Should().Be(-500m);
        result.IsNegative.Should().BeTrue();
    }

    [Fact]
    public void Multiply_ByProportion_ReturnsRoundedAmount()
    {
        var monthly = Money.Of(59_600m);

        var antonioShare = monthly.Multiply(0.7m);
        var adrianaShare = monthly.Multiply(0.3m);

        antonioShare.Amount.Should().Be(41_720m);
        adrianaShare.Amount.Should().Be(17_880m);
    }

    [Fact]
    public void Divide_ByTwo_ReturnsHalfForBiweekly()
    {
        var monthly = Money.Of(7_100m);

        var biweekly = monthly.Divide(2);

        biweekly.Amount.Should().Be(3_550m);
    }

    [Fact]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        var act = () => Money.Of(100m).Divide(0);

        act.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void Add_DifferentCurrencies_ThrowsInvalidOperationException()
    {
        var mxn = new Money(100m, "MXN");
        var usd = new Money(100m, "USD");

        var act = () => mxn.Add(usd);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*MXN*USD*");
    }

    [Fact]
    public void Zero_IsZero_ReturnsTrue()
    {
        Money.Zero.IsZero.Should().BeTrue();
        Money.Zero.IsPositive.Should().BeFalse();
        Money.Zero.IsNegative.Should().BeFalse();
    }

    [Theory]
    [InlineData(1000, 500, "1500")]
    [InlineData(0, 0, "0")]
    public void Add_VariousAmounts_ProducesCorrectSum(decimal a, decimal b, string expectedStr)
    {
        var result = Money.Of(a).Add(Money.Of(b));

        result.Amount.Should().Be(decimal.Parse(expectedStr));
    }
}
