using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Features.Products.Queries.GetProducts;
using CleanArch.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace CleanArch.UnitTests.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        _handler = new GetProductsQueryHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WhenProductsExist_ShouldReturnListOfProductDto()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Item 1", Price = 10m, Sku = "SKU1" },
            new() { Id = 2, Name = "Item 2", Price = 20m, Sku = "SKU2" }
        };

        _productRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Product>>(products));

        var query = new GetProductsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[0].Name.Should().Be("Item 1");
        result[1].Id.Should().Be(2);
        result[1].Name.Should().Be("Item 2");
    }

    [Fact]
    public async Task Handle_WhenNoProductsExist_ShouldReturnEmptyList()
    {
        // Arrange
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Product>>(new List<Product>()));

        var query = new GetProductsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
