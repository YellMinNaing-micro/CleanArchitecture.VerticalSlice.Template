using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CleanArch.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;

    public ApplicationDbContextInitializer(ApplicationDbContext context, ILogger<ApplicationDbContextInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        if (!await _context.Products.AnyAsync())
        {
            _context.Products.AddRange(
                new Product { Name = "iPhone 15 Pro", Description = "Apple flagship smartphone", Price = 999.99m, Sku = "APL-IP15P" },
                new Product { Name = "Samsung Galaxy S24 Ultra", Description = "Samsung flagship smartphone", Price = 1199.99m, Sku = "SAM-S24U" },
                new Product { Name = "Sony WH-1000XM5", Description = "Wireless Noise Cancelling Headphones", Price = 349.99m, Sku = "SNY-WH1000XM5" }
            );

            await _context.SaveChangesAsync();
        }
    }
}
