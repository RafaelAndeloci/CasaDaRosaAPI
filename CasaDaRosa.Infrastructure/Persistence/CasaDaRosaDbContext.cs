using CasaDaRosa.Domain.Entities.Address;
using CasaDaRosa.Domain.Entities.Cart;
using CasaDaRosa.Domain.Entities.Category;
using CasaDaRosa.Domain.Entities.Order;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CasaDaRosa.Infrastructure.Persistence;

public class CasaDaRosaDbContext(DbContextOptions<CasaDaRosaDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CasaDaRosaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
