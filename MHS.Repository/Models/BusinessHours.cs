using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
