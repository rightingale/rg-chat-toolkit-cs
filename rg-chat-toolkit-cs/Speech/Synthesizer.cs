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

        public async Task SynthesizeSpeech(string text)
        {
            var client = new AmazonPollyClient(ConfigurationHelper.AWSAccessKeyId, ConfigurationHelper.AWSSecretAccessKey);

            var request = new SynthesizeSpeechRequest
            {
                OutputFormat = OutputFormat.Mp3,
                Text = text,
                VoiceId = "Joanna",
            };

            SynthesizeSpeechResponse response;
            try
            {
                response = await client.SynthesizeSpeechAsync(request);
                using (var memoryStream = new MemoryStream())
                {
                    response.AudioStream.CopyTo(memoryStream);
                    var byteArr = memoryStream.ToArray();
                    // you now have a byte array of the synthesized speech in MP3 format

                    // Write to c:\temp\synthesize.mp3, overwrite if exists:
                    File.WriteAllBytes(@"c:\temp\synthesize.mp3", byteArr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
