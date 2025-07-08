using MHS.Common.Enums;

namespace MHS.Service.DTOs;

// Payment-related DTOs
public class PaymentResponse
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayName { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? RefundedAt { get; set; }
    public decimal? RefundAmount { get; set; }
    public string? RefundReason { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
} 