using Microsoft.AspNetCore.Mvc;
using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_cs.Chat;

namespace rg_chat_toolkit_api_cs.Chat;



// Wrapper class for string SystemPrompt, Message[] Messages
public class ChatCompletionRequest
{
    public Guid TenantID { get; set; }
    public Guid UserID { get; set; }
    public Guid SessionID { get; set; }

    public string? PromptName { get; set; }
    public Guid AccessKey { get; set; }

    public string? RequestMessageContent { get; set; }
    //public List<Message> ResponseMessages { get; set; } = new List<Message>();
    public bool DoStreamResponse { get; set; } = false;
}



// Sample JSON for ChatCompletionRequest API
/*

{
    "TenantID": "00000000-0000-0000-0000-000000000000",
    "SessionID": "00000000-0000-0000-0000-000000000000",
    "PromptName": "demo_greeter_weather",
    "AccessKey": "00000000-0000-0000-0000-000000000000",
    "RequestMessageContent": "Guten tag!",
    "DoStreamResponse": false
}

*/

[Route("[controller]")]
[ApiController]
public class ChatCompletionController : ControllerBase
{
    [HttpPost]
    public async IAsyncEnumerable<string> SendChatCompletion([FromBody] ChatCompletionRequest request)
    {
        if (request.PromptName == null)
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
        var prompt = DataMethods.Prompt_Get(request.TenantID, request.PromptName);
        if (prompt?.SystemPrompt != null)
        {
            if (request.DoStreamResponse)
            {
                if (prompt.DoStreamResponse == false) throw new ApplicationException("Prompt does not support streaming.");

                var allowStreamResponse = prompt.ReponseContentTypeNameNavigation.AllowStreamResponse;
                if (allowStreamResponse == false)
                {
                    // Format Content type XXXX does not support streaming.
                    throw new ApplicationException($"Content type {prompt.ReponseContentTypeNameNavigation.Name} does not support streaming.");

                }
            }

            // Build the response:
            var service = new ChatCompletion();
            var response = service.SendChatCompletion(request.SessionID, prompt.SystemPrompt, _messages?.ToArray() ?? [],
                                   true /*allowTools*/);

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
                yield return responseString;
            }
        }
        else
        {
            throw new ApplicationException("Prompt not found.");
        }
    }
}
