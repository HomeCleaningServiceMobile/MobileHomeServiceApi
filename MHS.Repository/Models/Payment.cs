using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MHS.Common.Enums;

namespace MHS.Repository.Models;

public class Payment : BaseEntity
{
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PaymentNumber { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    public PaymentMethod Method { get; set; }
    
    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    [MaxLength(255)]
    public string? TransactionId { get; set; }
    
    [MaxLength(255)]
    public string? GatewayTransactionId { get; set; }
    
    [MaxLength(100)]
    public string? GatewayName { get; set; }
    
    public DateTime? PaidAt { get; set; }
    
    public DateTime? FailedAt { get; set; }
    
    [MaxLength(500)]
    public string? FailureReason { get; set; }
    
    public DateTime? RefundedAt { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal? RefundAmount { get; set; }
    
    [MaxLength(500)]
    public string? RefundReason { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(BookingId))]
    public virtual Booking Booking { get; set; } = null!;
} 