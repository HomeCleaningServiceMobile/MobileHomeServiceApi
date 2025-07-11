using System.ComponentModel.DataAnnotations;
using MHS.Common.Enums;

namespace MHS.Service.DTOs;

// Request DTOs
public class CreateBookingRequest
{
    [Required]
    public int ServiceId { get; set; }
    
    public int? ServicePackageId { get; set; }
    
    [Required]
    public DateTime ScheduledDate { get; set; }
    
    [Required]
    public TimeSpan ScheduledTime { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string ServiceAddress { get; set; } = string.Empty;
    
    [Required]
    public decimal AddressLatitude { get; set; }
    
    [Required]
    public decimal AddressLongitude { get; set; }
    
    [MaxLength(1000)]
    public string? SpecialInstructions { get; set; }
    
    public PaymentMethod PreferredPaymentMethod { get; set; }
}

public class UpdateBookingRequest
{
    public DateTime? ScheduledDate { get; set; }
    public TimeSpan? ScheduledTime { get; set; }
    public string? ServiceAddress { get; set; }
    public decimal? AddressLatitude { get; set; }
    public decimal? AddressLongitude { get; set; }
    public string? SpecialInstructions { get; set; }
}

public class StaffResponseRequest
{
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    public bool Accept { get; set; }
    
    [MaxLength(500)]
    public string? DeclineReason { get; set; }
}

public class CheckInRequest
{
    [Required]
    public int BookingId { get; set; }
    
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class CheckOutRequest
{
    [Required]
    public int BookingId { get; set; }
    
    [MaxLength(1000)]
    public string? CompletionNotes { get; set; }
    
    public List<string>? CompletionImageUrls { get; set; }
}

public class BookingListRequest
{
    public BookingStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? CustomerId { get; set; }
    public int? StaffId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// Response DTOs
public class BookingResponse
{
    public int Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan ScheduledTime { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? FinalAmount { get; set; }
    public string ServiceAddress { get; set; } = string.Empty;
    public string? SpecialInstructions { get; set; }
    public string? Notes { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal AddressLatitude { get; set; }
    public decimal AddressLongitude { get; set; }

    // Related entities
    public CustomerResponse Customer { get; set; } = null!;
    public ServiceResponse Service { get; set; } = null!;
    public ServicePackageResponse? ServicePackage { get; set; }
    public StaffResponse? Staff { get; set; }
    public PaymentResponse? Payment { get; set; }
    public ReviewResponse? Review { get; set; }
    public List<BookingImageResponse> BookingImages { get; set; } = new();
}

public class BookingImageResponse
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? TakenAt { get; set; }
    public string? TakenBy { get; set; }
}

public class BookingSummaryResponse : BookingResponse
{
    // Simplified version without related entities for list views
} 