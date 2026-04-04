using FamilyFinance.Application.Expenses.Commands;
using FamilyFinance.Application.Expenses.Queries;
using FamilyFinance.Domain.Aggregates.Expenses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FamilyFinance.Api.Controllers;

[ApiController]
[Route("api/family-groups/{familyGroupId:guid}/[controller]")]
public class ExpensesController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists all expenses for a family group.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid familyGroupId, [FromQuery] bool activeOnly = true, CancellationToken ct = default)
    {
        var expenses = await mediator.Send(new GetExpensesQuery(familyGroupId, activeOnly), ct);
        return Ok(expenses.Select(e => new
        {
            e.Id,
            e.Description,
            Category = e.Category.ToString(),
            MonthlyAmount = e.MonthlyAmount.Amount,
            FirstFortnightAmount = e.FirstFortnightAmount.Amount,
            SecondFortnightAmount = e.SecondFortnightAmount.Amount,
            PaymentDay = e.Schedule?.PaymentDay,
            e.IsRecurring,
            e.IsActive,
            e.Notes
        }));
    }

    /// <summary>Adds a new shared expense.</summary>
    [HttpPost]
    public async Task<IActionResult> Add(Guid familyGroupId, [FromBody] AddExpenseRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<ExpenseCategory>(request.Category, true, out var category))
            return BadRequest($"Invalid category. Valid values: {string.Join(", ", Enum.GetNames<ExpenseCategory>())}");

        var id = await mediator.Send(new AddExpenseCommand(
            familyGroupId, request.Description, category,
            request.MonthlyAmount, request.IsRecurring,
            request.PaymentDay, request.Currency ?? "MXN", request.Notes), ct);
        return CreatedAtAction(nameof(GetAll), new { familyGroupId }, new { id });
    }

    /// <summary>Updates an expense amount or description.</summary>
    [HttpPut("{expenseId:guid}")]
    public async Task<IActionResult> Update(Guid familyGroupId, Guid expenseId,
        [FromBody] UpdateExpenseRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateExpenseCommand(expenseId, request.MonthlyAmount, request.Description, request.Currency ?? "MXN"), ct);
        return NoContent();
    }

    /// <summary>Suspends an expense (e.g. Leonel, Boda).</summary>
    [HttpPost("{expenseId:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid familyGroupId, Guid expenseId, CancellationToken ct)
    {
        await mediator.Send(new SuspendExpenseCommand(expenseId), ct);
        return NoContent();
    }
}

public record AddExpenseRequest(
    string Description,
    string Category,
    decimal MonthlyAmount,
    bool IsRecurring = true,
    int? PaymentDay = null,
    string? Currency = null,
    string? Notes = null);

public record UpdateExpenseRequest(decimal? MonthlyAmount, string? Description, string? Currency);
