using eCommerce.OrdersService.Application.DTOs;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrdersByUserId;

public record GetOrdersByUserIdQuery(Guid UserId) : IRequest<Result<List<OrderDto>>>;