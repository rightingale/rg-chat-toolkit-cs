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

namespace rg.integration.interfaces.qdrant;

public class QdrantHelper
{
    protected readonly string qdrantApiKey;
    protected readonly string qdrantEndpoint;
    protected readonly string collectionName;
    public readonly EmbeddingBase embeddingModel;

    public QdrantHelper (string qdrantApiKey, string qdrantEndpoint, string collectionName, EmbeddingBase embeddingModel)
    {
        this.qdrantApiKey = qdrantApiKey;
        this.qdrantEndpoint = qdrantEndpoint;
        this.collectionName = collectionName;
        this.embeddingModel = embeddingModel;
    }

    public async Task<String> Search(string text, int topN)
    {
        var embedding = this.embeddingModel.GetEmbedding(text).Result;

        // Search qdrant
        HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Add("api-key", this.qdrantApiKey);
        var qdrant = new QdrantVectorDbClient(httpClient, embeddingModel.EmbeddingSize, this.qdrantEndpoint);
        var searchResultsAsync = qdrant.FindNearestInCollectionAsync(collectionName, embedding, 0, topN, false);

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
        await foreach (var result in searchResultsAsync)
        {
            //if (result.Item2 > mean || Math.Abs(result.Item2 - mean) < stddev)
            //{
            //    Console.Write("*");
            //}
            //else
            //{
            //    Console.Write(".");
            //}

            Console.WriteLine(result.Item1.Payload["Text"] + "\t" + result.Item2);

            var currentResult = result.Item1.Payload["Text"];

            stringBuilder.AppendLine(currentResult.ToString());
        }

        return stringBuilder.ToString();
    }
}