using Amazon.Runtime.Internal.Util;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_api_cs.Chat;
using rg_integration_abstractions.Embedding;
using System.Runtime.CompilerServices;

namespace rg_chat_toolkit_api_cs.Cache;

public class RGEmbedding
    : OpenAIEmbedding
{
    private static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddUserSecrets<ChatCompletionController>()
        .AddEnvironmentVariables()
        .Build();

    public RGEmbedding(IRGEmbeddingCache embeddingCache)
        : base(embeddingCache, config["openai-apikey"] ?? "", config["openai-endpoint-embeddings"] ?? "")
    {
    }
}
