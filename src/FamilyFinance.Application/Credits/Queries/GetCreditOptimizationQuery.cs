using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.Services;
using FamilyFinance.Domain.ValueObjects;
using MediatR;

namespace FamilyFinance.Application.Credits.Queries;

public record GetCreditOptimizationQuery(
    Guid FamilyGroupId,
    decimal MonthlyBudget,
    string Currency = "MXN") : IRequest<CreditOptimizationResult>;

public record CreditOptimizationResult(
    OptimizationPlan Avalanche,
    OptimizationPlan Snowball,
    string Recommendation);

public class GetCreditOptimizationHandler(
    ICreditLineRepository creditRepo,
    CreditOptimizationService optimizationService)
    : IRequestHandler<GetCreditOptimizationQuery, CreditOptimizationResult>
{
    public async Task<CreditOptimizationResult> Handle(GetCreditOptimizationQuery request, CancellationToken ct)
    {
        var credits = await creditRepo.GetByFamilyGroupAsync(request.FamilyGroupId, ct);
        var budget = new Money(request.MonthlyBudget, request.Currency);

        var avalanche = optimizationService.CalculateAvalanchePlan(credits, budget);
        var snowball = optimizationService.CalculateSnowballPlan(credits, budget);

        var recommendation = BuildRecommendation(avalanche, snowball);
        return new CreditOptimizationResult(avalanche, snowball, recommendation);
    }

    private static string BuildRecommendation(OptimizationPlan avalanche, OptimizationPlan snowball)
    {
        if (avalanche.IsInsufficientFunds)
            return "Presupuesto insuficiente para cubrir pagos mínimos. Considera renegociar algún crédito.";

        var savings = avalanche.InterestSavingsVsMinimum.Amount;
        var monthsDiff = snowball.MonthsToFullPayoff - avalanche.MonthsToFullPayoff;

        if (avalanche.TotalInterestCost.Amount <= snowball.TotalInterestCost.Amount)
            return $"Se recomienda el método Avalancha: ahorra ${avalanche.InterestSavingsVsMinimum.Amount:N2} MXN en intereses " +
                   $"y liquida {Math.Abs(monthsDiff)} mes(es) antes que Bola de Nieve. " +
                   "Ideal si puedes mantener disciplina con el crédito de mayor tasa.";

        return $"Ambos métodos son similares. Bola de Nieve ofrece mayor motivación al liquidar cuentas antes. " +
               $"Avalancha ahorra ${savings:N2} MXN en intereses totales.";
    }
}
