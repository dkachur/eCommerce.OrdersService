namespace eCommerce.OrdersService.Domain.Entities;

public class OrderItem
{
    public Guid ProductId { get; private set; }

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal TotalPrice { get; private set; }

    public static OrderItem New(Guid productId, decimal unitPrice, int quantity)
    {
        return new()
        {
            ProductId = productId,
            UnitPrice = unitPrice,
            Quantity = quantity,
            TotalPrice = unitPrice * quantity
        };
    }

    public static OrderItem Restore(Guid productId, decimal unitPrice, int quantity, decimal totalPrice)
    {
        return new()
        {
            ProductId = productId,
            UnitPrice = unitPrice,
            Quantity = quantity,
            TotalPrice = totalPrice
        };
    }

    private OrderItem() { }
}
