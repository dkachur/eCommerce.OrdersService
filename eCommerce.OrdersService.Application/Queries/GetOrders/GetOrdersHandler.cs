using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.RepositoryContracts;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<List<OrderDto>>>
{
    private readonly IOrdersRepository _repo;
    private readonly IMapper _mapper;

    public GetOrdersHandler(IOrdersRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<List<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var orders = await _repo.GetOrdersAsync(ct);
        var orderDtos = _mapper.Map<List<OrderDto>>(orders);
        return Result.Ok(orderDtos);
    }
}
