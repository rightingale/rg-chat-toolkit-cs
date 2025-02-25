using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integrations_tilley;

public class ConfigLock { }

internal static class ConfigurationHelper
{
    public static string? GetConnectionString(string key)
    {
        // Step 1: Set up the configuration to include user secrets
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // For appsettings.json
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<rg_integrations_tilley.ConfigLock>()
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetConnectionString(key);
    }

    //public static string? GetConfigurationValue(string key)
    //{
    //    // Step 1: Set up the configuration to include user secrets
    //    var configuration = new ConfigurationBuilder()
    //        .SetBasePath(Directory.GetCurrentDirectory()) // For appsettings.json
    //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //        .AddUserSecrets<Program>()
    //        .AddEnvironmentVariables()
    //        .Build();

    //    return configuration.GetValue<string>(key);
    //}
}
