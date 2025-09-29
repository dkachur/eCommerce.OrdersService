using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.Errors;
using eCommerce.OrdersService.Application.Extensions;
using eCommerce.OrdersService.Application.RepositoryContracts;
using eCommerce.OrdersService.Domain.Entities;
using FluentResults;
using FluentValidation;
using MediatR;

namespace eCommerce.OrdersService.Application.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Result<OrderDto>>
{
    private readonly IValidator<UpdateOrderDto> _validator;
    private readonly IMapper _mapper;
    private readonly IOrdersRepository _repo;

    public UpdateOrderCommandHandler(IValidator<UpdateOrderDto> validator, IMapper mapper, IOrdersRepository repo)
    {
        _validator = validator;
        _mapper = mapper;
        _repo = repo;
    }

    public async Task<Result<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var updateOrder = request.UpdateOrder;
        var validRes = await _validator.ValidateAsync(updateOrder);
        if (!validRes.IsValid)
            return Result.Fail<OrderDto>(validRes.ToValidationErrors());

        var order = _mapper.Map<Order>(updateOrder);
        var updatedOrder = await _repo.UpdateOrderAsync(order);

        if (updatedOrder is null)
            return Result.Fail<OrderDto>(OrderNotFoundError.WithId(updateOrder.OrderId));

        return Result.Ok(_mapper.Map<OrderDto>(updatedOrder));
    }


}
