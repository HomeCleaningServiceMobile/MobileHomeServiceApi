using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models
{
    public class BusinessHours : BaseEntity
    {
        [Required]
        [Range(0, 6, ErrorMessage = "DayOfWeek must be between 0 (Sunday) and 6 (Saturday)")]
        public int DayOfWeek { get; set; }


        [Required]
        [Column(TypeName = "time")]
        public TimeSpan OpenTime { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan CloseTime { get; set; }

        public bool IsClosed { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [NotMapped]
        public DayOfWeek DayOfWeekEnum
        {
            get => (DayOfWeek)DayOfWeek;
            set => DayOfWeek = (int)value;
        }

        [NotMapped]
        public string DayName => ((DayOfWeek)DayOfWeek).ToString();

        [NotMapped]
        public bool IsCurrentlyOpen
        {
            get
            {
                if (IsClosed) return false;

                var now = DateTime.Now;
                var currentTime = now.TimeOfDay;
                var currentDay = (int)now.DayOfWeek;

                return currentDay == DayOfWeek &&
                       currentTime >= OpenTime &&
                       currentTime <= CloseTime;
            }
        }

        [NotMapped]
        public string TimeDisplay => IsClosed ? "Closed" : $"{OpenTime:hh\\:mm} - {CloseTime:hh\\:mm}";

        // Validation method
        public bool IsValid()
        {
            if (IsClosed) return true;
            return CloseTime > OpenTime;
        }

        // Helper method to check if business is open at specific time
        public bool IsOpenAt(DateTime dateTime)
        {
            if (IsClosed || !IsActive) return false;

            var dayOfWeek = (int)dateTime.DayOfWeek;
            var timeOfDay = dateTime.TimeOfDay;

            return dayOfWeek == DayOfWeek &&
                   timeOfDay >= OpenTime &&
                   timeOfDay <= CloseTime;
        }
    }

}
