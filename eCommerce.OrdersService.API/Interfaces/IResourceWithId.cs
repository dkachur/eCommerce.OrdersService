namespace eCommerce.OrdersService.API.Interfaces;

/// <summary>
/// Represents a resource with a unique identifier.
/// </summary>
public interface IResourceWithId
{
    /// <summary>
    /// Gets the unique identifier of the resource.
    /// </summary>
    Guid ResourceId { get; }
}
