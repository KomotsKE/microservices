using CoreLib.Entities;
using OrderService.Domain.Enums;
namespace OrderService.Domain.Entities;

public class Order: BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
}