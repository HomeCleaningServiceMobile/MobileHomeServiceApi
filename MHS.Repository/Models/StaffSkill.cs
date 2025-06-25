using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class StaffSkill : BaseEntity
{
    [Required]
    public int StaffId { get; set; }
    
    [Required]
    public int ServiceId { get; set; }
    
    [Range(1, 5)]
    public int SkillLevel { get; set; } = 1;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? CertifiedAt { get; set; }
    
    [MaxLength(255)]
    public string? CertificationUrl { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(StaffId))]
    public virtual Staff Staff { get; set; } = null!;
    
    [ForeignKey(nameof(ServiceId))]
    public virtual Service Service { get; set; } = null!;
} 