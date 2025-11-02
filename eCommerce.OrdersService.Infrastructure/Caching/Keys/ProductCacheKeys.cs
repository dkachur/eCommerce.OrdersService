namespace eCommerce.OrdersService.Infrastructure.Caching.Keys;

public static class ProductCacheKeys
{
    public const string InfoKeyPrefix = "product-info:";
    public const string ExistsKeyPrefix = "product-exists:";

    public static string CreateInfoKey(Guid productId) => $"{InfoKeyPrefix}{productId}";
    public static string CreateExistsKey(Guid productId) => $"{ExistsKeyPrefix}{productId}";

    public static Guid ExtractIdFromInfoKey(string infoKey)
        => Guid.Parse(infoKey.AsSpan(InfoKeyPrefix.Length));

    public static Guid ExtractIdFromExistsKey(string existsKey)
        => Guid.Parse(existsKey.AsSpan(ExistsKeyPrefix.Length));
}
