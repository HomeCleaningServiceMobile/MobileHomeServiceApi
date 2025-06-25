using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class Booking : BaseEntity
{
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    public int ServiceId { get; set; }
    
    public int? ServicePackageId { get; set; }
    
    public int? StaffId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string BookingNumber { get; set; } = string.Empty;
    
    [Required]
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    
    [Required]
    public DateTime ScheduledDate { get; set; }
    
    [Required]
    public TimeSpan ScheduledTime { get; set; }
    
    public int EstimatedDurationMinutes { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal? FinalAmount { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string ServiceAddress { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(10,8)")]
    public decimal AddressLatitude { get; set; }
    
    [Column(TypeName = "decimal(11,8)")]
    public decimal AddressLongitude { get; set; }
    
    [MaxLength(1000)]
    public string? SpecialInstructions { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime? StartedAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public DateTime? CancelledAt { get; set; }
    
    [MaxLength(500)]
    public string? CancellationReason { get; set; }
    
    // Staff response to auto-assignment
    public DateTime? StaffResponseDeadline { get; set; }
    
    public DateTime? StaffAcceptedAt { get; set; }
    
    public DateTime? StaffDeclinedAt { get; set; }
    
    [MaxLength(500)]
    public string? StaffDeclineReason { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
    
    [ForeignKey(nameof(ServiceId))]
    public virtual Service Service { get; set; } = null!;
    
    [ForeignKey(nameof(ServicePackageId))]
    public virtual ServicePackage? ServicePackage { get; set; }
    
    [ForeignKey(nameof(StaffId))]
    public virtual Staff? Staff { get; set; }
    
    public virtual Payment? Payment { get; set; }
    public virtual Review? Review { get; set; }
    public virtual ICollection<BookingImage> BookingImages { get; set; } = new List<BookingImage>();
    public virtual ICollection<StaffReport> StaffReports { get; set; } = new List<StaffReport>();
} 