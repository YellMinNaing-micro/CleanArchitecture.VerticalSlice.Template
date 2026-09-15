using CleanArch.Application.Features.Products.Commands.CreateProduct;
using CleanArch.Application.Features.Products.Commands.DeleteProduct;
using CleanArch.Application.Features.Products.Commands.UpdateProduct;
using CleanArch.Application.Features.Products.Queries.GetProductById;
using CleanArch.Application.Features.Products.Queries.GetProducts;
using CleanArch.WebApi.Controllers;
using CleanArch.WebApi.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CleanArch.UnitTests.WebApi.Controllers;

public class ProductsControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_mediator);
        var serviceProvider = services.BuildServiceProvider();

        _controller = new ProductsController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider
                }
            }
        };
    }

    [Fact]
    public async Task Create_ShouldSendCreateProductCommand_AndReturnId()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Smartphone",
            Price = 799m
        };

        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(123));

        // Act
        var result = await _controller.Create(command);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<int>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().Be(123);
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_ShouldSendGetProductsQuery_AndReturnOkWithProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new() { Id = 1, Name = "Laptop", Price = 1200m }
        };

        _mediator.Send(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ProductDto>>(products));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IReadOnlyList<ProductDto>>>().Subject;
        var returnedProducts = response.Data!;
        returnedProducts.Should().HaveCount(1);
        returnedProducts[0].Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetById_WhenProductExists_ShouldReturnOkWithProduct()
    {
        // Arrange
        var product = new ProductDto { Id = 1, Name = "Tablet", Price = 499m };
        _mediator.Send(Arg.Is<GetProductByIdQuery>(q => q.Id == 1), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ProductDto?>(product));

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ProductDto>>().Subject;
        response.Data.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task GetById_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _mediator.Send(Arg.Is<GetProductByIdQuery>(q => q.Id == 99), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ProductDto?>(null));

        // Act
        var result = await _controller.GetById(99);

        // Assert
        var notFound = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().BeOfType<ApiResponse>().Which.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Update_WhenIdDoesNotMatchCommandId_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new UpdateProductCommand { Id = 2, Name = "Tablet Pro", Price = 599m };

        // Act
        var result = await _controller.Update(1, command);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var response = badRequestResult.Value.Should().BeOfType<ApiResponse>().Subject;
        response.Success.Should().BeFalse();
        response.Message.Should().Be("Product ID in path must match Product ID in request body.");
        await _mediator.DidNotReceive().Send(Arg.Any<UpdateProductCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_WhenIdMatchesCommandId_ShouldSendUpdateCommand_AndReturnSuccessResponse()
    {
        // Arrange
        var command = new UpdateProductCommand { Id = 1, Name = "Tablet Pro", Price = 599m };

        // Act
        var result = await _controller.Update(1, command);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<ApiResponse>().Which.Success.Should().BeTrue();
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_ShouldSendDeleteProductCommand_AndReturnSuccessResponse()
    {
        // Arrange & Act
        var result = await _controller.Delete(10);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<ApiResponse>().Which.Success.Should().BeTrue();
        await _mediator.Received(1).Send(Arg.Is<DeleteProductCommand>(c => c.Id == 10), Arg.Any<CancellationToken>());
    }
}
