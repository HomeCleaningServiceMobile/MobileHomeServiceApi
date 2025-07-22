using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class WorkSchedule : BaseEntity
{
    [Required]
    [ForeignKey("Staff")]
    public int StaffId { get; set; }
    public bool IsActive { get; set; } = true;


    [Required]
    [Range(0, 6, ErrorMessage = "DayOfWeek must be between 0 (Sunday) and 6 (Saturday)")]
    public int DayOfWeek { get; set; }

    [Required]
    [Column(TypeName = "time")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Column(TypeName = "time")]
    public TimeSpan EndTime { get; set; }

    public virtual Staff Staff { get; set; }

    [NotMapped]
    public DayOfWeek DayOfWeekEnum
    {
        get => (DayOfWeek)DayOfWeek;
        set => DayOfWeek = (int)value;
    }

    [NotMapped]
    public string DayName => ((DayOfWeek)DayOfWeek).ToString();

    [NotMapped]
    public TimeSpan WorkDuration => EndTime - StartTime;

    [NotMapped]
    public double WorkHours => WorkDuration.TotalHours;

    [NotMapped]
    public bool IsWorkingNow
    {
        get
        {
            if (!IsActive) return false;

            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            var currentDay = (int)now.DayOfWeek;

            return currentDay == DayOfWeek &&
                   currentTime >= StartTime &&
                   currentTime <= EndTime;
        }
    }

    // Helper method to check if staff is available at specific time
    public bool IsAvailableAt(DateTime dateTime)
    {
        if (!IsActive) return false;

        var dayOfWeek = (int)dateTime.DayOfWeek;
        var timeOfDay = dateTime.TimeOfDay;

        return dayOfWeek == DayOfWeek &&
               timeOfDay >= StartTime &&
               timeOfDay <= EndTime;
    }

    // Helper method to format schedule display
    [NotMapped]
    public string ScheduleDisplay => $"{DayName}: {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

    // Validation method
    public bool IsValid()
    {
        return EndTime > StartTime;
    }
}
