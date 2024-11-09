using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Markup;

namespace rg.integration.interfaces.qdrant;

internal class QdrantHelper
{

    const int VECTOR_SIZE = 3072;
    public static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddUserSecrets<QdrantHelper>()
        .Build();

    public static async Task<String> Search(string text)
    {
        var embedding = GetEmbedding(text).Result;

        Console.WriteLine($"Searching for '{text}'...");

        // Search qdrant
        HttpClient httpClient = new();
        var qdrantApiKey = config["qdrant-apikey"];
        httpClient.DefaultRequestHeaders.Add("api-key", qdrantApiKey);
        var qdrantEndpoint = config["qdrant-endpoint"];
        var qdrant = new QdrantVectorDbClient(httpClient, VECTOR_SIZE, qdrantEndpoint);
        var searchResultsAsync = qdrant.FindNearestInCollectionAsync("grocery-embeddings", embedding, 0, 10, false);

        // Find the mean & stddev & max of the result.Item2 (distance) values
        double sum = 0;
        double sum2 = 0;
        int count = 0;
        double max = 0;
        await foreach (var result in searchResultsAsync)
        {
            sum += result.Item2;
            sum2 += result.Item2 * result.Item2;
            count++;
            if (result.Item2 > max)
            {
                max = result.Item2;
            }
        }
        double mean = sum / count;
        double stddev = Math.Sqrt(sum2 / count - mean * mean);

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
            //Console.WriteLine(result.Item1.Payload["ItemName"] + "\t" + result.Item2);

            var currentResult = result.Item1.Payload["Data"];

            stringBuilder.AppendLine(currentResult.ToString());
        }

