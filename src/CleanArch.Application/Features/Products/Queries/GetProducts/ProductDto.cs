namespace CleanArch.Application.Features.Products.Queries.GetProducts;

public class ProductDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Sku { get; set; }
}
