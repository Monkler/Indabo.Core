namespace Indabo.Core
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DatabaseType
    {
        SQLite = 0,
        MySQL = 1
    }
}
