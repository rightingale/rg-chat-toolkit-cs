using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class PromptFilter
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public Guid PromptId { get; set; }

    public Guid FilterId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual Filter Filter { get; set; } = null!;

    public virtual Prompt Prompt { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
