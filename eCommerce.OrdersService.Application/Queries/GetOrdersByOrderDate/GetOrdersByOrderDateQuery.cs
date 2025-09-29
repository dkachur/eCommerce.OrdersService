using eCommerce.OrdersService.Application.DTOs;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Queries.GetOrdersByOrderDate;

public record GetOrdersByOrderDateQuery(DateTime OrderDate) : IRequest<Result<List<OrderDto>>>;
