using FamilyFinance.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyFinance.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<ProportionalContributionService>();
        services.AddScoped<CreditOptimizationService>();

        return services;
    }
}
