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

    public virtual DbSet<Object> Objects { get; set; }

    public virtual DbSet<Prompt> Prompts { get; set; }

    public virtual DbSet<PromptObject> PromptObjects { get; set; }

    public virtual DbSet<PromptTool> PromptTools { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<Tool> Tools { get; set; }

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
