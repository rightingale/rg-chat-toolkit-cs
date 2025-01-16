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

    public bool? DoStreamResponse { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<PromptFilter> PromptFilters { get; set; } = new List<PromptFilter>();

    public virtual ICollection<PromptMemory> PromptMemories { get; set; } = new List<PromptMemory>();

    public virtual ICollection<PromptObject> PromptObjects { get; set; } = new List<PromptObject>();

    public virtual ICollection<PromptPersona> PromptPersonas { get; set; } = new List<PromptPersona>();

    public virtual ICollection<PromptTool> PromptTools { get; set; } = new List<PromptTool>();

    public virtual ICollection<PromptUtterance> PromptUtterances { get; set; } = new List<PromptUtterance>();

    public virtual ContentType ReponseContentTypeNameNavigation { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
