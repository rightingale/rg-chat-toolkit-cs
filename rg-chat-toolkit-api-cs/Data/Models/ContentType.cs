using System;
using System.Collections.Generic;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class ContentType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool AllowStreamResponse { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();

    public virtual ICollection<Prompt> Prompts { get; set; } = new List<Prompt>();
}
