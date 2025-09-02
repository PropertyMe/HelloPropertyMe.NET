namespace PropertyMeOAuthDemo.Models;

public static class SessionExtensions
{
    public static void SetObject(this ISession session, string key, object value)
    {
        session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
    }

    public static T? GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }
        var deser = System.Text.Json.JsonSerializer.Deserialize<T>(value);
        return deser;
    }
}