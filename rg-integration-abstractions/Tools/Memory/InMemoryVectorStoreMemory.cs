using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using rg.integration.interfaces.qdrant;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_integration_abstractions.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using rg_integration_abstractions.InMemoryVector;

namespace rg_integration_abstractions.Tools.Memory;

public class InMemoryVectorStoreMemory : MemoryBase
{
    IRGEmbeddingCache? embeddingCache;

    private readonly int _topN;
    protected override int TopN => _topN;

    protected readonly EmbeddingBase EMBEDDING;

    private InMemoryVectorStore vectorStore = new InMemoryVectorStore();

    public static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddUserSecrets<InMemoryVectorStoreMemory>()
        .AddEnvironmentVariables()
        .Build();

    protected InMemoryVectorStoreMemory (IRGEmbeddingCache embeddingCache, int topN)
    {
        this.embeddingCache = embeddingCache;
        this._topN = topN;

        var openaiApiKey = config["openai-apikey"];
        var openaiEndpoint = config["openai-endpoint-embeddings"];

        if (String.IsNullOrEmpty(openaiApiKey) || String.IsNullOrEmpty(openaiEndpoint))
        {
            throw new ApplicationException("Error: Invalid configuration. Missing openai-apikey or openai-endpoint-embeddings.");
        }
        this.EMBEDDING = new OpenAIEmbedding(embeddingCache, openaiApiKey, openaiEndpoint);
    }

    public override string ToolInterpretationPrompt
    {
        get
        {
            return "Use the following MEMORY RESULTS to answer the original question:";
        }
    }

    // ---

    public override async Task<Message?> Search(string text)
    {
#if DEBUG
        Console.WriteLine($"Searching for: {text}");
#endif

        // timer
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();

        var searchResponse = await this.Search_Intern(text);

        // timer
        timer.Stop();
        Console.WriteLine($"Search took {timer.ElapsedMilliseconds} ms");

        if (!String.IsNullOrEmpty(searchResponse))
        {
            return new Message(role: Message.ROLE_TOOL, content: ToolInterpretationPrompt + "\n" + searchResponse);
        }
        else
        {
            return null;
        }
    }

    protected async Task<string> Search_Intern(string text)
    {
        var queryVector = await this.EMBEDDING.GetEmbedding(text);
        if (queryVector != null)
        {
            var topResults = this.vectorStore.Search(queryVector, this.TopN);
            // Write json
            Console.WriteLine(JsonSerializer.Serialize(topResults));

            return "";
        }
        else
        {
            return "";
        }
    }

    public override ChatCompletionsFunctionToolDefinition GetToolDefinition()
    {
        return new ChatCompletionsFunctionToolDefinition()
        {
            Name = this.ToolName,
            Description = "Find memory items.",
            Parameters = BinaryData.FromObjectAsJson(
            new
            {
                Type = "object",
                Properties = new
                {
                    Text = new
                    {
                        Type = "string",
                        Description = "The memory item to search for.",
                    },
                },
                Required = new[] { "Text" },
            },
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
        };
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?view=azure-dotnet-preview
    /// </summary>
    public override async Task<Message> GetToolResponse(Message toolCall)
    {
        string unvalidatedArguments = toolCall.FunctionAguments ?? string.Empty;

        var arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(unvalidatedArguments)
            ?? default;

        string text = null;
        if (arguments != null
            && arguments.TryGetValue("text", out object objParamText) && objParamText is JsonElement paramText)
        {
            if (paramText.ValueKind == JsonValueKind.String)
            {
                text = paramText.GetString();
            }
        }

        if (text == null)
        {
            // Handle missing or invalid latitude or longitude
            throw new ApplicationException("Error: Invalid arguments. Parameter 'text' is required.");
        }

        if (text != null)
        {
#if DEBUG
            Console.WriteLine($"GetToolResponse Searching for: {text}");
#endif

            // Lookup in vector store
            var searchResponse = await this.Search_Intern(text);
            return new Message(role: "tool", content: ToolInterpretationPrompt + "\n" + searchResponse ?? string.Empty);
        }
        else
        {
            return new Message(role: "tool", content: "No results found.");
        }
    }
}
