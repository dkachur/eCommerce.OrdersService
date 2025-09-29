using eCommerce.OrdersService.Application.DTOs;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Commands.AddOrder;

public record AddOrderCommand(AddOrderDto AddOrder) : IRequest<Result<OrderDto>>;