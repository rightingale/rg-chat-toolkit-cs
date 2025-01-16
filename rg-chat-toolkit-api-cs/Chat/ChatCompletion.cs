using Amazon.Polly;
using Microsoft.AspNetCore.Mvc;
using rg_chat_toolkit_api_cs.Cache;
using rg_chat_toolkit_api_cs.Chat.Helpers;
using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Speech;
using rg_integration_abstractions.Tools;
using rg_integration_abstractions.Tools.Memory;
using System.Diagnostics;
using System.Text;

namespace rg_chat_toolkit_api_cs.Chat;

public class RequestBase
{
    public Guid TenantID { get; set; }
    public Guid UserID { get; set; }
    public Guid SessionID { get; set; }
    public Guid AccessKey { get; set; }
}


public class ChatCompletionRequest : RequestBase
{
    public string? PromptName { get; set; }
    public string? RequestMessageContent { get; set; }
    public bool DoStreamResponse { get; set; } = false;
    public string? Persona { get; set; } = null;
    public string? LanguageCode { get; set; }
}

public class ChatCompletionResponse
{
    public ChatCompletionRequest? Request { get; set; }
    public string? Response { get; set; }
}



// Sample JSON for ChatCompletionRequest API
/*


{
    "TenantID": "787923AB-0D9F-EF11-ACED-021FE1D77A3B",
    "SessionID": "00000000-0000-0000-0000-000000000000",
    "PromptName": "instore_experience_helper",
    "AccessKey": "00000000-0000-0000-0000-000000000000",
    "RequestMessageContent": "Where's the wheat bread?",
    "DoStreamResponse": false
}


*/

[Route("[controller]")]
[ApiController]
public class ChatCompletionController : ControllerBase
{
    protected readonly IRGEmbeddingCache EmbeddingCache;
    protected readonly ChatCompletion RGChatInstance;

    public ChatCompletionController(IRGEmbeddingCache embeddingCache)
    {
        this.EmbeddingCache = embeddingCache;
        RGChatInstance = new ChatCompletion(EmbeddingCache);
    }

    [HttpPost("SendChatCompletion_Sync")]
    public async Task<IActionResult> SendChatCompletion_Sync([FromBody] ChatCompletionRequest request)
    {
        StringBuilder stringBuilder = new StringBuilder();

        try
        {
            var response = SendChatCompletion(request);
            await foreach (var item in response)
            {
                stringBuilder.Append(item);
            }
            return Ok(stringBuilder.ToString());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async IAsyncEnumerable<string> SendChatCompletion([FromBody] ChatCompletionRequest request)
    {
        // timer
        var timer = new Stopwatch();
        timer.Start();

        string? chosenPromptName = request.PromptName;
        if (request.PromptName == null)
        {
            chosenPromptName = await PromptChooser.ChoosePrompt(request.TenantID, request.RequestMessageContent ?? "");
        }

        if (chosenPromptName == null)
        {
            throw new ApplicationException("Prompt name is required.");
        }

        //AddMessageDelegate _handleAddMessage = (message) =>
        //{
        //    request.ResponseMessages.Add(message);
        //    return request.ResponseMessages;
        //};

        // To support context, look up messages:
        List<Message> _messages = new List<Message>();
        if (request.RequestMessageContent != null)
        {
            _messages.Add(new Message(
                role: Message.ROLE_USER,
                content: request.RequestMessageContent
            ));
        }

        // Lookup the prompt:
        var prompt = DataMethods.Prompt_Get(request.TenantID, chosenPromptName);

        if (prompt?.SystemPrompt != null)
        {
            if (request.DoStreamResponse)
            {
                if (prompt.DoStreamResponse == false && request.PromptName != null) throw new ApplicationException("Prompt does not support streaming.");
                if (request.PromptName == null &&
                    (prompt.DoStreamResponse == false || prompt.ReponseContentTypeNameNavigation.AllowStreamResponse == false))
                {
                    // Auto-chosen prompt, we can allow override:
                    request.DoStreamResponse = false;
                }

                if (request.DoStreamResponse)
                {
                    var allowStreamResponse = prompt.ReponseContentTypeNameNavigation.AllowStreamResponse;
                    if (allowStreamResponse == false)
                    {
                        // Format Content type XXXX does not support streaming.
                        throw new ApplicationException($"Content type {prompt.ReponseContentTypeNameNavigation.Name} does not support streaming.");
                    }
                }
            }

            // Construct memories:
            var memories = new List<MemoryBase>();
            var tools = new List<ToolBase>();
            foreach (var promptMemory in prompt.PromptMemories.Where(mem => mem.Memory.IsActive))
            {
                var memory = MemoryBase.Create(promptMemory.Memory.Name, promptMemory.Memory.Description, promptMemory.Memory.MemoryType, RG.Instance.EmbeddingCache);
                memories.Add(memory);

                // Add memories as tools,
                // if it doesn't already exist in PromptTools, and DoPreload is false.
                if (prompt.PromptTools.Any(tool => tool.Tool.Name == memory.ToolName) == false && memory.DoPreload == false)
                {
                    tools.Add(memory);
                }
            }

            foreach (var promptTool in prompt.PromptTools.Where(tool => tool.Tool.IsActive))
            {
                var tool = ToolBase.Create(promptTool.Tool.Name, promptTool.Tool.Description, promptTool.Tool.Assembly, promptTool.Tool.Type, RG.Instance.EmbeddingCache);
                if (tool != null)
                {
                    // Add the tool if it is not already in the list
                    if (tools.Any(t => t.ToolName == tool.ToolName) == false)
                    {
                        tools.Add(tool);
                    }
                }
            }

            // Build the response:
            var response = RGChatInstance.SendChatCompletion(request.SessionID, prompt.SystemPrompt, _messages?.ToArray() ?? [],
                                   true /*allowTools*/, null, request.LanguageCode, prompt.ReponseContentTypeName, 
                                   memories, tools);

            if (request.DoStreamResponse)
            {
                await foreach (var item in response)
                {
                    yield return item;
                }
            }
            else
            {
                var responseList = new List<string>();
                await foreach (var item in response)
                {
                    responseList.Add(item);
                }
                // Join into 1 string:
                var responseString = string.Join(String.Empty, responseList);

                // Save in cache: for related functions (e.g., SynthesizeSpeech).
                var cacheResponse = new ChatCompletionResponse() { Request = request, Response = responseString };
                var cacheKey = RGCache.Instance.GetMessageCacheKey(request.TenantID, request.SessionID, request.AccessKey);
                if (prompt.ReponseContentTypeName != ChatCompletion.RESPONSE_FORMAT_TEXT)
                {
                    cacheResponse.Response = "...";
                }
                await RGCache.Instance.PutResponse(cacheKey, cacheResponse);

                timer.Stop();
                Console.WriteLine($"API: {timer.ElapsedMilliseconds}ms");

                yield return responseString;
            }
        }
        else
        {
            throw new ApplicationException("Prompt not found.");
        }
    }

}
