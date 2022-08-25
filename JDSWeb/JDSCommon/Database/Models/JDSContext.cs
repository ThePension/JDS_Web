using System;
using System.Collections.Generic;
using JDSCommon.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JDSCommon.Database.Models
{
    public partial class JDSContext : DbContext
    {
        public JDSContext()
        {
        }

        public JDSContext(DbContextOptions<JDSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cloth> Cloths { get; set; } = null!;
        public virtual DbSet<ClothColor> ClothColors { get; set; } = null!;
        public virtual DbSet<ClothSize> ClothSizes { get; set; } = null!;
        public virtual DbSet<ClothType> ClothTypes { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<EventGallery> EventGalleries { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<ShopGallery> ShopGalleries { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DBSettings.ToString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cloth>(entity =>
            {
                entity.ToTable("Cloth");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ColorNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Color)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cloth__Color__5EBF139D");

                entity.HasOne(d => d.SizeNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Size)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cloth__Size__5DCAEF64");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cloth__Type__5BE2A6F2");
            });

            modelBuilder.Entity<ClothColor>(entity =>
            {
                entity.ToTable("ClothColor");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Hexa)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClothSize>(entity =>
            {
                entity.ToTable("ClothSize");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Shortcut)
                    .HasMaxLength(5)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClothType>(entity =>
            {
                entity.ToTable("ClothType");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(4, 2)");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EventGallery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EventGallery");

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EventGall__Event__619B8048");

                entity.HasOne(d => d.ImageNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Image)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EventGall__Image__628FA481");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Alt)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShopGallery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ShopGallery");

                entity.HasOne(d => d.Cloth)
                    .WithMany()
                    .HasForeignKey(d => d.ClothId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShopGalle__Cloth__5FB337D6");

                entity.HasOne(d => d.ImageNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Image)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShopGalle__Image__60A75C0F");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__Role__5AEE82B9");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
