using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebCV.Models;

public partial class CvContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public CvContext()
    {
    }

    public CvContext(DbContextOptions<CvContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Cv> Cvs { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Recruitment> Recruitments { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:CVConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2B40231617");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(255);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Company__2D971C4CA95AF372");

            entity.ToTable("Company");

            entity.HasIndex(e => e.Link, "Link4");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CopmpanyName).HasMaxLength(255);
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.Hotline)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Information).HasColumnType("ntext");
            entity.Property(e => e.Link)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.CvId).HasName("PK__CV__4FB514990A468CEE");

            entity.ToTable("CV");

            entity.Property(e => e.CvId).HasColumnName("CvID");
            entity.Property(e => e.File)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.TemplateId).HasColumnName("TemplateID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Template).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV__TemplateID__4BAC3F29");

            entity.HasOne(d => d.User).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV__UserID__48CFD27E");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__Job__056690E2B1B8B79B");

            entity.ToTable("Job");

            entity.HasIndex(e => e.Link, "Link5");

            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.EndDay).HasColumnType("datetime");
            entity.Property(e => e.Jd)
                .HasColumnType("ntext")
                .HasColumnName("JD");
            entity.Property(e => e.JobName).HasMaxLength(255);
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UpdateDay).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Job__CategoryID__4F7CD00D");

            entity.HasOne(d => d.Company).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Job__CompanyID__4E88ABD4");

            entity.HasOne(d => d.Level).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Job__LevelID__5070F446");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Level__09F03C067D00BA19");

            entity.ToTable("Level");

            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(255);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E320AE36A93");

            entity.ToTable("Notification");

            entity.HasIndex(e => e.Link, "Link1");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.Content).HasColumnType("ntext");
            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SendAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__4AB81AF0");
        });

        modelBuilder.Entity<Recruitment>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.JobId }).HasName("PK__Recruitm__27DEA5A211C07C9B");

            entity.ToTable("Recruitment");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.FileCv)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("FileCV");
            entity.Property(e => e.SendAt).HasColumnType("datetime");

            entity.HasOne(d => d.Job).WithMany(p => p.Recruitments)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recruitme__JobID__4CA06362");

            entity.HasOne(d => d.User).WithMany(p => p.Recruitments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recruitme__UserI__49C3F6B7");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__Skill__DFA091E726076B0D");

            entity.ToTable("Skill");

            entity.Property(e => e.SkillId).HasColumnName("SkillID");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.SkillName).HasMaxLength(255);

            entity.HasOne(d => d.Job).WithMany(p => p.Skills)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Skill__JobID__4D94879B");
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__Template__F87ADD0732FD63B8");

            entity.ToTable("Template");

            entity.HasIndex(e => e.Link, "Link3");

            entity.Property(e => e.TemplateId).HasColumnName("TemplateID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.File)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Link)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UploadedBy).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC64ACF623");

            entity.ToTable("User");

            entity.HasIndex(e => e.Link, "Link2");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasColumnType("ntext");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
