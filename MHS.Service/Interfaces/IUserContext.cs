using System.Security.Claims;
using MHS.Service.DTOs;

namespace MHS.Service.Interfaces;

public interface IUserContext
{
    /// <summary>
    /// Gets the current user's ID from the HTTP context
    /// </summary>
    /// <returns>User ID or null if not authenticated</returns>
    int? GetCurrentUserId();

    /// <summary>
    /// Gets the current user's email from the HTTP context
    /// </summary>
    /// <returns>User email or null if not authenticated</returns>
    string? GetCurrentUserEmail();

    /// <summary>
    /// Gets the current user's role from the HTTP context
    /// </summary>
    /// <returns>User role or null if not authenticated</returns>
    string? GetCurrentUserRole();

    /// <summary>
    /// Gets the current user's full name from the HTTP context
    /// </summary>
    /// <returns>User full name or null if not authenticated</returns>
    string? GetCurrentUserName();

    /// <summary>
    /// Gets all claims for the current user
    /// </summary>
    /// <returns>Collection of claims</returns>
    IEnumerable<Claim> GetCurrentUserClaims();

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    /// <returns>True if authenticated, false otherwise</returns>
    bool IsAuthenticated();

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role, false otherwise</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Gets the current user's complete profile information
    /// </summary>
    /// <returns>Current user response or null if not authenticated</returns>
    Task<UserResponse?> GetCurrentUserAsync();
} 