using FamilyFinance.Application.FamilyGroups.Commands;
using FamilyFinance.Application.FamilyGroups.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FamilyFinance.Api.Controllers;

[ApiController]
[Route("api/family-groups")]
public class FamilyGroupsController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a new family group.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFamilyGroupRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateFamilyGroupCommand(request.Name, request.Currency ?? "MXN"), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Gets a family group with its members.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var group = await mediator.Send(new GetFamilyGroupQuery(id), ct);
        return Ok(new
        {
            group.Id,
            group.Name,
            group.Currency,
            TotalMonthlyIncome = group.TotalMonthlyIncome.Amount,
            Members = group.Members.Select(m => new
            {
                m.Id,
                m.Name,
                MonthlyNetIncome = m.MonthlyNetIncome.Amount,
                ProportionPercentage = m.Proportion.Percentage
            })
        });
    }

    /// <summary>Gets the full biweekly budget summary with proportional contributions.</summary>
    [HttpGet("{id:guid}/budget-summary")]
    public async Task<IActionResult> GetBudgetSummary(Guid id, CancellationToken ct)
    {
        var summary = await mediator.Send(new GetBudgetSummaryQuery(id), ct);
        return Ok(summary);
    }

    /// <summary>Adds a member to the family group.</summary>
    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberRequest request, CancellationToken ct)
    {
        var memberId = await mediator.Send(
            new AddMemberCommand(id, request.Name, request.MonthlyNetIncome, request.Currency ?? "MXN"), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { memberId });
    }

    /// <summary>Updates a member's monthly income and recalculates proportions.</summary>
    [HttpPut("{id:guid}/members/{memberId:guid}/income")]
    public async Task<IActionResult> UpdateMemberIncome(
        Guid id, Guid memberId, [FromBody] UpdateIncomeRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateMemberIncomeCommand(id, memberId, request.MonthlyNetIncome, request.Currency ?? "MXN"), ct);
        return NoContent();
    }
}

public record CreateFamilyGroupRequest(string Name, string? Currency);
public record AddMemberRequest(string Name, decimal MonthlyNetIncome, string? Currency);
public record UpdateIncomeRequest(decimal MonthlyNetIncome, string? Currency);
