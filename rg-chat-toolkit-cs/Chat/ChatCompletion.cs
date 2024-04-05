using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Configuration;

namespace rg_chat_toolkit_cs.Chat
{
    public class ChatCompletion
    {
        public async IAsyncEnumerable<string> SendChatCompletion(string systemPrompt, Message[] messages)
        {
            List<Message> messagesList = new List<Message>();
            messagesList.Add(new Message("system", systemPrompt));
            messagesList.AddRange(messages);
            messages = messagesList.ToArray();

            // Init azure ai openai client
            var client = new OpenAIClient(ConfigurationHelper.OpenAIApiKey);
            var streamingResponse = client.GetChatCompletionsStreamingAsync(
                new ChatCompletionsOptions("gpt-4", messages?.ToChatRequestMessages()) { }
            );
            if (streamingResponse != null)
            {
                // Await foreach to process each response as it arrives
                await foreach (var response in streamingResponse.Result)
                {
                    yield return response.ContentUpdate;
                }
            }
        }
    }
}
