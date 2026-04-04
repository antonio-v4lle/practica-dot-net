using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.Events;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Aggregates.Credits;

public class CreditLine : AggregateRoot
{
    public Guid FamilyGroupId { get; private set; }
    public Guid MemberId { get; private set; }
    public string Name { get; private set; } = default!;
    public Money CreditLimit { get; private set; } = default!;
    public Money CurrentBalance { get; private set; } = default!;
    public InterestRate AnnualInterestRate { get; private set; } = default!;
    public int PaymentDueDay { get; private set; }
    public Money MinimumPayment { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    private readonly List<CreditPayment> _payments = [];
    public IReadOnlyList<CreditPayment> Payments => _payments.AsReadOnly();

    // EF Core constructor
    private CreditLine() { }

    public static CreditLine Create(
        Guid familyGroupId,
        Guid memberId,
        string name,
        Money creditLimit,
        Money currentBalance,
        InterestRate annualInterestRate,
        int paymentDueDay,
        Money? minimumPayment = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Credit line name cannot be empty.", nameof(name));
        if (paymentDueDay < 1 || paymentDueDay > 31)
            throw new ArgumentOutOfRangeException(nameof(paymentDueDay), "Payment due day must be between 1 and 31.");

        var credit = new CreditLine
        {
            FamilyGroupId = familyGroupId,
            MemberId = memberId,
            Name = name.Trim(),
            CreditLimit = creditLimit,
            CurrentBalance = currentBalance,
            AnnualInterestRate = annualInterestRate,
            PaymentDueDay = paymentDueDay,
            MinimumPayment = minimumPayment ?? currentBalance.Multiply(0.05m) // Default 5% minimum
        };

        return credit;
    }

    /// <summary>Monthly interest charge based on current balance.</summary>
    public Money MonthlyInterestCharge =>
        CurrentBalance.Multiply(AnnualInterestRate.MonthlyRate);

    /// <summary>Records a payment and updates the balance.</summary>
    public CreditPayment MakePayment(Money amount, DateOnly paymentDate, string? notes = null)
    {
        if (!amount.IsPositive)
            throw new ArgumentException("Payment amount must be positive.", nameof(amount));

        var interestPortion = MonthlyInterestCharge;
        var principalPortion = amount.Subtract(interestPortion);
        if (principalPortion.IsNegative)
            principalPortion = Money.Zero;

        var newBalance = CurrentBalance.Subtract(principalPortion);
        if (newBalance.IsNegative) newBalance = Money.Zero;
        CurrentBalance = newBalance;

        var payment = CreditPayment.Create(Id, amount, interestPortion, principalPortion, paymentDate, notes);
        _payments.Add(payment);
        AddDomainEvent(new CreditPaymentMadeEvent(Id, FamilyGroupId, MemberId, amount, newBalance));
        return payment;
    }

    public void UpdateBalance(Money balance) => CurrentBalance = balance;
    public void UpdateInterestRate(InterestRate rate) => AnnualInterestRate = rate;

    /// <summary>
    /// Estimates months to pay off with a given monthly payment using standard amortization.
    /// Returns null if payment doesn't cover interest.
    /// </summary>
    public int? EstimateMonthsToPayOff(Money monthlyPayment)
    {
        if (CurrentBalance.Amount <= 0) return 0;
        var r = AnnualInterestRate.MonthlyRate;
        var p = CurrentBalance.Amount;
        var payment = monthlyPayment.Amount;

        var monthlyInterest = p * r;
        if (payment <= monthlyInterest) return null;

        if (r == 0)
            return (int)Math.Ceiling(p / payment);

        var months = Math.Log((double)(payment / (payment - r * p))) / Math.Log((double)(1 + r));
        return (int)Math.Ceiling(months);
    }

    /// <summary>Total interest paid if paying the given monthly amount until payoff.</summary>
    public Money EstimateTotalInterest(Money monthlyPayment)
    {
        var months = EstimateMonthsToPayOff(monthlyPayment);
        if (months == null) return Money.Of(decimal.MaxValue / 2);
        var total = monthlyPayment.Multiply(months.Value);
        return total.Subtract(CurrentBalance);
    }
}
