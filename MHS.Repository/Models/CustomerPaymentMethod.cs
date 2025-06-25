using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class CustomerPaymentMethod : BaseEntity
{
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty; // My Visa Card, Work Card, etc.
    
    [Required]
    public PaymentMethod Type { get; set; }
    
    [MaxLength(100)]
    public string? CardHolderName { get; set; }
    
    [MaxLength(20)]
    public string? MaskedCardNumber { get; set; } // Last 4 digits only
    
    [MaxLength(50)]
    public string? BankName { get; set; }
    
    [MaxLength(255)]
    public string? TokenOrReference { get; set; } // Encrypted token from payment gateway
    
    public DateTime? ExpiryDate { get; set; }
    
    public bool IsDefault { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
} 