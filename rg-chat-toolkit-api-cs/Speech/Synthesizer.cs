using Microsoft.AspNetCore.Mvc;
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

namespace rg_chat_toolkit_api_cs.Speech
{
    [Route("[controller]")]
    [ApiController]
    public class SynthesizerController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SynthesizeSpeech([FromQuery] string text, [FromQuery] string languageCode)
        {
            Synthesizer service = new Synthesizer();

            return new FileStreamResult(await service.SynthesizeSpeech(text, languageCode), "audio/mpeg");
        }
    }
}
