using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using rg.integration.interfaces.qdrant;
using rg_chat_toolkit_cs.Chat;
using rg_integration_abstractions.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Tools.Memory;

public abstract class VectorStoreMemory : MemoryBase
{
    public abstract EmbeddingBase EmbeddingModel { get; }

    // ---

    protected abstract QdrantHelper QdrantInstance { get; }


    // ---

    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?view=azure-dotnet-preview
    /// </summary>
    public override async Task<Message> GetToolResponse(Message toolCall)
    {
        if (toolCall?.FunctionName == this.ToolName)
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
                // Lookup in vector store
                var searchResponse = await QdrantInstance.Search(text, TopN);
                return new Message(role: "tool", content: ToolInterpretationPrompt + "\n" + searchResponse ?? string.Empty);
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
