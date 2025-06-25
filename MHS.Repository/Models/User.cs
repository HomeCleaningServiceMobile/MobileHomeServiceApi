using System.ComponentModel.DataAnnotations;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class User : BaseEntity
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
    
    public UserStatus Status { get; set; } = UserStatus.Active;
    
    [MaxLength(255)]
    public string? ProfileImageUrl { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
    
    public DateTime? EmailVerifiedAt { get; set; }
    
    public DateTime? PhoneVerifiedAt { get; set; }
    
    [MaxLength(6)]
    public string? EmailVerificationCode { get; set; }
    
    [MaxLength(6)]
    public string? PhoneVerificationCode { get; set; }
    
    public DateTime? VerificationCodeExpiry { get; set; }
    
    // Navigation properties
    public virtual Customer? Customer { get; set; }
    public virtual Staff? Staff { get; set; }
    public virtual Admin? Admin { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
} 