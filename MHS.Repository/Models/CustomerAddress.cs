using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class CustomerAddress : BaseEntity
{
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty; // Home, Office, etc.
    
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
    
    [Column(TypeName = "decimal(10,8)")]
    public decimal Latitude { get; set; }
    
    [Column(TypeName = "decimal(11,8)")]
    public decimal Longitude { get; set; }
    
    public bool IsDefault { get; set; } = false;
    
    [MaxLength(500)]
    public string? SpecialInstructions { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
} 