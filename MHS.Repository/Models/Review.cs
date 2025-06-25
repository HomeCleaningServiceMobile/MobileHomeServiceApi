using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class Review : BaseEntity
{
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    public int StaffId { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [MaxLength(1000)]
    public string? Comment { get; set; }
    
    [Range(1, 5)]
    public int? QualityRating { get; set; }
    
    [Range(1, 5)]
    public int? TimelinessRating { get; set; }
    
    [Range(1, 5)]
    public int? ProfessionalismRating { get; set; }
    
    [Range(1, 5)]
    public int? CommunicationRating { get; set; }
    
    public bool IsPublic { get; set; } = true;
    
    public bool IsReported { get; set; } = false;
    
    [MaxLength(500)]
    public string? ReportReason { get; set; }
    
    public DateTime? ReportedAt { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(BookingId))]
    public virtual Booking Booking { get; set; } = null!;
    
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
    
    [ForeignKey(nameof(StaffId))]
    public virtual Staff Staff { get; set; } = null!;
} 