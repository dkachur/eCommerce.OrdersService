using eCommerce.OrdersService.Application.DTOs;
using FluentValidation;

namespace eCommerce.OrdersService.Application.Validators;

public class UpdateOrderItemDtoValidator : AbstractValidator<UpdateOrderItemDto>
{
    public UpdateOrderItemDtoValidator()
    {
        RuleFor(o => o.UserId)
            .NotEmpty();

        RuleFor(o => o.OrderDate)
            .NotEmpty()
            .GreaterThan(DateTime.Parse("2000-01-01"));

        RuleFor(o => o.AddOrderItemDtos)
            .NotEmpty();

        RuleForEach(o => o.AddOrderItemDtos)
            .SetValidator(new AddOrderItemDtoValidator());
    }
}

