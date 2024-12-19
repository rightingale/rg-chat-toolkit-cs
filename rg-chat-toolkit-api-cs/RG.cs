using rg_chat_toolkit_cs.Cache;
using rg_integration_abstractions.Embedding;

namespace rg_chat_toolkit_api_cs;

public class RG
{
    public static RG? Instance { get; set; } = null;

    public readonly IRGEmbeddingCache EmbeddingCache;
    public readonly EmbeddingBase EmbeddingModel;

    public RG (IRGEmbeddingCache embeddingCache, EmbeddingBase embeddingModel)
    {
        this.EmbeddingCache = embeddingCache;
        this.EmbeddingModel = embeddingModel;
    }
}
