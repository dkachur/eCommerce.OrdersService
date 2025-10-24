using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.Errors;
using eCommerce.OrdersService.Application.Extensions;
using eCommerce.OrdersService.Application.RepositoryContracts;
using eCommerce.OrdersService.Application.ServiceContracts;
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
    private readonly IUsersServiceClient _usersServiceClient;
    private readonly IProductsServiceClient _productsServiceClient;

    public UpdateOrderCommandHandler(
        IValidator<UpdateOrderDto> validator, 
        IMapper mapper, IOrdersRepository repo, 
        IUsersServiceClient usersServiceClient, 
        IProductsServiceClient productsServiceClient)
    {
        _validator = validator;
        _mapper = mapper;
        _repo = repo;
        _usersServiceClient = usersServiceClient;
        _productsServiceClient = productsServiceClient;
    }

    public async Task<Result<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken ct)
    {
        var updateOrder = request.UpdateOrder;
        var validRes = await _validator.ValidateAsync(updateOrder, ct);
        if (!validRes.IsValid)
            return Result.Fail<OrderDto>(validRes.ToValidationErrors());

        var userExist = await _usersServiceClient.CheckUserExistsAsync(updateOrder.UserId, ct);
        if (!userExist)
            return Result.Fail<OrderDto>(InvalidUserIdError.WithId(updateOrder.UserId));

        var productsExistRes = await _productsServiceClient
            .CheckProductsExistAsync(updateOrder.UpdateOrderItemDtos.Select(o => o.ProductId), ct);

        var invalidProductIds = productsExistRes
            .Where(kvp => kvp.Value is false)
            .Select(kvp => kvp.Key)
            .ToList();

        if (invalidProductIds.Any())
            return Result.Fail<OrderDto>(
                invalidProductIds.Select(id => InvalidProductIdError.WithId(id)));

        var order = _mapper.Map<Order>(updateOrder);
        var updatedOrder = await _repo.UpdateOrderAsync(order, ct);

        if (updatedOrder is null)
            return Result.Fail<OrderDto>(OrderNotFoundError.WithId(updateOrder.OrderId));

        return Result.Ok(_mapper.Map<OrderDto>(updatedOrder));
    }
}
