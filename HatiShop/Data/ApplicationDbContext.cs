// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using HatiShop.Models;

namespace HatiShop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staff { get; set; } // Đảm bảo có dòng này

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50).HasColumnName("Id");
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired().HasColumnName("Username");
                entity.Property(e => e.Password).HasMaxLength(100).IsRequired().HasColumnName("Password");
                entity.Property(e => e.FullName).HasMaxLength(50).IsRequired().HasColumnName("FullName");
                entity.Property(e => e.Gender).HasMaxLength(4).HasColumnName("Gender");
                entity.Property(e => e.BirthDate).HasColumnType("datetime").HasColumnName("BirthDate");
                entity.Property(e => e.PhoneNumber).HasMaxLength(10).HasColumnName("PhoneNumber");
                entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("Email");
                entity.Property(e => e.Address).HasMaxLength(200).HasColumnName("Address");
                entity.Property(e => e.AvatarPath).HasColumnName("AvatarPath");
                entity.Property(e => e.Revenue).HasDefaultValue(0).HasColumnName("Revenue");
                entity.Property(e => e.Rank).HasMaxLength(50).HasDefaultValue("ĐỒNG").HasColumnName("Rank");
            });

            // Staff configuration
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("Staff");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50).HasColumnName("Id");
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired().HasColumnName("Username");
                entity.Property(e => e.Password).HasMaxLength(100).IsRequired().HasColumnName("Password");
                entity.Property(e => e.FullName).HasMaxLength(50).IsRequired().HasColumnName("FullName");
                entity.Property(e => e.Gender).HasMaxLength(4).HasColumnName("Gender");
                entity.Property(e => e.BirthDate).HasColumnType("datetime").HasColumnName("BirthDate");
                entity.Property(e => e.PhoneNumber).HasMaxLength(10).HasColumnName("PhoneNumber");
                entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("Email");
                entity.Property(e => e.Address).HasMaxLength(200).HasColumnName("Address");
                entity.Property(e => e.AvatarPath).HasColumnName("AvatarPath");
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired().HasColumnName("Role");
            });
        }
    }
}