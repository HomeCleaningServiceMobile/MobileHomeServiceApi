using MHS.Repository.Models;
using MHS.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MHS.Service.Implementations;

public class UserTokenService : IUserTokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserTokenService> _logger;

    public UserTokenService(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        ILogger<UserTokenService> logger)
    {
        _configuration = configuration;
        _userManager = userManager;
        _logger = logger;
    }

    public string CreateAccessToken(ApplicationUser user, string role)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            string secretKey = jwtSettings["SecretKey"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role),
                new Claim("status", user.Status.ToString()),
                new Claim("token_type", "access")
            };

            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");
            if (expirationMinutes <= 0) expirationMinutes = 60; // Default to 1 hour

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating access token for user {UserId}", user.Id);
            throw;
        }
    }

    public string CreateRefreshToken(ApplicationUser user)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("token_type", "refresh")
            };

            var expirationDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays");
            if (expirationDays <= 0) expirationDays = 7; // Default to 7 days

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expirationDays),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating refresh token for user {UserId}", user.Id);
            throw;
        }
    }

    public DateTime GetAccessTokenExpiration()
    {
        var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");
        if (expirationMinutes <= 0) expirationMinutes = 60; // Default to 1 hour

        return DateTime.UtcNow.AddMinutes(expirationMinutes);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            if (!IsJwtToken(refreshToken))
            {
                return false;
            }

            return await ValidateJwtRefreshTokenAsync(userId, refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating refresh token for user {UserId}", userId);
            return false;
        }
    }

    public async Task RevokeUserTokensAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            // Update security stamp to invalidate existing tokens
            await _userManager.UpdateSecurityStampAsync(user);

            _logger.LogInformation("Revoked all tokens for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking tokens for user {UserId}", userId);
            throw;
        }
    }

    public string? GetUserIdFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            return jsonToken.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting user ID from token");
            return null;
        }
    }

    public bool IsTokenExpired(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            return jsonToken.ValidTo < DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token expiration");
            return true; // Consider invalid tokens as expired
        }
    }

    #region Private Methods

    private async Task<bool> ValidateJwtRefreshTokenAsync(string userId, string refreshToken)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                return false;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };

            var handler = new JwtSecurityTokenHandler();
            var result = handler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);

            if (result == null)
            {
                return false;
            }

            // Verify the token belongs to the correct user
            var tokenUserId = result.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (tokenUserId != userId)
            {
                return false;
            }

            // Verify token type
            var tokenType = result.FindFirst("token_type")?.Value;
            if (tokenType != "refresh")
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JWT refresh token for user {UserId}", userId);
            return false;
        }
    }

    private bool IsJwtToken(string token)
    {
        return !string.IsNullOrWhiteSpace(token) && token.Split('.').Length == 3;
    }

    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }

    #endregion
}
