using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;

namespace MHS.Service.Implementations;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public UserContext(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public int? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        return null;
    }

    public string? GetCurrentUserEmail()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    public string? GetCurrentUserRole()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        return user.FindFirst(ClaimTypes.Role)?.Value;
    }

    public string? GetCurrentUserName()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    public IEnumerable<Claim> GetCurrentUserClaims()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return Enumerable.Empty<Claim>();

        return user.Claims;
    }

    public bool IsAuthenticated()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated == true;
    }

    public bool IsInRole(string role)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return false;

        return user.IsInRole(role);
    }

    public async Task<UserResponse?> GetCurrentUserAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return null;

        var result = await _userService.GetUserByIdAsync(userId.Value);
        return result.IsSucceeded ? result.Data : null;
    }
}
