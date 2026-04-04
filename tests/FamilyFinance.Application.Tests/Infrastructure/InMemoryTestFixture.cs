using FamilyFinance.Application;
using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Repositories;
using FamilyFinance.Infrastructure.Persistence;
using FamilyFinance.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FamilyFinance.Domain.Services;

namespace FamilyFinance.Application.Tests.Infrastructure;

/// <summary>
/// Fixture compartida para feature tests.
/// Usa EF Core InMemory para aislamiento sin SQL Server.
/// Cada test recibe su propio ServiceScope para evitar colisiones.
/// </summary>
public class InMemoryTestFixture : IDisposable
{
    private readonly ServiceProvider _provider;

    public InMemoryTestFixture()
    {
        var services = new ServiceCollection();

        services.AddDbContext<FamilyFinanceDbContext>(opts =>
            opts.UseInMemoryDatabase(Guid.NewGuid().ToString())); // DB única por fixture

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFamilyGroupRepository, FamilyGroupRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<ICreditLineRepository, CreditLineRepository>();
        services.AddScoped<ProportionalContributionService>();
        services.AddScoped<CreditOptimizationService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(FamilyFinance.Application.DependencyInjection).Assembly));

        _provider = services.BuildServiceProvider();
    }

    /// <summary>Crea un scope independiente por test para evitar contaminación entre tests.</summary>
    public IServiceScope CreateScope() => _provider.CreateScope();

    public void Dispose() => _provider.Dispose();
}
