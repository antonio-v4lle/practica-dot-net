using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using UltraPlatform.Worker.Factories;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Services;

namespace UltraPlatform.Worker.Tests.Factories;

public class DataServiceFactoryTests
{
    private static IConfiguration BuildConfig(bool useFiltering) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Features:UseDataFiltering"] = useFiltering.ToString()
            })
            .Build();

    private static IServiceProvider BuildScopedProvider()
    {
        var helper = Substitute.For<IDataValidationHelper>();
        var services = new ServiceCollection();

        services.AddSingleton(helper);
        services.AddScoped<DataService>();
        services.AddScoped<DataFilteredService>(sp =>
        {
            // Minimal stub — not exercising full DI graph here
            var inner  = Substitute.For<IDataService>();
            var h      = sp.GetRequiredService<IDataValidationHelper>();
            var specs  = Enumerable.Empty<UltraPlatform.Worker.Specifications.ISpecification<UltraPlatform.Worker.Models.DataRecord>>();
            var ctx    = UltraPlatform.Worker.Tests.TestDbContextFactory.Create(Guid.NewGuid().ToString());
            return new DataFilteredService(inner, h, specs, ctx);
        });

        return services.BuildServiceProvider();
    }

    // ── CreateDataService ─────────────────────────────────────────────────

    [Fact]
    public void CreateDataService_ReturnsDataFilteredService_WhenFeatureFlagIsTrue()
    {
        // Arrange
        var config   = BuildConfig(useFiltering: true);
        var sut      = new DataServiceFactory(config);
        var provider = BuildScopedProvider();

        // Act
        var result = sut.CreateDataService(provider);

        // Assert
        Assert.IsType<DataFilteredService>(result);
    }

    [Fact]
    public void CreateDataService_ReturnsDataService_WhenFeatureFlagIsFalse()
    {
        // Arrange
        var config   = BuildConfig(useFiltering: false);
        var sut      = new DataServiceFactory(config);
        var provider = BuildScopedProvider();

        // Act
        var result = sut.CreateDataService(provider);

        // Assert
        Assert.IsType<DataService>(result);
    }

    [Fact]
    public void CreateDataService_ReturnsDataFilteredService_WhenFlagIsMissing()
    {
        // Arrange — missing key defaults to true per DataServiceFactory implementation
        var config   = new ConfigurationBuilder().Build();
        var sut      = new DataServiceFactory(config);
        var provider = BuildScopedProvider();

        // Act
        var result = sut.CreateDataService(provider);

        // Assert — default is true → filtered
        Assert.IsType<DataFilteredService>(result);
    }
}
