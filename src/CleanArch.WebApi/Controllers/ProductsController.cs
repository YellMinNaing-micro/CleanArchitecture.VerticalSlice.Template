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
    public async Task<ActionResult<ApiResult<int>>> Create(CreateProductCommand command)
    {
        var productId = await Mediator.Send(command);
        return ApiResponse.OkResult(
            $"/api/products/{productId}",
            new MessageResponse
            {
                EN = "Product created successfully.",
                MM = "ကုန်ပစ္စည်းအသစ် ဖန်တီးပြီးပါပြီ။"
            },
            productId);
    }

    [HttpGet]
    [EndpointSummary("Get all products")]
    [EndpointDescription("Retrieves a list of all products currently in the database.")]
    public async Task<ActionResult<ApiResult<IReadOnlyList<ProductDto>>>> GetAll()
    {
        var products = await Mediator.Send(new GetProductsQuery());
        return ApiResponse.OkResult(
            null,
            new MessageResponse
            {
                EN = "Products retrieved successfully.",
                MM = "ကုန်ပစ္စည်းများ ရယူပြီးပါပြီ။"
            },
            products);
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get product details by ID")]
    [EndpointDescription("Retrieves details of a specific product using its unique ID.")]
    public async Task<ActionResult<ApiResult<ProductDto>>> GetById(int id)
    {
        ProductDto? product = await Mediator.Send(new GetProductByIdQuery(id));

        if (product == null)
        {
            return ApiResponse.ErrorResult(
                StatusCodes.Status404NotFound,
                new MessageResponse
                {
                    EN = $"Product with ID {id} was not found.",
                    MM = $"ID {id} ဖြင့် ကုန်ပစ္စည်းကို ရှာမတွေ့ပါ။"
                });
        }

        return ApiResponse.OkResult(
            null,
            new MessageResponse
            {
                EN = "Product retrieved successfully.",
                MM = "ကုန်ပစ္စည်းအချက်အလက် ရယူပြီးပါပြီ။"
            },
            product);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a product")]
    [EndpointDescription("Updates details of an existing product in the database.")]
    public async Task<ActionResult> Update(int id, UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return ApiResponse.ErrorResult(
                StatusCodes.Status400BadRequest,
                new MessageResponse
                {
                    EN = "Product ID in path must match Product ID in request body.",
                    MM = "လမ်းကြောင်းရှိ Product ID နှင့် request body ရှိ Product ID တူညီရပါမည်။"
                });
        }

        await Mediator.Send(command);

        return ApiResponse.OkResult(
            null,
            new MessageResponse
            {
                EN = "Product updated successfully.",
                MM = "ကုန်ပစ္စည်းအချက်အလက် ပြင်ဆင်ပြီးပါပြီ။"
            });
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Delete a product")]
    [EndpointDescription("Removes a product from the database using its unique ID.")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteProductCommand(id));

        return ApiResponse.OkResult(
            null,
            new MessageResponse
            {
                EN = "Product deleted successfully.",
                MM = "ကုန်ပစ္စည်းကို ဖျက်ပြီးပါပြီ။"
            });
    }
}
