using MHS.Common.Enums;

namespace MHS.Service.DTOs;

// Customer-related DTOs
public class CustomerResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? PreferredServices { get; set; }
    public string? SpecialInstructions { get; set; }
    public decimal TotalSpent { get; set; }
    public int TotalBookings { get; set; }
    public decimal AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public UserResponse User { get; set; } = null!;
    public List<CustomerAddressResponse> Addresses { get; set; } = new();
}

public class CustomerAddressResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string? Street { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsDefault { get; set; }
    public string? SpecialInstructions { get; set; }
}

// Staff-related DTOs
public class StaffResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? Skills { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalCompletedJobs { get; set; }
    public bool IsAvailable { get; set; }
    public string? CertificationImageUrl { get; set; }
    public string? IdCardImageUrl { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public int ServiceRadiusKm { get; set; }
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public UserResponse User { get; set; } = null!;
    public List<StaffSkillResponse> StaffSkills { get; set; } = new();
}

public class StaffSkillResponse
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int ServiceId { get; set; }
    public int SkillLevel { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CertifiedAt { get; set; }
    public string? CertificationUrl { get; set; }
    public string? Notes { get; set; }
    
    public ServiceResponse Service { get; set; } = null!;
}

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

// Review-related DTOs
public class ReviewResponse
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public int StaffId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public int? QualityRating { get; set; }
    public int? TimelinessRating { get; set; }
    public int? ProfessionalismRating { get; set; }
    public int? CommunicationRating { get; set; }
    public bool IsPublic { get; set; }
    public bool IsReported { get; set; }
    public string? ReportReason { get; set; }
    public DateTime? ReportedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public CustomerResponse Customer { get; set; } = null!;
    public StaffResponse Staff { get; set; } = null!;
}

// Notification-related DTOs
public class NotificationResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public int? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Work Schedule DTOs
public class WorkScheduleResponse
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public DateTime WorkDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public WorkScheduleStatus Status { get; set; }
    public int? BookingId { get; set; }
    public string? Notes { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurringPattern { get; set; }
    public DateTime? RecurringEndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public StaffResponse Staff { get; set; } = null!;
    public BookingResponse? Booking { get; set; }
}

// Common response wrappers
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
} 