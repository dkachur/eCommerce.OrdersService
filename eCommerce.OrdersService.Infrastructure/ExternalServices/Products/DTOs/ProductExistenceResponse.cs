namespace eCommerce.OrdersService.Infrastructure.ExternalServices.Products.DTOs;

public record ProductExistenceResponse(Guid ProductId, bool Exists);