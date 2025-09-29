using eCommerce.OrdersService.Application.Errors;
using eCommerce.OrdersService.Application.RepositoryContracts;
using FluentResults;
using MediatR;

namespace eCommerce.OrdersService.Application.Commands.DeleteOrder;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Result>
{
    private readonly IOrdersRepository _repo;

    public DeleteOrderCommandHandler(IOrdersRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var orderId = request.OrderId;
        var deleted = await _repo.DeleteOrderAsync(orderId);
        if (!deleted)
            return Result.Fail(OrderNotFoundError.WithId(orderId));

        return Result.Ok();
    }
}
