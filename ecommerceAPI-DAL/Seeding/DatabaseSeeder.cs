using System.Globalization;
using System.Text;
using System.Text.Json;
using ecommerceAPI.DAL.Data;
using ecommerceAPI.DAL.Entities;
using ecommerceAPI.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ecommerceAPI.DAL.Seeding;

public static class DatabaseSeeder
{
    private const string DummyJsonApiUrl = "https://dummyjson.com/products?limit=0";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager, IHttpClientFactory httpClientFactory)
    {
        await context.Database.MigrateAsync();

        // ===== Roles =====
        if (!await context.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }

        // ===== Categories, Brands, Products & Reviews (seeded from the DummyJSON API) =====
        if (!await context.Products.AnyAsync())
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var dummyProducts = await FetchDummyJsonProductsAsync(httpClient);

            // ----- Categories -----
            if (!await context.Categories.AnyAsync())
            {
                var categories = dummyProducts
                    .Select(p => p.Category)
                    .Distinct()
                    .Select(slug => new Category
                    {
                        Name = HumanizeCategory(slug),
                        Image = $"https://picsum.photos/seed/category-{slug}/400/400"
                    })
                    .ToList();

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // ----- Brands -----
            if (!await context.Brands.AnyAsync())
            {
                var brands = dummyProducts
                    .Select(p => ResolveBrandName(p.Brand))
                    .Distinct()
                    .Select(name => new Brand
                    {
                        Name = name,
                        Logo = $"https://picsum.photos/seed/brand-{Slugify(name)}/400/400"
                    })
                    .ToList();

                await context.Brands.AddRangeAsync(brands);
                await context.SaveChangesAsync();
            }

            var categoriesById = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
            var brandsById = await context.Brands.ToDictionaryAsync(b => b.Name, b => b.Id);

            // ----- Products -----
            var products = new List<Product>(dummyProducts.Count);
            foreach (var dummy in dummyProducts)
            {
                products.Add(new Product
                {
                    Title = dummy.Title,
                    Description = dummy.Description,
                    Price = dummy.Price,
                    Image = dummy.Thumbnail,
                    DiscountPercentage = dummy.DiscountPercentage > 0 ? dummy.DiscountPercentage : null,
                    IsOnSale = dummy.DiscountPercentage > 0,
                    StockQuantity = dummy.Stock,
                    CategoryId = categoriesById[HumanizeCategory(dummy.Category)],
                    BrandId = brandsById[ResolveBrandName(dummy.Brand)]
                });
            }

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // ----- Reviews -----
            if (!await context.Reviews.AnyAsync())
            {
                var reviews = new List<Review>();
                for (var i = 0; i < dummyProducts.Count; i++)
                {
                    if (dummyProducts[i].Reviews is not { Count: > 0 }) continue;

                    foreach (var r in dummyProducts[i].Reviews)
                    {
                        reviews.Add(new Review
                        {
                            Rating = r.Rating,
                            Comment = r.Comment,
                            Date = r.Date,
                            ReviewerName = r.ReviewerName,
                            ProductId = products[i].Id
                        });
                    }
                }

                await context.Reviews.AddRangeAsync(reviews);
                await context.SaveChangesAsync();
            }
        }

        // ===== Users =====
        if (!await context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = "admin@ecommerce.com",
                PhoneNumber = "01000000001",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");

            var demoUsers = new List<(User user, string password)>
            {
                (new User { UserName = "john_doe", Email = "john@test.com", PhoneNumber = "0111111111", EmailConfirmed = true }, "P@ssw0rd1"),
                (new User { UserName = "jane_smith", Email = "jane@test.com", PhoneNumber = "0111111112", EmailConfirmed = true }, "P@ssw0rd2"),
                (new User { UserName = "bob_wilson", Email = "bob@test.com", PhoneNumber = "0111111113", EmailConfirmed = true }, "P@ssw0rd3"),
                (new User { UserName = "alice_brown", Email = "alice@test.com", PhoneNumber = "0111111114", EmailConfirmed = true }, "P@ssw0rd4"),
                (new User { UserName = "mike_davis", Email = "mike@test.com", PhoneNumber = "0111111115", EmailConfirmed = true }, "P@ssw0rd5"),
            };

            foreach (var (user, password) in demoUsers)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Customer");
            }
        }

        // ===== Sample Cart Items =====
        if (!await context.CartItems.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            var products = await context.Products.ToListAsync();
            var rng = new Random(123);

            foreach (var user in users.Take(3))
            {
                var cartItemCount = rng.Next(2, 5);
                var shuffled = products.OrderBy(_ => rng.Next()).Take(cartItemCount);
                foreach (var product in shuffled)
                {
                    await context.CartItems.AddAsync(new CartItem
                    {
                        UserId = user.Id,
                        ProductId = product.Id,
                        Quantity = rng.Next(1, 4)
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        // ===== Sample Orders =====
        if (!await context.Orders.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            var products = await context.Products.ToListAsync();
            var rng = new Random(456);

            foreach (var user in users.Take(3))
            {
                var orderCount = rng.Next(2, 5);
                for (int o = 0; o < orderCount; o++)
                {
                    var statuses = new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered };
                    var itemsCount = rng.Next(1, 4);
                    var selectedProducts = products.OrderBy(_ => rng.Next()).Take(itemsCount).ToList();
                    var order = new Order
                    {
                        UserId = user.Id,
                        Total = selectedProducts.Sum(p => p.Price * rng.Next(1, 3)),
                        Date = DateTime.UtcNow.AddDays(-rng.Next(1, 90)),
                        Status = statuses[rng.Next(statuses.Length)]
                    };

                    await context.Orders.AddAsync(order);
                    await context.SaveChangesAsync();

                    foreach (var product in selectedProducts)
                    {
                        await context.OrderItems.AddAsync(new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = product.Id,
                            Quantity = rng.Next(1, 3),
                            UnitPrice = product.Price
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        // ===== Sample Wishlist Items =====
        if (!await context.Wishlists.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            var products = await context.Products.ToListAsync();
            var rng = new Random(789);

            foreach (var user in users.Take(3))
            {
                var wishlistCount = rng.Next(3, 7);
                var shuffled = products.OrderBy(_ => rng.Next()).Take(wishlistCount);
                foreach (var product in shuffled)
                {
                    var exists = await context.Wishlists.AnyAsync(w => w.UserId == user.Id && w.ProductId == product.Id);
                    if (!exists)
                    {
                        await context.Wishlists.AddAsync(new Wishlist
                        {
                            UserId = user.Id,
                            ProductId = product.Id
                        });
                    }
                }
            }
            await context.SaveChangesAsync();
        }
    }

    // ===== DummyJSON helpers =====

    private static async Task<List<DummyJsonProduct>> FetchDummyJsonProductsAsync(HttpClient httpClient)
    {
        Exception? lastError = null;

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                var json = await httpClient.GetStringAsync(DummyJsonApiUrl);
                var response = JsonSerializer.Deserialize<DummyJsonResponse>(json, JsonOptions);
                if (response is { Products.Count: > 0 })
                    return response.Products;

                throw new InvalidOperationException("DummyJSON API returned an empty product list.");
            }
            catch (Exception ex)
            {
                lastError = ex;
                if (attempt < 3)
                    await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        throw new InvalidOperationException(
            "Failed to fetch products from the DummyJSON API after 3 attempts. Check network connectivity and try again.",
            lastError);
    }

    private static string HumanizeCategory(string slug) =>
        CultureInfo.InvariantCulture.TextInfo.ToTitleCase(slug.Replace("-", " "));

    private static string ResolveBrandName(string? brand) =>
        string.IsNullOrWhiteSpace(brand) ? "Generic" : brand;

    private static string Slugify(string name)
    {
        var builder = new StringBuilder();
        foreach (var ch in name.ToLowerInvariant())
            builder.Append(char.IsLetterOrDigit(ch) ? ch : '-');
        return builder.ToString();
    }

    // ===== DummyJSON DTOs =====

    private record DummyJsonResponse(List<DummyJsonProduct> Products, int Total, int Skip, int Limit);

    private record DummyJsonProduct(
        string Title,
        string Description,
        decimal Price,
        decimal DiscountPercentage,
        int Stock,
        string Category,
        string? Brand,
        string Thumbnail,
        List<DummyJsonReview> Reviews);

    private record DummyJsonReview(int Rating, string Comment, DateTime Date, string ReviewerName);
}
