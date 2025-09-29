using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.Errors;
using eCommerce.OrdersService.Application.Extensions;
using eCommerce.OrdersService.Application.RepositoryContracts;
using eCommerce.OrdersService.Application.Validators;
using eCommerce.OrdersService.Domain.Entities;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Commands.AddOrder;

public class AddOrderCommandHandler : IRequestHandler<AddOrderCommand, Result<OrderDto>>
{
    private readonly AddOrderDtoValidator _validator;
    private readonly IMapper _mapper;
    private readonly IOrdersRepository _repo;

    public AddOrderCommandHandler(AddOrderDtoValidator validator, IMapper mapper, IOrdersRepository repo)
    {
        _validator = validator;
        _mapper = mapper;
        _repo = repo;
    }

    public async Task<Result<OrderDto>> Handle(AddOrderCommand request, CancellationToken cancellationToken)
    {
        var addOrder = request.AddOrder;
        var validRes = await _validator.ValidateAsync(addOrder);
        if (!validRes.IsValid)
            return Result.Fail<OrderDto>(validRes.ToValidationErrors());

        var order = _mapper.Map<Order>(addOrder);
        var addedOrder = await _repo.AddOrderAsync(order);

        if (addedOrder is null)
            return Result.Fail<OrderDto>(new PersistenceError("Order cannot be saved."));

        return Result.Ok(_mapper.Map<OrderDto>(addedOrder));
    }
}
