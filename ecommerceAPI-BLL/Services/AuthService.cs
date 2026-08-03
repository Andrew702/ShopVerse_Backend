using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.DTOs.Response;
using ecommerceAPI.BLL.Exceptions;
using ecommerceAPI.BLL.Interfaces;
using ecommerceAPI.DAL.Entities;
using ecommerceAPI.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ecommerceAPI.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, UserManager<User> userManager,
        IMapper mapper, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new BadRequestException("Email is already registered.");

        var user = _mapper.Map<User>(request);
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException(errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            throw new BadRequestException($"Failed to assign Customer role: {errors}");
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("Invalid email or password.");

        return await GenerateAuthResponseAsync(user);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var durationHours = double.Parse(_configuration["Jwt:DurationInHours"] ?? "24");
        var expiresAt = DateTime.UtcNow.AddHours(durationHours);
        var token = await GenerateJwtTokenAsync(user, expiresAt);

        var userWithIncludes = await _unitOfWork.Users.GetQueryable()
            .Include(u => u.CartItems).ThenInclude(ci => ci.Product)
            .Include(u => u.Wishlists).ThenInclude(w => w.Product)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        var response = new AuthResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Token = token,
            ExpiresAt = expiresAt
        };

        if (userWithIncludes?.CartItems != null)
            response.CartItems = _mapper.Map<List<CartItemResponse>>(userWithIncludes.CartItems);

        if (userWithIncludes?.Wishlists != null)
            response.Wishlist = userWithIncludes.Wishlists.Select(w => w.ProductId).ToList();

        return response;
    }

    private async Task<string> GenerateJwtTokenAsync(User user, DateTime expiresAt)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
