using eCommerce.OrdersService.Application.Commands.UpdateOrder;
using FluentValidation;

namespace eCommerce.OrdersService.Application.Validators;

public class UpdateOrderItemDtoValidator : AbstractValidator<UpdateOrderItemDto>
{
    public UpdateOrderItemDtoValidator()
    {
        RuleFor(o => o.ProductId)
            .NotEmpty();

        RuleFor(o => o.UnitPrice)
            .InclusiveBetween(0, 9999.99m);

        RuleFor(o => o.Quantity)
            .InclusiveBetween(1, 100);
    }
}

