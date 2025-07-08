using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class ApplicationUser : IdentityUser<int>
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    public UserRole Role { get; set; }
    
    [Required]
    public UserStatus Status { get; set; }
    
    [MaxLength(255)]
    public string? ProfileImageUrl { get; set; }

    
    // Additional profile fields
    public DateTime? DateOfBirth { get; set; }
    
    [MaxLength(20)]
    public string? Gender { get; set; }
    
    [MaxLength(100)]
    public string? EmergencyContactName { get; set; }
    
    [MaxLength(20)]
    public string? EmergencyContactPhone { get; set; }
    
    // Timestamp fields
    public DateTime? LastLoginAt { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? PhoneVerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Customer? Customer { get; set; }
    public virtual Staff? Staff { get; set; }
    public virtual Admin? Admin { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
} 