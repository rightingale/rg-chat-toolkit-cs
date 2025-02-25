using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class Memory
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? DescriptionEmbedding { get; set; }

    public string MemoryType { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public bool IsPrivate { get; set; }

    public virtual ICollection<PromptMemory> PromptMemories { get; set; } = new List<PromptMemory>();

    public virtual Tenant Tenant { get; set; } = null!;
}
