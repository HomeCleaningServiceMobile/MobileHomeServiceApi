using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class Notification : BaseEntity
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;
    
    [Required]
    public NotificationType Type { get; set; }
    
    public int? RelatedEntityId { get; set; }
    
    [MaxLength(50)]
    public string? RelatedEntityType { get; set; }
    
    public bool IsRead { get; set; } = false;
    
    public DateTime? ReadAt { get; set; }
    
    public bool IsSent { get; set; } = false;
    
    public DateTime? SentAt { get; set; }
    
    [MaxLength(1000)]
    public string? Metadata { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;
} 