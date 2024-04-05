using Microsoft.AspNetCore.Mvc;
using rg_chat_toolkit_cs.Chat;

/*
Sample JSON for this request:
{
  "systemPrompt": "You are a helpful assistant. Be very verbose.",
  "messages": [
    {
      "role": "system",
      "content": "Respond in ES-419."
    },
    {
      "role": "assistant",
      "content": "How can I help?"
    },
    {
      "role": "user",
      "content": "Please make a single combined list of presidents of both US and Argentina in alphabetical order."
    }
  ]
}
*/

/*
// Sample JS fetch command for calling this api at http://localhost:5210/
// Wrap the call in a SendChatCompletion_api function that accepts systemPrompt and messages parameters:
// Accept the stream results and progressively call the callback function with each response.

async function SendChatCompletion_api(systemPrompt, messages) {
    const response = await fetch(`http://localhost:5210/ChatCompletion`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ systemPrompt, messages })
    });

    const reader = response.body.getReader();
    const decoder = new TextDecoder();
    let result = '';
    while (true) {
        const { done, value } = await reader.read();
        if (done) {
            break;
        }
        result += decoder.decode(value, { stream: true });
        // Call the callback function with each response
        callback(result);
    }
}


// Sample call:
SendChatCompletion("You are a helpful assistant. Be very verbose.", [
    { role: "system", content: "Respond in ES-419." },
    { role: "assistant", content: "How can I help?" },
    { role: "user", content: "Please make a single combined list of presidents of both US and Argentina in alphabetical order." }
]);


*/

namespace rg_chat_toolkit_api_cs.Chat
{
    // Wrapper class for string SystemPrompt, Message[] Messages
    public class ChatCompletionRequest
    {
        public string SystemPrompt { get; set; } = "";
        public Message[] Messages { get; set; } = [];
    }

    [Route("[controller]")]
    [ApiController]
    public class ChatCompletionController : ControllerBase
    {
        [HttpPost]
        public IAsyncEnumerable<string> SendChatCompletion([FromBody] ChatCompletionRequest request)
        {
            var service = new ChatCompletion();
            return service.SendChatCompletion(request.SystemPrompt, request.Messages);
        }
    }
}
