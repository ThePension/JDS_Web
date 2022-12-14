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
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
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

                entity.HasOne(d => d.BookedNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Booked)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Cloth__Booked__4277DAAA");

                entity.HasOne(d => d.SizeNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Size)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Cloth__Size__4183B671");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Cloths)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("FK__Cloth__Type__408F9238");
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

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");

                entity.HasOne(d => d.ColorNavigation)
                    .WithMany(p => p.ClothTypes)
                    .HasForeignKey(d => d.Color)
                    .HasConstraintName("FK__ClothType__Color__3BCADD1B");

                entity.HasMany(d => d.Images)
                    .WithMany(p => p.ClothTypes)
                    .UsingEntity<Dictionary<string, object>>(
                        "ShopGallery",
                        l => l.HasOne<Image>().WithMany().HasForeignKey("ImageId").HasConstraintName("FK__ShopGalle__Image__4A18FC72"),
                        r => r.HasOne<ClothType>().WithMany().HasForeignKey("ClothTypeId").HasConstraintName("FK__ShopGalle__Cloth__4924D839"),
                        j =>
                        {
                            j.HasKey("ClothTypeId", "ImageId").HasName("PK__ShopGall__C4BEF453FEA0B781");

                            j.ToTable("ShopGallery");
                        });
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

                entity.HasMany(d => d.Images)
                    .WithMany(p => p.Events)
                    .UsingEntity<Dictionary<string, object>>(
                        "EventGallery",
                        l => l.HasOne<Image>().WithMany().HasForeignKey("ImageId").HasConstraintName("FK__EventGall__Image__4DE98D56"),
                        r => r.HasOne<Event>().WithMany().HasForeignKey("EventId").HasConstraintName("FK__EventGall__Event__4CF5691D"),
                        j =>
                        {
                            j.HasKey("EventId", "ImageId").HasName("PK__EventGal__AE15A7604339D58A");

                            j.ToTable("EventGallery");
                        });
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.Alt)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(100)
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
                    .HasConstraintName("FK__User__Role__370627FE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
