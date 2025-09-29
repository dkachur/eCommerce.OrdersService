using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.RepositoryContracts;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrdersByOrderDate;

public class GetOrdersByOrderDateHandler : IRequestHandler<GetOrdersByOrderDateQuery, Result<List<OrderDto>>>
{
    private readonly IOrdersRepository _repo;
    private readonly IMapper _mapper;

    public GetOrdersByOrderDateHandler(IOrdersRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<List<OrderDto>>> Handle(GetOrdersByOrderDateQuery request, CancellationToken ct)
    {
        var date = request.OrderDate;
        var orders = await _repo.GetByOrderDate(date, ct);
        var orderDtos = _mapper.Map<List<OrderDto>>(orders);
        return Result.Ok(orderDtos);
    }
}
