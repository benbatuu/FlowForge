namespace FlowForge.Api.Models;

public class Product
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Image { get; set; }

    public decimal UnitPrice { get; set; }

    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}