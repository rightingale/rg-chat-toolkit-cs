using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using rg_integration_abstractions.Embedding;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Markup;

using Qdrant.Client;
using static Qdrant.Client.Grpc.Conditions;
using Qdrant.Client.Grpc;

namespace rg.integration.interfaces.qdrant;

public class QdrantHelper
{
    protected readonly string qdrantApiKey;
    protected readonly string qdrantEndpoint;
    protected readonly string collectionName;
    public readonly EmbeddingBase embeddingModel;

    public QdrantHelper(string qdrantApiKey, string qdrantEndpoint, string collectionName, EmbeddingBase embeddingModel)
    {
        this.qdrantApiKey = qdrantApiKey;
        this.qdrantEndpoint = qdrantEndpoint;
        this.collectionName = collectionName;
        this.embeddingModel = embeddingModel;
    }

    public async Task<String> Search(string text, int topN, string? filterUserID)
    {
        // timer
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();

        var embedding = this.embeddingModel.GetEmbedding(text).Result;

        // timer
        timer.Stop();
        Console.WriteLine($"Embedding took {timer.ElapsedMilliseconds} ms");

        //// Search qdrant
        //HttpClient httpClient = new();
        //httpClient.DefaultRequestHeaders.Add("api-key", this.qdrantApiKey);
        //var qdrant = new QdrantVectorDbClient(httpClient, embeddingModel.EmbeddingSize, this.qdrantEndpoint);
        //IEnumerable<string>? requiredTags = null;
        //if (filterUserID != null)
        //{
        //    requiredTags = new string[] { filterUserID };
        //}
        //var searchResultsAsync = qdrant.FindNearestInCollectionAsync(collectionName, embedding, 0, topN, false, requiredTags);

        var client = new QdrantClient(address: new Uri(this.qdrantEndpoint), apiKey: this.qdrantApiKey);
        // Filter where producer_token = filterUserID
        var searchResults = await client.SearchAsync(collectionName: this.collectionName, vector: embedding, filter: MatchKeyword("Producer_token", filterUserID), limit: (ulong)topN);
        //var searchResults = await client.SearchAsync(collectionName: this.collectionName, vector: embedding);

        //// Find the mean & stddev & max of the result.Item2 (distance) values
        //double sum = 0;
        //double sum2 = 0;
        //int count = 0;
        //double max = 0;
        //await foreach (var result in searchResultsAsync)
        //{
        //    sum += result.Item2;
        //    sum2 += result.Item2 * result.Item2;
        //    count++;
        //    if (result.Item2 > max)
        //    {
        //        max = result.Item2;
        //    }
        //}
        //double mean = sum / count;
        //double stddev = Math.Sqrt(sum2 / count - mean * mean);

        StringBuilder stringBuilder = new();
        foreach (var result in searchResults)
        {
            //if (result.Item2 > mean || Math.Abs(result.Item2 - mean) < stddev)
            //{
            //    Console.Write("*");
            //}
            //else
            //{
            //    Console.Write(".");
            //}

            if (result.Payload.ContainsKey("json"))
            {
                var currentResult = result.Payload["json"];
                stringBuilder.AppendLine(currentResult.ToString());
            }
            else if (result.Payload.ContainsKey("content"))
            {
                var currentResult = result.Payload["content"];
                stringBuilder.AppendLine(currentResult.ToString());
            }
        }

        return stringBuilder.ToString();
    }

    public async Task Upsert(string key, string value, Dictionary<string, object> attributes)
    {
        HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Add("api-key", this.qdrantApiKey);
        var qdrant = new QdrantVectorDbClient(httpClient, embeddingModel.EmbeddingSize, this.qdrantEndpoint);

        var embedding = await this.embeddingModel.GetEmbedding(value);
        var record = new QdrantVectorRecord[]
        {
            new QdrantVectorRecord(
                key/*point id*/,
                embedding,
                attributes
            )
        };
        await qdrant.UpsertVectorsAsync(this.collectionName, record);

        Console.WriteLine("Wrote id: " + key);
    }
}