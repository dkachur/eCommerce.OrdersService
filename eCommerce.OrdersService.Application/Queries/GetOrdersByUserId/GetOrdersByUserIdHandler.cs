using AutoMapper;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.RepositoryContracts;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrdersByUserId;

public class GetOrdersByUserIdHandler : IRequestHandler<GetOrdersByUserIdQuery, Result<List<OrderDto>>>
{
    private readonly IOrdersRepository _repo;
    private readonly IMapper _mapper;

    public GetOrdersByUserIdHandler(IOrdersRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<List<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken ct)
    {
        var userId = request.UserId;
        var orders = await _repo.GetByUserIdAsync(userId, ct);

        return Result.Ok(_mapper.Map<List<OrderDto>>(orders));
    }
}
