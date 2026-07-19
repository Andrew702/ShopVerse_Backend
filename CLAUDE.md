# ecommerceAPI — 3-Layer Architecture Backend

## Project Overview

A **.NET 10 C# Web API** for an ecommerce platform consumed by an Angular frontend. Uses **ASP.NET Core Identity**, **JWT Bearer authentication**, **Entity Framework Core 10** with **SQL Server**, and follows a clean **3-layer architecture**: API → BLL → DAL.

- **Target Framework:** .NET 10
- **Database:** SQL Server (LocalDB / SQLEXPRESS)
- **Auth:** JWT Bearer tokens + ASP.NET Core Identity
- **API Docs:** Swagger UI via OpenAPI
- **Mapping:** AutoMapper 13
- **Validation:** FluentValidation 11

---

## File Hierarchy

```
ecommerce-API/
├── ecommerceAPI.slnx                          # Solution file (3 projects)
│
├── ecommerceAPI/                              # 🟢 API Layer (ASP.NET Core Web API)
│   ├── Controllers/
│   │   ├── AuthController.cs                  # POST /api/auth/register, /api/auth/login
│   │   ├── ProductController.cs               # GET /api/products (paginated, filtered)
│   │   ├── CartController.cs                  # CRUD /api/cart [Authorize]
│   │   ├── OrderController.cs                 # POST/GET /api/orders [Authorize]
│   │   └── WishlistController.cs              # CRUD /api/wishlist [Authorize]
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs             # Global error handling (ProblemDetails)
│   ├── Program.cs                             # DI composition root, middleware pipeline
│   └── appsettings.json                       # Connection strings, JWT config
│
├── ecommerceAPI-BLL/                          # 🔵 Business Logic Layer (Class Library)
│   ├── DTOs/
│   │   ├── Request/                           # Input DTOs (Register, Login, Cart, etc.)
│   │   └── Response/                          # Output DTOs (Auth, Product, Order, etc.)
│   ├── Exceptions/
│   │   └── AppException.cs                    # NotFound, BadRequest, Unauthorized exceptions
│   ├── Extensions/
│   │   └── ServiceRegistration.cs             # DI extension: AddBLLServices()
│   ├── Interfaces/                            # Service contracts
│   │   ├── IAuthService.cs
│   │   ├── IProductService.cs
│   │   ├── ICartService.cs
│   │   ├── IOrderService.cs
│   │   └── IWishlistService.cs
│   ├── MappingProfiles/
│   │   └── MappingProfile.cs                  # AutoMapper: Entity ↔ DTO
│   ├── Services/                              # Business logic implementations
│   │   ├── AuthService.cs                     # Register, Login, JWT token generation
│   │   ├── ProductService.cs                  # Product queries with pagination
│   │   ├── CartService.cs                     # Incremental cart management
│   │   ├── OrderService.cs                    # Order creation with price snapshots
│   │   └── WishlistService.cs                 # Wishlist CRUD
│   └── Validators/                            # FluentValidation validators
│       ├── RegisterRequestValidator.cs
│       ├── LoginRequestValidator.cs
│       ├── AddToCartRequestValidator.cs
│       └── UpdateCartItemRequestValidator.cs
│
├── ecommerceAPI-DAL/                          # 🟡 Data Access Layer (Class Library)
│   ├── Data/
│   │   └── AppDbContext.cs                    # EF Core DbContext (IdentityDbContext<User>)
│   ├── Entities/                              # EF Core entity classes
│   │   ├── User.cs                            # Extends IdentityUser
│   │   ├── Product.cs                         # Title, Price, Category FK, Brand FK
│   │   ├── Category.cs                        # Name, Description
│   │   ├── Brand.cs                           # Name, Logo
│   │   ├── Review.cs                          # Rating, Comment, FK → Product
│   │   ├── Order.cs                           # Total, Status (enum), FK → User
│   │   ├── OrderItem.cs                       # Quantity, UnitPrice, FKs
│   │   ├── CartItem.cs                        # Quantity, FK → User, FK → Product
│   │   └── Wishlist.cs                        # FK → User, FK → Product (unique)
│   ├── Enums/
│   │   └── OrderStatus.cs                     # Pending, Processing, Shipped, Delivered, Cancelled
│   ├── Interfaces/
│   │   ├── IRepository.cs                     # Generic repository contract
│   │   └── IUnitOfWork.cs                     # Unit of Work contract
│   ├── Repositories/
│   │   └── Repository.cs                      # Generic repository implementation
│   ├── UnitOfWork/
│   │   └── UnitOfWork.cs                      # UoW with lazy-loaded repositories
│   └── Seeding/
│       └── DatabaseSeeder.cs                  # Seeds 60+ products, users, reviews, orders
│
└── CLAUDE.md                                  # This file — AI documentation
```

