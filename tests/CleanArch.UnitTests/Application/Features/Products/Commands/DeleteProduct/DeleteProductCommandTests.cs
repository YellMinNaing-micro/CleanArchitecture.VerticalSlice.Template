using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Features.Products.Commands.DeleteProduct;
using CleanArch.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace CleanArch.UnitTests.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _handler = new DeleteProductCommandHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldCallDeleteAsync()
    {
        // Arrange
        var product = new Product
        {
            Id = 5,
            Name = "Mouse",
            Price = 25m
        };

        _productRepository.GetByIdAsync(5, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(product));

        var command = new DeleteProductCommand(5);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _productRepository.Received(1).DeleteAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _productRepository.GetByIdAsync(100, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(null));

        var command = new DeleteProductCommand(100);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Product with ID 100 was not found.");

        await _productRepository.DidNotReceive().DeleteAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}
