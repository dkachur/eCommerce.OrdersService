using eCommerce.OrdersService.Application.Commands.UpdateOrder;
using FluentValidation;

namespace eCommerce.OrdersService.Application.Validators;

public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderDtoValidator()
    {
        RuleFor(o => o.UserId)
            .NotEmpty();

        RuleFor(o => o.OrderDate)
            .NotEmpty()
            .GreaterThan(DateTime.Parse("2000-01-01"));

        RuleFor(o => o.UpdateOrderItemDtos)
            .NotEmpty();

        RuleForEach(o => o.UpdateOrderItemDtos)
            .SetValidator(new UpdateOrderItemDtoValidator());
    }
}
