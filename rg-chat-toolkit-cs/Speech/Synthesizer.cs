﻿using Amazon.Polly.Model;
using Amazon.Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using rg_chat_toolkit_cs.Configuration;
using System.Globalization;

namespace rg_chat_toolkit_cs.Speech
{
    public class Synthesizer
    {
        public const string LANGUAGECODE_ENGLISH = "en-US";
        public const string LANGUAGECODE_SPANISH = "es";

        public const string VOICE_DEFAULT_MALE_ENGLISH = "Stephen";
        public const string VOICE_ENGINE_MALE_ENGLISH = "Generative";

        public const string VOICE_DEFAULT_FEMALE_ENGLISH = "Ruth";
        public const string VOICE_ENGINE_FEMALE_ENGLISH = "Generative";

        public const string VOICE_DEFAULT_MALE_SPANISH = "Pedro";
        public const string VOICE_ENGINE_MALE_SPANISH = "Generative";

        public const string VOICE_DEFAULT_FEMALE_SPANISH = "Lupe";
        public const string VOICE_ENGINE_FEMALE_SPANISH = "Generative";

        public async Task<System.IO.Stream> SynthesizeSpeech(string text, string? voiceName)
        {
            return await this.SynthesizeSpeech(text, LANGUAGECODE_ENGLISH);
        }

        /// <summary>
        /// languageCode: Example: "en-US", "es-MX"
        /// </summary>
        public async Task<System.IO.Stream> SynthesizeSpeech(string text, string? voiceName, string? languageCode)
        {
            //const string VOICE_ID_NAME_ENGLISH = "Danielle";
            ////const string VOICE_ID_NAME_ENGLISH_MALE = "Gregory";//Neural large voice
            //const string VOICE_ID_NAME_ENGLISH_MALE = "Stephen";//Generative sm male voice
            ////const string VOICE_ID_NAME_SPANISH = "Mia";//Spanish female
            //const string VOICE_ID_NAME_SPANISH = "Pedro";//Neural Spanish male sm voice


            string voiceId = voiceName ?? VOICE_DEFAULT_FEMALE_ENGLISH;
            var engine = Engine.Generative;
            if (languageCode?.ToLower()?.StartsWith(LANGUAGECODE_SPANISH) == true)
            {
                voiceId = voiceName ?? VOICE_DEFAULT_FEMALE_SPANISH;
                engine = Engine.Neural;
            }

            var client = new AmazonPollyClient(ConfigurationHelper.AWSAccessKeyId, ConfigurationHelper.AWSSecretAccessKey, RegionEndpoint.USEast1);

            var request = new SynthesizeSpeechRequest
            {
                OutputFormat = OutputFormat.Mp3,
                Text = text,
                VoiceId = voiceId,
                Engine = engine
            };

            SynthesizeSpeechResponse response = await client.SynthesizeSpeechAsync(request);
            //// Read from the response.AudioStream until it is exhausted; yield return the bytes:
            //byte[] buffer = new byte[16 * 1024];
            //int bytesRead;
            //while ((bytesRead = await response.AudioStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            //{
            //    yield return buffer.Take(bytesRead).ToArray();
            //}

            //// Write to temp file
            //using (var fileStream = System.IO.File.Create("c:\\temp\\speech.mp3"))
            //{
            //    // Stream the results from speechResponse into fileStream
            //    await response.AudioStream.CopyToAsync(fileStream);
            //}
            //// Seek beginning:
            //response.AudioStream.Seek(0, System.IO.SeekOrigin.Begin);

            return response.AudioStream;
        }

    }
}
