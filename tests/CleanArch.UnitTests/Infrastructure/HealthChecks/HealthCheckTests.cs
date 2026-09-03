using CleanArch.Infrastructure;
using CleanArch.Infrastructure.Persistence;
using CleanArch.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArch.UnitTests.Infrastructure.HealthChecks;

public class HealthCheckTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public HealthCheckTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterHealthCheckService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddInfrastructureServices(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        var healthCheckService = provider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();
    }

    [Fact]
    public async Task HealthCheck_WhenDatabaseIsAvailable_ShouldReturnHealthy()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = new ConfigurationBuilder().Build();

        services.AddInfrastructureServices(configuration);

        // Override ApplicationDbContext with an open in-memory SQLite connection
        var interceptor = new AuditableEntityInterceptor();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .AddInterceptors(interceptor)
            .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated();

        services.AddScoped(_ => dbContext);

        var provider = services.BuildServiceProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();

        // Act
        var report = await healthCheckService.CheckHealthAsync();

        // Assert
        report.Status.Should().Be(HealthStatus.Healthy);
        report.Entries.Should().ContainKey("database");
        report.Entries["database"].Status.Should().Be(HealthStatus.Healthy);
    }
}
