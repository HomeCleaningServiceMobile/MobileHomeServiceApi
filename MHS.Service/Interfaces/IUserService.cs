using MHS.Service.DTOs;

namespace MHS.Service.Interfaces;

public interface IUserService
{
    // Authentication
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> LogoutAsync(int userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    
    // Registration & Profile Management
    Task<ApiResponse<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponse>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponse>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponse>> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
    
    // OTP Verification
    Task<ApiResponse<string>> SendEmailVerificationAsync(string email, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> SendPhoneVerificationAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);
    
    // Password Management
    Task<ApiResponse<string>> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> ResetPasswordAsync(string email, string resetToken, string newPassword, CancellationToken cancellationToken = default);
    
    // User Status
    Task<ApiResponse<string>> ActivateUserAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> DeactivateUserAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> SuspendUserAsync(int id, string reason, CancellationToken cancellationToken = default);
} 