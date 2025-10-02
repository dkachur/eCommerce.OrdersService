namespace eCommerce.OrdersService.Application.DTOs;

public record UserDto(
    Guid UserId,
    string Email,
    string PersonName,
    string Gender);
