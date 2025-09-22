namespace eCommerce.OrdersService.Domain.ValueObjects;

public record Quantity
{
    public int Value { get; set; }

    public Quantity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(value));

        Value = value;
    }

    public static implicit operator int(Quantity quantity) => quantity.Value;
}
