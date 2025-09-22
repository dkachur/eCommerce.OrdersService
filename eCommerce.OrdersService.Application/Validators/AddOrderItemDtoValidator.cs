using eCommerce.OrdersService.Application.DTOs;
using FluentValidation;

namespace eCommerce.OrdersService.Application.Validators;

public class AddOrderItemDtoValidator : AbstractValidator<AddOrderItemDto>
{
    public AddOrderItemDtoValidator()
    {
        RuleFor(o => o.ProductId)
            .NotEmpty();

        RuleFor(o => o.UnitPrice)
            .InclusiveBetween(0, 9999m);

        RuleFor(o => o.Quantity)
            .InclusiveBetween(1, 100);
    }
}
