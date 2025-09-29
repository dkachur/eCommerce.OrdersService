using eCommerce.OrdersService.Application.Errors;
using FluentValidation.Results;

namespace eCommerce.OrdersService.Application.Extensions;

public static class ValidationResultExtensions
{
    public static IEnumerable<ValidationError> ToValidationErrors(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .Select(e => new ValidationError(e.ErrorMessage, e.PropertyName));
    }
}
