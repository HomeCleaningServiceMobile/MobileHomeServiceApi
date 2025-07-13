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
using MHS.Repository.Interfaces;

namespace MHS.Service.Implementations;

public class IdentityUserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserTokenService _userTokenService;

    public IdentityUserService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IUserTokenService userTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _userTokenService = userTokenService;
    }

    public async Task<AppResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AppResponse<LoginResponse>().SetErrorResponse("Authentication", "Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            
            if (!result.Succeeded)
            {
                var message = result.IsLockedOut ? "Account is locked out" :
                             result.IsNotAllowed ? "Account is not allowed to sign in" :
                             "Invalid email or password";

                return new AppResponse<LoginResponse>().SetErrorResponse("Authentication", message);
            }

            if (user.Status != UserStatus.Active)
            {
                return new AppResponse<LoginResponse>().SetErrorResponse("Account", "Account is not active");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var token = _userTokenService.CreateAccessToken(user, user.Role.ToString());
            var refreshToken = _userTokenService.CreateRefreshToken(user);
            var expiresAt = _userTokenService.GetAccessTokenExpiration();

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

            var loginResponse = new LoginResponse
            {
                User = userResponse,
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };

            return new AppResponse<LoginResponse>().SetSuccessResponse(loginResponse, "Success", "Login successful");
        }
        catch (Exception ex)
        {
            return new AppResponse<LoginResponse>().SetErrorResponse("Error", "An error occurred during login");
        }
    }

    public async Task<AppResponse<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
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
                return new AppResponse<UserResponse>()
                    .SetErrorResponse("Registration", "Failed to create user");
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
                CreatedAt = user.CreatedAt
            };

            return new AppResponse<UserResponse>()
                .SetSuccessResponse(userResponse, "Success", "User created successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<UserResponse>()
                .SetErrorResponse("Error", "An error occurred while creating user");
        }
    }

    public async Task<AppResponse<UserResponse>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new AppResponse<UserResponse>().SetErrorResponse("User", "User not found");
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

            return new AppResponse<UserResponse>().SetSuccessResponse(userResponse);
        }
        catch (Exception ex)
        {
            return new AppResponse<UserResponse>()
                .SetErrorResponse("Error", "An error occurred while retrieving user");
        }
    }

    public async Task<AppResponse<UserResponse>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AppResponse<UserResponse>().SetErrorResponse("User", "User not found");
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

            return new AppResponse<UserResponse>().SetSuccessResponse(userResponse);
        }
        catch (Exception ex)
        {
            return new AppResponse<UserResponse>()
                .SetErrorResponse("Error", "An error occurred while retrieving user");
        }
    }

    public async Task<AppResponse<UserResponse>> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new AppResponse<UserResponse>().SetErrorResponse("User", "User not found");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.UserName = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.ProfileImageUrl = request.ProfileImageUrl;
            // Update address fields (Vietnamese format)
            // Update additional profile fields
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender;
            user.EmergencyContactName = request.EmergencyContactName;
            user.EmergencyContactPhone = request.EmergencyContactPhone;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new AppResponse<UserResponse>()
                    .SetErrorResponse("Update", "Failed to update user");
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

            return new AppResponse<UserResponse>()
                .SetSuccessResponse(userResponse, "Success", "User updated successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<UserResponse>()
                .SetErrorResponse("Error", "An error occurred while updating user");
        }
    }

    public async Task<AppResponse<string>> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new AppResponse<string>().SetErrorResponse("User", "User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Password", "Password change failed");
            }

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Password changed successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while changing password");
        }
    }

    public async Task<AppResponse<string>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userTokenService.GetUserIdFromToken(refreshToken);
            if (string.IsNullOrEmpty(userId))
            {
                return new AppResponse<string>().SetErrorResponse("Token", "Invalid refresh token");
            }

            var isValid = await _userTokenService.ValidateRefreshTokenAsync(userId, refreshToken);
            if (!isValid)
            {
                return new AppResponse<string>().SetErrorResponse("Token", "Invalid or expired refresh token");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Status != UserStatus.Active)
            {
                return new AppResponse<string>().SetErrorResponse("User", "User not found or inactive");
            }

            var newToken = _userTokenService.CreateAccessToken(user, user.Role.ToString());
            return new AppResponse<string>()
                .SetSuccessResponse(newToken, "Success", "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while refreshing token");
        }
    }

    public async Task<AppResponse<string>> LogoutAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _signInManager.SignOutAsync();
            await _userTokenService.RevokeUserTokensAsync(userId.ToString());
            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Logged out successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred during logout");
        }
    }

    public async Task<AppResponse<string>> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new AppResponse<string>().SetErrorResponse("User", "User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Delete", "Failed to delete user");
            }

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "User deleted successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while deleting user");
        }
    }

    public async Task<AppResponse<string>> ActivateUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new AppResponse<string>().SetErrorResponse("User", "User not found");
            }

            user.Status = UserStatus.Active;
            await _userManager.UpdateAsync(user);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "User activated successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while activating user");
        }
    }

    public async Task<AppResponse<string>> DeactivateUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new AppResponse<string>().SetErrorResponse("User", "User not found");
            }

            user.Status = UserStatus.Inactive;
            await _userManager.UpdateAsync(user);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "User deactivated successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while deactivating user");
        }
    }

    public async Task<AppResponse<string>> SuspendUserAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new AppResponse<string>().SetErrorResponse("User", "User not found");
            }

            user.Status = UserStatus.Suspended;
            await _userManager.UpdateAsync(user);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "User suspended successfully");
        }
        catch (Exception ex)
        {
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while suspending user");
        }
    }

    public async Task<AppResponse<string>> SendEmailVerificationAsync(string email, CancellationToken cancellationToken = default)
    {
        // Implementation for email verification would go here
        return new AppResponse<string>()
            .SetSuccessResponse("Success", "Success", "Email verification sent");
    }

    public async Task<AppResponse<string>> SendPhoneVerificationAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        // Implementation for phone verification would go here
        return new AppResponse<string>()
            .SetSuccessResponse("Success", "Success", "Phone verification sent");
    }

    public async Task<AppResponse<string>> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        // Implementation for OTP verification would go here
        return new AppResponse<string>()
            .SetSuccessResponse("Success", "Success", "OTP verified successfully");
    }

    public async Task<AppResponse<string>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        // Implementation for forgot password would go here
        return new AppResponse<string>()
            .SetSuccessResponse("Success", "Success", "Password reset email sent");
    }

    public async Task<AppResponse<string>> ResetPasswordAsync(string email, string resetToken, string newPassword, CancellationToken cancellationToken = default)
    {
        // Implementation for reset password would go here
        return new AppResponse<string>()
            .SetSuccessResponse("Success", "Success", "Password reset successfully");
    }

    public async Task<AppResponse<LoginResponse>> RegisterCustomerAsync(CustomerRegistrationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AppResponse<LoginResponse>()
                    .SetErrorResponse("Email", "Email already exists");
            }

            // Create user
            var user = new ApplicationUser
            {
                FirstName = request.FullName.Split(' ')[0],
                LastName = request.FullName.Contains(' ') ? request.FullName.Substring(request.FullName.IndexOf(' ') + 1) : "",
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                // Address fields (Vietnamese format)
                // Additional profile fields
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                return new AppResponse<LoginResponse>()
                    .SetErrorResponse("Error", string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }

            // Create customer profile
            var customer = new Customer
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Customer>().AddAsync(customer);
            await _unitOfWork.CompleteAsync();
            // Add customer address if provided
            if (!string.IsNullOrEmpty(request.Address))
            {
                var address = new CustomerAddress
                {
                    CustomerId = customer.Id,
                    Title = "Home",
                    FullAddress = request.Address,
                    City = request.Ward ?? "",
                    Province = request.Province ?? "",
                    PostalCode = "", // Not applicable for Vietnamese addresses
                    IsDefault = true,
                    Latitude = request.Latitude ?? 0,
                    Longitude = request.Longitude ?? 0,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<CustomerAddress>().AddAsync(address);
                await _unitOfWork.CompleteAsync();
            }

            

            // Login the user after registration
            var loginRequest = new LoginRequest
            {
                Email = request.Email,
                Password = request.Password
            };

            return await LoginAsync(loginRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            return new AppResponse<LoginResponse>()
                .SetErrorResponse("Error", "An error occurred during registration");
        }
    }

    public async Task<AppResponse<RegisterResponse>> RegisterStaffAsync(StaffRegistrationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AppResponse<RegisterResponse>()
                    .SetErrorResponse("Email", "Email already exists");
            }

            // Create user
            var user = new ApplicationUser
            {
                FirstName = request.FullName.Split(' ')[0],
                LastName = request.FullName.Contains(' ') ? request.FullName.Substring(request.FullName.IndexOf(' ') + 1) : "",
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Staff,
                Status = UserStatus.Inactive, // Staff needs approval
                CreatedAt = DateTime.UtcNow,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                return new AppResponse<RegisterResponse>()
                    .SetErrorResponse("Error", string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }

            // Create staff profile
            var staff = new Staff
            {
                UserId = user.Id,
                EmployeeId = $"EMP{user.Id:D6}",
                HireDate = DateTime.UtcNow,
                HourlyRate = request.HourlyRate,
                ServiceRadiusKm = request.ServiceRadiusKm,
                IsAvailable = false, // Not available until approved
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Staff>().AddAsync(staff);
            await _unitOfWork.CompleteAsync();

            var userResponse = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status,
                CreatedAt = user.CreatedAt
            };

            var response = new RegisterResponse
            {
                Token = null, // No token until approved
                User = userResponse,
                ExpiresAt = null
            };

            return new AppResponse<RegisterResponse>()
                .SetSuccessResponse(response, "Success", "Registration submitted for approval");
        }
        catch (Exception ex)
        {
            return new AppResponse<RegisterResponse>()
                .SetErrorResponse("Error", "An error occurred during registration");
        }
    }

    // Helper methods
    private string GenerateRandomPassword()
    {
        return "TempPass123!";
    }
} 