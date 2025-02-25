using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class Jwtauthorization
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }

    public Guid AuthenticationId { get; set; }

    public string ValidIssuer { get; set; } = null!;

    public string JwksUri { get; set; } = null!;

    public string UserIdattributeName { get; set; } = null!;

    public string RoleAttributeName { get; set; } = null!;

    public string SuperUserAttributeValue { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual Authentication Authentication { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
