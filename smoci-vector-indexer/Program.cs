using FileHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Markup;

namespace weather_vector_indexer
{
    internal class Program
    {

        const int VECTOR_SIZE = 3072;
        public static readonly IConfigurationRoot config = new ConfigurationManager()
            .AddUserSecrets<Program>()
            .Build();


        static void Main(string[] args)
        {
            // Configure logger service to work with SK


            //PersistEmbeddingsForAllFiles();
            Search();
        }

        static void Search()
        {
            Task.Run(async () =>
            {
                // Generate embedding for this string and copy results to the clipboard.
                var text = "Show me expenses";

                var embedding = GetEmbedding(text).Result;
                //Console.WriteLine(string.Join(",", embedding));
                // Save to clipboard
                var json = JsonConvert.SerializeObject(embedding);
                await TextCopy.ClipboardService.SetTextAsync(json);
                //Console.WriteLine("Embedding copied to clipboard.");

                Console.WriteLine($"Searching for '{text}'...");

                // Search qdrant
                HttpClient httpClient = new();
                var qdrantApiKey = config["qdrant-apikey"];
                httpClient.DefaultRequestHeaders.Add("api-key", qdrantApiKey);
                var qdrantEndpoint = config["qdrant-endpoint"];
                var qdrant = new QdrantVectorDbClient(httpClient, VECTOR_SIZE, qdrantEndpoint);
                var searchResultsAsync = qdrant.FindNearestInCollectionAsync("memory", embedding, 0, 10, false);

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

                await foreach (var result in searchResultsAsync)
                {
                    if (result.Item2 > mean || Math.Abs(result.Item2 - mean) < stddev)
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                    Console.WriteLine(result.Item1.Payload["description"] + "\t" + result.Item2);

                    SmociItem resultItem = new SmociItem();
                    resultItem.Section = result.Item1.Payload["section"]?.ToString();
                    resultItem.Module = result.Item1.Payload["module"]?.ToString();
                    resultItem.Object = result.Item1.Payload["object"]?.ToString();
                    resultItem.Category = result.Item1.Payload["category"]?.ToString();
                    resultItem.Item = result.Item1.Payload["item"]?.ToString();
                    resultItem.Description = result.Item1.Payload["description"]?.ToString();

                    Console.WriteLine(JsonConvert.SerializeObject(resultItem));
                }
                Console.WriteLine("Search results printed.");
            }).Wait();
        }

        static void PersistEmbeddingsForAllFiles()
        {
            // All files in GroceryItems folder
            List<Task> tasks = new();
            // Build a list of async tasks PersistEmbeddings(file);
            var files = Directory.GetFiles("data");
            foreach (var file in files)
            {
                Console.WriteLine("--- --- ---\n" + file + "\n--- --- ---");
                var task = Task.Run(() => PersistEmbeddings(file));
                tasks.Add(task);
                Console.WriteLine("--- --- --- end");
            }
            Task.WaitAll(tasks.ToArray());
        }


        static async Task PersistEmbeddings(string filename)
        {
            //var CountyYears = CountyYear.ReadVectors(FILEPATH);
            //UpsertVectors(CountyYears);

            var lines = System.IO.File.ReadLines(filename);

            // Output tab formatted report; limit the embedding vector to 10 elements; vector may be null (then make it empty array)
            foreach (var currentLine in lines)
            {
                // Calculate embedding for this text
                var embedding = await GetEmbedding(currentLine);

                // Json deserialize to SmociItem
                var smociItem = JsonConvert.DeserializeObject<SmociItem>(currentLine);
                smociItem.EmbeddingVector3072 = embedding;

                // Upsert the embedding vector
                await UpsertTextLine(smociItem);
            }
        }

        public class SmociItem
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("section")]
            public string Section { get; set; }

            [JsonProperty("module")]
            public string Module { get; set; }

            [JsonProperty("object")]
            public string? Object { get; set; }

            [JsonProperty("category")]
            public string? Category { get; set; }

            [JsonProperty("item")]
            public string Item { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonIgnore]
            public float[]? EmbeddingVector3072 { get; set; } // Store embedding as an array of floats
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


        static async Task UpsertTextLine(SmociItem item)
        {
            var loggerFactory = LoggerFactory.Create(builder => 
                builder.AddConsole(options => options.IncludeScopes = true)
            );
            var logger = loggerFactory.CreateLogger(nameof(Program));


            List<Task> tasks = new();

            // Persist vectors:
            HttpClient httpClient = new();
            var qdrantApiKey = config["qdrant-apikey"];
            httpClient.DefaultRequestHeaders.Add("api-key", qdrantApiKey);

            var qdrantEndpoint = config["qdrant-endpoint"];
            var qdrant = new QdrantVectorDbClient(httpClient, VECTOR_SIZE, qdrantEndpoint, loggerFactory);

            var record = new QdrantVectorRecord[]
            {
                new QdrantVectorRecord(
                    item.Id ?? Guid.NewGuid().ToString(),
                    item.EmbeddingVector3072,
                    new Dictionary<string, object>() {
                        ["section"] = item.Section,
                        ["module"] = item.Module,
                        ["object"] = item.Object ?? "",
                        ["category"] = item.Category ?? "",
                        ["item"] = item.Item ?? "",
                        ["description"] = item.Description
                    }
                )
            };
            await qdrant.UpsertVectorsAsync("memory", record);

            Console.WriteLine("Wrote:" + item.Description + " to vector index.");
        }
    }
}
