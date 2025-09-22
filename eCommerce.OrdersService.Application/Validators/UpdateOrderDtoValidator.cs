using eCommerce.OrdersService.Application.DTOs;
using FluentValidation;

namespace eCommerce.OrdersService.Application.Validators;

public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderItemDto>
{
    public UpdateOrderDtoValidator()
    {
        RuleFor(o => o.ProductId)
            .NotEmpty();

        RuleFor(o => o.UnitPrice)
            .InclusiveBetween(0, 9999m);

        RuleFor(o => o.Quantity)
            .InclusiveBetween(1, 100);
    }
}
