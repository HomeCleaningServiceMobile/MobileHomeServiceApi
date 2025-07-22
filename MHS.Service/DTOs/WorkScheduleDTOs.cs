using MHS.Common.Enums;

namespace MHS.Service.DTOs;

// Work Schedule DTOs
public class WorkScheduleResponse
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan WorkDuration { get; set; }
    public double WorkHours { get; set; }
    public bool IsActive { get; set; }
    public bool IsWorkingNow { get; set; }
    public string ScheduleDisplay { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public StaffResponse Staff { get; set; } = null!;
}

public class WorkScheduleRequest
{
    public int StaffId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; } = true;
}

// Business Hours DTOs
public class BusinessHoursResponse
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
    public bool IsClosed { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrentlyOpen { get; set; }
    public string TimeDisplay { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BusinessHoursRequest
{
    public int DayOfWeek { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
    public bool IsClosed { get; set; } = false;
    public bool IsActive { get; set; } = true;
} 