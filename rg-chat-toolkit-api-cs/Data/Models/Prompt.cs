using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class Prompt
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string SystemPrompt { get; set; } = null!;

    public string ReponseContentTypeName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual ICollection<PromptObject> PromptObjects { get; set; } = new List<PromptObject>();

    public virtual ContentType ReponseContentTypeNameNavigation { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
