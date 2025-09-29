using FluentResults;

namespace eCommerce.OrdersService.Application.Errors;

public class ValidationError(string message, string propertyName) : Error(message)
{
    public string PropertyName => _propertyName;
    private readonly string _propertyName = propertyName;
}
