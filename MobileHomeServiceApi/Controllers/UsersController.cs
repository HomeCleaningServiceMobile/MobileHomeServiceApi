using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MHS.Service.Interfaces;
using MHS.Service.DTOs;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.LoginAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// User logout
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        // Get user ID from claims
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Invalid user ID");
        }

        var result = await _userService.LogoutAsync(userId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken = default)
    {
        var result = await _userService.RefreshTokenAsync(refreshToken, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Create new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.CreateUserAsync(request, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetUserById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetUserByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateUserAsync(id, request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.DeleteUserAsync(id, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Send email verification
    /// </summary>
    [HttpPost("send-email-verification")]
    public async Task<IActionResult> SendEmailVerification([FromBody] string email, CancellationToken cancellationToken = default)
    {
        var result = await _userService.SendEmailVerificationAsync(email, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Send phone verification
    /// </summary>
    [HttpPost("send-phone-verification")]
    public async Task<IActionResult> SendPhoneVerification([FromBody] string phoneNumber, CancellationToken cancellationToken = default)
    {
        var result = await _userService.SendPhoneVerificationAsync(phoneNumber, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Verify OTP code
    /// </summary>
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.VerifyOtpAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        // Get user ID from claims
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Invalid user ID");
        }

        var result = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Forgot password
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email, CancellationToken cancellationToken = default)
    {
        var result = await _userService.ForgotPasswordAsync(email, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Reset password
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.ResetPasswordAsync(request.Email, request.ResetToken, request.NewPassword, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// Additional DTOs for password operations
public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
} 