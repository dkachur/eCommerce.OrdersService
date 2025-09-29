using FluentResults;

namespace eCommerce.OrdersService.Application.Errors;

public class PersistenceError(string message) : Error(message);
