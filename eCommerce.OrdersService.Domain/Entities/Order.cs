using eCommerce.OrdersService.Domain.ValueObjects;

namespace eCommerce.OrdersService.Domain.Entities;

public class Order
{
    public Guid OrderId { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime OrderDate { get; private set; }

    public Money TotalBill { get; private set; }

    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private List<OrderItem> _orderItems = [];

    public static Order New(Guid userId, 
        DateTime orderDate,
        List<OrderItem> orderItems)
    {
        return new()
        {
            OrderId = Guid.NewGuid(),
            UserId = userId,
            OrderDate = orderDate,
            TotalBill = new Money(orderItems.Sum(o => o.TotalPrice)),
            _orderItems = orderItems.ToList()
        };
    }

    public static Order Restore(Guid orderId, 
        Guid userId, 
        DateTime orderDate, 
        decimal totalBill, 
        List<OrderItem> orderItems)
    {
        return new()
        {
            OrderId = orderId,
            UserId = userId,
            OrderDate = orderDate,
            TotalBill = new Money(totalBill),
            _orderItems = orderItems.ToList()
        };
    }

    private Order() { }
}
