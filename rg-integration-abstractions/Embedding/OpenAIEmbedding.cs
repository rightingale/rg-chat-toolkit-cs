using Newtonsoft.Json;
using RestSharp;
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

    public OpenAIEmbedding(string openaiApiKey, string opanaiEndpoint)
    {
        this.openaiApiKey = openaiApiKey;
        this.opanaiEndpoint = opanaiEndpoint;
    }

    public override int EmbeddingSize => VECTOR_SIZE;

    public override async Task<float[]?> GetEmbedding(string text)
    {
        var client = new RestClient(this.opanaiEndpoint);
        var request = new RestRequest();

        request.AddHeader("Authorization", $"Bearer {this.openaiApiKey}");
        request.AddJsonBody(new
        {
            model = MODEL_NAME, // OpenAI model for embeddings
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
                return embeddingResponse.Data[0].Embedding;
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
