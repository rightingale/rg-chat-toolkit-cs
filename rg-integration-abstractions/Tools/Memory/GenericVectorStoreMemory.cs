using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.Qdrant;
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

namespace rg_integration_abstractions.Tools.Memory;

public class GenericVectorStoreMemory : VectorStoreMemory
{
    public const int TOP_N_RESULTS = 10;
    protected override int TopN { get { return TOP_N_RESULTS; } }

    protected readonly QdrantHelper QDRANT;
    protected readonly EmbeddingBase EMBEDDING;

    protected readonly string collectionName;

    public static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddUserSecrets<GenericVectorStoreMemory>()
        .AddEnvironmentVariables()
        .Build();

    public GenericVectorStoreMemory(string collectionName, IRGEmbeddingCache embeddingCache)
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
        this.collectionName = collectionName;

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
            return "Use the following SEARCH RESULTS to answer the user message.\n\nSEARCH RESULTS:";
        }
    }

    // ---

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
        throw new NotImplementedException();
    }

}