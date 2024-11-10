using Azure.AI.OpenAI;
using rg_chat_toolkit_cs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Tools;

public abstract class ToolBase
{
    public string? ToolName { get; set; }

    public abstract Task<Message> GetToolResponse(Message toolCall);

    public abstract ChatCompletionsFunctionToolDefinition GetToolDefinition();

}