---

## Architecture Rules

### Layer Dependency
```
API ──→ BLL ──→ DAL
```
- **API** references BLL (and transitively DAL)
- **BLL** references DAL
- **DAL** has no external dependencies (only EF Core + Identity NuGet packages)

### What Goes Where

| Layer | Contains | Must NOT contain |
|-------|----------|-----------------|
| **API** | Controllers, Middleware, Program.cs, appsettings | Business logic, direct DbContext access |
| **BLL** | Services, DTOs, Validators, AutoMapper profiles | HTTP concerns, direct DbContext access |
| **DAL** | Entities, DbContext, Repositories, UnitOfWork, Seeder | Business logic, validation, DTOs |

### Key Patterns
- **Repository + Unit of Work:** All data access goes through `IUnitOfWork` → `IRepository<T>`. Never inject `AppDbContext` into services.
- **DTO Mapping:** All API responses are DTOs mapped via AutoMapper. Never return entities from controllers.
- **Validation:** FluentValidation validators run automatically before controller actions via `AddFluentValidationAutoValidation()`.
- **Error Handling:** Services throw `NotFoundException`, `BadRequestException`, `UnauthorizedException`. The `ExceptionMiddleware` catches them globally and returns ProblemDetails JSON.
- **Auth:** All user-specific endpoints use `[Authorize]`. Extract user ID from JWT: `User.FindFirstValue(ClaimTypes.NameIdentifier)`.

---

## Services Reference

| Interface | Implementation | Description |
|-----------|---------------|-------------|
| `IAuthService` | `AuthService` | User registration, login, JWT token generation. Injects `UserManager<User>`, `IUnitOfWork`, `IMapper`, `IConfiguration`. |
| `IProductService` | `ProductService` | Product listing with pagination/filtering/search, categories, brands. Injects `IUnitOfWork`, `IMapper`. |
| `ICartService` | `CartService` | Incremental cart: add (or increment qty), update qty, remove item, clear. Injects `IUnitOfWork`, `IMapper`. |
| `IOrderService` | `OrderService` | Create order from cart (snapshots prices), view orders, update status. Injects `IUnitOfWork`, `IMapper`. |
| `IWishlistService` | `WishlistService` | Add/remove/view wishlist. Deduplicates (idempotent add). Injects `IUnitOfWork`, `IMapper`. |

---

## Controllers & Endpoints

### AuthController — `api/auth`
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/register` | Anonymous | Register new user, returns JWT + profile |
| POST | `/api/auth/login` | Anonymous | Login, returns JWT + profile |

### ProductController — `api/products`
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/products` | Anonymous | Paginated list (`?page=1&pageSize=12&search=&categoryId=&brandId=`) |
| GET | `/api/products/{id}` | Anonymous | Product detail with reviews |
| GET | `/api/products/category/{id}` | Anonymous | Filter by category |
| GET | `/api/products/brand/{id}` | Anonymous | Filter by brand |
| GET | `/api/products/search?q=` | Anonymous | Text search (title + description) |
| GET | `/api/products/categories` | Anonymous | All categories |
| GET | `/api/products/brands` | Anonymous | All brands |

### CartController — `api/cart` [Authorize]
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/cart` | Get current user's cart |
| POST | `/api/cart/items` | Add item (increments qty if exists) |
| PUT | `/api/cart/items/{cartItemId}` | Update item quantity |
| DELETE | `/api/cart/items/{cartItemId}` | Remove item |
| DELETE | `/api/cart` | Clear entire cart |

### OrderController — `api/orders` [Authorize]
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/orders` | Create order from cart |
| GET | `/api/orders` | User's order history |
| GET | `/api/orders/{orderId}` | Order detail |

### WishlistController — `api/wishlist` [Authorize]
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/wishlist` | Get wishlist |
| POST | `/api/wishlist/{productId}` | Add to wishlist (idempotent) |
| DELETE | `/api/wishlist/{productId}` | Remove from wishlist |

---

## Entity Relationship Diagram

```
User (IdentityUser, PK: string Id)
  ├── 1:many → Order (PK: int Id)
  │              ├── FK: UserId
  │              ├── Status: string (Pending/Processing/Shipped/Delivered/Cancelled)
  │              └── 1:many → OrderItem (PK: int Id)
  │                             ├── FK: OrderId, ProductId
  │                             └── UnitPrice: decimal (snapshot)
  ├── 1:many → CartItem (PK: int Id)
  │              ├── FK: UserId, ProductId
  │              └── Quantity: int
  └── 1:many → Wishlist (PK: int Id)
                 ├── FK: UserId, ProductId
                 └── UNIQUE(UserId, ProductId)

