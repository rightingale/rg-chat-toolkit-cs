using System.Text.Json;

namespace rg_integration_abstractions.Utility;

// Usage:
// string jsonContent = "{ \"name\": \"John\", \"age\": 30, \"isStudent\": false, \"hobbies\": [\"reading\", \"swimming\"] }";
// var result = JsonDeserializer.DeserializeJsonToDictionary(jsonContent);
public static class JsonSimpleDeserializer
{
    public static Dictionary<string, object> DeserializeJsonToDictionary(string content)
    {
        var json = new ReadOnlySpan<byte>(System.Text.Encoding.UTF8.GetBytes(content));
        var reader = new Utf8JsonReader(json);

        Dictionary<string, object> attributes = new Dictionary<string, object>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                continue; // Start of the document
            }
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break; // End of the document
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                if (propertyName != null)
                {
                    reader.Read(); // Move to the value
                    var convertedValue = ConvertJsonElement(ref reader);
                    if (convertedValue != null)
                    {
                        attributes[propertyName] = convertedValue;
                    }
                }
            }
        }

        return attributes;
    }

    private static object? ConvertJsonElement(ref Utf8JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                return ConvertObject(ref reader);
            case JsonTokenType.StartArray:
                return ConvertArray(ref reader);
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out long longValue))
                    return longValue;
                if (reader.TryGetDouble(out double doubleValue))
                    return doubleValue;
                return reader.GetDecimal();
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.Null:
                reader.Skip(); // Move past the null value
                return null;
            default:
                throw new NotSupportedException($"Unsupported JSON token type: {reader.TokenType}");
        }
    }

    private static Dictionary<string, object> ConvertObject(ref Utf8JsonReader reader)
    {
        var dict = new Dictionary<string, object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dict;
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read(); // Move to the value
                dict[propertyName] = ConvertJsonElement(ref reader);
            }
        }
        throw new JsonException("Unexpected end of JSON data while parsing object.");
    }

    private static List<object> ConvertArray(ref Utf8JsonReader reader)
    {
        List<object> list = new List<object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return list;
            }
            list.Add(ConvertJsonElement(ref reader));
        }
        throw new JsonException("Unexpected end of JSON data while parsing array.");
    }

}
