using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class AccessKey
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public Guid KeyValue { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}
