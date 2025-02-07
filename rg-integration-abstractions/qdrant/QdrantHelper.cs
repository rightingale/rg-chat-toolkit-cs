using Microsoft.Extensions.Configuration;
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
using System.Linq;
using System;
using Google.Protobuf.Collections;

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
#if DEBUG
        // timer
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
#endif

        var embedding = this.embeddingModel.GetEmbedding(text).Result;

#if DEBUG
        // timer
        timer.Stop();
        Console.WriteLine($"Embedding took {timer.ElapsedMilliseconds} ms");
#endif

        var client = new QdrantClient(address: new Uri(this.qdrantEndpoint), apiKey: this.qdrantApiKey);
        IReadOnlyList<ScoredPoint> searchResults;
        if (String.IsNullOrEmpty(this.embeddingModel.VectorName))
        {
            // Filter where producer_token = filterUserID
            searchResults = await client.SearchAsync(collectionName: this.collectionName,
                vector: embedding,
                filter: MatchKeyword("Producer_token", filterUserID),
                limit: (ulong)topN);
        }
        else
        {
            // Filter where producer_token = filterUserID
            searchResults = await client.SearchAsync(collectionName: this.collectionName,
                vector: embedding,
                vectorName: this.embeddingModel.VectorName,
                filter: MatchKeyword("Producer_token", filterUserID),
                limit: (ulong)topN);
        }
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
        var client = new QdrantClient(address: new Uri(this.qdrantEndpoint), apiKey: this.qdrantApiKey);

        var embedding = await this.embeddingModel.GetEmbedding(value);

        Vectors vectors = new Vectors();

        // Convert attributes from Dictionary<string, object> to MapField<string, Value>
        var attributesMap = new MapField<string, Value>();
        foreach (var attribute in attributes)
        {
            if (attribute.Value is string)
            {
                attributesMap.Add(attribute.Key, new Value { StringValue = attribute.Value.ToString() });
            }
            else if (attribute.Value is int)
            {
                attributesMap.Add(attribute.Key, new Value { IntegerValue = (int)attribute.Value });
            }
            else if (attribute.Value is double)
            {
                attributesMap.Add(attribute.Key, new Value { DoubleValue = (double)attribute.Value });
            }
            else if (attribute.Value is bool)
            {
                attributesMap.Add(attribute.Key, new Value { BoolValue = (bool)attribute.Value });
            }
            else
            {
                throw new Exception($"Unsupported attribute type: {attribute.Value.GetType()}");
            }
        }

        // convert embedding from float[] to QDrant client IReadOnlyList<PointStruct>
        var point = new PointStruct
        {
            Id = new PointId() { Uuid = Guid.NewGuid().ToString() },
            Vectors = new Dictionary<string, Vector>
            {
                [this.embeddingModel.VectorName] = new Vector(embedding)
            }
        };
        // For each dictionary, add point.Payload.Add(key,value);
        foreach (var attribute in attributesMap)
        {
            point.Payload.Add(attribute.Key, attribute.Value);
        }

        var points = new List<PointStruct> { point };
        await client.UpsertAsync(collectionName: this.collectionName, points: points);

#if DEBUG
        Console.WriteLine("Wrote id: " + key);
#endif
    }
}