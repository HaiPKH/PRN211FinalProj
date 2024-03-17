using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PRN211FinalProj.Model
{
    public partial class VIDLContext : DbContext
    {
        public VIDLContext()
        {
        }

        public VIDLContext(DbContextOptions<VIDLContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<VideoInfo> VideoInfos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("VIDL"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Otp)
                    .IsUnicode(false)
                    .HasColumnName("OTP");

                entity.Property(e => e.PhoneNum)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VideoInfo>(entity =>
            {
                entity.ToTable("VideoInfo");

                entity.Property(e => e.DriveId)
                    .IsUnicode(false)
                    .HasColumnName("DriveID");

                entity.Property(e => e.Notes).IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.VideoUrl)
                    .IsUnicode(false)
                    .HasColumnName("VideoURL");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.VideoInfos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoInfo_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
