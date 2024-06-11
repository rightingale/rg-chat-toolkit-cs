using Azure.AI.OpenAI;
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

namespace rg_chat_toolkit_cs.Chat
{
    public class ChatCompletion
    {
        Dictionary<int, string> toolCallIdsByIndex = new();
        Dictionary<int, string> functionNamesByIndex = new();
        Dictionary<int, StringBuilder> functionArgumentBuildersByIndex = new();
        StringBuilder contentBuilder = new();

        public async IAsyncEnumerable<string> SendChatCompletion(Guid sessionID, string systemPrompt, Message[] messages, AddMessageDelegate handleAddMessage)
        {
            await foreach (var result in SendChatCompletion(sessionID, systemPrompt, messages, handleAddMessage, true))
            {
                yield return result;
            }
        }

        public async IAsyncEnumerable<string> SendChatCompletion(Guid sessionID, string systemPrompt, Message[] messages, AddMessageDelegate handleAddMessage, bool allowTools)
        {
            contentBuilder.Clear();

            // Init azure ai openai client
            var client = new OpenAIClient(ConfigurationHelper.OpenAIApiKey);

            List<Message> messagesList = new List<Message>();
            messagesList.Add(new Message("system", systemPrompt));
            messagesList.AddRange(messages);

            var options = new ChatCompletionsOptions("gpt-4o", messagesList.ToArray()?.ToChatRequestMessages()) { };
            // Add tools:
            options.Tools.Add(getWeatherTool);

            var streamingResponse = client.GetChatCompletionsStreamingAsync(options);
            if (streamingResponse != null)
            {
                // Await foreach to process each response as it arrives
                await foreach (var response in streamingResponse.Result)
                {
                    if (response.ToolCallUpdate is StreamingFunctionToolCallUpdate functionToolCallUpdate)
                    {
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
                            StringBuilder argumentsBuilder
                                = functionArgumentBuildersByIndex.TryGetValue(
                                    functionToolCallUpdate.ToolCallIndex,
                                    out StringBuilder existingBuilder) ? existingBuilder : new StringBuilder();
                            argumentsBuilder.Append(functionToolCallUpdate.ArgumentsUpdate);
                            functionArgumentBuildersByIndex[functionToolCallUpdate.ToolCallIndex] = argumentsBuilder;
                        }
                    }

                    if (response.ContentUpdate != null)
                    {
                        yield return response.ContentUpdate;
                    }
                }

                foreach (KeyValuePair<int, string> indexIdPair in toolCallIdsByIndex)
                {
                    var toolRequestMessage = new Message("tool", String.Empty)
                    {
                        ID = indexIdPair.Value,
                        FunctionName = functionNamesByIndex[indexIdPair.Key],
                        FunctionAguments = functionArgumentBuildersByIndex[indexIdPair.Key].ToString()
                    };

                    if (allowTools)
                    {
                        var toolResponseMessage = await GetToolCallResponseMessage(toolRequestMessage);

                        bool DO_INTERPRET = false;

                        if (DO_INTERPRET)
                        {
                            // (Optional) interpretation step:
                            List<Message> newMessages = new List<Message>();
                            newMessages.AddRange(messages);
                            newMessages.Add(new Message(role: "user", content: "Considering this information, please concisely answer the user's question:\n\n" + toolResponseMessage.Content));
                            ChatCompletion recursiveChatCompletion = new ChatCompletion();
                            var interpretedResults = recursiveChatCompletion.SendChatCompletion(sessionID, systemPrompt, newMessages.ToArray(), handleAddMessage, false);

                            // Add the TOOL request.
                            var allMessages = handleAddMessage(toolRequestMessage);
                            // Populate cache:
                            CacheManager.Populate<List<Message>>(sessionID, allMessages);


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
                    else
                    {
                        yield return toolRequestMessage.Content;
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
            if (toolCall?.FunctionName == getWeatherTool.Name)
            {
                string unvalidatedArguments = toolCall.FunctionAguments;

                // Deserialize the JSON in unvalidatedArguments; get the latitude and longitude
                // {"latitude": 35.1495, "longitude": -90.049, "unit": "celsius"}
                var arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(unvalidatedArguments);
                decimal latitude;
                decimal longitude;


                if (arguments.TryGetValue("latitude", out object latitudeObj) && latitudeObj is JsonElement latitudeElement &&
                        arguments.TryGetValue("longitude", out object longitudeObj) && longitudeObj is JsonElement longitudeElement)
                {
                    latitude = latitudeElement.GetDecimal();
                    longitude = longitudeElement.GetDecimal();
                }
                else
                {
                    // Handle missing or invalid latitude or longitude
                    throw new ArgumentException("Invalid arguments. 'latitude' and 'longitude' are required and must be decimal.");
                }


                // Lookup data via https://api.open-meteo.com/v1/forecast?latitude=35.1495&longitude=-90.0490&current=temperature_2m
                const string URL_TEMPLATE = "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&current=temperature_2m";

                //decimal latitude = 35.1495m;
                //decimal longitude = -90.0490m;
                string url = String.Format(URL_TEMPLATE, latitude, longitude);
                // Get whole URL contents as string: with HttpClient class
                string htmlContent = await new System.Net.Http.HttpClient().GetStringAsync(url);
                return new Message(role: "tool", content: htmlContent);
            }
            else
            {
                // Handle other or unexpected calls
                throw new NotImplementedException();
            }
        }
    }
}
