using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Newtonsoft.Json;
using rg.integration.interfaces.qdrant;
using rg_integration_abstractions.Embedding;
using rg_integration_abstractions.Tools.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rg.integrations.epic.Tools.Memory;

public class FindGroceryItemVectorStoreMemory : VectorStoreMemory
{

    public const string COLLECTION_NAME = "grocery-embeddings";

    public const int TOP_N_RESULTS = 20;
    protected override int TopN { get { return TOP_N_RESULTS; } }

    protected override string ToolInterpretationPrompt
    {
        get
        {
            return "Using the following GROCERY ITEMS, concisely answer the user's message. Name individual products. Be conversational but concise. Do not say anything about 'search results' or computer talk. Only report unique aisles found across the most relevant GROCERY ITEMS:";
            //return "Using these SEARCH RESULTS, concisely answer the user's message. Always include an upsell opportunity & aisle. Only upsell replac goods. Be exceedingly concise!! Only report unique aisles found across the most relevant SEARCH RESULTS:";
        }
    }

    // ---

    public static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddUserSecrets<FindGroceryItemVectorStoreMemory>()
        .Build();

    public override EmbeddingBase EmbeddingModel
    {
        get
        {
            var openaiApiKey = config["openai-apikey"];
            var openaiEndpoint = config["openai-endpoint-embeddings"];

            if (String.IsNullOrEmpty(openaiApiKey) || String.IsNullOrEmpty(openaiEndpoint))
            {
                throw new ApplicationException("Error: Invalid configuration. Missing openai-apikey or openai-endpoint.");
            }

            return new OpenAIEmbedding(openaiApiKey, openaiEndpoint);
        }
    }

    protected override QdrantHelper QdrantInstance
    {
        get
        {
            var qdrantApiKey = config["qdrant-apikey"];
            var qdrantEndpoint = config["qdrant-endpoint"];
            var collectionName = COLLECTION_NAME;

            if (String.IsNullOrEmpty(qdrantApiKey) || String.IsNullOrEmpty(qdrantEndpoint) || String.IsNullOrEmpty(collectionName))
            {
                throw new ApplicationException("Error: Invalid configuration. Missing qdrant-apikey, qdrant-endpoint, or qdrant-collection-name.");
            }

            return new QdrantHelper(qdrantApiKey, qdrantEndpoint, collectionName, this.EmbeddingModel);
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





//static async Task<Item[]> ReadArrayElementsFromJsonFile(string FILEPATH)
//{
//    var jsonContent = System.IO.File.ReadAllText(FILEPATH);
//    var items = JsonConvert.DeserializeObject<List<Item>>(jsonContent);
//    // Step 2: Generate embeddings for each item
//    foreach (var item in items)
//    {
//        //// Normalize the JSON encoding of this instance and get the embedding
//        //var itemJson = JsonConvert.SerializeObject(item);
//        item.EmbeddingVector3072 = await GetEmbedding(item.ItemName);

//        // Upsert vector store for each item
//        UpsertEmbeddingVectors(new Item[] { item });
//    }

//    return items.ToArray();
//}


//static void UpsertEmbeddingVectors(Item[] records)
//{

//    List<Task> tasks = new();

//    // Persist vectors:
//    HttpClient httpClient = new();
//    var qdrantApiKey = config["qdrant-apikey"];
//    httpClient.DefaultRequestHeaders.Add("api-key", qdrantApiKey);
//    var qdrantEndpoint = config["qdrant-endpoint"];
//    var qdrant = new QdrantVectorDbClient(httpClient, VECTOR_SIZE, qdrantEndpoint);
//    foreach (var rec in records)
//    {
//        //tasks.Add(
//        Task.Run(async () =>
//        {
//            /*
//                JSON format:
//             {
//                "id": "b-001-101",
//                "department": "bakery",
//                "item_name": "Sara Lee Classic White Bread",
//                "aisle": "1",
//                "bin": "AA",
//                "bin_description": "top shelf",
//                "price": 3.49
//              }
//            */

//            string json = JsonConvert.SerializeObject(rec);

//            await qdrant.UpsertVectorsAsync("grocery-embeddings", new QdrantVectorRecord[]
//            {
//                    new QdrantVectorRecord(
//                        (Guid.NewGuid().ToString()),
//                        rec.EmbeddingVector3072.ToQdrantVector(VECTOR_SIZE),
//                        new Dictionary<string, object>() {
//                            ["Text"] = rec.ItemName,
//                            ["ItemName"] = rec.ItemName,
//                            ["Department"] = rec.Department,
//                            ["Aisle"] = rec.Aisle,
//                            ["Bin"] = rec.Bin,
//                            ["BinDescription"] = rec.BinDescription,
//                            ["Price"] = rec.Price,
//                            ["Data"] = json
//                        }
//                    )
//            });

//            Console.WriteLine("Wrote:" + rec.ItemName + "/" + rec.Aisle);

//            //if (tasks.Count > 10)
//            //{
//            //    Task.WaitAll(tasks.ToArray());
//            //    tasks = new List<Task>();
//            //}
//            //}));
//        }).Wait();
//    }

//    //Task.WaitAll(tasks.ToArray());
//}


/*



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

*/