using eCommerce.OrdersService.Application.DTOs;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrdersByProductId;

public record GetOrdersByProductIdQuery(Guid ProductId) : IRequest<Result<List<OrderDto>>>;