using eCommerce.OrdersService.Application.DTOs;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId): IRequest<Result<OrderDto>>;