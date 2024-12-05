using Microsoft.EntityFrameworkCore;
using OrderModule.Entities;

namespace OrderModule.Data
{
    public class OrderModuleDbContext : DbContext
    {
        public OrderModuleDbContext(DbContextOptions<OrderModuleDbContext> options)
            : base(options) { }

        // Veritabanı tablolarını temsil eden DbSet'ler
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // İlişkileri ve kuralları tanımlayın
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.Addresses)
                .WithOne(a => a.Member)
                .HasForeignKey(a => a.MemberId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Taxes)
                .WithOne(t => t.Product)
                .HasForeignKey(t => t.ProductId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
