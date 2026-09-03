using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Features.Products.Commands.CreateProduct;
using CleanArch.Domain.Entities;
using FluentAssertions;
using FluentValidation.TestHelper;
using NSubstitute;

namespace CleanArch.UnitTests.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new CreateProductCommand { Name = "", Price = 10 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Should_HaveError_When_NameExceeds200Characters()
    {
        var command = new CreateProductCommand { Name = new string('A', 201), Price = 10 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("Name must not exceed 200 characters.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_HaveError_When_PriceIsZeroOrNegative(decimal price)
    {
        var command = new CreateProductCommand { Name = "Valid Name", Price = price };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Price)
            .WithErrorMessage("Price must be greater than 0.");
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new CreateProductCommand
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            Price = 1200.50m,
            Sku = "LAP-001"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _handler = new CreateProductCommandHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_AndReturnGeneratedId()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Mechanical Keyboard",
            Description = "Wireless mechanical keyboard",
            Price = 89.99m,
            Sku = "KB-001"
        };

        _productRepository.AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var product = callInfo.Arg<Product>();
                product.Id = 42;
                return Task.FromResult(product);
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(42);
        await _productRepository.Received(1).AddAsync(
            Arg.Is<Product>(p =>
                p.Name == command.Name &&
                p.Description == command.Description &&
                p.Price == command.Price &&
                p.Sku == command.Sku),
            Arg.Any<CancellationToken>());
    }
}
