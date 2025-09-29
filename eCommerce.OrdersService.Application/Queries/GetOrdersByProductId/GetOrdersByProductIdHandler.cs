using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.RepositoryContracts;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrdersByProductId;

public class GetOrdersByProductIdHandler : IRequestHandler<GetOrdersByProductIdQuery, Result<List<OrderDto>>>
{
    private readonly IOrdersRepository _repo;
    private readonly IMapper _mapper;

    public GetOrdersByProductIdHandler(IOrdersRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<List<OrderDto>>> Handle(GetOrdersByProductIdQuery request, CancellationToken cancellationToken)
    {
        var productId = request.ProductId;
        var orders = await _repo.GetWithProductIdAsync(productId);
        var orderDtos = _mapper.Map<List<OrderDto>>(orders);
        return Result.Ok(orderDtos);
    }
}
