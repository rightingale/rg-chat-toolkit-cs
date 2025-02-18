using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using rg.integration.interfaces.qdrant;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_integration_abstractions.Embedding;
using rg_integration_abstractions.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Tools.Memory;

public abstract class VectorStoreMemory : MemoryBase
{
    IRGEmbeddingCache? embeddingCache;

    protected VectorStoreMemory(IRGEmbeddingCache embeddingCache)
    {
        this.embeddingCache = embeddingCache;
    }

    public abstract EmbeddingBase EmbeddingModel { get; }

    // ---

    protected abstract QdrantHelper QdrantInstance { get; }


    // ---

    public override async Task Add(string key, string value, string content, Guid? filterUserID)
    {
        if (String.IsNullOrEmpty(value))
        {
            value = content;
        }

        // Deserialize content into generic JSON object
        var attributes = JsonSimpleDeserializer.DeserializeJsonToDictionary(content);

        if (content != null)
        {
            // add content as attribute
            attributes["content"] = content;
        }

        if (filterUserID != null && filterUserID.HasValue)
        {
            attributes["user_id"] = filterUserID.Value.ToString();
        }

        await this.QdrantInstance.Upsert(key, value, attributes);
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                return ConvertJsonToString(element);
            case JsonValueKind.Array:
                return ConvertJsonArray(element);
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                if (element.TryGetInt64(out long longValue))
                    return longValue;
                if (element.TryGetDouble(out double doubleValue))
                    return doubleValue;
                return element.GetDecimal();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Null:
                return null;
            default:
                throw new NotSupportedException($"Unsupported JSON value kind: {element.ValueKind}");
        }
    }

    private static string ConvertJsonToString(JsonElement element)
    {
        return element.GetRawText();
    }

    private static List<object> ConvertJsonArray(JsonElement element)
    {
        var list = new List<object>();
        foreach (var item in element.EnumerateArray())
        {
            list.Add(ConvertJsonElement(item));
        }
        return list;
    }

    public override async Task<Message?> Search(string text, string? userID)
    {
#if DEBUG
        Console.WriteLine($"Searching for: {text}");
#endif

#if DEBUG
        // timer
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
#endif

        var searchResponse = await this.Search_Intern(text, userID);

#if DEBUG
        // timer
        timer.Stop();
        Console.WriteLine($"Search took {timer.ElapsedMilliseconds} ms");
        //Console.WriteLine("Results:\n" + searchResponse + "\n---\n");
#endif

        if (!String.IsNullOrEmpty(searchResponse))
        {
            return new Message(role: Message.ROLE_TOOL, content: ToolInterpretationPrompt + "\n" + searchResponse);
        }
        else
        {
            return null;
        }
    }

    protected async Task<string> Search_Intern(string text, string? userID)
    {
        return await this.QdrantInstance.Search(text, TopN, userID);
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?view=azure-dotnet-preview
    /// </summary>
    public override async Task<Message> GetToolResponse(Message toolCall)
    {
        string unvalidatedArguments = toolCall.FunctionAguments ?? string.Empty;

        var arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(unvalidatedArguments)
            ?? default;

        string text = null;
        if (arguments != null
            && arguments.TryGetValue("text", out object objParamText) && objParamText is JsonElement paramText)
        {
            if (paramText.ValueKind == JsonValueKind.String)
            {
                text = paramText.GetString();
            }
        }

        if (text == null)
        {
            // Handle missing or invalid latitude or longitude
            throw new ApplicationException("Error: Invalid arguments. Parameter 'text' is required.");
        }

        if (text != null)
        {
#if DEBUG
            Console.WriteLine($"GetToolResponse Searching for: {text}");
#endif

            // Lookup in vector store
            var searchResponse = await this.Search_Intern(text, this.Settings?.AuthorizedUserID);
#if DEBUG
            //Console.WriteLine("Results:\n" + searchResponse + "\n---\n");
#endif
            return new Message(role: "tool", content: ToolInterpretationPrompt + "\n" + searchResponse ?? string.Empty);
        }
        else
        {
            return new Message(role: "tool", content: "No results found.");
        }
    }
}
