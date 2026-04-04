using FamilyFinance.Application.Credits.Commands;
using FamilyFinance.Application.Credits.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FamilyFinance.Api.Controllers;

[ApiController]
[Route("api/family-groups/{familyGroupId:guid}/[controller]")]
public class CreditsController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists all active credit lines for a family group.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid familyGroupId, CancellationToken ct)
    {
        var credits = await mediator.Send(new GetCreditLinesQuery(familyGroupId), ct);
        return Ok(credits.Select(c => new
        {
            c.Id,
            c.Name,
            CreditLimit = c.CreditLimit.Amount,
            CurrentBalance = c.CurrentBalance.Amount,
            AnnualInterestRate = c.AnnualInterestRate.AnnualPercentage,
            MonthlyInterestCharge = c.MonthlyInterestCharge.Amount,
            c.PaymentDueDay,
            MinimumPayment = c.MinimumPayment.Amount,
            c.MemberId
        }));
    }

    /// <summary>Adds a new credit line.</summary>
    [HttpPost]
    public async Task<IActionResult> Add(Guid familyGroupId, [FromBody] AddCreditLineRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new AddCreditLineCommand(
            familyGroupId, request.MemberId, request.Name,
            request.CreditLimit, request.CurrentBalance,
            request.AnnualInterestRate, request.PaymentDueDay,
            request.MinimumPayment, request.Currency ?? "MXN"), ct);
        return CreatedAtAction(nameof(GetAll), new { familyGroupId }, new { id });
    }

    /// <summary>Updates the current balance of a credit line.</summary>
    [HttpPatch("{creditId:guid}/balance")]
    public async Task<IActionResult> UpdateBalance(Guid familyGroupId, Guid creditId,
        [FromBody] UpdateBalanceRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateCreditBalanceCommand(creditId, request.NewBalance, request.Currency ?? "MXN"), ct);
        return NoContent();
    }

    /// <summary>Records a payment on a credit line.</summary>
    [HttpPost("{creditId:guid}/payments")]
    public async Task<IActionResult> MakePayment(Guid familyGroupId, Guid creditId,
        [FromBody] MakePaymentRequest request, CancellationToken ct)
    {
        var paymentDate = request.PaymentDate ?? DateOnly.FromDateTime(DateTime.Today);
        var paymentId = await mediator.Send(
            new MakeCreditPaymentCommand(creditId, request.Amount, paymentDate, request.Currency ?? "MXN", request.Notes), ct);
        return CreatedAtAction(nameof(GetAll), new { familyGroupId }, new { paymentId });
    }

    /// <summary>
    /// Gets the payment optimization plan (Avalanche vs Snowball) for the given monthly budget.
    /// </summary>
    [HttpGet("optimization")]
    public async Task<IActionResult> GetOptimization(
        Guid familyGroupId,
        [FromQuery] decimal monthlyBudget,
        [FromQuery] string currency = "MXN",
        CancellationToken ct = default)
    {
        if (monthlyBudget <= 0)
            return BadRequest("Monthly budget must be greater than zero.");

        var result = await mediator.Send(new GetCreditOptimizationQuery(familyGroupId, monthlyBudget, currency), ct);
        return Ok(new
        {
            result.Recommendation,
            Avalanche = FormatPlan(result.Avalanche),
            Snowball = FormatPlan(result.Snowball)
        });
    }

    private static object FormatPlan(Domain.Services.OptimizationPlan plan) => new
    {
        Strategy = plan.Strategy.ToString(),
        plan.IsInsufficientFunds,
        TotalInterestCost = plan.TotalInterestCost.Amount,
        InterestSavingsVsMinimum = plan.InterestSavingsVsMinimum.Amount,
        plan.MonthsToFullPayoff,
        Allocations = plan.Allocations.Select(a => new
        {
            a.CreditName,
            CurrentBalance = a.CurrentBalance.Amount,
            AnnualInterestRate = a.AnnualInterestRate.AnnualPercentage,
            MinimumPayment = a.MinimumPayment.Amount,
            ExtraPayment = a.ExtraPayment.Amount,
            TotalMonthlyPayment = a.TotalMonthlyPayment.Amount,
            a.EstimatedMonthsToPayoff,
            EstimatedTotalInterest = a.EstimatedTotalInterest.Amount < decimal.MaxValue / 2
                ? a.EstimatedTotalInterest.Amount : (decimal?)null
        })
    };
}

public record AddCreditLineRequest(
    Guid MemberId,
    string Name,
    decimal CreditLimit,
    decimal CurrentBalance,
    decimal AnnualInterestRate,
    int PaymentDueDay,
    decimal? MinimumPayment = null,
    string? Currency = null);

public record UpdateBalanceRequest(decimal NewBalance, string? Currency);
public record MakePaymentRequest(decimal Amount, DateOnly? PaymentDate, string? Notes, string? Currency);
