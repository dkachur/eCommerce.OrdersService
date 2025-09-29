using eCommerce.OrdersService.Domain.Entities;

namespace eCommerce.OrdersService.Application.RepositoryContracts;

/// <summary>
/// Represents data access logic for managing <see cref="Order"/> and <see cref="OrderItem"/> entities.
/// </summary>
public interface IOrdersRepository
{
    /// <summary>
    /// Retrieves all orders from the storage.
    /// </summary>
    /// <returns>
    /// A list of all <see cref="Order"/> instances.
    /// If the storage contains no orders, an empty collection is returned.
    /// </returns>
    Task<List<Order>> GetOrdersAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves order with the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>
    /// A <see cref="Order"/> with the specified ID if found;
    /// otherwise, <c>null</c>.
    /// </returns>
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all orders that contain order item with the specified <paramref name="productId"/>.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <returns>
    /// A list of <see cref="Order"/> instances that match the search criteria.  
    /// If no orders are found, an empty collection is returned.
    /// </returns>
    Task<List<Order>> GetWithProductIdAsync(Guid productId, CancellationToken ct = default);

    /// <summary>
    /// Adds order to the storage.
    /// </summary>
    /// <param name="order">The order to add.</param>
    /// <returns>
    /// The added <see cref="Order"/> if adding is successful;
    /// otherwise, <c>null</c>.
    /// </returns>
    Task<Order?> AddOrderAsync(Order order, CancellationToken ct = default);

    /// <summary>
    /// Updates order in the storage.
    /// </summary>
    /// <param name="order">The updated order.</param>
    /// <returns>
    /// The updated <see cref="Order"/> if updation is successful;
    /// otherwise, <c>null</c>.
    /// </returns>
    Task<Order?> UpdateOrderAsync(Order order, CancellationToken ct = default);

    /// <summary>
    /// Deletes order with the specified ID from the storage.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>
    /// <c>true</c> if delition is successful;
    /// otherwise, <c>false.</c>
    /// </returns>
    Task<bool> DeleteOrderAsync(Guid id, CancellationToken ct = default);
}
