using eCommerce.OrdersService.Application.DTOs;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Commands.UpdateOrder;

public record UpdateOrderCommand(UpdateOrderDto UpdateOrder): IRequest<Result<OrderDto>>;