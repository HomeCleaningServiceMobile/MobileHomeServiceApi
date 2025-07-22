using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MHS.Service.DTOs;
using MHS.Common.Enums;
using AutoMapper;
using MHS.Service.Interfaces;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;
    private readonly IUserTokenService _userTokenService;

    public AuthController(
        IConfiguration configuration,
        IMapper mapper,
        ILogger<AuthController> logger,
        IUserService userService,
        IUserContext userContext,
        IUserTokenService userTokenService)
    {
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
        _userService = userService;
        _userContext = userContext;
        _userTokenService = userTokenService;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user info</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _userService.LoginAsync(request);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", request.Email);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred during login"));
        }
    }

    /// <summary>
    /// Customer registration
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Created user info</returns>
    [HttpPost("register/customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationRequest request)
    {
        try
        {
            var result = await _userService.RegisterCustomerAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetProfile), new { }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during customer registration for email {Email}", request.Email);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred during registration"));
        }
    }

    /// <summary>
    /// Staff registration
    /// </summary>
    /// <param name="request">Staff registration details</param>
    /// <returns>Created staff info</returns>
    [HttpPost("register/staff")]
    public async Task<IActionResult> RegisterStaff([FromBody] StaffRegistrationRequest request)
    {
        try
        {
            var result = await _userService.RegisterStaffAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetProfile), new { }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during staff registration for email {Email}", request.Email);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred during registration"));
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>User profile information</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var currentUser = await _userContext.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return NotFound(new AppResponse<string>()
                    .SetErrorResponse("User", "User not found"));
            }

            return Ok(new AppResponse<UserResponse>()
                .SetSuccessResponse(currentUser));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving profile"));
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="request">Profile update request</param>
    /// <returns>Updated profile</returns>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = _userContext.GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new AppResponse<string>()
                    .SetErrorResponse("User", "User not authenticated"));
            }

            var currentUser = await _userContext.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return NotFound(new AppResponse<string>()
                    .SetErrorResponse("User", "User not found"));
            }

            var updateRequest = new UpdateUserRequest
            {
                FirstName = request.FullName?.Split(' ')[0] ?? currentUser.FirstName,
                LastName = request.FullName?.Contains(' ') == true ? request.FullName.Substring(request.FullName.IndexOf(' ') + 1) : currentUser.LastName,
                Email = currentUser.Email,
                PhoneNumber = request.PhoneNumber ?? currentUser.PhoneNumber,
                // Address fields (Vietnamese format)
                Address = request.Address ?? currentUser.Address,
                Ward = request.Ward ?? currentUser.Ward,
                District = request.District ?? currentUser.District,
                Province = request.Province ?? currentUser.Province,
                Country = request.Country ?? currentUser.Country,
                Latitude = request.Latitude ?? currentUser.Latitude,
                Longitude = request.Longitude ?? currentUser.Longitude,
                // Additional profile fields
                DateOfBirth = request.DateOfBirth ?? currentUser.DateOfBirth,
                Gender = request.Gender ?? currentUser.Gender,
                EmergencyContactName = request.EmergencyContactName ?? currentUser.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone ?? currentUser.EmergencyContactPhone
            };

            var result = await _userService.UpdateUserAsync(userId.Value, updateRequest);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while updating profile"));
        }
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <returns>Success result</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = _userContext.GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new AppResponse<string>()
                    .SetErrorResponse("User", "User not authenticated"));
            }
            
            var result = await _userService.ChangePasswordAsync(userId.Value, request.CurrentPassword, request.NewPassword);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while changing password"));
        }
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _userService.RefreshTokenAsync(request.RefreshToken);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            var response = new RefreshTokenResponse
            {
                Token = result.Data!,
                ExpiresAt = _userTokenService.GetAccessTokenExpiration()
            };

            return Ok(new AppResponse<RefreshTokenResponse>()
                .SetSuccessResponse(response, "Success", "Token refreshed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while refreshing token"));
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    /// <returns>Success result</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = _userContext.GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new AppResponse<string>()
                    .SetErrorResponse("User", "User not authenticated"));
            }

            var result = await _userService.LogoutAsync(userId.Value);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred during logout"));
        }
    }

    /// <summary>
    /// Google login (auto-register if not exists)
    /// </summary>
    [HttpPost("google-login")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var result = await _userService.GoogleLoginAsync(request);
        if (!result.IsSucceeded)
            return BadRequest(result);

        return Ok(result);
    }
}