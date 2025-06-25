using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class ServicePackage : BaseEntity
{
    [Required]
    public int ServiceId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    public int DurationMinutes { get; set; }
    
    [MaxLength(1000)]
    public string? IncludedItems { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
    
    // Navigation properties
    [ForeignKey(nameof(ServiceId))]
    public virtual Service Service { get; set; } = null!;
    
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
} 