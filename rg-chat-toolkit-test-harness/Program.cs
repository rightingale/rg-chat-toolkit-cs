using Amazon.Polly.Model;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OpenAIApiExample;
using rg_chat_toolkit_api_cs;
using rg_chat_toolkit_api_cs.Chat;
using rg_chat_toolkit_api_cs.Chat.Helpers;
using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_api_cs.Speech;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Speech;
using rg_chat_toolkit_test_harness;
using rg_integration_abstractions.Embedding;
using rg_integration_abstractions.InMemoryVector;
using rg_integration_abstractions.Tools.Memory;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TestHarness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var embeddingCache = new RGCache();


            var config = new ConfigurationManager()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<InMemoryVectorStoreMemory>()
                .AddEnvironmentVariables()
                .Build();
            var openaiApiKey = config["openai-apikey"];
            var openaiEndpoint = config["openai-endpoint-embeddings"];
            if (String.IsNullOrEmpty(openaiApiKey) || String.IsNullOrEmpty(openaiEndpoint))
            {
                throw new ApplicationException("Error: Invalid configuration. Missing openai-apikey or openai-endpoint-embeddings.");
            }
            var EMBEDDING = new OpenAIEmbedding(embeddingCache, openaiApiKey, openaiEndpoint);

            RG.Instance = new RG(embeddingCache, EMBEDDING);


            //TestToolFunctionGroceryApi();

            Task.Run(async () =>
            {
                await Test_MemorySearch();
                //await Test_MemoryUpdate();
                //await Test_PromptChooser();
                //await TestInMemoryVectorStore_Server();
                //await TestInMemoryVectorStore();
            }).Wait();

            //TestChatCompletion();

            //TestToolFunction();

            //TestTilleyNavigation();

            //TestToolFunctionGroceryApi();
            //TestSpeechApi();

            //TestToolFunctionGroceryApi();
            //TestToolFunctionGroceryApi();
            //TestToolFunctionGroceryApi();
            //TestToolFunctionGroceryApi();
            //TestSynthesizeSpeech();

            //// Run TestToolFunction 10 times:
            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine("--- --- ---");
            //}

            //TestSynthesizeSpeech();

            //TestMedia_AWS();
            //TestMedia();

            //TestClaude();

            //TestWebScraper();
        }

        public static async Task Test_MemorySearch()
        {
            MemoryController ws = new MemoryController(RG.Instance.EmbeddingCache);
            var result = await ws.MemoryItem_Search(new MemoryItemSearchRequest()
            {
                TenantID = Guid.Parse("902544DA-67E6-4FA8-A346-D1FAA8B27A08"),
                MemoryName = "ProducerFarms",
                SearchText = "Where do I grow cotton?"
            });

            // Serialize to json
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        public static async Task Test_MemoryUpdate()
        {
            MemoryController ws = new MemoryController(RG.Instance.EmbeddingCache);
            await ws.MemoryItem_Update(new MemoryItemUpdateRequest()
            {
                TenantID = Guid.Parse("902544DA-67E6-4FA8-A346-D1FAA8B27A08"),
                MemoryItemID = Guid.NewGuid().ToString(),
                MemoryName = "Budget",
                Value = "Test",
                Json = "{\"producer_token\": \"test-test-test-testing-test\", \"county_name\":\"Lubbock\", \"reported_acreage\":\"1000\"}"
            });
        }

        public static async Task Test_PromptChooser()
        {
            var searchQuery = "what is my total assets on balance sheet";
            var prompt = await PromptChooser.ChoosePrompt(Guid.Parse("902544DA-67E6-4FA8-A346-D1FAA8B27A08"), searchQuery);

            Console.WriteLine("Search query: " + searchQuery);
            Console.WriteLine("Prompt: " + prompt);
        }


        public static async Task TestInMemoryVectorStore_Server()
        {
            var memoryStore = await DataMethods.Prompt_EnsureEmbedding(Guid.Parse("902544DA-67E6-4FA8-A346-D1FAA8B27A08"));
            var searchQuery = "show the page where I analyze my balance sheet and financials";

            // Get a 2-gram bigram, splitting on word boundaries:
            var searchQueryHalf = searchQuery.Split(" ").Take(2).Aggregate((a, b) => a + " " + b);
            //// Get a 3-gram
            //var searchQueryThird = searchQuery.Split(" ").Take(3).Aggregate((a, b) => a + " " + b);

            var searchEmbedding = await RG.Instance.EmbeddingModel.GetEmbedding(searchQuery);
            var searchEmbeddingHalf = await RG.Instance.EmbeddingModel.GetEmbedding(searchQueryHalf);
            //var searchEmbeddingThird = await RG.Instance.EmbeddingModel.GetEmbedding(searchQueryThird);

            var searchResponse = memoryStore.Search(searchEmbedding, 10);
            var searchResponseHalf = memoryStore.Search(searchEmbeddingHalf, 10);
            //var searchResponseThird = memoryStore.Search(searchEmbeddingThird, 10);

            Console.WriteLine("Search results:");
            foreach (var currentResult in searchResponse)
            {
                // Find the "half string" distance and add:
                var currentHalfResult = searchResponseHalf.Where(halfKey => halfKey.Item.ID == currentResult.Item.ID)
                    .SingleOrDefault();
                //var currentThirdResult = searchResponseThird.Where(thirdKey => currentResult.Item.Key.Contains(thirdKey.Item.Key))
                //    .SingleOrDefault();

                // Half key for CURRENTRESULT is CURRENTHALFRESULT - output keys
                Console.WriteLine("Combining: " + currentResult.Item.Key + "\t" + currentResult.Distance
                    + "\t" + currentHalfResult?.Distance);
                //+ "\t" + currentThirdResult.Distance);

                var weightedDistance = currentResult.Distance * searchQuery.Length
                        + (currentHalfResult?.Distance ?? 0) * (currentHalfResult?.Item.Value.Length ?? 0);
                var weightedAvgDistance = weightedDistance / (searchQuery.Length + (currentHalfResult?.Item.Value.Length ?? 0));

                currentResult.Distance = weightedAvgDistance;

                //if (currentHalfResult != null)
                //{
                //    currentResult.Distance += currentHalfResult.Distance;
                //    //+ currentThirdResult.Distance;
                //}
            }

            // Sort desc by distance
            searchResponse = searchResponse.OrderByDescending(x => x.Distance).ToArray();
            foreach (var currentResult in searchResponse)
            {
                Console.WriteLine(currentResult.Item.Key + "\t" + currentResult.Distance + "\t" + currentResult.Item.Value.Substring(0, currentResult.Item.Value.Length > 20 ? 20 : currentResult.Item.Value.Length));
            }

            // Take top 1 by distance desc
            var topResult = searchResponse.OrderByDescending(x => x.Distance).Take(1).SingleOrDefault();
            string? promptName = null;
            if (topResult != null)
            {
                promptName = topResult.Item.Key;
            }

            Console.WriteLine("Top result: " + promptName);
        }

        public static async Task TestInMemoryVectorStore()
        {
            var embeddingCache = RG.Instance.EmbeddingCache;

            var config = new ConfigurationManager()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<InMemoryVectorStoreMemory>()
                .AddEnvironmentVariables()
                .Build();

            var openaiApiKey = config["openai-apikey"];
            var openaiEndpoint = config["openai-endpoint-embeddings"];
            if (String.IsNullOrEmpty(openaiApiKey) || String.IsNullOrEmpty(openaiEndpoint))
            {
                throw new ApplicationException("Error: Invalid configuration. Missing openai-apikey or openai-endpoint-embeddings.");
            }
            var EMBEDDING = new OpenAIEmbedding(embeddingCache, openaiApiKey, openaiEndpoint);

            var memoryItems = new List<InMemoryVectorStore.KeyValueItem>();
            memoryItems.Add(new InMemoryVectorStore.KeyValueItem()
            {
                Key = "financials_answers",
                Value = "Analyze or report on data including Balance Sheet, Income Statement, Schedule F tax form.",
                ValueEmbedding = []
            });
            memoryItems.Add(new InMemoryVectorStore.KeyValueItem()
            {
                Key = "tilley_navigation",
                Value = "Navigate: 'Go To' or 'Show Me' a section including Farm Vault, Budget, ARC/PLC, Financials, Insurance, Marketing.",
                ValueEmbedding = []
            });
            memoryItems.Add(new InMemoryVectorStore.KeyValueItem()
            {
                Key = "logout",
                Value = "Quit: 'Exit' or 'Close' the application.",
                ValueEmbedding = []
            });

            InMemoryVectorStore vectorStore = new InMemoryVectorStore();
            foreach (var memoryItem in memoryItems)
            {
                memoryItem.ValueEmbedding = await EMBEDDING.GetEmbedding(memoryItem.Value);
                // Output JSON of the embedding
                Console.WriteLine("Embedding: " + JsonConvert.SerializeObject(memoryItem.ValueEmbedding).Length);
                vectorStore.Add(memoryItem);
            }

            // Find match:
            string searchQuery = "what is my total long term assets";
            var searchEmbedding = await EMBEDDING.GetEmbedding(searchQuery);
            var searchResponse = vectorStore.Search(searchEmbedding, 10);

            Console.WriteLine("Search results:");
            foreach (var currentResult in searchResponse)
            {
                Console.WriteLine(currentResult.Item.Key + "\t" + currentResult.Distance);
            }
        }

        public static void TestTilleyNavigation()
        {
            Guid tenantID = Guid.Parse("902544DA-67E6-4FA8-A346-D1FAA8B27A08");
            Guid sessionID = Guid.Parse("00000000-0000-0000-0000-000000000000");
            Guid accessKey = Guid.Parse("00000000-0000-0000-0000-000000000000");

            Task.Run(async () =>
            {
                ChatCompletionController api = new(new RGCache());
                var responseAsync = api.SendChatCompletion(new ChatCompletionRequest()
                {
                    TenantID = tenantID,
                    SessionID = sessionID,
                    AccessKey = accessKey,
                    PromptName = "tilley_navigation",
                    RequestMessageContent = "customer support email",
                    //Persona = "chef_female",
                    //LanguageCode = "en"
                });

                StringBuilder stringBuilder = new StringBuilder();
                // Await foreach to process each response as it arrives
                await foreach (var str in responseAsync)
                {
                    stringBuilder.Append(str);
                }

                Console.WriteLine(stringBuilder.ToString());
            }).Wait();
        }

//        public static void TestClaude()
//        {
//            const string USER_PROMPT_IDENTIFIER = @"You are a customer service agent at a pharmacy.
//Identify the medicine in the photo: color, shape, imprint
//Enable search capabilities.  Include search link: ""https://www.drugs.com/imprints.php?imprint=L015&color=12&shape=24""
//Substitute correct imprint, color, and shape querystring parameters, according to drugs.com pill identification API documentation.
//Reply only in JSON: 
//{ color: ""white"", imprint: ""X555.3"", shape: ""square"", search_url: ""https://www.drugs.com/imprints.php?imprint=XXX34&color=99&shape=99"" }
//Code only.";

//            Task.Run(async () =>
//            {
//                await ClaudeChatCompletion.ChatCompletion(Resources_Extensions.L484_Bytes, USER_PROMPT_IDENTIFIER);
//            }).Wait();
//        }

        //public static void TestMedia()
        //{
        //    Guid sessionID = Guid.NewGuid();

        //    Task.Run(async () =>
        //    {
        //        // Get base64 of image from Resources
        //        string base64Image = Convert.ToBase64String(Resources_Extensions.L484_Bytes);
        //        var response = ImageChatCompletion.ExplainImage(sessionID, base64Image);

        //        // Await foreach to process each response as it arrives
        //        await foreach (var str in response)
        //        {
        //            Console.Write(str);
        //        }
        //    }).Wait();
        //}

        //public static void TestMedia_AWS()
        //{
        //    // Get base64 of image from Resources
        //    string base64Image = Convert.ToBase64String(Resources_Extensions.L484_Bytes);

        //    Task.Run(async () =>
        //    {
        //        var imageAnalysisService = new ImageAnalysisService();
        //        var result = await imageAnalysisService.AnalyzeImage(base64Image);
        //        Console.WriteLine("Labels:" + string.Join(", ", result.labels.Select(l => l.Name)));
        //        Console.WriteLine("Text:" + string.Join(", ", result.textDetections.Select(t => t.DetectedText)));
        //    }).Wait();
        //}

        public static void TestSynthesizeSpeech()
        {
            Task.Run(async () =>
            {
                Synthesizer synthesizer = new Synthesizer();
                // Format current date time, e.g., "4:04 pm on Monday, January 1st, 2035"
                string timeAnnouncement = System.DateTime.Now.ToString("h:mm tt 'on' dddd, MMMM d, yyyy");

                //var speechResponse = await synthesizer.SynthesizeSpeech("The current time is " + timeAnnouncement + ". This is just a test!", "es-US");
                var speechResponse = await synthesizer.SynthesizeSpeech("La hora actual es " + timeAnnouncement + ". ¡Esto es solo una prueba!", "es-MX");

                // Stream bytes into "c:\temp\speech.mp3"
                using (var fileStream = System.IO.File.Create("c:\\temp\\speech.mp3"))
                {
                    // Stream the results from speechResponse into fileStream
                    await speechResponse.CopyToAsync(fileStream);
                }

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    UseShellExecute = false,
                    Arguments = "c:\\temp\\speech.mp3"
                };
                System.Diagnostics.Process.Start(psi);
            }).Wait();
        }

        public static void TestChatCompletion()
        {
            Guid sessionID = Guid.NewGuid();

            Task.Run(async () =>
            {
                ChatCompletion chatCompletion = new ChatCompletion(new RGCache());
                var response = chatCompletion.SendChatCompletion(sessionID, "You are a helpful assistant. Be very verbose.",
                    new[] {
                new Message("system", "Respond in ES-419."),
                new Message("assistant", "How can I help?"),
                new Message("user", "Please make a single combined list of presidents of both US and Argentina in alphabetical order. Consider only family surname. But count distinct people as separate entries. Group by letter. Finally, which letter has the most entries?"),
                }, true/*allowTools*/, null, null, null, null);

                // Await foreach to process each response as it arrives
                await foreach (var str in response)
                {
                    Console.Write(str);
                }
            }).Wait();
        }

        public static void TestWebScraper()
        {
            Task.Run(async () =>
            {
                await WebScraper.Parse("https://www.drugs.com/imprints.php?imprint=L015&color=12&shape=24");

                await WebScraper.Parse("https://www.drugs.com/imprints.php?imprint=L484&color=0&shape=5");
            }).Wait();
        }

        public static void TestToolFunction()
        {
            Guid sessionID = Guid.NewGuid();

            Task.Run(async () =>
            {
                var messages = new[] {
                        //new Message("system", "Respond in ES-419."),
                        new Message("assistant", "How can I help?"),
                        new Message("user", "What is the current weather in Paris? Please give Celius, F, and Kelvin."),
                }.ToList();

                ChatCompletion chatCompletion = new ChatCompletion(new RGCache());
                var response = chatCompletion.SendChatCompletion(sessionID, "You are a helpful assistant. Please be exceedingly concise (!).",
                    messages.ToArray(),
                    true /*allowTools*/, null, null, null, null);

                // Await foreach to process each response as it arrives
                await foreach (var str in response)
                {
                    Console.Write(str);
                }
            }).Wait();
        }

        public static async void TestToolFunctionGroceryApi()
        {
            Guid tenantID = Guid.Parse("787923AB-0D9F-EF11-ACED-021FE1D77A3B");
            Guid sessionID = Guid.Parse("00000000-0000-0000-0000-000000000000");
            Guid accessKey = Guid.Parse("00000000-0000-0000-0000-000000000000");

            Task.Run(async () =>
            {
                ChatCompletionController api = new(new RGCache());
                var responseAsync = api.SendChatCompletion(new ChatCompletionRequest()
                {
                    TenantID = tenantID,
                    SessionID = sessionID,
                    AccessKey = accessKey,
                    //PromptName = "instore_experience_helper",
                    RequestMessageContent = "Do you have large ice cream?",
                    //Persona = "chef_female",
                    LanguageCode = "es"
                });

                StringBuilder stringBuilder = new StringBuilder();
                // Await foreach to process each response as it arrives
                await foreach (var str in responseAsync)
                {
                    stringBuilder.Append(str);
                }

                Console.WriteLine(stringBuilder.ToString());
            }).Wait();
        }

        public static async void TestSpeechApi()
        {
            Guid tenantID = Guid.Parse("787923AB-0D9F-EF11-ACED-021FE1D77A3B");
            Guid sessionID = Guid.Parse("00000000-0000-0000-0000-000000000000");
            Guid accessKey = Guid.Parse("00000000-0000-0000-0000-000000000000");

            try
            {
                Task.Run(async () =>
                {
                    SynthesizerController api = new();
                    var speechResponseAsync = await api.SynthesizeSpeech(new rg_chat_toolkit_api_cs.Speech.SynthesizeSpeechRequest()
                    {
                        TenantID = tenantID,
                        SessionID = sessionID,
                        AccessKey = accessKey,
                        DoStreamResponse = false
                    });

                    // Upcast to FileStreamResult
                    FileStreamResult speechResponse = (FileStreamResult)speechResponseAsync;
                    var responseStream = speechResponse.FileStream;

                    // Stream bytes into "c:\temp\speech.mp3"
                    using (var fileStream = System.IO.File.Create("c:\\temp\\speech.mp3"))
                    {
                        // Stream the results from speechResponse into fileStream
                        await responseStream.CopyToAsync(fileStream);
                    }

                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        UseShellExecute = false,
                        Arguments = "c:\\temp\\speech.mp3"
                    };
                    System.Diagnostics.Process.Start(psi);
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void TestToolFunctionGrocery()
        {
            Guid sessionID = Guid.NewGuid();

            Task.Run(async () =>
            {
                var messages = new[] {
                        //new Message("system", "Respond in ES-419."),
                        new Message("assistant", "How can I help?"),
                        new Message("user", "Sargento cheese"),
                }.ToList();

                ChatCompletion chatCompletion = new ChatCompletion(new RGCache());
                var response = chatCompletion.SendChatCompletion(sessionID, "You are a helpful assistant. Be concise.",
                    messages.ToArray(),
                    true /*allowTools*/, null, null, null, null);

                StringBuilder stringBuilder = new StringBuilder();
                // Await foreach to process each response as it arrives
                await foreach (var str in response)
                {
                    stringBuilder.Append(str);
                }

                Console.WriteLine(stringBuilder.ToString());
            }).Wait();
        }
    }
}
