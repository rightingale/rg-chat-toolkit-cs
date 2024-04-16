using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using Anthropic.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using rg_chat_toolkit_cs.Configuration;

namespace rg_chat_toolkit_cs.Chat
{
    public class ClaudeChatCompletion
    {
        public async static Task ChatCompletion(byte[] imageBytes, string userPrompt)
        {
            // Convert the byte array to a base64 string
            string base64String = Convert.ToBase64String(imageBytes);

            APIAuthentication auth = new APIAuthentication(ConfigurationHelper.ClaudeApiKey);
            var client = new AnthropicClient(auth);
            var messages = new List<Anthropic.SDK.Messaging.Message>();
            messages.Add(new Anthropic.SDK.Messaging.Message()
            {
                Role = RoleType.User,
                Content = new dynamic[]
                {
        new ImageContent()
        {
            Source = new ImageSource()
            {
                MediaType = "image/jpeg",
                Data = base64String
            }
        },
        new TextContent()
        {
            Text = userPrompt
        }
                }
            });
            var parameters = new MessageParameters()
            {
                Messages = messages,
                MaxTokens = 512,
                Model = AnthropicModels.Claude3Opus,
                Stream = true,
                Temperature = 1.0m,
            };
            var outputs = new List<MessageResponse>();
            await foreach (var res in client.Messages.StreamClaudeMessageAsync(parameters))
            {
                if (res.Delta != null)
                {
                    Console.Write(res.Delta.Text);
                }

                outputs.Add(res);
            }
            Console.WriteLine(string.Empty);
            Console.WriteLine($@"Used Tokens - Input:{outputs.First().StreamStartMessage.Usage.InputTokens}.
                            Output: {outputs.Last().Usage.OutputTokens}");
        }
    }
}
