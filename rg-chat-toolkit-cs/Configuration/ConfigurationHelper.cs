using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_chat_toolkit_cs.Configuration
{
    public class ConfigurationHelper
    {
        // Configuration builder to include environment variables and secrets:
        public static IConfigurationBuilder GetConfigurationBuilder()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<ConfigurationHelper>();

            return builder;
        }

        // Get OpenAI:ApiKey from configuration
        public static string? OpenAIApiKey
        {
            get
            {
                var configuration = GetConfigurationBuilder().Build();
                return configuration["openai-apikey"];
            }
        }

        // Get AWS:AccessKeyId from configuration
        public static string? AWSAccessKeyId
        {
            get
            {
                var configuration = GetConfigurationBuilder().Build();
                return configuration["AWS:AccessKeyId"];
            }
        }

        // Get AWS:SecretAccessKey from configuration
        public static string? AWSSecretAccessKey
        {
            get
            {
                var configuration = GetConfigurationBuilder().Build();
                return configuration["AWS:SecretAccessKey"];
            }
        }

        public static string? ClaudeApiKey
        {
            get
            {
                var configuration = GetConfigurationBuilder().Build();
                return configuration["Claude:ApiKey"];
            }
        }
    }
}
