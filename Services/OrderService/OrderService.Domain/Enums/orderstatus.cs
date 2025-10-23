namespace OrderService.Domain.Enums;

public enum OrderStatus
{
    Created = 0,
    Processing = 1,
    Paid = 2,
    Cancelled = 3,
    Delivered = 4
}