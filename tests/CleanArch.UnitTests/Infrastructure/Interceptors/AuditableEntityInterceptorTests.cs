using CleanArch.Application.Common.Interfaces;
using CleanArch.Domain.Entities;
using CleanArch.Infrastructure.Persistence;
using CleanArch.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CleanArch.UnitTests.Infrastructure.Interceptors;

public class AuditableEntityInterceptorTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public AuditableEntityInterceptorTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    private ApplicationDbContext CreateDbContext(IUser? user = null, TimeProvider? timeProvider = null)
    {
        var interceptor = new AuditableEntityInterceptor(user, timeProvider);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .AddInterceptors(interceptor)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task SavingChangesAsync_WhenEntityAdded_ShouldPopulateAuditTimestampsAndUser()
    {
        // Arrange
        var user = Substitute.For<IUser>();
        user.Id.Returns("test-user-id");

        using var context = CreateDbContext(user: user);

        var product = new Product
        {
            Name = "Mechanical Keyboard",
            Price = 99.99m,
            Sku = "KB-01"
        };

        // Act
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Assert
        product.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.CreatedBy.Should().Be("test-user-id");
        product.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.LastModifiedBy.Should().Be("test-user-id");
    }

    [Fact]
    public async Task SavingChangesAsync_WhenEntityModified_ShouldUpdateLastModifiedAndPreserveCreated()
    {
        // Arrange
        var initialUser = Substitute.For<IUser>();
        initialUser.Id.Returns("creator-user-id");

        using var context = CreateDbContext(user: initialUser);

        var product = new Product
        {
            Name = "Old Name",
            Price = 50m,
            Sku = "SKU-OLD"
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var originalCreated = product.Created;
        var originalCreatedBy = product.CreatedBy;

        // Change user for modification
        var updateUser = Substitute.For<IUser>();
        updateUser.Id.Returns("updater-user-id");

        using var updateContext = CreateDbContext(user: updateUser);
        var trackedProduct = await updateContext.Products.FindAsync(product.Id);

        // Act
        trackedProduct!.Name = "Updated Name";
        await Task.Delay(50); // slight time shift
        await updateContext.SaveChangesAsync();

        // Assert
        trackedProduct.Name.Should().Be("Updated Name");
        trackedProduct.Created.Should().Be(originalCreated);
        trackedProduct.CreatedBy.Should().Be(originalCreatedBy);
        trackedProduct.LastModifiedBy.Should().Be("updater-user-id");
        trackedProduct.LastModified.Should().BeOnOrAfter(originalCreated);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenUserIsNull_ShouldPopulateTimestampsWithNullUser()
    {
        // Arrange
        using var context = CreateDbContext(user: null);

        var product = new Product
        {
            Name = "Anonymous Product",
            Price = 29.99m,
            Sku = "ANON-01"
        };

        // Act
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Assert
        product.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.CreatedBy.Should().BeNull();
        product.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.LastModifiedBy.Should().BeNull();
    }
}
