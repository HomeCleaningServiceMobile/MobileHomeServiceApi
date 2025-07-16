using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class WorkSchedule : BaseEntity
{
    [Required]
    public int StaffId { get; set; }
    
    [Required]
    public DateTime WorkDate { get; set; }
    
    [Required]
    public TimeSpan StartTime { get; set; }
    
    [Required]
    public TimeSpan EndTime { get; set; }
    
    [Required]
    public WorkScheduleStatus Status { get; set; } = WorkScheduleStatus.Available;
    
    public int? BookingId { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public bool IsRecurring { get; set; } = false;
    
    [MaxLength(100)]
    public string? RecurringPattern { get; set; }
    
    public DateTime? RecurringEndDate { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(StaffId))]
    public virtual Staff Staff { get; set; } = null!;
    
    [ForeignKey(nameof(BookingId))]
    public virtual Booking? Booking { get; set; }

} 