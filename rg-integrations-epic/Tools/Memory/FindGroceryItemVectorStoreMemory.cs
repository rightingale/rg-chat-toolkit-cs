using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using rg.integration.interfaces.qdrant;
using rg_chat_toolkit_cs.Cache;
using rg_integration_abstractions.Embedding;
using rg_integration_abstractions.Tools.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rg.integrations.epic.Tools.Memory;

/*

    Sample JSON for relevant ChatCompletion request:


{
    "TenantID": "787923AB-0D9F-EF11-ACED-021FE1D77A3B",
    "PromptName": "instore_experience_helper",
    "SessionID": "00000000-0000-0000-0000-000000000000",
    "AccessKey": "00000000-0000-0000-0000-000000000000",
    "RequestMessageContent": "Do you have cleaning products?",
    "DoStreamResponse": false
}


*/

public class FindGroceryItemVectorStoreMemory : VectorStoreMemory
{

    public const string COLLECTION_NAME = "grocery-embeddings";

    public const int TOP_N_RESULTS = 10;
    protected override int TopN { get { return TOP_N_RESULTS; } }

    protected readonly QdrantHelper QDRANT;
    protected readonly EmbeddingBase EMBEDDING;

    public FindGroceryItemVectorStoreMemory(IRGEmbeddingCache embeddingCache)
        : base(embeddingCache)
    {
        var openaiApiKey = config["openai-apikey"];
        var openaiEndpoint = config["openai-endpoint-embeddings"];

        if (String.IsNullOrEmpty(openaiApiKey) || String.IsNullOrEmpty(openaiEndpoint))
        {
            throw new ApplicationException("Error: Invalid configuration. Missing openai-apikey or openai-endpoint-embeddings.");
        }
        this.EMBEDDING = new OpenAI3LargeEmbedding(embeddingCache, openaiApiKey, openaiEndpoint);

        var qdrantApiKey = config["qdrant-apikey"];
        var qdrantEndpoint = config["qdrant-endpoint"];
        var collectionName = COLLECTION_NAME;

        if (String.IsNullOrEmpty(qdrantApiKey) || String.IsNullOrEmpty(qdrantEndpoint) || String.IsNullOrEmpty(collectionName))
        {
            throw new ApplicationException("Error: Invalid configuration. Missing qdrant-apikey, qdrant-endpoint, or qdrant-collection-name.");
        }

        this.QDRANT = new QdrantHelper(qdrantApiKey, qdrantEndpoint, collectionName, this.EMBEDDING);

    }

    // ---

    public override string ToolInterpretationPrompt
    {
        get
        {
            return "Using the following GROCERY ITEMS, concisely answer the user's message. Name individual products. Expand abbreviations. Be conversational but concise. Do not say anything about 'search results' or computer talk. Only report unique aisles found across the most relevant GROCERY ITEMS:";
            //return "Using these SEARCH RESULTS, concisely answer the user's message. Always include an upsell opportunity & aisle. Only upsell replac goods. Be exceedingly concise!! Only report unique aisles found across the most relevant SEARCH RESULTS:";
        }
    }

    // ---

    public static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddUserSecrets<FindGroceryItemVectorStoreMemory>()
        .AddEnvironmentVariables()
        .Build();

    public override EmbeddingBase EmbeddingModel
    {
        get
        {
            return EMBEDDING;
        }
    }

    protected override QdrantHelper QdrantInstance
    {
        get
        {
            return QDRANT;
        }
    }

    public override ChatCompletionsFunctionToolDefinition GetToolDefinition()
    {
        return new ChatCompletionsFunctionToolDefinition()
        {
            Name = this.ToolName,
            Description = "Find similar grocery items, including name, producer description, aisle, and price.",
            Parameters = BinaryData.FromObjectAsJson(
            new
            {
                Type = "object",
                Properties = new
                {
                    Text = new
                    {
                        Type = "string",
                        Description = "The grocery item or type of product the user is looking for.",
                    },
                },
                Required = new[] { "Text" },
            },
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
        };
    }

}
