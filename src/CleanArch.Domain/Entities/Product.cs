using CleanArch.Domain.Common;

namespace CleanArch.Domain.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Sku { get; set; }
}
