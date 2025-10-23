namespace OrderService.Application.DTOs;

public record OrderDto(
    Guid Id,
    Guid UserId,
    Guid ProductId,
    int Quantity,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAt
);