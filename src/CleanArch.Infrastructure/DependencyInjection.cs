using CleanArch.Application.Common.Interfaces;
using CleanArch.Infrastructure.Persistence;
using CleanArch.Infrastructure.Persistence.Interceptors;
using CleanArch.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=CleanArch.db";

        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlite(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(name: "database");

        return services;
    }
}
