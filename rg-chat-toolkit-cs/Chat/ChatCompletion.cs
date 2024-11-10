using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Configuration;
using System.Text.Json;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using rg_chat_toolkit_cs.Cache;
using Azure.AI.OpenAI;
using rg.integrations.epic.Tools.Memory;
using rg_integration_abstractions.Tools;
using rg_integration_abstractions.Tools.Memory;

namespace rg_chat_toolkit_cs.Chat
{
    public class ChatCompletion
    {
        Dictionary<int, string> toolCallIdsByIndex = new();
        Dictionary<int, string> functionNamesByIndex = new();
        Dictionary<int, StringBuilder> functionArgumentBuildersByIndex = new();

        Dictionary<string, ToolBase> toolsByName = new();

        public async IAsyncEnumerable<string> SendChatCompletion(Guid sessionID, string systemPrompt, Message[] messages, bool allowTools)
        {
            var contentBuilder = new StringBuilder();

            // Init azure ai openai client
            var client = new OpenAIClient(ConfigurationHelper.OpenAIApiKey);

            List<Message> messagesList = new List<Message>{ new Message("system", systemPrompt) };
            messagesList.AddRange(messages);

            var options = new ChatCompletionsOptions("gpt-4o", messagesList.ToArray()?.ToChatRequestMessages());

            // Tools:
            FindGroceryItemVectorStoreMemory findGroceryItemTool = new FindGroceryItemVectorStoreMemory();
            findGroceryItemTool.ToolName = "find_grocery_item";//Name, as defined in the DB
            options.Tools.Add(findGroceryItemTool.GetToolDefinition());
            toolsByName.Add(findGroceryItemTool.ToolName, findGroceryItemTool);


            var streamingResponse = await client.GetChatCompletionsStreamingAsync(options);
            if (streamingResponse != null)
            {
                // Await foreach to process each response as it arrives
                await foreach (var response in streamingResponse)
                {
                    if (response.ToolCallUpdate is StreamingFunctionToolCallUpdate functionToolCallUpdate)
                    {
                        // Existing tool-handling logic remains unchanged
                        if (functionToolCallUpdate.Id != null)
                        {
                            toolCallIdsByIndex[functionToolCallUpdate.ToolCallIndex] = functionToolCallUpdate.Id;
                        }
                        if (functionToolCallUpdate.Name != null)
                        {
                            functionNamesByIndex[functionToolCallUpdate.ToolCallIndex] = functionToolCallUpdate.Name;
                        }
                        if (functionToolCallUpdate.ArgumentsUpdate != null)
                        {
                            StringBuilder argumentsBuilder = functionArgumentBuildersByIndex.TryGetValue(
                                functionToolCallUpdate.ToolCallIndex,
                                out StringBuilder existingBuilder) ? existingBuilder : new StringBuilder();
                            argumentsBuilder.Append(functionToolCallUpdate.ArgumentsUpdate);
                            functionArgumentBuildersByIndex[functionToolCallUpdate.ToolCallIndex] = argumentsBuilder;
                        }
                    }

                    if (response.ContentUpdate != null)
                    {
                        contentBuilder.Append(response.ContentUpdate);
                        //Console.WriteLine($"Received token: {response.ContentUpdate}"); // Optional logging

                        // Yield the updated content as a continuous stream
                        yield return contentBuilder.ToString();
                        contentBuilder.Clear(); // Clear after yielding to avoid duplicate streaming
                    }
                }

                if (allowTools)
                {
                    foreach (KeyValuePair<int, string> indexIdPair in toolCallIdsByIndex)
                    {
                        var toolRequestMessage = new Message("tool", String.Empty)
                        {
                            ID = indexIdPair.Value,
                            FunctionName = functionNamesByIndex[indexIdPair.Key],
                            FunctionAguments = functionArgumentBuildersByIndex[indexIdPair.Key].ToString()
                        };

                        var toolResponseMessage = await GetToolCallResponseMessage(toolRequestMessage);

                        bool DO_INTERPRET = true;

                        if (DO_INTERPRET)
                        {
                            // (Optional) interpretation step:
                            List<Message> newMessages = new List<Message>();
                            newMessages.AddRange(messages);
                            newMessages.Add(new Message(role: "user", content: "Considering this information, please concisely answer the user's question:\n\n" + toolResponseMessage.Content));
                            ChatCompletion recursiveChatCompletion = new ChatCompletion();
                            var interpretedResults = recursiveChatCompletion.SendChatCompletion(sessionID, systemPrompt, newMessages.ToArray(), false);

                            //// Add the TOOL request.
                            //var allMessages = handleAddMessage(toolRequestMessage);
                            //// Populate cache:
                            //CacheManager.Populate<List<Message>>(sessionID, allMessages);

                            // Stream the results (tool response).
                            await foreach (var interpretedResult in interpretedResults)
                            {
                                yield return interpretedResult;
                            }
                        }
                        else
                        {
                            yield return toolResponseMessage.Content;
                        }
                    }
                }
            }
        }

        // --- Tool Definitions ---

        static readonly ChatCompletionsFunctionToolDefinition getWeatherTool = new ChatCompletionsFunctionToolDefinition()
        {
            Name = "get_current_weather",
            Description = "Get the current weather in a given location",
            Parameters = BinaryData.FromObjectAsJson(
            new
            {
                Type = "object",
                Properties = new
                {
                    Latitude = new
                    {
                        Type = "number",
                        Description = "Signed Latitude of your location, e.g. 35.1495",
                    },
                    Longitude = new
                    {
                        Type = "number",
                        Description = "Signed Longitude of your location, e.g. -90.0490",
                    }
                },
                Required = new[] { "location" },
            },
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
        };

        /// <summary>
        /// https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?view=azure-dotnet-preview
        /// </summary>
        internal async Task<Message> GetToolCallResponseMessage(Message toolCall)
        {
            if (toolCall == null || string.IsNullOrEmpty(toolCall.FunctionName))
            {
                throw new ArgumentNullException(nameof(toolCall));
            }

            if (toolsByName.TryGetValue(toolCall.FunctionName, out ToolBase? value))
            {
                var tool = value;
                var toolResponse = await tool.GetToolResponse(toolCall);
                return toolResponse;
            }
            else
            {
                // Handle other or unexpected calls
                throw new NotImplementedException();
            }
        }
    }
}
