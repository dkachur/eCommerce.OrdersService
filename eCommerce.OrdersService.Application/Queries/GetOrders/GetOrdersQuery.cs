using eCommerce.OrdersService.Application.DTOs;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrders;

public record GetOrdersQuery : IRequest<Result<List<OrderDto>>>;
