using ecommerceAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ecommerceAPI.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().Property(u=>u.Id).ValueGeneratedNever();

            builder.Entity<cartItems>().Property(u => u.id).ValueGeneratedNever();

            builder.Entity<cartItems>().HasKey(c=>c.id);

            builder.Entity<Order>().Property(u => u.id).ValueGeneratedNever();

            builder.Entity<Order>().HasKey(c => c.id);

            builder.Entity<orderItems>().Property(u => u.id).ValueGeneratedNever();

            builder.Entity<orderItems>().HasKey(c => c.id);



            base.OnModelCreating(builder);

        }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<orderItems> OrderItems { get; set; }

        public virtual DbSet<cartItems> CartItems { get; set; }

        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<Product> Product { get; set; }

        public virtual DbSet<review> review { get; set; }

        public virtual DbSet<Wishlist> Wishlists { get; set; }

    }
}
