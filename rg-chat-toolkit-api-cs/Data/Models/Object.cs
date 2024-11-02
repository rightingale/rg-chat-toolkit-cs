using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class Object
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string ContentTypeName { get; set; } = null!;

    public string? ObjectSchema { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual ContentType ContentTypeNameNavigation { get; set; } = null!;

    public virtual ICollection<PromptObject> PromptObjects { get; set; } = new List<PromptObject>();

    public virtual Tenant Tenant { get; set; } = null!;
}
