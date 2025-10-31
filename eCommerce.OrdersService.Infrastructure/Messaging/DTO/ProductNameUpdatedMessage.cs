namespace eCommerce.OrdersService.Infrastructure.Messaging.DTO;

public record ProductNameUpdatedMessage(Guid ProductId, string NewName);