        return stringBuilder.ToString();
    }

    //static void PersistEmbeddings()
    //{
    //    string FILEPATH = @"input.json";

    //    //var CountyYears = CountyYear.ReadVectors(FILEPATH);
    //    //UpsertVectors(CountyYears);

    //    Task.Run(async () =>
    //    {
    //        var arrayItems = await ReadArrayElementsFromJsonFile(FILEPATH);

    //        // Output tab formatted report; limit the embedding vector to 10 elements; vector may be null (then make it empty array)
    //        foreach (var item in arrayItems)
    //        {
    //            var vectorTruncated = item.EmbeddingVector3072?.Take(10) ?? new float[0];
    //            Console.WriteLine($"{item.Id}\t{item.Department}\t{item.ItemName}\t{item.Aisle}\t{item.Bin}\t{item.BinDescription}\t{item.Price}\t{string.Join(",", vectorTruncated)}");
    //        }

    //        //UpsertEmbeddingVectors(arrayItems);
    //    }).Wait();
    //}

    public class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("item_name")]
        public string ItemName { get; set; }

        [JsonProperty("aisle")]
        public string Aisle { get; set; }

        [JsonProperty("bin")]
        public string Bin { get; set; }

        [JsonProperty("bin_description")]
        public string BinDescription { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("embedding_vector_3072")]
        public float[]? EmbeddingVector3072 { get; set; } // Store embedding as an array of floats
    }

    static async Task<Item[]> ReadArrayElementsFromJsonFile(string FILEPATH)
    {
        var jsonContent = System.IO.File.ReadAllText(FILEPATH);
        var items = JsonConvert.DeserializeObject<List<Item>>(jsonContent);
        // Step 2: Generate embeddings for each item
        foreach (var item in items)
        {
            //// Normalize the JSON encoding of this instance and get the embedding
            //var itemJson = JsonConvert.SerializeObject(item);
            item.EmbeddingVector3072 = await GetEmbedding(item.ItemName);

            // Upsert vector store for each item
            UpsertEmbeddingVectors(new Item[] { item });
        }

        return items.ToArray();
    }

    private static async Task<float[]> GetEmbedding(string text)
    {
        string ApiEndpoint = "https://api.openai.com/v1/embeddings";
        var client = new RestClient(ApiEndpoint);  // New RestClient
        var request = new RestRequest();  // New RestRequest

        string apiKey = config["openai-apikey"];
        request.AddHeader("Authorization", $"Bearer {apiKey}");
        request.AddJsonBody(new
        {
            model = "text-embedding-3-large", // OpenAI model for embeddings
            input = text
        });

        // Send request and get response
        var response = await client.PostAsync(request);  // Use PostAsync for POST requests in RestSharp v107+

        if (!response.IsSuccessful)
        {
            Console.WriteLine($"Error: Could not generate embeddings: {response.ErrorMessage} {response.Content}");
            return null;
        }

        // Deserialize the response
        var embeddingResponse = JsonConvert.DeserializeObject<EmbeddingResponse>(response.Content);
        return embeddingResponse.Data[0].Embedding;
    }


    public class EmbeddingResponse
    {
        public List<EmbeddingData> Data { get; set; }
    }

    public class EmbeddingData
    {
        public float[] Embedding { get; set; }
    }


    static void UpsertEmbeddingVectors(Item[] records)
    {

        List<Task> tasks = new();

        // Persist vectors:
        HttpClient httpClient = new();
        var qdrantApiKey = config["qdrant-apikey"];
        httpClient.DefaultRequestHeaders.Add("api-key", qdrantApiKey);
        var qdrantEndpoint = config["qdrant-endpoint"];
        var qdrant = new QdrantVectorDbClient(httpClient, VECTOR_SIZE, qdrantEndpoint);
        foreach (var rec in records)
        {
            //tasks.Add(
            Task.Run(async () =>
            {
                /*
                    JSON format:
                 {
                    "id": "b-001-101",
                    "department": "bakery",
                    "item_name": "Sara Lee Classic White Bread",
                    "aisle": "1",
                    "bin": "AA",
                    "bin_description": "top shelf",
                    "price": 3.49
                  }
                */

                string json = JsonConvert.SerializeObject(rec);

                await qdrant.UpsertVectorsAsync("grocery-embeddings", new QdrantVectorRecord[]
                {
                    new QdrantVectorRecord(
                        (Guid.NewGuid().ToString()),
                        rec.EmbeddingVector3072.ToQdrantVector(VECTOR_SIZE),
                        new Dictionary<string, object>() {
                            ["Text"] = rec.ItemName,
                            ["ItemName"] = rec.ItemName,
                            ["Department"] = rec.Department,
                            ["Aisle"] = rec.Aisle,
                            ["Bin"] = rec.Bin,
                            ["BinDescription"] = rec.BinDescription,
                            ["Price"] = rec.Price,
                            ["Data"] = json
                        }
                    )
                });

                Console.WriteLine("Wrote:" + rec.ItemName + "/" + rec.Aisle);

                //if (tasks.Count > 10)
                //{
                //    Task.WaitAll(tasks.ToArray());
                //    tasks = new List<Task>();
                //}
                //}));
            }).Wait();
        }

        //Task.WaitAll(tasks.ToArray());
    }

    //static void UpsertVectors(Dictionary<string, Dictionary<int, CountyYear>> CountyYears)
    //{
    //    var config = new ConfigurationManager()
    //        .AddUserSecrets<Program>()
    //        .Build();

    //    // Persist vectors:
    //    HttpClient httpClient = new();
    //    httpClient.DefaultRequestHeaders.Add("api-key", config["qdrant-apikey"]);
    //    var qdrant = new QdrantVectorDbClient(httpClient, 53, config["qdrant-endpoint"]);
    //    foreach (var fips in CountyYears.Keys)
    //    {
    //        foreach (var year in CountyYears[fips].Keys)
    //        {
    //            CountyYear currentCountyYear = CountyYears[fips][year];

    //            // Populate index
    //            Task.Run(async () =>
    //            {
    //                Console.WriteLine("Writing:" + currentCountyYear.fips + "/" + currentCountyYear.year + "...");

    //                const int VECTOR_SIZE = 53;

    //                await qdrant.UpsertVectorsAsync("gridmet-CountyYears-precipitation_amount", new QdrantVectorRecord[]
    //                {
    //                    new QdrantVectorRecord(
    //                    currentCountyYear.id.ToString(),
    //                    currentCountyYear.precipitation_amount.ToQdrantVector(VECTOR_SIZE),
    //                    new Dictionary<string, object>() { ["Text"] = (string)(currentCountyYear.fips + "/" + currentCountyYear.year) }
    //                    )
    //                });

    //                await qdrant.UpsertVectorsAsync("gridmet-CountyYears-maximum_air_temperature", new QdrantVectorRecord[]
    //                    {
    //                    new QdrantVectorRecord(
    //                    currentCountyYear.id.ToString(),
    //                    currentCountyYear.maximum_air_temperature.ToQdrantVector(VECTOR_SIZE),
    //                    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                    )
    //                });

    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-minimum_air_temperature", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.minimum_air_temperature.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-maximum_relative_humidity", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.maximum_relative_humidity.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-minimum_relative_humidity", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.minimum_relative_humidity.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-specific_humidity", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.specific_humidity.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-wind_speed", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.wind_speed.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-wind_from_direction", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.wind_from_direction.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-surface_downwelling_shortwave_flux_in_air", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.surface_downwelling_shortwave_flux_in_air.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-reference_evapotranspiration_grass", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.reference_evapotranspiration_grass.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-energy_release_component_g", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.energy_release_component_g.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-burning_index_g", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.burning_index_g.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-dead_fuel_moisture_100hr", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.dead_fuel_moisture_100hr.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-dead_fuel_moisture_1000hr", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.dead_fuel_moisture_1000hr.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-reference_evapotranspiration_alfalfa", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.reference_evapotranspiration_alfalfa.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});
    //                //await qdrant.UpsertVectorsAsync("gridmet-CountyYears-mean_vapor_pressure_deficit", new QdrantVectorRecord[]
    //                //{
    //                //    new QdrantVectorRecord(
    //                //    currentCountyYear.id.ToString(),
    //                //    currentCountyYear.mean_vapor_pressure_deficit.ToQdrantVector(),
    //                //    new Dictionary<string, object>() { ["Text"] = currentCountyYear.fips + "/" + currentCountyYear.year }
    //                //    )
    //                //});

    //                Console.WriteLine("Wrote:" + currentCountyYear.fips + "/" + currentCountyYear.year);
    //            }).Wait();
    //        }
    //    }
    //}
}