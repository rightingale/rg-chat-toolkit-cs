using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_chat_toolkit_api_cs.Config;

internal static class ConfigurationHelper
{
    public static string? GetConnectionString(string key)
    {
        // Step 1: Set up the configuration to include user secrets
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // For appsettings.json
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Optional JSON config
            .AddUserSecrets<Program>() // Add user secrets here!
            .AddEnvironmentVariables() // Include env variables
            .Build();

        return configuration.GetConnectionString(key);
    }

    public static string? GetConfigurationValue(string key)
    {
        // Step 1: Set up the configuration to include user secrets
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // For appsettings.json
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Optional JSON config
            .AddUserSecrets<Program>() // Add user secrets here!
            .AddEnvironmentVariables() // Include env variables
            .Build();

        return configuration.GetValue<string>(key);
    }
}
