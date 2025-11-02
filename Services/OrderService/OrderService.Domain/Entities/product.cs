using CoreLib.Entities;

namespace OrderService.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int Stock { get; set;}
    public DateTime UpdatedAt { get; set; }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        if (Stock < quantity)
            throw new InvalidOperationException("Not enough stock");

        Stock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}