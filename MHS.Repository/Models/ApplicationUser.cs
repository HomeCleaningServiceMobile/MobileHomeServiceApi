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
    
    public UserStatus Status { get; set; } = UserStatus.Active;
    
    [MaxLength(255)]
    public string? ProfileImageUrl { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    
    // Navigation properties
    public virtual Customer? Customer { get; set; }
    public virtual Staff? Staff { get; set; }
    public virtual Admin? Admin { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
} 