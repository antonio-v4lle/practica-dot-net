using FamilyFinance.Application.Credits.Commands;
using FamilyFinance.Application.Credits.Queries;
using FamilyFinance.Application.FamilyGroups.Commands;
using FamilyFinance.Application.Tests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyFinance.Application.Tests.Features;

/// <summary>
/// FT_Credits_LineManagement — gestión de líneas de crédito: creación, actualización de saldo, pagos.
/// </summary>
public class FT_Credits_LineManagement : IClassFixture<InMemoryTestFixture>
{
    private readonly InMemoryTestFixture _fixture;
    public FT_Credits_LineManagement(InMemoryTestFixture fixture) => _fixture = fixture;

    private async Task<(Guid familyId, Guid memberId)> CreateFamilyWithMember(IMediator mediator)
    {
        var familyId = await mediator.Send(new CreateFamilyGroupCommand("Test"));
        var memberId = await mediator.Send(new AddMemberCommand(familyId, "Antonio", 42_000m));
        return (familyId, memberId);
    }

    [Fact]
    public async Task AddCreditLine_WithValidData_PersistsWithCorrectFields()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var (familyId, memberId) = await CreateFamilyWithMember(mediator);

        await mediator.Send(new AddCreditLineCommand(
            familyId, memberId, "TDC BBVA",
            137_400m, 105_357.96m, 36m, 9, 9_000m));

        var credits = await mediator.Send(new GetCreditLinesQuery(familyId));
        credits.Should().HaveCount(1);
        var credit = credits.Single();
        credit.Name.Should().Be("TDC BBVA");
        credit.CurrentBalance.Amount.Should().Be(105_357.96m);
        credit.AnnualInterestRate.AnnualPercentage.Should().Be(36m);
        credit.PaymentDueDay.Should().Be(9);
        credit.MinimumPayment.Amount.Should().Be(9_000m);
    }

    [Fact]
    public async Task UpdateCreditBalance_ChangesCurrentBalance()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var (familyId, memberId) = await CreateFamilyWithMember(mediator);
        var creditId = await mediator.Send(new AddCreditLineCommand(
            familyId, memberId, "TDC", 50_000m, 30_000m, 36m, 10, 1_500m));

        await mediator.Send(new UpdateCreditBalanceCommand(creditId, 28_000m));

        var credits = await mediator.Send(new GetCreditLinesQuery(familyId));
        credits.Single().CurrentBalance.Amount.Should().Be(28_000m);
    }

    [Fact]
    public async Task MakeCreditPayment_ReducesBalance_AndCreatesPaymentRecord()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var (familyId, memberId) = await CreateFamilyWithMember(mediator);
        var creditId = await mediator.Send(new AddCreditLineCommand(
            familyId, memberId, "TDC BBVA",
            137_400m, 105_357.96m, 36m, 9, 9_000m));

        var paymentId = await mediator.Send(new MakeCreditPaymentCommand(
            creditId, 9_000m, DateOnly.FromDateTime(DateTime.Today)));

        paymentId.Should().NotBeEmpty();
        var credits = await mediator.Send(new GetCreditLinesQuery(familyId));
        credits.Single().CurrentBalance.Amount.Should().BeLessThan(105_357.96m);
    }

    [Fact]
    public async Task AddMultipleCreditLines_ForSameMember_AllPersisted()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var (familyId, memberId) = await CreateFamilyWithMember(mediator);

        await mediator.Send(new AddCreditLineCommand(familyId, memberId, "TDC BBVA",
            137_400m, 105_357.96m, 36m, 9, 9_000m));
        await mediator.Send(new AddCreditLineCommand(familyId, memberId, "TDC UN",
            5_000m, 3_881.31m, 42m, 20, 4_500m));
        await mediator.Send(new AddCreditLineCommand(familyId, memberId, "Hipoteca",
            716_000m, 606_863.40m, 10.32m, 19, 7_100m));

        var credits = await mediator.Send(new GetCreditLinesQuery(familyId));
        credits.Should().HaveCount(3);
    }
}
