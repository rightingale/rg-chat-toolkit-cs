using Azure.AI.OpenAI;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_integration_abstractions.Tools.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Tools;

public abstract class ToolBase
{
    public string? ToolName { get; set; }
    public string? ToolDescription { get; set; }

    public abstract Task<Message> GetToolResponse(Message toolCall);

    public abstract ChatCompletionsFunctionToolDefinition GetToolDefinition();

    public static ToolBase? Create(string name, string description, string? toolAssembly, string toolTypePath, IRGEmbeddingCache embeddingCache)
    {
        if (toolTypePath.ToLower().EndsWith("FindGroceryItemVectorStoreMemory"))
        {
            var type = Type.GetType(toolTypePath);
            if (type == null)
            {
                throw new Exception($"Could not find tool type: {toolTypePath}");
            }
            var tool = (ToolBase)Activator.CreateInstance(type, embeddingCache);
            return tool;
        }
        else
        {
            throw new Exception($"Unknown memory type: {toolTypePath}");
        }
    }

}
