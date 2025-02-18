using Newtonsoft.Json;
using RestSharp;
using rg_chat_toolkit_cs.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Embedding;

public class OpenAI3LargeEmbedding : OpenAIEmbedding
{
    public const int VECTOR_SIZE = 3072;
    public const string MODEL_NAME = "text-embedding-3-large";

    public OpenAI3LargeEmbedding(IRGEmbeddingCache embeddingCache, string openaiApiKey, string opanaiEndpoint)
        : base(embeddingCache, openaiApiKey, opanaiEndpoint)
    {
    }

    public override int EmbeddingSize => VECTOR_SIZE;

    public override string EmbeddingModel => MODEL_NAME;

}
