using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class Service : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public ServiceType Type { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Base price must be non-negative.")]
    public decimal BasePrice { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Hourly rate must be non-negative.")]
    public decimal? HourlyRate { get; set; }
    
    public int EstimatedDurationMinutes { get; set; }
    
    [MaxLength(255)]
    public string? ImageUrl { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(1000)]
    public string? Requirements { get; set; }
    
    [MaxLength(1000)]
    public string? Restrictions { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? AreaMeters { get; set; }

    // Navigation properties
    public virtual ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
    public virtual ICollection<StaffSkill> StaffSkills { get; set; } = new List<StaffSkill>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
} 