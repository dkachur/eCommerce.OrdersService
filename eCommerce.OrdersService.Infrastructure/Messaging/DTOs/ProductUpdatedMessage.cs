namespace eCommerce.OrdersService.Infrastructure.Messaging.DTOs;

public record ProductUpdatedMessage(
    Guid Id,
    string Name,
    string Category,
    double UnitPrice,
    int QuantityInStock);
