namespace eCommerce.OrdersService.Infrastructure.Caching.Keys;

public static class UserCacheKeys
{
    public const string InfoKeyPrefix = "user-info:";
    public const string ExistsKeyPrefix = "user-exists:";

    public static string CreateInfoKey(Guid userId) => $"{InfoKeyPrefix}{userId}"; 
    public static string CreateExistsKey(Guid userId) => $"{ExistsKeyPrefix}{userId}";

    public static Guid ExtractIdFromInfoKey(string infoKey)
        => Guid.Parse(infoKey.AsSpan(InfoKeyPrefix.Length));

    public static Guid ExtractIdFromExistsKey(string existsKey)
        => Guid.Parse(existsKey.AsSpan(ExistsKeyPrefix.Length));
}