Product (PK: int Id)
  ├── FK: CategoryId → Category (PK: int Id, Name unique)
  ├── FK: BrandId → Brand (PK: int Id, Name unique)
  ├── Price: decimal(18,2)
  └── 1:many → Review (PK: int Id, Rating: decimal(3,1))
```

---

## Coding Conventions

### Naming
- **PascalCase** for all: classes, properties, methods, interfaces (prefix `I`), files
- **camelCase** only for private fields and local variables
- File names match the class name exactly (e.g., `ProductService.cs` for `ProductService` class)
- Controllers named with `Controller` suffix; route uses the prefix (e.g., `ProductController` → `/api/product`)

### Async Patterns
- **All database operations must be async** — use `await` with `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`, etc.
- All service methods return `Task<T>` or `Task`
- All controller actions are `async Task<IActionResult>`
- Never use `.Result`, `.Wait()`, or sync `SaveChanges()`

### Dependency Injection
- Always use constructor injection
- Register services as **Scoped** (per-request lifetime)
- Use the `AddBLLServices()` extension method for BLL registration
- Controllers only depend on service interfaces

### DTOs
- **Request DTOs** go in `BLL/DTOs/Request/` (input from client)
- **Response DTOs** go in `BLL/DTOs/Response/` (output to client)
- DTO property names should match the JSON shape the Angular frontend expects
- Use `= string.Empty` for all string defaults to avoid null reference warnings
- Use `= new List<T>()` for all collection defaults

---

## Common Tasks

### Add a New Entity
1. Create entity class in `DAL/Entities/`
2. Add `DbSet<T>` to `AppDbContext.cs`
3. Configure relationships in `AppDbContext.OnModelCreating()`
4. Add repository via `IUnitOfWork` (add lazy getter + interface property)
5. Create DTOs in `BLL/DTOs/Request/` and/or `BLL/DTOs/Response/`
6. Create service interface in `BLL/Interfaces/`
7. Create service implementation in `BLL/Services/`
8. Add AutoMapper mapping in `MappingProfile.cs`
9. Create validator in `BLL/Validators/` (if needed)
10. Register service in `ServiceRegistration.cs`
11. Create controller in `API/Controllers/`
12. Add seed data in `DatabaseSeeder.cs`

### Add a Migration
```powershell
# From solution root
dotnet ef migrations add MigrationName --project ecommerceAPI-DAL --startup-project ecommerceAPI

# Apply
dotnet ef database update --project ecommerceAPI-DAL --startup-project ecommerceAPI
```

### Run the App
```powershell
dotnet run --project ecommerceAPI
# The seeder runs automatically on first startup
# Swagger UI: https://localhost:xxxx/swagger/index.html
```

### Add a New Endpoint
1. Add method to the appropriate service interface in `BLL/Interfaces/`
2. Implement the method in the service in `BLL/Services/` (business logic only)
3. Add action method to the controller in `API/Controllers/` (HTTP only — call service, return result)
4. Use `[Authorize]` if the endpoint requires authentication
5. Extract user ID from JWT: `User.FindFirstValue(ClaimTypes.NameIdentifier)!`

---

## AI Rules

When working on this codebase, any AI (Claude, Copilot, etc.) MUST follow these rules:

1. **Always use async/await** — never call sync EF methods like `SaveChanges()`, `ToList()`, `FirstOrDefault()`
2. **Never return entities from controllers** — always map to DTOs via `_mapper.Map<T>()`
3. **Never inject AppDbContext into services or controllers** — use `IUnitOfWork` instead
4. **Always validate input** — create FluentValidation validators for request DTOs; they run automatically
5. **Use `IUnitOfWork` for data access** — call `CompleteAsync()` to persist changes
6. **Throw custom exceptions** from services: `NotFoundException`, `BadRequestException`, `UnauthorizedException` — the middleware handles them
7. **Add `[Authorize]` to protected endpoints** — use `ClaimTypes.NameIdentifier` for current user ID
8. **No client-provided IDs** — all primary keys are auto-generated (int identity or Identity GUID)
9. **PascalCase everywhere** — class names, property names, method names, file names
10. **New files go in the correct layer** — entities in DAL, business logic in BLL, HTTP in API
11. **Update this CLAUDE.md** when adding new services, controllers, or significant patterns
12. **Always seed new entities** in `DatabaseSeeder.cs` so the database has realistic demo data

---

## Demo Users (from Seeder)

| Username | Email | Password | Role |
|----------|-------|----------|------|
| admin | admin@ecommerce.com | Admin@123 | Admin |
| john_doe | john@test.com | P@ssw0rd1 | Customer |
| jane_smith | jane@test.com | P@ssw0rd2 | Customer |
| bob_wilson | bob@test.com | P@ssw0rd3 | Customer |
| alice_brown | alice@test.com | P@ssw0rd4 | Customer |
| mike_davis | mike@test.com | P@ssw0rd5 | Customer |
