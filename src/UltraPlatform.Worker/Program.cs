using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UltraPlatform.Worker.Database;
using UltraPlatform.Worker.Factories;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Models;
using UltraPlatform.Worker.Repositories;
using UltraPlatform.Worker.Services;
using UltraPlatform.Worker.Specifications;
using UltraPlatform.Worker.Specifications.Data;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // ── DbContext (scoped — one per cycle via IServiceScopeFactory) ──────
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("UltraPlatformDb"));

        // ── Utility & shared validation helper ───────────────────────────────
        services.AddScoped<IUtilityService, UtilityService>();
        services.AddScoped<IDataValidationHelper, DataValidationHelper>();

        // ── Repository ────────────────────────────────────────────────────────
        services.AddScoped<IDataRecordRepository, DataRecordRepository>();

        // ── Specifications (scoped — each cycle gets fresh instances with fresh DbContext) ──
        services.AddScoped<ISpecification<DataRecord>, TodaysRecordsSpecification>();
        services.AddScoped<ISpecification<DataRecord>, ExcludeInactiveCodesSpecification>();

        // ── Services ──────────────────────────────────────────────────────────
        // Register concrete types so factory can resolve them by type
        services.AddScoped<DataService>();
        services.AddScoped<DataFilteredService>();

        // IDataService points to DataService by default (factory overrides at runtime)
        services.AddScoped<IDataService>(sp => sp.GetRequiredService<DataService>());

        // ── Factory (singleton — only reads feature flag from IConfiguration) ─
        services.AddSingleton<IDataServiceFactory, DataServiceFactory>();

        // ── BackgroundService ─────────────────────────────────────────────────
        services.AddHostedService<PrimaryBackground>();
    })
    .Build()
    .Run();
