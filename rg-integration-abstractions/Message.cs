using Azure.AI.OpenAI;
using rg_chat_toolkit_cs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_chat_toolkit_cs.Chat
{

    // Define a delegate called AddMessage that accepts a message 
    public delegate List<Message> AddMessageDelegate(Message message);

    // Simplify the Azure OpenAI ChatRequestMessage class;
    // Only have Role and Content properties
    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }

        public string? ID { get; set; }
        public string? FunctionName { get; set; }
        public string? FunctionAguments { get; set; }

        public const string ROLE_ASSISTANT = "assistant";
        public const string ROLE_USER = "user";
        public const string ROLE_SYSTEM = "system";
        public const string ROLE_TOOL = "tool";

        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }

        // Convert the simplified Message class to the Azure OpenAI ChatRequestMessage class
        // Return either ChatRequestAssistantMessage or ChatRequestUserMessage by role:
        public ChatRequestMessage ToChatRequestMessage()
        {
            if (Role == ROLE_ASSISTANT)
            {
                return new ChatRequestAssistantMessage(Content);
            }
            else if (Role == ROLE_USER)
            {
                return new ChatRequestUserMessage(Content);
            }
            else if (Role == ROLE_SYSTEM)
            {
                return new ChatRequestSystemMessage(Content);
            }
            else if (Role == ROLE_TOOL)
            {
                var msg = new ChatRequestToolMessage(Content, this.ID);
                return msg;
            }
            else
            {
                throw new Exception("Invalid role: [" + Role + "]");
            }
        }

        // Utility function to convert an array of Message to an array of ChatRequestMessage
        public static ChatRequestMessage[] ToChatRequestMessages(Message[] messages)
        {
            return messages.Select(message => message.ToChatRequestMessage()).ToArray();
        }

        // Implicit conversion operator:
        // Convert a Message to a ChatRequestMessage
        public static implicit operator ChatRequestMessage(Message message)
        {
            return message.ToChatRequestMessage();
        }
    }
}

public static class MessageExtensions
{
    public static ChatRequestMessage[] ToChatRequestMessages(this Message[] arr)
    {
        return arr.Select(a => (ChatRequestMessage)a).ToArray();
    }
}