using rg_chat_toolkit_api_cs.Data;
using rg_integration_abstractions.Embedding;
using rg_integration_abstractions.InMemoryVector;
using rg_integration_abstractions.Tools.Memory;

namespace rg_chat_toolkit_api_cs.Chat.Helpers;

public static class PromptCache
{
    private static Dictionary<Guid, InMemoryVectorStore> promptVectorStores = new Dictionary<Guid, InMemoryVectorStore>();

    private static EmbeddingBase EMBEDDING;

    static PromptCache()
    {
        var config = new ConfigurationManager()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddUserSecrets<InMemoryVectorStoreMemory>()
            .AddEnvironmentVariables()
            .Build();

        var openaiApiKey = config["openai-apikey"];
        var openaiEndpoint = config["openai-endpoint-embeddings"];
        if (String.IsNullOrEmpty(openaiApiKey) || String.IsNullOrEmpty(openaiEndpoint))
        {
            throw new ApplicationException("Error: Invalid configuration. Missing openai-apikey or openai-endpoint-embeddings.");
        }
        PromptCache.EMBEDDING = new OpenAIEmbedding(RG.Instance.EmbeddingCache, openaiApiKey, openaiEndpoint);
    }

    public static async Task<float[]?> GetEmbedding (string text)
    {
        return await PromptCache.EMBEDDING.GetEmbedding(text);
    }

    public static async Task<InMemoryVectorStore?> GetOrCreatePromptVectorStore(Guid tenantID)
    {
        if (promptVectorStores.ContainsKey(tenantID))
        {
            promptVectorStores[tenantID] = new InMemoryVectorStore();
            return promptVectorStores[tenantID];
        }
        else
        {
            // Load prompts: Initialize this Tenant:
            var prompts = DataMethods.Prompts_Select(tenantID);
            if (prompts != null)
            {
                var promptVectorStore = new InMemoryVectorStore();

                foreach (var prompt in prompts)
                {
                    var promptItem = new InMemoryVectorStore.KeyValueItem()
                    {
                        Key = prompt.Name,
                        Value = prompt.SystemPrompt,
                        ValueEmbedding = await PromptCache.EMBEDDING.GetEmbedding(prompt.Name)
                    };

                    promptVectorStore.Add(promptItem);
                }

                promptVectorStores[tenantID] = promptVectorStore;
                return promptVectorStore;
            }
            else
            {
                return null;
            }
        }

    }
}
