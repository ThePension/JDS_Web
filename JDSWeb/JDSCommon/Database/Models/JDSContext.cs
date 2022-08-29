using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using JDSCommon.Settings;

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

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ColorNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Color)
                    .HasConstraintName("FK__Cloth__Color__3A4CA8FD");

                entity.HasOne(d => d.SizeNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Size)
                    .HasConstraintName("FK__Cloth__Size__395884C4");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("FK__Cloth__Type__3864608B");
            });

            modelBuilder.Entity<ClothColor>(entity =>
            {
                entity.ToTable("ClothColor");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Hexa)
                    .HasMaxLength(7)
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
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClothType>(entity =>
            {
                entity.ToTable("ClothType");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EventGallery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EventGallery");

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK__EventGall__Event__47A6A41B");

                entity.HasOne(d => d.Image)
                    .WithMany()
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK__EventGall__Image__489AC854");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.Alt)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShopGallery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ShopGallery");

                entity.HasOne(d => d.Cloth)
                    .WithMany()
                    .HasForeignKey(d => d.ClothId)
                    .HasConstraintName("FK__ShopGalle__Cloth__44CA3770");

                entity.HasOne(d => d.Image)
                    .WithMany()
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK__ShopGalle__Image__45BE5BA9");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

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
                    .HasConstraintName("FK__User__Role__3F115E1A");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
