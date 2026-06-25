using CleanArch.Application.Features.Products.Commands.CreateProduct;
using CleanArch.Application.Features.Products.Commands.UpdateProduct;
using CleanArch.Application.Features.Products.Commands.DeleteProduct;
using CleanArch.Application.Features.Products.Queries.GetProductById;
using CleanArch.Application.Features.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArch.WebApi.Controllers;

public class ProductsController : ApiControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a new product")]
    [EndpointDescription("Creates a new product in the database and returns its unique ID.")]
    public async Task<ActionResult<int>> Create(CreateProductCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet]
    [EndpointSummary("Get all products")]
    [EndpointDescription("Retrieves a list of all products currently in the database.")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll()
    {
        return Ok(await Mediator.Send(new GetProductsQuery()));
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get product details by ID")]
    [EndpointDescription("Retrieves details of a specific product using its unique ID.")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await Mediator.Send(new GetProductByIdQuery(id));

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a product")]
    [EndpointDescription("Updates details of an existing product in the database.")]
    public async Task<ActionResult> Update(int id, UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Product ID in path must match Product ID in request body.");
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Delete a product")]
    [EndpointDescription("Removes a product from the database using its unique ID.")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteProductCommand(id));

        return NoContent();
    }
}
