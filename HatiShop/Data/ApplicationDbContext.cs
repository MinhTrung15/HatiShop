using Microsoft.EntityFrameworkCore;
using HatiShop.Models;

namespace HatiShop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customer{ get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<BillDetail> BillDetail { get; set; }
        public DbSet<Product> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Revenue).HasDefaultValue(0);
                entity.Property(e => e.Rank).HasMaxLength(20).HasDefaultValue("ĐỒNG");
            });

            // Staff configuration
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Role).HasMaxLength(50);
            });

            // Bill configuration
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.StaffId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CustomerId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CreationTime).HasDefaultValueSql("GETDATE()");

                // 🟢 XÓA .HasColumnType("decimal(18,2)") HOẶC THAY BẰNG .HasDefaultValue(0.0)
                entity.Property(e => e.DiscountAmount).HasDefaultValue(0.0);
                entity.Property(e => e.OriginalPrice).HasDefaultValue(0.0);
                entity.Property(e => e.DiscountedTotal).HasDefaultValue(0.0);

                // Relationships
                entity.HasOne(b => b.Customer)
                    .WithMany(c => c.Bills)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Staff)
                    .WithMany(s => s.Bills)
                    .HasForeignKey(b => b.StaffId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BillDetail configuration
            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.ToTable("BillDetail");
                entity.HasKey(e => new { e.Id, e.ProductId });
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.ProductId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();

                // 🟢 XÓA .HasColumnType("decimal(18,2)")
                entity.Property(e => e.Total).HasDefaultValue(0.0);

                // Relationships
                entity.HasOne(bd => bd.Bill)
                    .WithMany(b => b.BillDetails)
                    .HasForeignKey(bd => bd.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bd => bd.Product)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(bd => bd.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

                // 🟢 ĐẢM BẢO KHÔNG CÓ .HasColumnType("decimal(18,2)")
                entity.Property(e => e.Cost).IsRequired();
                entity.Property(e => e.Price).IsRequired();

                entity.Property(e => e.Type).HasMaxLength(50);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Size).HasMaxLength(10); ;
            });
        }
    }
}