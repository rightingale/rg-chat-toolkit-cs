using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using rg.integration.interfaces.qdrant;
using rg_chat_toolkit_cs.Cache;
using rg_integration_abstractions.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Tools.Memory;

public class SmociVectorStoreMemory : VectorStoreMemory
{
    public const string COLLECTION_NAME = "memory";

    public const int TOP_N_RESULTS = 3;
    protected override int TopN { get { return TOP_N_RESULTS; } }

    protected readonly QdrantHelper QDRANT;
    protected readonly EmbeddingBase EMBEDDING;

    public SmociVectorStoreMemory(IRGEmbeddingCache embeddingCache)
        : base(embeddingCache)
    {
        var openaiApiKey = config["openai-apikey"];
        var openaiEndpoint = config["openai-endpoint-embeddings"];

        if (String.IsNullOrEmpty(openaiApiKey) || String.IsNullOrEmpty(openaiEndpoint))
        {
            throw new ApplicationException("Error: Invalid configuration. Missing openai-apikey or openai-endpoint-embeddings.");
        }
        this.EMBEDDING = new OpenAIEmbedding(embeddingCache, openaiApiKey, openaiEndpoint);

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
            return @"
Return results in JSON format using this schema:
{
    ""section"": ""tilley"",
    ""module"": ""module name"",
    ""object"": ""object name"",
    ""container"": ""container"",
    ""item"": ""item name""
}
JSON Rules:
- ""section"" is a required field.
- ""module"" is a required field.
- ""container"" is nullable.
- ""item"" is nullable.
- Do not wrap the json codes in JSON markers.
- Do not substitute conjecture for missing props.
- Use only the props provided in the MEMORY DATA.

Use the following MEMORY DATA to answer questions:
";
        }
    }

    // ---

    public static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddUserSecrets<SmociVectorStoreMemory>()
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
            Description = "Search for items for this prompt.",
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
