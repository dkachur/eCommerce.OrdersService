using eCommerce.OrdersService.API.Interfaces;
using System.Text.Json.Serialization;

namespace eCommerce.OrdersService.API.DTOs;

/// <summary>
/// Data transfer object that represents the response order information.
/// </summary>
/// <param name="OrderId">The unique identifier of the order.</param>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="UserPersonName">The full name of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="OrderDate">The date of the order.</param>
/// <param name="TotalBill">The total bill of the order.</param>
/// <param name="OrderItems">The collection of items in the order.</param>
public record OrderResponse(
    Guid OrderId,
    Guid UserId,
    string? UserPersonName,
    string? Email,
    DateTime OrderDate,
    decimal TotalBill,
    List<OrderItemResponse> OrderItems) : IResourceWithId
{
    [JsonIgnore]
    public Guid ResourceId => OrderId;

    public OrderResponse() : this(default, default, default, default, default, default, []) { }
}
