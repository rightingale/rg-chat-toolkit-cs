using Azure.AI.OpenAI;
using OpenAIApiExample;
using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Speech;
using rg_chat_toolkit_test_harness;
using System.Drawing;
using System.Drawing.Imaging;

namespace TestHarness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TestChatCompletion();
            //TestSynthesizeSpeech();

            //TestMedia_AWS();
            TestMedia();
        }

        public static void TestMedia()
        {
            Task.Run(async () =>
            {
                // Get base64 of image from Resources
                string base64Image = Convert.ToBase64String(Resources.IMG_2204_small);
                var response = ImageChatCompletion.ExplainImage(base64Image);

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
            string base64Image = Convert.ToBase64String(Resources.IMG_2204_small);

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
                var speechResponse = await synthesizer.SynthesizeSpeech("The current time is " + timeAnnouncement);
                // Stream bytes into "c:\temp\speech.mp3"
                using (var fileStream = System.IO.File.Create("c:\\temp\\speech.mp3"))
                {
                    // Stream the results from speechResponse into fileStream
                    await speechResponse.CopyToAsync(fileStream);
                }
            }).Wait();
        }

        public static void TestChatCompletion()
        {
            Task.Run(async () =>
            {
                ChatCompletion chatCompletion = new ChatCompletion();
                var response = chatCompletion.SendChatCompletion("You are a helpful assistant. Be very verbose.",
                    new[] {
                new Message("system", "Respond in ES-419."),
                new Message("assistant", "How can I help?"),
                new Message("user", "Please make a single combined list of presidents of both US and Argentina in alphabetical order."),
                });

                // Await foreach to process each response as it arrives
                await foreach (var str in response)
                {
                    Console.Write(str);
                }
            }).Wait();
        }
    }
}
