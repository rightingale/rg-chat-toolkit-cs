using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Newtonsoft.Json;
using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Configuration;
using System.Collections.Generic;


namespace OpenAIApiExample
{
    /*
     Deserialization classes for:

    {
  "id": "chatcmpl-9DQyeHDSICzQQvrecowDLnfoBKCmv",
  "object": "chat.completion",
  "created": 1712988992,
  "model": "gpt-4-turbo-2024-04-09",
  "choices": [
    {
      "index": 0,
      "message": {
        "role": "assistant",
        "content": "White, round pill with the imprint L015."
      },
      "logprobs": null,
      "finish_reason": "stop"
    }
  ],
  "usage": {
    "prompt_tokens": 341,
    "completion_tokens": 10,
    "total_tokens": 351
  },
  "system_fingerprint": "fp_67e6987839"
}

    */
    public class OAIResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
        public string SystemFingerprint { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public ResponseMessage Message { get; set; }
        public object Logprobs { get; set; }
        public string FinishReason { get; set; }
    }

    public class ResponseMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }


    public class MessageContent
    {
        public string? type { get; set; } = null;
        public string? text { get; set; } = null;
        public ImageUrl? image_url { get; set; }
    }
    public class ImageUrl
    {
        public string url { get; set; } = "";
    }


    public class ImageChatCompletion
    {
        private const string OpenAiEndpoint = "https://api.openai.com/v1/chat/completions";

        public const string USER_PROMPT_IDENTIFY_MEDICATION =
            @"Identify the medications in this image. Be very brief.
            ===
            Example response:
            Bottle labeled Aspirin, 500mg
            ===
            Example response:
            White, round pill with the imprint X1234-3
            ===
            Example response:
            Blue and white capsule with no imprint.
            Gray round pill with the imprint 1234
            ";

        public const string SYSTEM_PROMPT_LOOKUP_MEDICATION =
            @"You are a professional pharmacy tech who helps identify medications.
            Search online resources as required.
            Respond with the imprint, drug name, brand name, and strength.
            Be very brief.
            ";

        public static async IAsyncEnumerable<string> ExplainImage(Guid sessionID, string base64Image)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ConfigurationHelper.OpenAIApiKey}");

            var payload = new
            {
                model = "gpt-4-turbo",
                messages = new[] {
                        new
                        {
                            role = "user",
                            content = new MessageContent[]
                            {
                                new MessageContent { type = "text", text = USER_PROMPT_IDENTIFY_MEDICATION },
                                new MessageContent { type = "image_url", image_url = new ImageUrl { url = $"data:image/jpeg;base64,{base64Image}" } }
                            }
                        }
                    },
                max_tokens = 300
            };

            string jsonPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(OpenAiEndpoint, content);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            // Lookup:
            ChatCompletion chatCompletion = new ChatCompletion(null, null);
            var lookupResponse = chatCompletion.SendChatCompletion(
                sessionID,
                SYSTEM_PROMPT_LOOKUP_MEDICATION,
                new[] { new Message("user", jsonResponse) },
                true/*allowTools*/, null, null, null, null, null);
            if (lookupResponse != null)
            {
                // Await foreach to process each response as it arrives
                await foreach (var currentLookupResponse in lookupResponse)
                {
                    yield return currentLookupResponse;
                }
            }
        }
    }

}
