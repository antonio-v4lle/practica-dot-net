using FamilyFinance.Application.Expenses.Commands;
using FamilyFinance.Application.FamilyGroups.Commands;
using FamilyFinance.Application.FamilyGroups.Queries;
using FamilyFinance.Application.Tests.Infrastructure;
using FamilyFinance.Domain.Aggregates.Expenses;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyFinance.Application.Tests.Features;

/// <summary>
/// FT_Budget_SummaryCalculation — resumen presupuestal quincenal con distribución proporcional.
/// </summary>
public class FT_Budget_SummaryCalculation : IClassFixture<InMemoryTestFixture>
{
    private readonly InMemoryTestFixture _fixture;
    public FT_Budget_SummaryCalculation(InMemoryTestFixture fixture) => _fixture = fixture;

    private async Task<Guid> SetupFamilyWithExpenses(IMediator mediator)
    {
        var familyId = await mediator.Send(new CreateFamilyGroupCommand("Familia Principal"));
        await mediator.Send(new AddMemberCommand(familyId, "Antonio", 42_000m));
        await mediator.Send(new AddMemberCommand(familyId, "Adriana", 17_600m));
        await mediator.Send(new AddExpenseCommand(familyId, "Hipoteca",
            ExpenseCategory.Patrimonio, 7_100m, true, 19));
        await mediator.Send(new AddExpenseCommand(familyId, "Automóvil",
            ExpenseCategory.Patrimonio, 11_800m, true, 4));
        await mediator.Send(new AddExpenseCommand(familyId, "Piso",
            ExpenseCategory.RentasRelocaciones, 3_800m, true, 15));
        return familyId;
    }

    [Fact]
    public async Task GetBudgetSummary_WithTwoMembersAndExpenses_ReturnsCorrectTotals()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupFamilyWithExpenses(mediator);

        var summary = await mediator.Send(new GetBudgetSummaryQuery(familyId));

        summary.TotalMonthlyIncome.Amount.Should().Be(59_600m);
        summary.TotalMonthlyExpenses.Amount.Should().Be(22_700m); // 7100+11800+3800
        summary.Members.Should().HaveCount(2);
        summary.Expenses.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetBudgetSummary_MemberContributions_AreProportionalToIncome()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupFamilyWithExpenses(mediator);

        var summary = await mediator.Send(new GetBudgetSummaryQuery(familyId));

        var antonio = summary.Members.First(m => m.Name == "Antonio");
        var adriana = summary.Members.First(m => m.Name == "Adriana");

        antonio.MonthlyContribution.Amount.Should()
            .BeGreaterThan(adriana.MonthlyContribution.Amount);
        (antonio.MonthlyContribution.Amount + adriana.MonthlyContribution.Amount)
            .Should().BeApproximately(summary.TotalMonthlyExpenses.Amount, precision: 1m);
    }

    [Fact]
    public async Task GetBudgetSummary_BiweeklyAmounts_AreHalfOfMonthly()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupFamilyWithExpenses(mediator);

        var summary = await mediator.Send(new GetBudgetSummaryQuery(familyId));

        summary.TotalBiweeklyExpenses.Amount.Should()
            .BeApproximately(summary.TotalMonthlyExpenses.Amount / 2, precision: 1m);
        foreach (var member in summary.Members)
        {
            member.BiweeklyContribution.Amount.Should()
                .BeApproximately(member.MonthlyContribution.Amount / 2, precision: 1m);
        }
    }

    [Fact]
    public async Task GetBudgetSummary_AfterSuspendingExpense_ExcludesItFromTotal()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupFamilyWithExpenses(mediator);
        // Add and suspend a "Leonel" expense
        var leonelId = await mediator.Send(new AddExpenseCommand(familyId, "Leonel",
            ExpenseCategory.Educacion, 3_000m));
        await mediator.Send(new SuspendExpenseCommand(leonelId));

        var summary = await mediator.Send(new GetBudgetSummaryQuery(familyId));

        summary.Expenses.Should().NotContain(e => e.Description == "Leonel");
        summary.TotalMonthlyExpenses.Amount.Should().Be(22_700m); // unchanged
    }
}
