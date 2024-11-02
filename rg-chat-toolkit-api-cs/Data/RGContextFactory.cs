using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using rg_chat_toolkit_api_cs.Data.Models;
using rg_chat_toolkit_api_cs.Config;

namespace rg_chat_toolkit_api_cs.Data;

internal class RGDatabaseContextFactory : IDesignTimeDbContextFactory<RgToolkitContext>
{
    public static RGDatabaseContextFactory Instance = new RGDatabaseContextFactory();

    public RgToolkitContext CreateDbContext()
    {
        return CreateDbContext([]);
    }

    public RgToolkitContext CreateDbContext(string[] args)
    {
        // Step 2: Retrieve the connection string from user secrets
        var connectionString = ConfigurationHelper.GetConnectionString("RG-Toolkit");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Connection string not found in configuration.");
        }

        // Step 3: Configure DbContext
        var optionsBuilder = new DbContextOptionsBuilder<RgToolkitContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new RgToolkitContext(optionsBuilder.Options);
    }
}
