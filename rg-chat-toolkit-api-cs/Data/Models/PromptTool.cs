using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class PromptTool
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public Guid PromptId { get; set; }

    public Guid ToolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual Prompt Prompt { get; set; } = null!;

    public virtual Tool Tool { get; set; } = null!;
}
