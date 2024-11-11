using Newtonsoft.Json;
using RestSharp;
using rg_chat_toolkit_cs.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Embedding;

public class OpenAIEmbedding : EmbeddingBase
{
    protected readonly string opanaiEndpoint;
    protected readonly string openaiApiKey;

    public const int VECTOR_SIZE = 3072;
    public const string MODEL_NAME = "text-embedding-3-large";

    protected readonly IRGEmbeddingCache embeddingCache;

    public OpenAIEmbedding(IRGEmbeddingCache embeddingCache, string openaiApiKey, string opanaiEndpoint)
        : base(embeddingCache)
    {
        this.embeddingCache = embeddingCache;
        this.openaiApiKey = openaiApiKey;
        this.opanaiEndpoint = opanaiEndpoint;
    }

    public override int EmbeddingSize => VECTOR_SIZE;

    public override async Task<float[]?> GetEmbedding(string text)
    {
        if (embeddingCache != null)
        {
            var cacheKey = embeddingCache.GetEmbeddingCacheKey(text);
            var cacheValue = await embeddingCache.Get(cacheKey);
            if (cacheValue != null)
            {
                Console.WriteLine($"*** *** ***Cache hit for {text}");
                return JsonConvert.DeserializeObject<float[]>(cacheValue);
            }
        }

        // Use external API

        var client = new RestClient(this.opanaiEndpoint);
        var request = new RestRequest();

        request.AddHeader("Authorization", $"Bearer {this.openaiApiKey}");
        request.AddJsonBody(new
        {
            model = MODEL_NAME,
            input = text
        });

        // Send request and get response
        var response = await client.PostAsync(request);

        if (!response.IsSuccessful)
        {
            Console.WriteLine($"Error: Could not generate embeddings: {response.ErrorMessage} {response.Content}");
            return null;
        }

        if (response.Content != null)
        {
            // Deserialize the response
            var embeddingResponse = JsonConvert.DeserializeObject<EmbeddingResponse>(response.Content);
            if (embeddingResponse != null && embeddingResponse.Data != null && embeddingResponse.Data.Count() > 0)
            {
                var embedding = embeddingResponse.Data[0].Embedding;

                // Persist cache 
                if (embeddingCache != null)
                {
                    var cacheKey = embeddingCache.GetEmbeddingCacheKey(text);
                    await embeddingCache.Put(cacheKey, JsonConvert.SerializeObject(embedding));
                }

                return embedding;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

}
