using MHS.Common.Enums;

namespace MHS.Service.DTOs;

// Work Schedule DTOs
public class WorkScheduleResponse
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public DateTime WorkDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public WorkScheduleStatus Status { get; set; }
    public int? BookingId { get; set; }
    public string? Notes { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurringPattern { get; set; }
    public DateTime? RecurringEndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public StaffResponse Staff { get; set; } = null!;
    public BookingResponse? Booking { get; set; }
} 