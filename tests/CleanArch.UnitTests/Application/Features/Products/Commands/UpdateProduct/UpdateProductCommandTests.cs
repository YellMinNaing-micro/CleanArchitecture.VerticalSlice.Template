using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Features.Products.Commands.UpdateProduct;
using CleanArch.Domain.Entities;
using FluentAssertions;
using FluentValidation.TestHelper;
using NSubstitute;

namespace CleanArch.UnitTests.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    public void Should_HaveError_When_IdIsZero(int id)
    {
        var command = new UpdateProductCommand { Id = id, Name = "Laptop", Price = 100 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Id)
            .WithErrorMessage("Product ID is required.");
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new UpdateProductCommand { Id = 1, Name = "", Price = 100 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Should_HaveError_When_NameExceeds200Characters()
    {
        var command = new UpdateProductCommand { Id = 1, Name = new string('B', 201), Price = 100 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("Name must not exceed 200 characters.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Should_HaveError_When_PriceIsZeroOrNegative(decimal price)
    {
        var command = new UpdateProductCommand { Id = 1, Name = "Laptop", Price = price };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Price)
            .WithErrorMessage("Price must be greater than 0.");
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new UpdateProductCommand
        {
            Id = 1,
            Name = "Monitor 4K",
            Description = "32 inch 4K UHD",
            Price = 499.99m,
            Sku = "MON-001"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _handler = new UpdateProductCommandHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldUpdatePropertiesAndCallRepository()
    {
        // Arrange
        var existingProduct = new Product
        {
            Id = 1,
            Name = "Old Monitor",
            Description = "Old 1080p",
            Price = 150m,
            Sku = "MON-OLD"
        };

        var command = new UpdateProductCommand
        {
            Id = 1,
            Name = "New Monitor",
            Description = "New 4K OLED",
            Price = 699m,
            Sku = "MON-NEW"
        };

        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(existingProduct));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingProduct.Name.Should().Be("New Monitor");
        existingProduct.Description.Should().Be("New OLED 4K" != null ? "New 4K OLED" : null);
        existingProduct.Price.Should().Be(699m);
        existingProduct.Sku.Should().Be("MON-NEW");
        existingProduct.LastModified.Should().NotBeNull();
        existingProduct.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        await _productRepository.Received(1).UpdateAsync(existingProduct, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new UpdateProductCommand
        {
            Id = 999,
            Name = "Missing Product",
            Price = 50m
        };

        _productRepository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(null));

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Product with ID 999 was not found.");

        await _productRepository.DidNotReceive().UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}
