using System.ComponentModel.DataAnnotations;
using MHS.Common.DTOs;
using MHS.Common.Enums;
using MHS.Service.DTOs;

namespace MHS.Service.DTOs;

// Staff-related DTOs
public class StaffResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? Skills { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalCompletedJobs { get; set; }
    public bool IsAvailable { get; set; }
    public string? CertificationImageUrl { get; set; }
    public string? IdCardImageUrl { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public int ServiceRadiusKm { get; set; }
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserResponse User { get; set; } = null!;
    public List<StaffSkillResponse> StaffSkills { get; set; } = new();
}

public class StaffSkillResponse
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int ServiceId { get; set; }
    public int SkillLevel { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CertifiedAt { get; set; }
    public string? CertificationUrl { get; set; }
    public string? Notes { get; set; }

    public ServiceResponse Service { get; set; } = null!;
}

/// CRUD Profile Staff
public class GetStaffProfileResponse
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Skills { get; set; }
    public string? Bio { get; set; }
}

public class UpdateStaffProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? ProfileImageUrl { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(100)]
    public string? EmergencyContactName { get; set; }

    [MaxLength(20)]
    public string? EmergencyContactPhone { get; set; }

    [MaxLength(500)]
    public string? Skills { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }
}

// Admin Staff Management DTOs
public class AdminStaffListResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime HireDate { get; set; }
    public string? Skills { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalCompletedJobs { get; set; }
    public bool IsAvailable { get; set; }
    public UserStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public int ServiceRadiusKm { get; set; }
    public string? CertificationImageUrl { get; set; }
    public string? IdCardImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AdminStaffDetailResponse : AdminStaffListResponse
{
    public string? Bio { get; set; }
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public string FullAddress { get; set; } = string.Empty;
    public string? Street { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    
    public UserResponse User { get; set; } = null!;
    public List<StaffSkillResponse> StaffSkills { get; set; } = new();
    public List<BookingResponse> RecentBookings { get; set; } = new();
}

public class AdminCreateStaffRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? Skills { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public int ServiceRadiusKm { get; set; } = 10;
    public string FullAddress { get; set; } = string.Empty;
    public string? Street { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? CertificationImageUrl { get; set; }
    public string? IdCardImageUrl { get; set; }
}

public class AdminUpdateStaffRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmployeeId { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Skills { get; set; }
    public string? Bio { get; set; }
    public decimal? HourlyRate { get; set; }
    public bool? IsAvailable { get; set; }
    public int? ServiceRadiusKm { get; set; }
    public string? FullAddress { get; set; }
    public string? Street { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? CertificationImageUrl { get; set; }
    public string? IdCardImageUrl { get; set; }
}

public class AdminStaffSearchRequest : PaginationRequest
{
    public string? SearchTerm { get; set; }
    public string? EmployeeId { get; set; }
    public bool? IsAvailable { get; set; }
    public UserStatus? Status { get; set; }
    public DateTime? HireDateFrom { get; set; }
    public DateTime? HireDateTo { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public string? Skills { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

public class StaffStatusChangeRequest
{
    public UserStatus Status { get; set; }
    public string? Reason { get; set; }
}

public class StaffSkillManagementRequest
{
    public int ServiceId { get; set; }
    public int SkillLevel { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CertifiedAt { get; set; }
    public string? CertificationUrl { get; set; }
    public string? Notes { get; set; }
}
