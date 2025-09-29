using eCommerce.OrdersService.API.Interfaces;
using System.Text.Json.Serialization;

namespace eCommerce.OrdersService.API.DTOs;

public record OrderResponse(
    Guid OrderId,
    Guid UserId,
    DateTime OrderDate,
    decimal TotalBill,
    List<OrderItemResponse> OrderItems) : IResourceWithId
{
    [JsonIgnore]
    public Guid ResourceId => OrderId;

    public OrderResponse() : this(default, default, default, default, []) { }
}
