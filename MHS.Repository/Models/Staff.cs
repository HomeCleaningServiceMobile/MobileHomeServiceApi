using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class Staff : BaseEntity
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;
    
    public DateTime HireDate { get; set; }
    
    [MaxLength(500)]
    public string? Skills { get; set; }
    
    [MaxLength(1000)]
    public string? Bio { get; set; }
    
    public decimal HourlyRate { get; set; }
    
    public decimal AverageRating { get; set; } = 0;
    
    public int TotalCompletedJobs { get; set; } = 0;
    
    public bool IsAvailable { get; set; } = true;
    
    [MaxLength(255)]
    public string? CertificationImageUrl { get; set; }
    
    [MaxLength(255)]
    public string? IdCardImageUrl { get; set; }
    
    public DateTime? LastActiveAt { get; set; }
    
    // Service area (radius in kilometers)
    public int ServiceRadiusKm { get; set; } = 10;
    
    [Column(TypeName = "decimal(10,8)")]
    public decimal? CurrentLatitude { get; set; }
    
    [Column(TypeName = "decimal(11,8)")]
    public decimal? CurrentLongitude { get; set; }
    [Required]
    [MaxLength(500)]
    public string FullAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Street { get; set; }

    [MaxLength(100)]
    public string? District { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Province { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }
    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;
    
    public virtual ICollection<StaffSkill> StaffSkills { get; set; } = new List<StaffSkill>();
    public virtual ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
    public virtual ICollection<StaffReport> Reports { get; set; } = new List<StaffReport>();
} 