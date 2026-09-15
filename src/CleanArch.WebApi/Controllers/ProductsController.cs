using CleanArch.Application.Features.Products.Commands.CreateProduct;
using CleanArch.Application.Features.Products.Commands.UpdateProduct;
using CleanArch.Application.Features.Products.Commands.DeleteProduct;
using CleanArch.Application.Features.Products.Queries.GetProductById;
using CleanArch.Application.Features.Products.Queries.GetProducts;
using CleanArch.WebApi.Helpers;
using CleanArch.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArch.WebApi.Controllers;

public class ProductsController : ApiControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a new product")]
    [EndpointDescription("Creates a new product in the database and returns its unique ID.")]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateProductCommand command)
    {
        var productId = await Mediator.Send(command);
        return Ok(ApiResponseHelper.Success(productId, "Product created successfully."));
    }

    [HttpGet]
    [EndpointSummary("Get all products")]
    [EndpointDescription("Retrieves a list of all products currently in the database.")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProductDto>>>> GetAll()
    {
        var products = await Mediator.Send(new GetProductsQuery());
        return Ok(ApiResponseHelper.Success(products, "Products retrieved successfully."));
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get product details by ID")]
    [EndpointDescription("Retrieves details of a specific product using its unique ID.")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
    {
        ProductDto? product = await Mediator.Send(new GetProductByIdQuery(id));

        if (product == null)
        {
            return NotFound(ApiResponseHelper.Failure($"Product with ID {id} was not found."));
        }

        return Ok(ApiResponseHelper.Success(product, "Product retrieved successfully."));
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a product")]
    [EndpointDescription("Updates details of an existing product in the database.")]
    public async Task<ActionResult> Update(int id, UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponseHelper.Failure("Product ID in path must match Product ID in request body."));
        }

        await Mediator.Send(command);

        return Ok(ApiResponseHelper.Success("Product updated successfully."));
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Delete a product")]
    [EndpointDescription("Removes a product from the database using its unique ID.")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteProductCommand(id));

        return Ok(ApiResponseHelper.Success("Product deleted successfully."));
    }
}
