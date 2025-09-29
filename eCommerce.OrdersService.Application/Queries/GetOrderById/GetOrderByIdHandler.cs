using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.Errors;
using eCommerce.OrdersService.Application.RepositoryContracts;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IOrdersRepository _repo;
    private readonly IMapper _mapper;

    public GetOrderByIdHandler(IOrdersRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var orderId = request.OrderId;
        var order = await _repo.GetByIdAsync(orderId, ct);
        if (order is null)
            return Result.Fail<OrderDto>(OrderNotFoundError.WithId(orderId));

        return Result.Ok(_mapper.Map<OrderDto>(order));
    }
}
