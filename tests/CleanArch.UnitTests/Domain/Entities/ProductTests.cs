using CleanArch.Domain.Entities;
using FluentAssertions;

namespace CleanArch.UnitTests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Should_CreateProduct_WithSpecifiedProperties()
    {
        // Arrange & Act
        var product = new Product
        {
            Name = "Mechanical Keyboard",
            Description = "RGB Wireless Gaming Keyboard",
            Price = 99.99m,
            Sku = "KB-001"
        };

        // Assert
        product.Name.Should().Be("Mechanical Keyboard");
        product.Description.Should().Be("RGB Wireless Gaming Keyboard");
        product.Price.Should().Be(99.99m);
        product.Sku.Should().Be("KB-001");
        product.Id.Should().Be(0);
        product.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.CreatedBy.Should().BeNull();
        product.LastModified.Should().BeNull();
        product.LastModifiedBy.Should().BeNull();
    }
}
