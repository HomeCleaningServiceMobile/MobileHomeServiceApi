using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MHS.Repository.Models;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using MHS.Common.Enums;

namespace MHS.Service.Implementations;

public class IdentityUserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;

    public IdentityUserService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            
            if (!result.Succeeded)
            {
                var message = result.IsLockedOut ? "Account is locked out" :
                             result.IsNotAllowed ? "Account is not allowed to sign in" :
                             "Invalid email or password";

                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = message
                };
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate JWT token
            var token = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            var userResponse = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Role = user.Role,
                Status = user.Status,
                ProfileImageUrl = user.ProfileImageUrl,
                LastLoginAt = user.LastLoginAt,
                EmailVerifiedAt = user.EmailConfirmed ? DateTime.UtcNow : null,
                PhoneVerifiedAt = user.PhoneNumberConfirmed ? DateTime.UtcNow : null,
                CreatedAt = user.CreatedAt
            };

            return new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginResponse
                {
                    User = userResponse,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "An error occurred during login",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, GenerateRandomPassword());
            
            if (!result.Succeeded)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    Message = "Failed to create user",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            // Add user to role
            var roleName = request.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
            await _userManager.AddToRoleAsync(user, roleName);

            var userResponse = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt = user.CreatedAt
            };

            return new ApiResponse<UserResponse>
            {
                Success = true,
                Message = "User created successfully",
                Data = userResponse
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<UserResponse>
            {
                Success = false,
                Message = "An error occurred while creating user",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var userResponse = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Role = user.Role,
                Status = user.Status,
                ProfileImageUrl = user.ProfileImageUrl,
                LastLoginAt = user.LastLoginAt,
                EmailVerifiedAt = user.EmailConfirmed ? DateTime.UtcNow : null,
                PhoneVerifiedAt = user.PhoneNumberConfirmed ? DateTime.UtcNow : null,
                CreatedAt = user.CreatedAt
            };

            return new ApiResponse<UserResponse>
            {
                Success = true,
                Data = userResponse
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<UserResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving user",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    // Helper methods
    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("Role", user.Role.ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    private string GenerateRandomPassword()
    {
        return "TempPass123!";
    }

    // Implement remaining interface methods with NotImplementedException for now
    public Task<ApiResponse<string>> LogoutAsync(int userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<UserResponse>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<UserResponse>> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> SendEmailVerificationAsync(string email, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> SendPhoneVerificationAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> ResetPasswordAsync(string email, string resetToken, string newPassword, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> ActivateUserAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> DeactivateUserAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<ApiResponse<string>> SuspendUserAsync(int id, string reason, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
} 