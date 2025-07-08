using System.ComponentModel.DataAnnotations;
using MHS.Common.Enums;

namespace MHS.Service.DTOs;

// Request DTOs
public class CreateUserRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public UserRole Role { get; set; }
}

public class UpdateUserRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? ProfileImageUrl { get; set; }
    
    // Address fields (Vietnamese format)
    [MaxLength(500)]
    public string? Address { get; set; } // House number + Street name
    
    [MaxLength(100)]
    public string? Ward { get; set; } // Phường/Xã
    
    [MaxLength(100)]
    public string? District { get; set; } // Quận/Huyện
    
    [MaxLength(100)]
    public string? Province { get; set; } // Tỉnh/Thành phố
    
    [MaxLength(100)]
    public string? Country { get; set; }
    
    public decimal? Latitude { get; set; }
    
    public decimal? Longitude { get; set; }
    
    // Additional profile fields
    public DateTime? DateOfBirth { get; set; }
    
    [MaxLength(20)]
    public string? Gender { get; set; }
    
    [MaxLength(100)]
    public string? EmergencyContactName { get; set; }
    
    [MaxLength(20)]
    public string? EmergencyContactPhone { get; set; }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class VerifyOtpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(6)]
    public string OtpCode { get; set; } = string.Empty;
    
    [Required]
    public string VerificationType { get; set; } = string.Empty; // EMAIL or PHONE
}

// Response DTOs
public class UserResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public string? ProfileImageUrl { get; set; }
    
    // Address fields (Vietnamese format)
    public string? Address { get; set; } // House number + Street name
    public string? Ward { get; set; } // Phường/Xã
    public string? District { get; set; } // Quận/Huyện
    public string? Province { get; set; } // Tỉnh/Thành phố
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Additional profile fields
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    
    // Timestamp fields
    public DateTime? LastLoginAt { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? PhoneVerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LoginResponse
{
    public UserResponse User { get; set; } = null!;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class AdminResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public UserResponse User { get; set; } = null!;
}

// Registration DTOs
public class CustomerRegistrationRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; } // House number + Street name
    public string? Ward { get; set; } // Phường/Xã
    public string? District { get; set; } // Quận/Huyện
    public string? Province { get; set; } // Tỉnh/Thành phố
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

public class StaffRegistrationRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string Address { get; set; } = string.Empty; // House number + Street name
    public string Ward { get; set; } = string.Empty; // Phường/Xã
    public string District { get; set; } = string.Empty; // Quận/Huyện
    public string Province { get; set; } = string.Empty; // Tỉnh/Thành phố
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }
    public int ServiceRadiusKm { get; set; } = 10;
}

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    
    // Address fields (Vietnamese format)
    public string? Address { get; set; } // House number + Street name
    public string? Ward { get; set; } // Phường/Xã
    public string? District { get; set; } // Quận/Huyện
    public string? Province { get; set; } // Tỉnh/Thành phố
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Additional profile fields
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

// Auth Response DTOs
public class RegisterResponse
{
    public string? Token { get; set; }
    public UserResponse User { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

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