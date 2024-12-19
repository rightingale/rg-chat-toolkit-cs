using Microsoft.AspNetCore.Mvc;
using rg_chat_toolkit_api_cs.Cache;
using rg_chat_toolkit_api_cs.Chat;
using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_cs.Speech;

/*
Sample JS fetch command for calling this api at http://localhost:5210/
Stream the results and play it as a stream.
Wrap the call in a SynthesizeSpeech function that accepts text parameter:
async function SynthesizeSpeech(text) {
    const response = await fetch(`http://localhost:5210/Synthesizer?text=${text}`);
    const audio = new Audio();
    audio.src = URL.createObjectURL(await response.blob());
    audio.play();
}


*/

namespace rg_chat_toolkit_api_cs.Speech;


public class SynthesizeSpeechRequest : RequestBase
{
    public bool DoStreamResponse { get; set; } = false;
}

/*

Sample JSON for SynthesizeSpeechRequest API

{
    "TenantID": "787923AB-0D9F-EF11-ACED-021FE1D77A3B",
    "SessionID": "00000000-0000-0000-0000-000000000000",
    "AccessKey": "9ca151e3-d544-4b85-b44b-bbe055220808",
    "DoStreamResponse": false
}


*/


[Route("[controller]")]
[ApiController]
public class SynthesizerController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SynthesizeSpeech([FromQuery] Guid TenantID, [FromQuery] Guid SessionID, [FromQuery] Guid AccessKey)
    {
        return await SynthesizeSpeech(new SynthesizeSpeechRequest() { TenantID = TenantID, SessionID = SessionID, AccessKey = AccessKey });
    }

    [HttpPost]
    public async Task<IActionResult> SynthesizeSpeech([FromBody] SynthesizeSpeechRequest request)
    {
        try
        {
            // Check cache
            var cacheKey = RGCache.Instance.GetMessageCacheKey(request.TenantID, request.SessionID, request.AccessKey);
            //var sessionText = await RGCache.Instance.Get(cacheKey);

            // We can only render text from valid session responses:
            var cacheResponse = await RGCache.Instance.GetResponse(cacheKey);
            var sessionText = cacheResponse?.Response;
            var originalRequest = cacheResponse?.Request;

            if (originalRequest == null)
            {
                throw new ApplicationException("No session content was found.");
            }

            string voiceName = null;
            if (originalRequest.Persona != null && originalRequest.PromptName != null)
            {
                var persona = DataMethods.Persona_Get(request.TenantID, originalRequest.PromptName, originalRequest.Persona);
                if (persona?.Name != null)
                {
                    const string GENDER_MALE = "M";
                    const string GENDER_FEMALE = "F";
                    if (originalRequest?.LanguageCode?.ToLower()?.StartsWith(Synthesizer.LANGUAGECODE_SPANISH) == true
                        && persona.Gender == GENDER_MALE)
                    {
                        voiceName = Synthesizer.VOICE_DEFAULT_MALE_SPANISH;
                    }
                    else if (originalRequest?.LanguageCode?.ToLower()?.StartsWith(Synthesizer.LANGUAGECODE_SPANISH) == true)
                    {
                        voiceName = Synthesizer.VOICE_DEFAULT_FEMALE_SPANISH;
                    }
                    else if (persona.Gender == GENDER_MALE)
                    {
                        voiceName = Synthesizer.VOICE_DEFAULT_MALE_ENGLISH;
                    }
                    else if (persona.Gender == GENDER_FEMALE)
                    {
                        voiceName = Synthesizer.VOICE_DEFAULT_FEMALE_ENGLISH;
                    }
                }
            }

            if (!String.IsNullOrEmpty(sessionText))
            {
                // Remove emojis
                sessionText = System.Text.RegularExpressions.Regex.Replace(sessionText, @"\p{Cs}", "");

                rg_chat_toolkit_cs.Speech.Synthesizer synthesizer = new rg_chat_toolkit_cs.Speech.Synthesizer();
                return new FileStreamResult(await synthesizer.SynthesizeSpeech(sessionText, voiceName, originalRequest?.LanguageCode), "audio/mpeg")
                {
                    FileDownloadName = "speech.mp3"
                };
            }
            else
            {
                throw new ApplicationException("No session content was found.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
