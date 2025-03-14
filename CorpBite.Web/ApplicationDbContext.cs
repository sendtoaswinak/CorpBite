using Microsoft.EntityFrameworkCore;
using CorpBite.Web.Models;

namespace CorpBite.Web
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<FoodCourt> FoodCourts { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FavoriteItem> FavoriteItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.EmployeeId).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EmployeeId).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Department).HasMaxLength(50).IsRequired();
            });

            // Location configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.BuildingName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasMany(e => e.FoodCourts)
                    .WithOne(e => e.Location)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // FoodCourt configuration
            modelBuilder.Entity<FoodCourt>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ContactNumber).HasMaxLength(15);
                entity.Property(e => e.GSTNumber).HasMaxLength(15);
                entity.HasMany(e => e.MenuItems)
                    .WithOne(e => e.FoodCourt)
                    .HasForeignKey(e => e.FoodCourtId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MenuItem configuration
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Price).HasPrecision(10, 2);
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.SubTotal).HasPrecision(10, 2);
                entity.Property(e => e.TaxAmount).HasPrecision(10, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
                entity.Property(e => e.OrderStatus).HasMaxLength(20);
                entity.Property(e => e.SpecialInstructions).HasMaxLength(500);
                entity.HasMany(e => e.OrderItems)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(10, 2);
                entity.Property(e => e.SpecialInstructions).HasMaxLength(500);
            });

            // Cart configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Cart)
                    .HasForeignKey(e => e.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CartItem configuration
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.Property(e => e.SpecialInstructions).HasMaxLength(500);
            });

            // Feedback configuration
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(500);
            });

            // FavoriteItem configuration
            modelBuilder.Entity<FavoriteItem>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.MenuItemId }).IsUnique();
            });
        }
    }
}