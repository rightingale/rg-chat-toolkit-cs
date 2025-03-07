﻿using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class Tenant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual ICollection<AccessKey> AccessKeys { get; set; } = new List<AccessKey>();

    public virtual ICollection<Authentication> Authentications { get; set; } = new List<Authentication>();

    public virtual ICollection<Filter> Filters { get; set; } = new List<Filter>();

    public virtual ICollection<Jwtauthorization> Jwtauthorizations { get; set; } = new List<Jwtauthorization>();

    public virtual ICollection<Memory> Memories { get; set; } = new List<Memory>();

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();

    public virtual ICollection<PromptFilter> PromptFilters { get; set; } = new List<PromptFilter>();

    public virtual ICollection<Prompt> Prompts { get; set; } = new List<Prompt>();

    public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();
}
