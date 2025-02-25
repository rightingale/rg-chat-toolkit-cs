using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class PromptUtterance
{
    public Guid TenantId { get; set; }

    public Guid PromptId { get; set; }

    public Guid Id { get; set; }

    public string Utterance { get; set; } = null!;

    public DateTime LastUpdate { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual Prompt Prompt { get; set; } = null!;
}
