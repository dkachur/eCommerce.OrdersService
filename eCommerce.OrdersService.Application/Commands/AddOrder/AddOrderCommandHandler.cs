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

namespace eCommerce.OrdersService.Application.Commands.AddOrder;

public class AddOrderCommandHandler : IRequestHandler<AddOrderCommand, Result<OrderDto>>
{
    private readonly IValidator<AddOrderDto> _validator;
    private readonly IMapper _mapper;
    private readonly IOrdersRepository _repo;
    private readonly IUsersServiceClient _usersServiceClient;
    private readonly IProductsServiceClient _productsServiceClient;

    public AddOrderCommandHandler(
        IValidator<AddOrderDto> validator, 
        IMapper mapper, 
        IOrdersRepository repo, 
        IUsersServiceClient usersServiceClient, 
        IProductsServiceClient productsServiceClient)
    {
        _validator = validator;
        _mapper = mapper;
        _repo = repo;
        _usersServiceClient = usersServiceClient;
        _productsServiceClient = productsServiceClient;
    }

    public async Task<Result<OrderDto>> Handle(AddOrderCommand request, CancellationToken ct)
    {
        var addOrder = request.AddOrder;
        var validRes = await _validator.ValidateAsync(addOrder, ct);
        if (!validRes.IsValid)
            return Result.Fail<OrderDto>(validRes.ToValidationErrors());

        var userExist = await _usersServiceClient.CheckUserExistsAsync(addOrder.UserId, ct);
        if (!userExist)
            return Result.Fail<OrderDto>(InvalidUserIdError.WithId(addOrder.UserId));

        var productsExistRes = await _productsServiceClient
            .CheckProductsExistAsync(addOrder.AddOrderItemDtos.Select(o => o.ProductId), ct);

        var invalidProductIds = productsExistRes
            .Where(kvp => kvp.Value is false)
            .Select(kvp => kvp.Key)
            .ToList();

        if (invalidProductIds.Any())
            return Result.Fail<OrderDto>(
                invalidProductIds.Select(id => InvalidProductIdError.WithId(id)));

        var order = _mapper.Map<Order>(addOrder);
        var addedOrder = await _repo.AddOrderAsync(order, ct);

        if (addedOrder is null)
            return Result.Fail<OrderDto>(new PersistenceError("Order cannot be saved."));

        return Result.Ok(_mapper.Map<OrderDto>(addedOrder));
    }
}
