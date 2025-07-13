using System.ComponentModel.DataAnnotations;

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
