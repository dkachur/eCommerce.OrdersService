using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Commands.DeleteOrder;

public record DeleteOrderCommand(Guid OrderId) : IRequest<Result>;
