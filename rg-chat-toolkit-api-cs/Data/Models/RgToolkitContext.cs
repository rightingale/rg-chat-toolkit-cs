using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace rg_chat_toolkit_api_cs.Data.Models;

public partial class RgToolkitContext : DbContext
{
    public RgToolkitContext()
    {
    }

    public RgToolkitContext(DbContextOptions<RgToolkitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessKey> AccessKeys { get; set; }

    public virtual DbSet<ContentType> ContentTypes { get; set; }

    public virtual DbSet<Filter> Filters { get; set; }

    public virtual DbSet<Memory> Memories { get; set; }

    public virtual DbSet<Object> Objects { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Prompt> Prompts { get; set; }

    public virtual DbSet<PromptFilter> PromptFilters { get; set; }

    public virtual DbSet<PromptMemory> PromptMemories { get; set; }

    public virtual DbSet<PromptObject> PromptObjects { get; set; }

    public virtual DbSet<PromptPersona> PromptPersonas { get; set; }

    public virtual DbSet<PromptTool> PromptTools { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<Tool> Tools { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=db.TEST.datanac.io;Database=RG-Toolkit;User ID=admin;Password=predictiveAnalyticsEmpowersTheFarm;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessKey>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id }).HasName("PK_AccessKey_ID");

            entity.ToTable("AccessKey");

            entity.HasIndex(e => new { e.TenantId, e.KeyValue }, "IX_AccessKey").IsUnique();

            entity.HasIndex(e => e.Id, "UQ_AccessKey_ID").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.KeyValue).HasDefaultValueSql("(newid())");
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Tenant).WithMany(p => p.AccessKeys)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccessKey_TenantID");
        });

        modelBuilder.Entity<ContentType>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK_ContentType_ID");

            entity.ToTable("ContentType");

            entity.HasIndex(e => e.Name, "IX_ContentType").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Filter>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id }).HasName("PK_Filter_ID");

            entity.ToTable("Filter");

            entity.HasIndex(e => new { e.TenantId, e.Name }, "IX_Filter").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID");
            entity.Property(e => e.Assembly).HasMaxLength(500);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Method).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(500);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Filters)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Filter_TenantID");
        });

        modelBuilder.Entity<Memory>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id });

            entity.ToTable("Memory");

            entity.HasIndex(e => new { e.TenantId, e.Id }, "IX_Memory_ID").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.Name }, "IX_Memory_Name").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("Is_Active");
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MemoryType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Memories)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Memory_TenantID");
        });

        modelBuilder.Entity<Object>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id }).HasName("PK_Object_ID");

            entity.ToTable("Object");

            entity.HasIndex(e => new { e.TenantId, e.Name }, "IX_Object").IsUnique();

            entity.HasIndex(e => e.Id, "UQ_Object_ID").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.ContentTypeName)
                .HasMaxLength(100)
                .HasDefaultValue("text/plain");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.ContentTypeNameNavigation).WithMany(p => p.Objects)
                .HasForeignKey(d => d.ContentTypeName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Object_ContentTypeName");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Objects)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Object_TenantID");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id });

            entity.ToTable("Persona");

            entity.HasIndex(e => new { e.TenantId, e.Id }, "IX_Persona_ID").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.Name }, "IX_Persona_Name").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("Is_Active");
            entity.Property(e => e.LanguageCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Prompt>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id }).HasName("PK_Prompt_ID");

            entity.ToTable("Prompt");

            entity.HasIndex(e => new { e.TenantId, e.Name }, "IX_Prompt").IsUnique();

            entity.HasIndex(e => e.Id, "UQ_Prompt_ID").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ReponseContentTypeName)
                .HasMaxLength(100)
                .HasDefaultValue("text/plain");

            entity.HasOne(d => d.ReponseContentTypeNameNavigation).WithMany(p => p.Prompts)
                .HasForeignKey(d => d.ReponseContentTypeName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prompt_ContentTypeName");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Prompts)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prompt_TenantID");
        });

        modelBuilder.Entity<PromptFilter>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id }).HasName("PK_PromptFilters_ID");

            entity.HasIndex(e => new { e.TenantId, e.PromptId, e.FilterId }, "IX_PromptFilters").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FilterId).HasColumnName("FilterID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PromptId).HasColumnName("PromptID");

            entity.HasOne(d => d.Tenant).WithMany(p => p.PromptFilters)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptFilters_TenantID");

            entity.HasOne(d => d.Filter).WithMany(p => p.PromptFilters)
                .HasForeignKey(d => new { d.TenantId, d.FilterId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptFilters_FilterID");

            entity.HasOne(d => d.Prompt).WithMany(p => p.PromptFilters)
                .HasForeignKey(d => new { d.TenantId, d.PromptId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptFilters_PromptID");
        });

        modelBuilder.Entity<PromptMemory>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id });

            entity.HasIndex(e => new { e.TenantId, e.Id }, "IX_PromptMemories_ID").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.PromptId, e.Ordinal }, "IX_PromptMemories_Ordinal")
                .IsUnique()
                .HasFilter("([Is_Active]=(1))");

            entity.HasIndex(e => new { e.TenantId, e.PromptId, e.MemoryId }, "IX_PromptMemories_PromptID_MemoryID")
                .IsUnique()
                .HasFilter("([Is_Active]=(1))");

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("Is_Active");
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MemoryId).HasColumnName("MemoryID");
            entity.Property(e => e.PromptId).HasColumnName("PromptID");

            entity.HasOne(d => d.Memory).WithMany(p => p.PromptMemories)
                .HasForeignKey(d => new { d.TenantId, d.MemoryId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptMemories_MemoryID");

            entity.HasOne(d => d.Prompt).WithMany(p => p.PromptMemories)
                .HasForeignKey(d => new { d.TenantId, d.PromptId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptMemories_PromptID");
        });

        modelBuilder.Entity<PromptObject>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id }).HasName("PK_PromptObjects_ID");

            entity.HasIndex(e => new { e.TenantId, e.PromptId, e.ObjectId }, "IX_PromptObjects").IsUnique();

            entity.HasIndex(e => e.Id, "UQ_PromptObjects_ID").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
            entity.Property(e => e.PromptId).HasColumnName("PromptID");

            entity.HasOne(d => d.Object).WithMany(p => p.PromptObjects)
                .HasForeignKey(d => new { d.TenantId, d.ObjectId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptObjects_ObjectID_TenantID");

            entity.HasOne(d => d.Prompt).WithMany(p => p.PromptObjects)
                .HasForeignKey(d => new { d.TenantId, d.PromptId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptObjects_PromptID_TenantID");
        });

        modelBuilder.Entity<PromptPersona>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id });

            entity.HasIndex(e => new { e.TenantId, e.Id }, "IX_PromptPersonas_ID").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.PromptId, e.Ordinal }, "IX_PromptPersonas_Ordinal")
                .IsUnique()
                .HasFilter("([Is_Active]=(1))");

            entity.HasIndex(e => new { e.TenantId, e.PromptId, e.PersonaId }, "IX_PromptPersonas_PromptID_PersonaID")
                .IsUnique()
                .HasFilter("([Is_Active]=(1))");

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("Is_Active");
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PersonaId).HasColumnName("PersonaID");
            entity.Property(e => e.PromptId).HasColumnName("PromptID");

            entity.HasOne(d => d.Persona).WithMany(p => p.PromptPersonas)
                .HasForeignKey(d => new { d.TenantId, d.PersonaId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptPersonas_Persona");

            entity.HasOne(d => d.Prompt).WithMany(p => p.PromptPersonas)
                .HasForeignKey(d => new { d.TenantId, d.PromptId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromptPersonas_Prompt");
        });

        modelBuilder.Entity<PromptTool>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id }).HasName("PK_PromptTools_ID");

            entity.HasIndex(e => new { e.TenantId, e.PromptId, e.ToolId }, "IX_PromptTools").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PromptId).HasColumnName("PromptID");
            entity.Property(e => e.ToolId).HasColumnName("ToolID");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tenant_ID");

            entity.ToTable("Tenant");

            entity.HasIndex(e => e.Name, "IX_Tenant").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Tool>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id }).HasName("PK_Tool_ID");

            entity.ToTable("Tool");

            entity.HasIndex(e => new { e.TenantId, e.Name }, "IX_Tool").IsUnique();

            entity.Property(e => e.TenantId).HasColumnName("TenantID");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID");
            entity.Property(e => e.Assembly).HasMaxLength(500);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Method).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(500);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Tools)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tool_TenantID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
