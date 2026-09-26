namespace FlowForge.Api.Models;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public required Customer Customer { get; set; }

    public OrderStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}