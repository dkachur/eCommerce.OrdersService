using eCommerce.OrdersService.Domain.ValueObjects;

namespace eCommerce.OrdersService.Domain.Entities;

public class OrderItem
{
    public Guid ProductId { get; private set; }

    public Money UnitPrice { get; private set; }

    public Quantity Quantity { get; private set; }

    public Money TotalPrice { get; private set; }

    public static OrderItem New(Guid productId, decimal unitPrice, int quantity)
    {
        return new()
        {
            ProductId = productId,
            UnitPrice = new Money(unitPrice),
            Quantity = new Quantity(quantity),
            TotalPrice = new Money(unitPrice * quantity)
        };
    }

    public static OrderItem Restore(Guid productId, decimal unitPrice, int quantity, decimal totalPrice)
    {
        return new()
        {
            ProductId = productId,
            UnitPrice = new Money(unitPrice),
            Quantity = new Quantity(quantity),
            TotalPrice = new Money(totalPrice)
        };
    }

    private OrderItem() { }
}
