using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class StaffReport : BaseEntity
{
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    public int StaffId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ReportType { get; set; } = string.Empty; // PROGRESS, ISSUE, COMPLETION, PROBLEM
    
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? IssueDetails { get; set; }
    
    [MaxLength(500)]
    public string? Resolution { get; set; }
    
    public int? ProgressPercentage { get; set; }
    
    public DateTime? ReportedAt { get; set; }
    
    [MaxLength(1000)]
    public string? AdminNotes { get; set; }
    
    public bool IsResolved { get; set; } = false;
    
    public DateTime? ResolvedAt { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(BookingId))]
    public virtual Booking Booking { get; set; } = null!;
    
    [ForeignKey(nameof(StaffId))]
    public virtual Staff Staff { get; set; } = null!;
} 