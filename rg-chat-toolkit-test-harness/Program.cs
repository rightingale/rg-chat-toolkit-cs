using Azure;
using Azure.AI.OpenAI;
using OpenAIApiExample;
using rg_chat_toolkit_api_cs.Chat;
using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Speech;
using rg_chat_toolkit_test_harness;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace TestHarness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TestChatCompletion();

            //TestToolFunction();

            //TestToolFunctionGrocery();
            TestToolFunctionGroceryApi();

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

        public static void TestClaude()
        {
            const string USER_PROMPT_IDENTIFIER = @"You are a customer service agent at a pharmacy.
Identify the medicine in the photo: color, shape, imprint
Enable search capabilities.  Include search link: ""https://www.drugs.com/imprints.php?imprint=L015&color=12&shape=24""
Substitute correct imprint, color, and shape querystring parameters, according to drugs.com pill identification API documentation.
Reply only in JSON: 
{ color: ""white"", imprint: ""X555.3"", shape: ""square"", search_url: ""https://www.drugs.com/imprints.php?imprint=XXX34&color=99&shape=99"" }
Code only.";

            Task.Run(async () =>
            {
                await ClaudeChatCompletion.ChatCompletion(Resources_Extensions.L484_Bytes, USER_PROMPT_IDENTIFIER);
            }).Wait();
        }

        public static void TestMedia()
        {
            Guid sessionID = Guid.NewGuid();

            Task.Run(async () =>
            {
                // Get base64 of image from Resources
                string base64Image = Convert.ToBase64String(Resources_Extensions.L484_Bytes);
                var response = ImageChatCompletion.ExplainImage(sessionID, base64Image);

                // Await foreach to process each response as it arrives
                await foreach (var str in response)
                {
                    Console.Write(str);
                }
            }).Wait();
        }

        public static void TestMedia_AWS()
        {
            // Get base64 of image from Resources
            string base64Image = Convert.ToBase64String(Resources_Extensions.L484_Bytes);

            Task.Run(async () =>
            {
                var imageAnalysisService = new ImageAnalysisService();
                var result = await imageAnalysisService.AnalyzeImage(base64Image);
                Console.WriteLine("Labels:" + string.Join(", ", result.labels.Select(l => l.Name)));
                Console.WriteLine("Text:" + string.Join(", ", result.textDetections.Select(t => t.DetectedText)));
            }).Wait();
        }

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
                ChatCompletion chatCompletion = new ChatCompletion();
                var response = chatCompletion.SendChatCompletion(sessionID, "You are a helpful assistant. Be very verbose.",
                    new[] {
                new Message("system", "Respond in ES-419."),
                new Message("assistant", "How can I help?"),
                new Message("user", "Please make a single combined list of presidents of both US and Argentina in alphabetical order. Consider only family surname. But count distinct people as separate entries. Group by letter. Finally, which letter has the most entries?"),
                }, true/*allowTools*/);

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

                ChatCompletion chatCompletion = new ChatCompletion();
                var response = chatCompletion.SendChatCompletion(sessionID, "You are a helpful assistant. Please be exceedingly concise (!).",
                    messages.ToArray(),
                    true /*allowTools*/);

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

            Task.Run(async () =>
            {
                ChatCompletionController api = new ChatCompletionController();
                var responseAsync = api.SendChatCompletion(new ChatCompletionRequest()
                {
                    TenantID = tenantID,
                    SessionID = sessionID,
                    PromptName = "instore_experience_helper",
                    RequestMessageContent = "Where is cream cheese?"
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

                ChatCompletion chatCompletion = new ChatCompletion();
                var response = chatCompletion.SendChatCompletion(sessionID, "You are a helpful assistant. Be concise.",
                    messages.ToArray(),
                    true /*allowTools*/);

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
