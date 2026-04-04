using FamilyFinance.Application.FamilyGroups.Commands;
using FamilyFinance.Application.FamilyGroups.Queries;
using FamilyFinance.Application.Tests.Infrastructure;
using FamilyFinance.Domain.Repositories;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyFinance.Application.Tests.Features;

/// <summary>
/// FT = Feature Test | FT_Budget_FamilyGroupSetup
/// Valida el flujo completo de creación de grupo familiar y gestión de miembros.
/// </summary>
public class FT_Budget_FamilyGroupSetup : IClassFixture<InMemoryTestFixture>
{
    private readonly InMemoryTestFixture _fixture;
    public FT_Budget_FamilyGroupSetup(InMemoryTestFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CreateFamilyGroup_WithValidName_PersistsAndReturnsId()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var id = await mediator.Send(new CreateFamilyGroupCommand("Familia García"));

        id.Should().NotBeEmpty();
        var group = await mediator.Send(new GetFamilyGroupQuery(id));
        group.Name.Should().Be("Familia García");
        group.Currency.Should().Be("MXN");
    }

    [Fact]
    public async Task AddMember_ToExistingGroup_PersistsMemberWithCorrectIncome()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await mediator.Send(new CreateFamilyGroupCommand("Test"));

        var memberId = await mediator.Send(new AddMemberCommand(familyId, "Antonio", 42_000m));

        memberId.Should().NotBeEmpty();
        var group = await mediator.Send(new GetFamilyGroupQuery(familyId));
        group.Members.Should().HaveCount(1);
        group.Members.First().Name.Should().Be("Antonio");
        group.Members.First().MonthlyNetIncome.Amount.Should().Be(42_000m);
    }

    [Fact]
    public async Task AddTwoMembers_ProportionsRecalculatedAndPersistedCorrectly()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await mediator.Send(new CreateFamilyGroupCommand("Test"));

        await mediator.Send(new AddMemberCommand(familyId, "Antonio", 42_000m));
        await mediator.Send(new AddMemberCommand(familyId, "Adriana", 17_600m));

        var group = await mediator.Send(new GetFamilyGroupQuery(familyId));
        var antonio = group.Members.First(m => m.Name == "Antonio");
        var adriana = group.Members.First(m => m.Name == "Adriana");

        antonio.Proportion.Percentage.Should().BeApproximately(70.47m, 0.01m);
        adriana.Proportion.Percentage.Should().BeApproximately(29.53m, 0.01m);
        group.TotalMonthlyIncome.Amount.Should().Be(59_600m);
    }

    [Fact]
    public async Task UpdateMemberIncome_ChangesIncomeAndRecalculatesProportions()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await mediator.Send(new CreateFamilyGroupCommand("Test"));
        await mediator.Send(new AddMemberCommand(familyId, "Antonio", 42_000m));
        await mediator.Send(new AddMemberCommand(familyId, "Adriana", 17_600m));
        var group = await mediator.Send(new GetFamilyGroupQuery(familyId));
        var antonioId = group.Members.First(m => m.Name == "Antonio").Id;

        await mediator.Send(new UpdateMemberIncomeCommand(familyId, antonioId, 50_000m));

        var updated = await mediator.Send(new GetFamilyGroupQuery(familyId));
        updated.Members.First(m => m.Name == "Antonio").MonthlyNetIncome.Amount
            .Should().Be(50_000m);
        updated.TotalMonthlyIncome.Amount.Should().Be(67_600m);
    }

    [Fact]
    public async Task AddMember_ToNonExistentGroup_ThrowsKeyNotFoundException()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Send(
            new AddMemberCommand(Guid.NewGuid(), "Test", 10_000m));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
