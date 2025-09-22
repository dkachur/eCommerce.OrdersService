namespace eCommerce.OrdersService.Domain.ValueObjects;

public record Money
{
    public decimal Value { get; set; }

    public Money(decimal value)
    {
        if (value < 0) 
            throw new ArgumentException("Money cannot be negative", nameof(value));

        Value = Math.Round(value, 2);
    }

    public static implicit operator decimal(Money money) => money.Value;
}
