using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using rg_chat_toolkit_api_cs.Data.Models;
using rg_integration_abstractions.InMemoryVector;

namespace rg_chat_toolkit_api_cs.Data;


public static partial class DataMethods
{
    public static readonly TimeSpan SLIDING_EXPIRATION_PROMPT_ENSURE_EMBEDDING = TimeSpan.FromHours(24);
    public static readonly TimeSpan ABSOLUTE_EXPIRATION_PROMPT_ENSURE_EMBEDDING = TimeSpan.FromDays(30);

    public static async Task<InMemoryVectorStore> Prompt_EnsureEmbedding(Guid tenantID)
    {
        // Check cache
        string cacheKey = $"Prompt_EnsureEmbedding_{tenantID}";
        if (cache.TryGetValue(cacheKey, out InMemoryVectorStore? memoryVectorStore))
        {
            Console.WriteLine($"Prompt_EnsureEmbedding: Cache hit for {cacheKey}");
            return memoryVectorStore ?? new InMemoryVectorStore();
        }
        else
        {
            var newEmbeddingValue = await Prompt_EnsureEmbedding_Intern(tenantID);
            cache.Set(cacheKey, newEmbeddingValue, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = SLIDING_EXPIRATION_PROMPT_ENSURE_EMBEDDING,
                AbsoluteExpiration = DateTime.Now + ABSOLUTE_EXPIRATION_PROMPT_ENSURE_EMBEDDING,
                Size = 1
            });
            return newEmbeddingValue;
        }
    }

    private async static Task<InMemoryVectorStore> Prompt_EnsureEmbedding_Intern(Guid tenantID)
    {
        var db = RGDatabaseContextFactory.Instance.CreateDbContext();
        var prompts = db.Prompts
            .Include(p => p.PromptUtterances)
            .Where(p => p.TenantId == tenantID);

        var vectorStore = new InMemoryVectorStore();
        foreach (var currentPrompt in prompts)
        {
            var embeddingValue = await RG.Instance.EmbeddingModel.GetEmbedding(currentPrompt.Description);
            if (embeddingValue != null)
            {
                vectorStore.Add(currentPrompt.Name, currentPrompt.Description, embeddingValue);
            }

            foreach (var currentUtterance in currentPrompt.PromptUtterances)
            {
                embeddingValue = await RG.Instance.EmbeddingModel.GetEmbedding(currentUtterance.Utterance);
                if (embeddingValue != null)
                {
                    vectorStore.Add(currentPrompt.Name, currentUtterance.Utterance, embeddingValue);
                }
            }
        }

        return vectorStore;
    }
}