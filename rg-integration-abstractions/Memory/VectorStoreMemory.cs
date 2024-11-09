using Azure.AI.OpenAI;
using rg.integration.interfaces.qdrant;
using rg_chat_toolkit_cs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Memory;

public class VectorStoreMemory
{
    // --- Tool Definitions ---

    public string ToolName
    {
        get
        {
            return "get_grocery_items";
        }
    }

    public string ToolInterpretationPrompt
    {
        get
        {
            return "Using these SEARCH RESULTS, concisely answer the user's message. Name individual products. Be conversational but concise. Do not say anything about 'search results' or computer talk. Only report unique aisles found across the most relevant SEARCH RESULTS:";
            //return "Using these SEARCH RESULTS, concisely answer the user's message. Always include an upsell opportunity & aisle. Only upsell replac goods. Be exceedingly concise!! Only report unique aisles found across the most relevant SEARCH RESULTS:";
        }
    }

    public static readonly ChatCompletionsFunctionToolDefinition Instance = new ChatCompletionsFunctionToolDefinition()
    {
        Name = "get_grocery_items",
        Description = "Find similar grocery items, including name, producer description, aisle, and price.",
        Parameters = BinaryData.FromObjectAsJson(
            new
            {
                Type = "object",
                Properties = new
                {
                    Text = new
                    {
                        Type = "string",
                        Description = "The grocery item or type of product the user is looking for.",
                    },
                },
                Required = new[] { "Text" },
            },
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
    };

    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?view=azure-dotnet-preview
    /// </summary>
    public async Task<Message> GetToolResponse(Message toolCall)
    {
        if (toolCall?.FunctionName == this.ToolName)
        {
            string unvalidatedArguments = toolCall.FunctionAguments ?? String.Empty;

            var arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(unvalidatedArguments)
                ?? default(Dictionary<string, object>);

            string text = null;
            if (arguments != null
                && arguments.TryGetValue("text", out object objParamText) && objParamText is JsonElement paramText)
            {
                if (paramText.ValueKind == JsonValueKind.String)
                {
                    text = paramText.GetString();
                }
            }
            else
            {
                // Handle missing or invalid latitude or longitude
                throw new ApplicationException("Invalid arguments. Parameter 'text' is required and must be decimal.");
            }

            if (text != null)
            {
                // Lookup in vector store
                var searchResponse = await QdrantHelper.Search(text);
                return new Message(role: "tool", content: this.ToolInterpretationPrompt + "\n" + searchResponse ?? String.Empty);
            }
            else
            {
                return new Message(role: "tool", content: "No results found.");
            }
        }
        else
        {
            // Handle other or unexpected calls
            throw new NotImplementedException();
        }
    }
}
