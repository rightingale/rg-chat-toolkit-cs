using Amazon.Runtime.Internal.Util;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_api_cs.Chat;
using rg_integration_abstractions.Embedding;
using System.Runtime.CompilerServices;

namespace rg_chat_toolkit_api_cs.Cache;

public class RGEmbedding
{
    private static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddUserSecrets<ChatCompletionController>()
        .AddEnvironmentVariables()
        .Build();

    public static EmbeddingBase CreateDefault(IRGEmbeddingCache embeddingCache)
    {
        return RGEmbedding.Create<OpenAI3LargeEmbedding>(embeddingCache);
    }

    public static EmbeddingBase Create<T>(IRGEmbeddingCache embeddingCache)
        where T : EmbeddingBase
    {
        // If modelname is typeof OpenAIEmbedding type. without magic string
        if (typeof(T) == typeof(OpenAIEmbedding))
        {
            var openaiApiKey = config["openai-apikey"];
            var openaiEndpoint = config["openai-endpoint-embeddings"];

            if (String.IsNullOrEmpty(openaiApiKey) || String.IsNullOrEmpty(openaiEndpoint))
            {
                throw new ApplicationException("Error: Invalid configuration. Missing openai-apikey or openai-endpoint-embeddings.");
            }
            // use Activator
            var instance = (EmbeddingBase?)Activator.CreateInstance(typeof(T), embeddingCache, openaiApiKey, openaiEndpoint);
            if (instance == null)
            {
                throw new ApplicationException("Error: Unable to create embedding instance [" + typeof(T).Name + "].");
            }
            return instance;
        }
        else
        {
            // Unknown embedding type
            throw new ApplicationException("Error: Unknown embedding type.");
        }
    }
}
