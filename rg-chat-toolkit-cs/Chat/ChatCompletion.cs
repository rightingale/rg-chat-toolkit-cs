using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using rg_chat_toolkit_cs.Configuration;
using Azure.AI.OpenAI;
using rg.integrations.epic.Tools.Memory;
using rg_integration_abstractions.Tools;
using rg_integration_abstractions.Tools.Memory;
using rg_chat_toolkit_cs.Cache;
using Azure.Core;
using rg_chat_toolkit_cs.Speech;

namespace rg_chat_toolkit_cs.Chat;

public class ChatCompletion
{
    protected readonly IRGEmbeddingCache EmbeddingCache;

    // Use reponse formats like HTTP content types
    public const string RESPONSE_FORMAT_JSON = "application/json";
    public const string RESPONSE_FORMAT_TEXT = "text/plain";

    public ChatCompletion(IRGEmbeddingCache embeddingCache)
    {
        this.EmbeddingCache = embeddingCache;
    }

    public async IAsyncEnumerable<string> SendChatCompletion(Guid sessionID, string systemPrompt, Message[] messages, bool allowTools, string? voiceName, string? languageCode, string? responseFormat, List<MemoryBase> memories, List<ToolBase> tools)
    {
        Dictionary<int, string> toolCallIdsByIndex = new();
        Dictionary<int, string> functionNamesByIndex = new();
        Dictionary<int, StringBuilder> functionArgumentBuildersByIndex = new();
        Dictionary<string, ToolBase> toolsByName = new();

        var contentBuilder = new StringBuilder();

        // Init azure ai openai client
        var client = new OpenAIClient(ConfigurationHelper.OpenAIApiKey);

        List<Message> messagesList = new List<Message> { new Message(Message.ROLE_SYSTEM, systemPrompt) };
        messagesList.AddRange(messages);

        // Language:
        if (languageCode?.ToLower()?.StartsWith(Synthesizer.LANGUAGECODE_SPANISH) == true)
        {
            messagesList.Add(new Message(Message.ROLE_SYSTEM, "Reply only in Spanish, ES-MX."));
        }


        // Memory:
        if (messages.Length > 0)
        {
            foreach (var memory in memories)
            {
                if (memory.DoPreload)
                {
                    var searchResults = await memory.Search(messages[0].Content);

                    if (searchResults != null)
                    {
                        Console.WriteLine("Search results:\n\n\t" + searchResults.Content + "\n\n");
                        messagesList.Add(new Message(Message.ROLE_SYSTEM, searchResults.Content));
                    }
                }
            }
        }

        // Tools:
        foreach (var tool in tools)
        {
            if (tool.ToolName != null)
            {
                toolsByName.Add(tool.ToolName, tool);
            }
        }

        List<ToolBase> enabledTools = new();

        // For each tool in toolsByName, if MemoryBase & DoPreload, execute and add a message to the context
        foreach (var tool in toolsByName)
        {
            if (tool.Value is MemoryBase memoryTool && memoryTool.DoPreload && messages.Length > 0)
            {
                // Get the latest message
                var latestMessage = messages[messages.Length - 1];

                // timer
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();

                // Preload memory tool with a "Search" operation.
                var toolResponse = await memoryTool.Search(latestMessage.Content);
                messagesList.Add(new Message(Message.ROLE_SYSTEM, toolResponse.Content));

                // timer
                timer.Stop();
                Console.WriteLine($"Preload took: {timer.ElapsedMilliseconds}ms");
            }
            else
            {
                enabledTools.Add(tool.Value);
            }
        }

        var options = new ChatCompletionsOptions("gpt-4o", messagesList.ToArray()?.ToChatRequestMessages());
        if (responseFormat == RESPONSE_FORMAT_JSON)
        {
            options.ResponseFormat = ChatCompletionsResponseFormat.JsonObject;
        }
        else if (responseFormat == RESPONSE_FORMAT_TEXT)
        {
            options.ResponseFormat = ChatCompletionsResponseFormat.Text;
        }
        foreach (var tool in enabledTools)
        {
            var def = tool.GetToolDefinition();
            options.Tools.Add(def);
        }

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

                    var toolResponseMessage = await GetToolCallResponseMessage(toolRequestMessage, toolsByName);

                    bool DO_INTERPRET = true;

                    if (DO_INTERPRET)
                    {
                        // (Optional) interpretation step:
                        List<Message> newMessages = new List<Message>();
                        newMessages.AddRange(messages);
                        newMessages.Add(new Message(role: Message.ROLE_SYSTEM, content: toolResponseMessage.Content));
                        ChatCompletion recursiveChatCompletion = new ChatCompletion(this.EmbeddingCache);
                        var interpretedResults = recursiveChatCompletion.SendChatCompletion(sessionID, systemPrompt, newMessages.ToArray(), false, null, languageCode, responseFormat, memories, tools);

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

    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?view=azure-dotnet-preview
    /// </summary>
    internal async Task<Message> GetToolCallResponseMessage(Message toolCall, Dictionary<string, ToolBase> toolsByName)
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
