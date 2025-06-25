using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class Admin : BaseEntity
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Position { get; set; } = string.Empty;
    
    public DateTime HireDate { get; set; }
    
    [MaxLength(500)]
    public string? Permissions { get; set; }
    
    public DateTime? LastActiveAt { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;
} 