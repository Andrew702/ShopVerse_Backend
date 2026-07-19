using ecommerceAPI.DAL.Data;
using ecommerceAPI.DAL.Entities;
using ecommerceAPI.DAL.Interfaces;
using ecommerceAPI.DAL.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ecommerceAPI.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private IRepository<Product>? _products;
    private IRepository<Review>? _reviews;
    private IRepository<Order>? _orders;
    private IRepository<OrderItem>? _orderItems;
    private IRepository<CartItem>? _cartItems;
    private IRepository<Wishlist>? _wishlists;
    private IRepository<Category>? _categories;
    private IRepository<Brand>? _brands;
    private IRepository<User>? _users;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<Product> Products =>
        _products ??= new Repository<Product>(_context);

    public IRepository<Review> Reviews =>
        _reviews ??= new Repository<Review>(_context);

    public IRepository<Order> Orders =>
        _orders ??= new Repository<Order>(_context);

    public IRepository<OrderItem> OrderItems =>
        _orderItems ??= new Repository<OrderItem>(_context);

    public IRepository<CartItem> CartItems =>
        _cartItems ??= new Repository<CartItem>(_context);

    public IRepository<Wishlist> Wishlists =>
        _wishlists ??= new Repository<Wishlist>(_context);

    public IRepository<Category> Categories =>
        _categories ??= new Repository<Category>(_context);

    public IRepository<Brand> Brands =>
        _brands ??= new Repository<Brand>(_context);

    public IRepository<User> Users =>
        _users ??= new Repository<User>(_context);

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
        => _transaction = await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
            await _transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
            await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
        }
    }
}
