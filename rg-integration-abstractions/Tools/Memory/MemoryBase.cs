using Azure.AI.OpenAI;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_integration_abstractions.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Tools.Memory;

public abstract class MemoryBase : ToolBase
{
    public abstract string ToolInterpretationPrompt { get; }
    protected abstract int TopN { get; }
    public bool DoPreload { get; set; } = false;

    public abstract Task Add(string key, string value, string content);

    public abstract Task<Message?> Search(string text);

    public static MemoryBase Create(string name, string description, string memoryType, IRGEmbeddingCache embeddingCache)
    {
        if (memoryType.ToLower().StartsWith("vector"))
        {
            var mem = new SmociVectorStoreMemory(embeddingCache);
            mem.ToolName = name;
            return mem;
        }
        else
        {
            throw new Exception($"Unknown memory type: {memoryType}");
        }
    }

}
