using ecommerceAPI.DAL.Data;
using ecommerceAPI.DAL.Entities;
using ecommerceAPI.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ecommerceAPI.DAL.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        // ===== Roles =====
        if (!await context.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }

        // ===== Categories =====
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "Electronics", Description = "Electronic devices and gadgets", Image = "electronics.jpg" },
                new() { Name = "Clothing", Description = "Apparel and fashion items", Image = "clothing.jpg" },
                new() { Name = "Home & Garden", Description = "Home improvement and garden supplies", Image = "home-garden.jpg" },
                new() { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear", Image = "sports.jpg" },
                new() { Name = "Books", Description = "Books across all genres", Image = "books.jpg" },
                new() { Name = "Beauty & Health", Description = "Beauty products and health supplies", Image = "beauty.jpg" },
                new() { Name = "Toys & Games", Description = "Toys, games and entertainment", Image = "toys.jpg" },
                new() { Name = "Automotive", Description = "Car parts and accessories", Image = "automotive.jpg" }
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // ===== Brands =====
        if (!await context.Brands.AnyAsync())
        {
            var brands = new List<Brand>
            {
                new() { Name = "Apple", Logo = "apple.png" },
                new() { Name = "Samsung", Logo = "samsung.png" },
                new() { Name = "Sony", Logo = "sony.png" },
                new() { Name = "Nike", Logo = "nike.png" },
                new() { Name = "Adidas", Logo = "adidas.png" },
                new() { Name = "Penguin Books", Logo = "penguin.png" },
                new() { Name = "KitchenAid", Logo = "kitchenaid.png" },
                new() { Name = "Wilson", Logo = "wilson.png" },
                new() { Name = "L'Oreal", Logo = "loreal.png" },
                new() { Name = "LEGO", Logo = "lego.png" },
                new() { Name = "Bosch", Logo = "bosch.png" },
                new() { Name = "Dell", Logo = "dell.png" }
            };
            await context.Brands.AddRangeAsync(brands);
            await context.SaveChangesAsync();
        }

        // ===== Products =====
        if (!await context.Products.AnyAsync())
        {
            var categoriesById = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
            var brandsById = await context.Brands.ToDictionaryAsync(b => b.Name, b => b.Id);

            var products = new List<Product>
            {
                // Electronics (Category 1, Brands 1-3, 12) — expensive ones on sale, mid ones not
                new() { Title = "iPhone 15 Pro Max", Description = "Apple's flagship smartphone with A17 Pro chip, 48MP camera, and titanium design.", Price = 1199.99m, Image = "https://picsum.photos/seed/iphone15/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Apple"], StockQuantity = 25, DiscountPercentage = 10.00m, IsOnSale = true },
                new() { Title = "MacBook Air M3", Description = "Ultra-thin laptop with Apple M3 chip, 15-inch Liquid Retina display, 16GB RAM.", Price = 1299.99m, Image = "https://picsum.photos/seed/macbookair/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Apple"], StockQuantity = 18, DiscountPercentage = 12.00m, IsOnSale = true },
                new() { Title = "AirPods Pro 2", Description = "Wireless earbuds with active noise cancellation, adaptive audio, and USB-C charging.", Price = 249.99m, Image = "https://picsum.photos/seed/airpods/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Apple"], StockQuantity = 75, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Samsung Galaxy S24 Ultra", Description = "Samsung's premium smartphone with AI features, S Pen, and 200MP camera.", Price = 1299.99m, Image = "https://picsum.photos/seed/s24ultra/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Samsung"], StockQuantity = 20, DiscountPercentage = 15.00m, IsOnSale = true },
                new() { Title = "Samsung 65\" OLED TV", Description = "65-inch OLED 4K Smart TV with Dolby Atmos, 120Hz refresh rate.", Price = 1799.99m, Image = "https://picsum.photos/seed/samsungtv/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Samsung"], StockQuantity = 12, DiscountPercentage = 10.00m, IsOnSale = true },
                new() { Title = "Samsung Galaxy Watch 6", Description = "Smartwatch with fitness tracking, sleep coaching, and heart rate monitor.", Price = 299.99m, Image = "https://picsum.photos/seed/watch6/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Samsung"], StockQuantity = 60, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Sony WH-1000XM5", Description = "Industry-leading noise canceling headphones with 30-hour battery life.", Price = 349.99m, Image = "https://picsum.photos/seed/sonyxm5/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Sony"], StockQuantity = 45, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Sony PlayStation 5", Description = "Next-gen gaming console with 4K gaming, ray tracing, and ultra-fast SSD.", Price = 499.99m, Image = "https://picsum.photos/seed/ps5/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Sony"], StockQuantity = 0, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Dell XPS 15 Laptop", Description = "15.6-inch 4K OLED laptop with Intel Core i9, 32GB RAM, 1TB SSD.", Price = 1899.99m, Image = "https://picsum.photos/seed/dellxps/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Dell"], StockQuantity = 15, DiscountPercentage = 10.00m, IsOnSale = true },
                new() { Title = "Samsung Galaxy Tab S9", Description = "11-inch Android tablet with S Pen, 120Hz AMOLED display.", Price = 799.99m, Image = "https://picsum.photos/seed/tabs9/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Samsung"], StockQuantity = 35, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Apple iPad Pro M4", Description = "13-inch iPad with M4 chip, Ultra Retina XDR display, Apple Pencil Pro support.", Price = 1099.99m, Image = "https://picsum.photos/seed/ipadpro/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Apple"], StockQuantity = 22, DiscountPercentage = 8.00m, IsOnSale = true },
                new() { Title = "Sony Alpha a7 IV", Description = "Full-frame mirrorless camera with 33MP sensor, 4K 60fps video.", Price = 2499.99m, Image = "https://picsum.photos/seed/sonya7/400/400", CategoryId = categoriesById["Electronics"], BrandId = brandsById["Sony"], StockQuantity = 10, DiscountPercentage = null, IsOnSale = false },

                // Clothing (Category 2, Brands 4-5, 10) — half on sale with 20-30% off
                new() { Title = "Nike Air Max 270", Description = "Men's lifestyle sneakers with Max Air cushioning and breathable mesh upper.", Price = 150.00m, Image = "https://picsum.photos/seed/airmax/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Nike"], StockQuantity = 80, DiscountPercentage = 25.00m, IsOnSale = true },
                new() { Title = "Nike Dri-FIT Training Tee", Description = "Moisture-wicking training t-shirt for intense workouts.", Price = 35.00m, Image = "https://picsum.photos/seed/niketee/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Nike"], StockQuantity = 200, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Nike Tech Fleece Joggers", Description = "Slim-fit joggers with lightweight fleece for warmth without bulk.", Price = 110.00m, Image = "https://picsum.photos/seed/nikejoggers/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Nike"], StockQuantity = 65, DiscountPercentage = 30.00m, IsOnSale = true },
                new() { Title = "Adidas Ultraboost 23", Description = "Women's running shoes with responsive Boost cushioning and Primeknit upper.", Price = 190.00m, Image = "https://picsum.photos/seed/ultraboost/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Adidas"], StockQuantity = 55, DiscountPercentage = 20.00m, IsOnSale = true },
                new() { Title = "Adidas Essentials Hoodie", Description = "Classic pullover hoodie with kangaroo pocket and ribbed cuffs.", Price = 65.00m, Image = "https://picsum.photos/seed/adidashoodie/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Adidas"], StockQuantity = 120, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Adidas Tiro Track Pants", Description = "Slim-fit soccer-inspired track pants with zippered ankles.", Price = 55.00m, Image = "https://picsum.photos/seed/tiro/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Adidas"], StockQuantity = 90, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Nike Pro leggings", Description = "High-waisted women's training leggings with Dri-FIT technology.", Price = 55.00m, Image = "https://picsum.photos/seed/nikeleggings/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Nike"], StockQuantity = 150, DiscountPercentage = 25.00m, IsOnSale = true },
                new() { Title = "Adidas Stan Smith Sneakers", Description = "Iconic tennis-inspired sneakers with premium leather upper.", Price = 100.00m, Image = "https://picsum.photos/seed/stansmith/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Adidas"], StockQuantity = 70, DiscountPercentage = 20.00m, IsOnSale = true },
                new() { Title = "Nike Club Fleece Shorts", Description = "Comfortable cotton-blend fleece shorts with elastic waistband.", Price = 40.00m, Image = "https://picsum.photos/seed/nikeshorts/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Nike"], StockQuantity = 0, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Adidas Running Cap", Description = "Lightweight running cap with moisture-wicking sweatband.", Price = 25.00m, Image = "https://picsum.photos/seed/adidascap/400/400", CategoryId = categoriesById["Clothing"], BrandId = brandsById["Adidas"], StockQuantity = 180, DiscountPercentage = null, IsOnSale = false },

                // Home & Garden (Category 3, Brands 7, 11), 8 — 2 items on 15% sale
                new() { Title = "KitchenAid Stand Mixer", Description = "Artisan Series 5-quart tilt-head stand mixer, perfect for baking.", Price = 449.99m, Image = "https://picsum.photos/seed/mixer/400/400", CategoryId = categoriesById["Home & Garden"], BrandId = brandsById["KitchenAid"], StockQuantity = 30, DiscountPercentage = 15.00m, IsOnSale = true },
                new() { Title = "KitchenAid Blender", Description = "High-performance blender with 5-speed dial and 60oz BPA-free pitcher.", Price = 129.99m, Image = "https://picsum.photos/seed/blender/400/400", CategoryId = categoriesById["Home & Garden"], BrandId = brandsById["KitchenAid"], StockQuantity = 40, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Bosch Dishwasher 300 Series", Description = "24\" stainless steel dishwasher with 44dBA quiet operation.", Price = 899.99m, Image = "https://picsum.photos/seed/dishwasher/400/400", CategoryId = categoriesById["Home & Garden"], BrandId = brandsById["Bosch"], StockQuantity = 10, DiscountPercentage = 15.00m, IsOnSale = true },
                new() { Title = "Bosch Electric Drill Kit", Description = "12V cordless drill driver with 2 batteries and 20 accessories.", Price = 129.99m, Image = "https://picsum.photos/seed/boschdrill/400/400", CategoryId = categoriesById["Home & Garden"], BrandId = brandsById["Bosch"], StockQuantity = 35, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "KitchenAid Cold Brew Maker", Description = "28oz cold brew coffee maker with stainless steel construction.", Price = 79.99m, Image = "https://picsum.photos/seed/coldbrew/400/400", CategoryId = categoriesById["Home & Garden"], BrandId = brandsById["KitchenAid"], StockQuantity = 25, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Bosch Laser Level", Description = "Self-leveling cross-line laser with 65ft range for home projects.", Price = 89.99m, Image = "https://picsum.photos/seed/laser/400/400", CategoryId = categoriesById["Home & Garden"], BrandId = brandsById["Bosch"], StockQuantity = 20, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Garden Tool Set 8-Piece", Description = "Complete garden tool set with ergonomic handles and storage bag.", Price = 49.99m, Image = "https://picsum.photos/seed/gardentools/400/400", CategoryId = categoriesById["Home & Garden"], BrandId = brandsById["Bosch"], StockQuantity = 40, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "KitchenAid Food Processor", Description = "9-cup food processor with ExactSlice system and 3 speeds.", Price = 199.99m, Image = "https://picsum.photos/seed/foodprocessor/400/400", CategoryId = categoriesById["Home & Garden"], BrandId = brandsById["KitchenAid"], StockQuantity = 28, DiscountPercentage = null, IsOnSale = false },

                // Sports & Outdoors (Category 4, Brands 4-5, 8), 8 — no discounts
                new() { Title = "Wilson NFL Football", Description = "Official NFL game football with premium leather and tacky grip.", Price = 79.99m, Image = "https://picsum.photos/seed/football/400/400", CategoryId = categoriesById["Sports & Outdoors"], BrandId = brandsById["Wilson"], StockQuantity = 50, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Wilson Tennis Racket Pro", Description = "Professional tennis racket with graphite frame and 100 sq in head.", Price = 199.99m, Image = "https://picsum.photos/seed/tennis/400/400", CategoryId = categoriesById["Sports & Outdoors"], BrandId = brandsById["Wilson"], StockQuantity = 25, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Nike Yoga Mat", Description = "6mm thick non-slip yoga mat with carrying strap.", Price = 35.00m, Image = "https://picsum.photos/seed/yogamat/400/400", CategoryId = categoriesById["Sports & Outdoors"], BrandId = brandsById["Nike"], StockQuantity = 100, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Nike Resistance Bands Set", Description = "5-piece resistance band set with light to extra-heavy levels.", Price = 29.99m, Image = "https://picsum.photos/seed/bands/400/400", CategoryId = categoriesById["Sports & Outdoors"], BrandId = brandsById["Nike"], StockQuantity = 80, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Wilson Basketball Evolution", Description = "Premium indoor game basketball with moisture-absorbing cover.", Price = 69.99m, Image = "https://picsum.photos/seed/basketball/400/400", CategoryId = categoriesById["Sports & Outdoors"], BrandId = brandsById["Wilson"], StockQuantity = 40, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Adidas Soccer Ball", Description = "FIFA-quality pro match soccer ball with seamless surface.", Price = 39.99m, Image = "https://picsum.photos/seed/soccerball/400/400", CategoryId = categoriesById["Sports & Outdoors"], BrandId = brandsById["Adidas"], StockQuantity = 60, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Nike Running Water Bottle", Description = "24oz squeeze water bottle with quick-shot valve.", Price = 14.99m, Image = "https://picsum.photos/seed/bottle/400/400", CategoryId = categoriesById["Sports & Outdoors"], BrandId = brandsById["Nike"], StockQuantity = 0, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Wilson Badminton Set", Description = "4-player badminton set with 4 rackets, 3 shuttlecocks, and net.", Price = 59.99m, Image = "https://picsum.photos/seed/badminton/400/400", CategoryId = categoriesById["Sports & Outdoors"], BrandId = brandsById["Wilson"], StockQuantity = 30, DiscountPercentage = null, IsOnSale = false },

                // Books (Category 5, Brand 6), 8 — 3 items on 5-10% sale
                new() { Title = "The Great Gatsby", Description = "F. Scott Fitzgerald's classic novel of the Jazz Age.", Price = 14.99m, Image = "https://picsum.photos/seed/gatsby/400/400", CategoryId = categoriesById["Books"], BrandId = brandsById["Penguin Books"], StockQuantity = 300, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "1984 by George Orwell", Description = "The dystopian masterpiece that remains relevant decades later.", Price = 12.99m, Image = "https://picsum.photos/seed/1984/400/400", CategoryId = categoriesById["Books"], BrandId = brandsById["Penguin Books"], StockQuantity = 250, DiscountPercentage = 5.00m, IsOnSale = true },
                new() { Title = "To Kill a Mockingbird", Description = "Harper Lee's Pulitzer Prize-winning novel about racial injustice.", Price = 15.99m, Image = "https://picsum.photos/seed/mockingbird/400/400", CategoryId = categoriesById["Books"], BrandId = brandsById["Penguin Books"], StockQuantity = 200, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "The Art of Programming", Description = "Comprehensive guide to software engineering principles and practices.", Price = 49.99m, Image = "https://picsum.photos/seed/programming/400/400", CategoryId = categoriesById["Books"], BrandId = brandsById["Penguin Books"], StockQuantity = 100, DiscountPercentage = 10.00m, IsOnSale = true },
                new() { Title = "Cookbook: World Flavors", Description = "200+ recipes from around the world, perfect for home cooks.", Price = 29.99m, Image = "https://picsum.photos/seed/cookbook/400/400", CategoryId = categoriesById["Books"], BrandId = brandsById["Penguin Books"], StockQuantity = 150, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Atomic Habits", Description = "James Clear's guide to building good habits and breaking bad ones.", Price = 16.99m, Image = "https://picsum.photos/seed/habits/400/400", CategoryId = categoriesById["Books"], BrandId = brandsById["Penguin Books"], StockQuantity = 500, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "The Hobbit", Description = "J.R.R. Tolkien's classic fantasy adventure of Bilbo Baggins.", Price = 13.99m, Image = "https://picsum.photos/seed/hobbit/400/400", CategoryId = categoriesById["Books"], BrandId = brandsById["Penguin Books"], StockQuantity = 180, DiscountPercentage = 8.00m, IsOnSale = true },
                new() { Title = "Sapiens: A Brief History", Description = "Yuval Noah Harari's bestselling history of humankind.", Price = 18.99m, Image = "https://picsum.photos/seed/sapiens/400/400", CategoryId = categoriesById["Books"], BrandId = brandsById["Penguin Books"], StockQuantity = 220, DiscountPercentage = null, IsOnSale = false },

                // Beauty & Health (Category 6, Brand 9), 6 — 2 items on 25% sale
                new() { Title = "L'Oreal Vitamin C Serum", Description = "10% pure Vitamin C serum for brighter, smoother skin.", Price = 29.99m, Image = "https://picsum.photos/seed/serum/400/400", CategoryId = categoriesById["Beauty & Health"], BrandId = brandsById["L'Oreal"], StockQuantity = 60, DiscountPercentage = 25.00m, IsOnSale = true },
                new() { Title = "L'Oreal Mascara Volume", Description = "Volumizing mascara with lash-lifting formula, waterproof.", Price = 12.99m, Image = "https://picsum.photos/seed/mascara/400/400", CategoryId = categoriesById["Beauty & Health"], BrandId = brandsById["L'Oreal"], StockQuantity = 40, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "L'Oreal Revitalift Cream", Description = "Anti-aging day cream with Pro-Retinol and SPF 30.", Price = 24.99m, Image = "https://picsum.photos/seed/cream/400/400", CategoryId = categoriesById["Beauty & Health"], BrandId = brandsById["L'Oreal"], StockQuantity = 45, DiscountPercentage = 25.00m, IsOnSale = true },
                new() { Title = "L'Oreal Hair Color Kit", Description = "Permanent hair color with pro-keratin complex, 30 shades.", Price = 14.99m, Image = "https://picsum.photos/seed/haircolor/400/400", CategoryId = categoriesById["Beauty & Health"], BrandId = brandsById["L'Oreal"], StockQuantity = 55, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "L'Oreal Men Expert Face Wash", Description = "Charcoal-infused face wash for deep pore cleansing.", Price = 9.99m, Image = "https://picsum.photos/seed/facewash/400/400", CategoryId = categoriesById["Beauty & Health"], BrandId = brandsById["L'Oreal"], StockQuantity = 30, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "L'Oreal Lipstick Color Riche", Description = "Creamy, long-lasting lipstick with 20+ vibrant shades.", Price = 11.99m, Image = "https://picsum.photos/seed/lipstick/400/400", CategoryId = categoriesById["Beauty & Health"], BrandId = brandsById["L'Oreal"], StockQuantity = 50, DiscountPercentage = null, IsOnSale = false },

                // Toys & Games (Category 7, Brand 10), 6 — 2 items on 15-20% sale
                new() { Title = "LEGO Star Wars Millennium Falcon", Description = "1351-piece building set of the iconic starship from Star Wars.", Price = 149.99m, Image = "https://picsum.photos/seed/legostarwars/400/400", CategoryId = categoriesById["Toys & Games"], BrandId = brandsById["LEGO"], StockQuantity = 20, DiscountPercentage = 15.00m, IsOnSale = true },
                new() { Title = "LEGO City Police Station", Description = "668-piece police station with helicopter, police car, and 5 minifigures.", Price = 69.99m, Image = "https://picsum.photos/seed/legopolice/400/400", CategoryId = categoriesById["Toys & Games"], BrandId = brandsById["LEGO"], StockQuantity = 30, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "LEGO Technic Bugatti", Description = "3599-piece 1:8 scale model of the Bugatti Chiron hypercar.", Price = 349.99m, Image = "https://picsum.photos/seed/legotechnic/400/400", CategoryId = categoriesById["Toys & Games"], BrandId = brandsById["LEGO"], StockQuantity = 10, DiscountPercentage = 20.00m, IsOnSale = true },
                new() { Title = "LEGO Classic Creative Bricks", Description = "790-piece creative brick set with 33 colors for open-ended building.", Price = 49.99m, Image = "https://picsum.photos/seed/legoclassic/400/400", CategoryId = categoriesById["Toys & Games"], BrandId = brandsById["LEGO"], StockQuantity = 25, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "LEGO Friends Heartlake City", Description = "987-piece friendship-themed building set with 4 mini-dolls.", Price = 79.99m, Image = "https://picsum.photos/seed/legofriends/400/400", CategoryId = categoriesById["Toys & Games"], BrandId = brandsById["LEGO"], StockQuantity = 18, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "LEGO Duplo Train Set", Description = "23-piece toddler train set with push-and-go motor and tracks.", Price = 19.99m, Image = "https://picsum.photos/seed/legoduplo/400/400", CategoryId = categoriesById["Toys & Games"], BrandId = brandsById["LEGO"], StockQuantity = 15, DiscountPercentage = null, IsOnSale = false },

                // Automotive (Category 8, Brand 11), 4 — no discounts
                new() { Title = "Bosch ICON Wiper Blades", Description = "Premium beam wiper blades, 20% longer life, fits most cars.", Price = 24.99m, Image = "https://picsum.photos/seed/wipers/400/400", CategoryId = categoriesById["Automotive"], BrandId = brandsById["Bosch"], StockQuantity = 50, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Bosch Car Battery Charger", Description = "6/12V automatic smart battery charger and maintainer.", Price = 59.99m, Image = "https://picsum.photos/seed/charger/400/400", CategoryId = categoriesById["Automotive"], BrandId = brandsById["Bosch"], StockQuantity = 30, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Bosch Automotive Diagnostic Tool", Description = "OBD2 scanner with Bluetooth connectivity and live data display.", Price = 79.99m, Image = "https://picsum.photos/seed/obd2/400/400", CategoryId = categoriesById["Automotive"], BrandId = brandsById["Bosch"], StockQuantity = 20, DiscountPercentage = null, IsOnSale = false },
                new() { Title = "Bosch Cabin Air Filter", Description = "HEPA cabin air filter with activated carbon for fresh air.", Price = 19.99m, Image = "https://picsum.photos/seed/filter/400/400", CategoryId = categoriesById["Automotive"], BrandId = brandsById["Bosch"], StockQuantity = 45, DiscountPercentage = null, IsOnSale = false },
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
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

        // ===== Reviews =====
        if (!await context.Reviews.AnyAsync())
        {
            var rng = new Random(42);
            var userIds = await context.Users.Select(u => u.Id).ToListAsync();
            var productIds = await context.Products.Select(p => p.Id).ToListAsync();
            var reviewerNames = new[] { "TechGuru42", "FashionFan99", "BookWorm123", "HomeChefPro", "AthleticAce",
                                        "BeautyBoss", "GameMasterX", "CarEnthusiast", "HonestReviewer", "DailyShopper" };
            var comments = new[]
            {
                "Excellent product! Exceeded my expectations. Highly recommend to everyone.",
                "Good value for money. Works as advertised. Delivery was fast too.",
                "Decent quality but could be better. The packaging was a bit damaged.",
                "Absolutely love this! Best purchase I've made this year.",
                "It's okay for the price. Nothing spectacular but gets the job done.",
                "Amazing quality! Will definitely buy again. Five stars!",
                "Not bad. I've seen better but this is a solid mid-range option.",
                "Perfect gift for my family. They absolutely loved it!",
                "Good product but the shipping took longer than expected.",
                "Outstanding quality and great customer support. 10/10 would recommend.",
                "Was skeptical at first but it turned out to be great quality.",
                "Exactly as described. Fits perfectly and works flawlessly.",
                "A bit pricey but the quality makes up for it. Very satisfied.",
                "One of the best in its category. You won't be disappointed.",
                "Solid product overall. Minor issues but nothing deal-breaking."
            };

            var reviews = new List<Review>();
            foreach (var productId in productIds)
            {
                var reviewCount = rng.Next(1, 4);
                for (int i = 0; i < reviewCount; i++)
                {
                    reviews.Add(new Review
                    {
                        Rating = Math.Round((decimal)(rng.NextDouble() * 4 + 1), 1),
                        Comment = comments[rng.Next(comments.Length)],
                        Date = DateTime.UtcNow.AddDays(-rng.Next(1, 180)),
                        ReviewerName = reviewerNames[rng.Next(reviewerNames.Length)],
                        ProductId = productId
                    });
                }
            }
            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();
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
}
