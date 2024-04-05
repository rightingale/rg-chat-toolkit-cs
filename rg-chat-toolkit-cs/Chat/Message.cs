using Azure.AI.OpenAI;
using rg_chat_toolkit_cs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_chat_toolkit_cs.Chat
{
    // Simplify the Azure OpenAI ChatRequestMessage class;
    // Only have Role and Content properties
    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }

        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }

        // Convert the simplified Message class to the Azure OpenAI ChatRequestMessage class
        // Return either ChatRequestAssistantMessage or ChatRequestUserMessage by role:
        public ChatRequestMessage ToChatRequestMessage()
        {
            if (Role == "assistant")
            {
                return new ChatRequestAssistantMessage(Content);
            }
            else if (Role == "user")
            {
                return new ChatRequestUserMessage(Content);
            }
            else if (Role == "system")
            {
                return new ChatRequestSystemMessage(Content);
            }
            else
            {
                throw new Exception("Invalid role");
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