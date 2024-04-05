using Amazon.Polly.Model;
using Amazon.Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using rg_chat_toolkit_cs.Configuration;

namespace rg_chat_toolkit_cs.Speech
{
    public class Synthesizer
    {

        public async Task<System.IO.Stream> SynthesizeSpeech(string text)
        {
            var client = new AmazonPollyClient(ConfigurationHelper.AWSAccessKeyId, ConfigurationHelper.AWSSecretAccessKey);

            var request = new SynthesizeSpeechRequest
            {
                OutputFormat = OutputFormat.Mp3,
                Text = text,
                VoiceId = "Danielle",
                Engine = Engine.Neural
            };

            SynthesizeSpeechResponse response = await client.SynthesizeSpeechAsync(request);
            //// Read from the response.AudioStream until it is exhausted; yield return the bytes:
            //byte[] buffer = new byte[16 * 1024];
            //int bytesRead;
            //while ((bytesRead = await response.AudioStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            //{
            //    yield return buffer.Take(bytesRead).ToArray();
            //}

            return response.AudioStream;
        }

    }
}
