using MHS.Service.DTOs;

namespace MHS.Service.Interfaces;

public interface IUserService
{
    Task<AppResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> LogoutAsync(int userId, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    
    Task<AppResponse<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<UserResponse>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppResponse<UserResponse>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<AppResponse<UserResponse>> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
    
    // Registration methods that handle complete profile creation
    Task<AppResponse<LoginResponse>> RegisterCustomerAsync(CustomerRegistrationRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<RegisterResponse>> RegisterStaffAsync(StaffRegistrationRequest request, CancellationToken cancellationToken = default);
    
    Task<AppResponse<string>> SendEmailVerificationAsync(string email, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> SendPhoneVerificationAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);
    
    Task<AppResponse<string>> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> ResetPasswordAsync(string email, string resetToken, string newPassword, CancellationToken cancellationToken = default);
    
    Task<AppResponse<string>> ActivateUserAsync(int id, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> DeactivateUserAsync(int id, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> SuspendUserAsync(int id, string reason, CancellationToken cancellationToken = default);

    Task<AppResponse<GoogleLoginResponse>> GoogleLoginAsync(GoogleLoginRequest request);

    //// Helper methods for getting Customer/Staff IDs from User ID
    //Task<AppResponse<int>> GetCustomerIdByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    //Task<AppResponse<int>> GetStaffIdByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}