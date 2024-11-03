using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class Filter
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Assembly { get; set; }

    public string? Type { get; set; }

    public string? Method { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual ICollection<PromptFilter> PromptFilters { get; set; } = new List<PromptFilter>();

    public virtual Tenant Tenant { get; set; } = null!;
}
