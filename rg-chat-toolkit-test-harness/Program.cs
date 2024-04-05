using Azure.AI.OpenAI;
using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Speech;

namespace TestHarness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TestChatCompletion();
            TestSynthesizeSpeech();
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
