using MHS.Repository.Models;

namespace MHS.Service.Interfaces;

public interface IUserTokenService
{
    /// <summary>
    /// Creates a JWT access token for the specified user
    /// </summary>
    /// <param name="user">User to create token for</param>
    /// <param name="role">User's role</param>
    /// <returns>JWT access token</returns>
    string CreateAccessToken(ApplicationUser user, string role);

    /// <summary>
    /// Creates a refresh token for the specified user
    /// </summary>
    /// <param name="user">User to create refresh token for</param>
    /// <returns>Refresh token</returns>
    string CreateRefreshToken(ApplicationUser user);

    /// <summary>
    /// Gets the expiration time for access tokens
    /// </summary>
    /// <returns>Expiration DateTime</returns>
    DateTime GetAccessTokenExpiration();

    /// <summary>
    /// Validates a refresh token for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="refreshToken">Refresh token to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);

    /// <summary>
    /// Revokes all tokens for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    Task RevokeUserTokensAsync(string userId);

    /// <summary>
    /// Extracts user ID from a JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID or null if invalid</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// Checks if a token is expired
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>True if expired, false otherwise</returns>
    bool IsTokenExpired(string token);
} 