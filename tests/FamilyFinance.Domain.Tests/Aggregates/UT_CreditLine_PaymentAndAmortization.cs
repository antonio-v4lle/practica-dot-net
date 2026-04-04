using FamilyFinance.Domain.Aggregates.Credits;
using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.Aggregates;

/// <summary>UT_CreditLine_PaymentAndAmortization — pagos, intereses y amortización de créditos.</summary>
public class UT_CreditLine_PaymentAndAmortization
{
    private static readonly Guid FamilyId = Guid.NewGuid();
    private static readonly Guid MemberId = Guid.NewGuid();

    private static CreditLine CreateBBVACard() => CreditLine.Create(
        FamilyId, MemberId, "TDC BBVA",
        Money.Of(137_400m),
        Money.Of(105_357.96m),
        InterestRate.From(36m),
        paymentDueDay: 9,
        Money.Of(9_000m));

    private static CreditLine CreateHipoteca() => CreditLine.Create(
        FamilyId, MemberId, "Hipoteca",
        Money.Of(716_000m),
        Money.Of(606_863.40m),
        InterestRate.From(10.32m),
        paymentDueDay: 19,
        Money.Of(7_100m));

    [Fact]
    public void MonthlyInterestCharge_BBVAAt36Percent_IsThreePercentOfBalance()
    {
        var credit = CreateBBVACard();

        var monthlyInterest = credit.MonthlyInterestCharge;

        // 105,357.96 × 0.03 ≈ 3,160.74
        monthlyInterest.Amount.Should().BeApproximately(3_160.74m, precision: 1m);
    }

    [Fact]
    public void MonthlyInterestCharge_HipotecaAt10_32Percent_IsCorrect()
    {
        var credit = CreateHipoteca();

        // 606,863.40 × (10.32% / 12) ≈ 5,219.07
        credit.MonthlyInterestCharge.Amount.Should().BeApproximately(5_219m, precision: 10m);
    }

    [Fact]
    public void MakePayment_ReducesBalance_AndSeparatesInterestFromPrincipal()
    {
        var credit = CreateBBVACard();
        var balanceBefore = credit.CurrentBalance.Amount;
        var payment = Money.Of(9_000m);

        var result = credit.MakePayment(payment, DateOnly.FromDateTime(DateTime.Today));

        result.TotalAmount.Amount.Should().Be(9_000m);
        result.InterestPortion.Amount.Should().BeApproximately(3_160.74m, precision: 1m);
        result.PrincipalPortion.Amount.Should().BeApproximately(9_000m - 3_160.74m, precision: 1m);
        credit.CurrentBalance.Amount.Should().BeLessThan(balanceBefore);
        credit.Payments.Should().HaveCount(1);
    }

    [Fact]
    public void MakePayment_AmountLessThanInterest_PrincipalIsZeroAndBalanceUnchanged()
    {
        var credit = CreateHipoteca();
        var balanceBefore = credit.CurrentBalance.Amount;

        // Payment less than interest (~5,219) → no principal reduction
        credit.MakePayment(Money.Of(1_000m), DateOnly.FromDateTime(DateTime.Today));

        credit.CurrentBalance.Amount.Should().Be(balanceBefore);
        credit.Payments.First().PrincipalPortion.IsZero.Should().BeTrue();
    }

    [Fact]
    public void MakePayment_WithNegativeAmount_ThrowsArgumentException()
    {
        var credit = CreateBBVACard();

        var act = () => credit.MakePayment(Money.Of(-100m), DateOnly.FromDateTime(DateTime.Today));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EstimateMonthsToPayOff_BBVA9000Monthly_ReturnsReasonableMonths()
    {
        var credit = CreateBBVACard();

        var months = credit.EstimateMonthsToPayOff(Money.Of(9_000m));

        months.Should().NotBeNull();
        months.Should().BeGreaterThan(12);   // More than 1 year
        months.Should().BeLessThan(120);     // Less than 10 years
    }

    [Fact]
    public void EstimateMonthsToPayOff_PaymentBelowInterest_ReturnsNull()
    {
        var credit = CreateBBVACard();

        // Monthly interest ≈ 3,160 → paying 1,000 doesn't cover interest
        var months = credit.EstimateMonthsToPayOff(Money.Of(1_000m));

        months.Should().BeNull();
    }

    [Fact]
    public void EstimateMonthsToPayOff_ZeroInterestRate_DividesBalanceByPayment()
    {
        var credit = CreditLine.Create(FamilyId, MemberId, "Sin Interés",
            Money.Of(10_000m), Money.Of(10_000m), InterestRate.Zero, 1, Money.Of(500m));

        var months = credit.EstimateMonthsToPayOff(Money.Of(1_000m));

        months.Should().Be(10);
    }

    [Fact]
    public void Create_WithInvalidPaymentDay_ThrowsArgumentOutOfRangeException()
    {
        var act = () => CreditLine.Create(FamilyId, MemberId, "Test",
            Money.Of(10_000m), Money.Of(5_000m), InterestRate.From(36m),
            paymentDueDay: 32);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
