using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Features.Products.Queries.GetProductById;
using CleanArch.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace CleanArch.UnitTests.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _handler = new GetProductByIdQueryHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnMappedProductDto()
    {
        // Arrange
        var product = new Product
        {
            Id = 10,
            Name = "Headphones",
            Description = "Noise Cancelling",
            Price = 249.99m,
            Sku = "HP-001"
        };

        _productRepository.GetByIdAsync(10, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(product));

        var query = new GetProductByIdQuery(10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
        result.Name.Should().Be("Headphones");
        result.Description.Should().Be("Noise Cancelling");
        result.Price.Should().Be(249.99m);
        result.Sku.Should().Be("HP-001");
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _productRepository.GetByIdAsync(99, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(null));

        var query = new GetProductByIdQuery(99);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
