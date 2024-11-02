using Microsoft.AspNetCore.Mvc;
using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_cs.Chat;

namespace rg_chat_toolkit_api_cs.Chat;


// Wrapper class for string SystemPrompt, Message[] Messages
public class ChatCompletionRequest
{
    public Guid TenantID { get; set; }
    public Guid SessionID { get; set; }
    public string? PromptName { get; set; }
    public Guid AccessKey { get; set; }
    public string? RequestMessageContent { get; set; }
    public List<Message> ResponseMessages { get; set; } = new List<Message>();
}

// Sample JSON for ChatCompletionRequest API
/*

{
    "TenantID": "00000000-0000-0000-0000-000000000000",
    "SessionID": "00000000-0000-0000-0000-000000000000",
    "PromptName": "demo_greeter",
    "AccessKey": "00000000-0000-0000-0000-000000000000",
    "RequestMessageContent": "Guten tag!",
    "ResponseMessages": []
}

*/

[Route("[controller]")]
[ApiController]
public class ChatCompletionController : ControllerBase
{
    [HttpPost]
    public IAsyncEnumerable<string> SendChatCompletion([FromBody] ChatCompletionRequest request)
    {
        if (request.PromptName == null)
        {
            throw new ApplicationException("Prompt name is required.");
        }

        // Instantiate the delegate as anonymous function
        AddMessageDelegate _handleAddMessage = (message) =>
        {
            request.ResponseMessages.Add(message);
            return request.ResponseMessages;
        };

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
            var service = new ChatCompletion();
            return service.SendChatCompletion(request.SessionID, prompt.SystemPrompt, _messages?.ToArray() ?? [], _handleAddMessage);
        }
        else
        {
            throw new ApplicationException("Prompt not found.");
        }
    }
}
