using FamilyFinance.Application.Expenses.Commands;
using FamilyFinance.Application.Expenses.Queries;
using FamilyFinance.Application.FamilyGroups.Commands;
using FamilyFinance.Application.FamilyGroups.Queries;
using FamilyFinance.Application.Tests.Infrastructure;
using FamilyFinance.Domain.Aggregates.Expenses;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyFinance.Application.Tests.Features;

/// <summary>
/// FT_Expenses_ManagementFlow — flujo de gestión de gastos: crear, actualizar, suspender.
/// </summary>
public class FT_Expenses_ManagementFlow : IClassFixture<InMemoryTestFixture>
{
    private readonly InMemoryTestFixture _fixture;
    public FT_Expenses_ManagementFlow(InMemoryTestFixture fixture) => _fixture = fixture;

    private async Task<Guid> CreateFamily(IMediator mediator)
    {
        var id = await mediator.Send(new CreateFamilyGroupCommand("Test"));
        await mediator.Send(new AddMemberCommand(id, "Antonio", 42_000m));
        return id;
    }

    [Fact]
    public async Task AddExpense_WithPaymentDay_PersistsWithCorrectSchedule()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await CreateFamily(mediator);

        var expenseId = await mediator.Send(new AddExpenseCommand(
            familyId, "Hipoteca", ExpenseCategory.Patrimonio, 7_100m, true, 19));

        var expenses = await mediator.Send(new GetExpensesQuery(familyId));
        var hipoteca = expenses.Single(e => e.Description == "Hipoteca");
        hipoteca.MonthlyAmount.Amount.Should().Be(7_100m);
        hipoteca.Schedule!.PaymentDay.Should().Be(19);
        hipoteca.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task AddMultipleExpenses_AllPersistedAndActiveByDefault()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await CreateFamily(mediator);

        await mediator.Send(new AddExpenseCommand(familyId, "Agua", ExpenseCategory.Servicios, 250m));
        await mediator.Send(new AddExpenseCommand(familyId, "Luz", ExpenseCategory.Servicios, 1_100m));
        await mediator.Send(new AddExpenseCommand(familyId, "Internet", ExpenseCategory.Servicios, 1_500m));

        var expenses = await mediator.Send(new GetExpensesQuery(familyId));
        expenses.Should().HaveCount(3);
        expenses.Should().AllSatisfy(e => e.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task UpdateExpense_ChangesAmount_PersistsCorrectly()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await CreateFamily(mediator);
        var expenseId = await mediator.Send(new AddExpenseCommand(
            familyId, "Internet", ExpenseCategory.Servicios, 1_500m));

        await mediator.Send(new UpdateExpenseCommand(expenseId, 1_800m, null));

        var expenses = await mediator.Send(new GetExpensesQuery(familyId));
        expenses.Single().MonthlyAmount.Amount.Should().Be(1_800m);
    }

    [Fact]
    public async Task SuspendExpense_MarksAsInactiveAndExcludesFromActiveQuery()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await CreateFamily(mediator);
        var expenseId = await mediator.Send(new AddExpenseCommand(
            familyId, "Leonel", ExpenseCategory.Educacion, 3_000m));

        await mediator.Send(new SuspendExpenseCommand(expenseId));

        var activeExpenses = await mediator.Send(new GetExpensesQuery(familyId, true));
        var allExpenses = await mediator.Send(new GetExpensesQuery(familyId, false));

        activeExpenses.Should().BeEmpty();
        allExpenses.Should().HaveCount(1);
        allExpenses.Single().IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task AddExpense_ForNonExistentFamily_ThrowsKeyNotFoundOnSummary()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Expense can be created (no FK check at app level), but summary will fail
        await mediator.Send(new AddExpenseCommand(Guid.NewGuid(),
            "Test", ExpenseCategory.Servicios, 100m));

        var act = async () => await mediator.Send(new GetBudgetSummaryQuery(Guid.NewGuid()));
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
