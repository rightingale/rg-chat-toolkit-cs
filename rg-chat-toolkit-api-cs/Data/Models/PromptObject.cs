using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class PromptObject
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public Guid PromptId { get; set; }

    public Guid ObjectId { get; set; }

    public bool IsInput { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual Object Object { get; set; } = null!;

    public virtual Prompt Prompt { get; set; } = null!;
}
