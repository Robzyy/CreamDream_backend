using CreamDream.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Database;

public class CreamDreamDbContext : DbContext
{
    public CreamDreamDbContext(DbContextOptions<CreamDreamDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<AuthorizedUser> AuthorizedUsers { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User inheritance (Table-Per-Type)
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<AuthorizedUser>().ToTable("AuthorizedUsers");
        modelBuilder.Entity<Customer>().ToTable("Customers");

        // Configure Product
        modelBuilder.Entity<Product>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Product>()
            .Property(p => p.Category)
            .HasConversion<int>();
        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Customer
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Order
        modelBuilder.Entity<Order>()
            .HasKey(o => o.Id);
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<int>();
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure OrderItem
        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => oi.Id);
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Add indexes for better query performance
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Category);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.CustomerId);
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Status);
    }
}
