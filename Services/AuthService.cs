using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxAccount.Data;
using TaxAccount.DTOs;
using TaxAccount.Exceptions;
using TaxAccount.Models;

namespace TaxAccount.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly DataSeeder _seeder;

        public AuthService(
            AppDbContext context,
            IConfiguration config,
            ILogger<AuthService> logger,
            DataSeeder seeder)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _seeder = seeder;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Check email uniqueness before starting transaction
            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (existingUser)
                throw new AppException("Email already registered", 409);

            // Begin atomic transaction
            await using var transaction = await _context.Database
                .BeginTransactionAsync();

            try
            {
                // Step 1: Create Tenant
                var tenant = new Tenant
                {
                    CompanyName = dto.CompanyName,
                    Email = dto.CompanyEmail,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();

                // Step 2: Create Owner User
                var passwordHash = BCrypt.Net.BCrypt
                    .HashPassword(dto.Password);

                var user = new User
                {
                    TenantId = tenant.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    RoleId = 1, // Owner
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Step 3: Seed default data for tenant
                await _seeder.SeedTenantDefaultsAsync(tenant.Id);

                // Commit transaction
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Tenant {CompanyName} registered successfully " +
                    "with owner {Email}",
                    dto.CompanyName, dto.Email);

                return await GenerateTokenAsync(user, tenant);
            }
            catch (Exception ex)
            {
                // Rollback everything if any step fails
                await transaction.RollbackAsync();

                _logger.LogError(ex,
                    "Registration failed for {CompanyName} - " +
                    "transaction rolled back", dto.CompanyName);

                throw new AppException(
                    "Registration failed. Please try again.", 500);
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !user.IsActive)
                throw new UnauthorizedException("Invalid email or password");

            if (!user.Tenant.IsActive)
                throw new UnauthorizedException(
                    "Your company account is inactive");

            var isValidPassword = BCrypt.Net.BCrypt
                .Verify(dto.Password, user.PasswordHash);

            if (!isValidPassword)
                throw new UnauthorizedException("Invalid email or password");

            _logger.LogInformation(
                "User {Email} logged in for tenant {TenantId}",
                dto.Email, user.TenantId);

            return await GenerateTokenAsync(user, user.Tenant);
        }

        private async Task<AuthResponseDto> GenerateTokenAsync(
            User user, Tenant tenant)
        {
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == user.RoleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();

            var secretKey = _config["JwtSettings:SecretKey"]!;
            var issuer = _config["JwtSettings:Issuer"]!;
            var audience = _config["JwtSettings:Audience"]!;
            var expiryMinutes = int.Parse(
                _config["JwtSettings:ExpiryInMinutes"]!);

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.Role, user.Role?.Name ?? string.Empty),
                new("tenantId", tenant.Id.ToString()),
                new("companyName", tenant.CompanyName)
            };

            foreach (var permission in permissions)
                claims.Add(new Claim("permission", permission));

            var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role?.Name ?? string.Empty,
                CompanyName = tenant.CompanyName,
                TenantId = tenant.Id,
                Permissions = permissions,
                ExpiresAt = expiry
            };
        }
    }
}