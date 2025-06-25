using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class BookingImage : BaseEntity
{
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string ImageUrl { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string ImageType { get; set; } = string.Empty; // BEFORE, AFTER, ISSUE, COMPLETION
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime? TakenAt { get; set; }
    
    [MaxLength(100)]
    public string? TakenBy { get; set; } // Staff or Customer
    
    public int SortOrder { get; set; } = 0;
    
    // Navigation properties
    [ForeignKey(nameof(BookingId))]
    public virtual Booking Booking { get; set; } = null!;
} 