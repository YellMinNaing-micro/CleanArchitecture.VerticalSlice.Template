using System.Net;
using System.Net.Http.Json;
using CleanArch.Application.Features.Products.Commands.CreateProduct;
using CleanArch.Application.Features.Products.Queries.GetProducts;
using CleanArch.WebApi.Models;
using FluentAssertions;

namespace CleanArch.IntegrationTests;

public class ProductsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new() { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task CreateAndGetProduct_ShouldPersistProduct()
    {
        var command = new CreateProductCommand
        {
            Name = "Integration Test Product",
            Description = "Created through the HTTP API",
            Price = 42.50m,
            Sku = $"TEST-{Guid.NewGuid():N}"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", command);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK,
            await createResponse.Content.ReadAsStringAsync());
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        createResult.Should().NotBeNull();
        createResult!.Success.Should().BeTrue();
        createResult.Data.Should().BeGreaterThan(0);

        var getResult = await _client.GetFromJsonAsync<ApiResponse<ProductDto>>(
            $"/api/products/{createResult.Data}");
        getResult.Should().NotBeNull();
        getResult!.Data.Should().NotBeNull();
        getResult.Data!.Name.Should().Be(command.Name);
        getResult.Data.Price.Should().Be(command.Price);
        getResult.Data.Sku.Should().Be(command.Sku);
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturnHealthy()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
        content.Should().Contain("database");
    }
}
