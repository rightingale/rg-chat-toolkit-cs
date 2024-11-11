using rg_chat_toolkit_cs.Cache;

namespace rg_chat_toolkit_api_cs;

public class RG
{
    public static RG? Instance { get; set; } = null;

    public readonly IRGEmbeddingCache EmbeddingCache;

    public RG (IRGEmbeddingCache embeddingCache)
    {
        this.EmbeddingCache = embeddingCache;
    }
}
