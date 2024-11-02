using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rg_chat_toolkit_api_cs.Data.Models;

namespace rg_chat_toolkit_api_cs.Data;

public static class DataMethods
{

    //public static readonly Guid PRODUCER_TOKEN_SYSTEM = Guid.Parse("00000000-0000-0000-0000-000000000000");

    public static Prompt? Prompt_Get(Guid tenantID, string name)
    {
        var db = RGDatabaseContextFactory.Instance.CreateDbContext();
        return db.Prompts
            .Where(p =>
                p.TenantId == tenantID
                && p.Name == name
            )
            .FirstOrDefault();
    }

}
