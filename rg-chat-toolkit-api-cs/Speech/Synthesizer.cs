using epic_retail_api_cs.Cache;
using Microsoft.AspNetCore.Mvc;
using rg_chat_toolkit_api_cs.Cache;
using rg_chat_toolkit_api_cs.Chat;
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
    "AccessKey": "00000000-0000-0000-0000-000000000000",
    "DoStreamResponse": false
}


*/


[Route("[controller]")]
[ApiController]
public class SynthesizerController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SynthesizeSpeech([FromBody] SynthesizeSpeechRequest request)
    {
        // Check cache
        var cacheKey = RGCache.GetCacheKey(request.TenantID, request.SessionID);
        var sessionText = await RGCache.Cache.Get(cacheKey);

        // Remove emojis
        sessionText = System.Text.RegularExpressions.Regex.Replace(sessionText, @"\p{Cs}", "");

        if (sessionText != null)
        {
            rg_chat_toolkit_cs.Speech.Synthesizer synthesizer = new rg_chat_toolkit_cs.Speech.Synthesizer();
            return new FileStreamResult(await synthesizer.SynthesizeSpeech(sessionText), "audio/mpeg");
        }
        else
        {
            throw new ApplicationException("No session content was found.");
        }
    }
}
