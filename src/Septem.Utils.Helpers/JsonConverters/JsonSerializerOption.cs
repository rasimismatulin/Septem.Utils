using System.Text.Json;

namespace Septem.Utils.Helpers.JsonConverters
{
    public static class JsonSerializerOption
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            Converters = { new DateOnlyConverter(), new TimeOnlyConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
