using System.ComponentModel.DataAnnotations;
using MHS.Common.Enums;

namespace MHS.Service.DTOs;

// Request DTOs
public class CreateServiceRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public ServiceType Type { get; set; }
    
    [Required]
    public decimal BasePrice { get; set; }
    
    public decimal? HourlyRate { get; set; }
    
    [Required]
    public int EstimatedDurationMinutes { get; set; }
    
    [MaxLength(255)]
    public string? ImageUrl { get; set; }
    
    [MaxLength(1000)]
    public string? Requirements { get; set; }
    
    [MaxLength(1000)]
    public string? Restrictions { get; set; }
}

public class UpdateServiceRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public decimal BasePrice { get; set; }
    
    public decimal? HourlyRate { get; set; }
    
    [Required]
    public int EstimatedDurationMinutes { get; set; }
    
    [MaxLength(255)]
    public string? ImageUrl { get; set; }
    
    [MaxLength(1000)]
    public string? Requirements { get; set; }
    
    [MaxLength(1000)]
    public string? Restrictions { get; set; }
    
    public bool IsActive { get; set; } = true;
}

public class CreateServicePackageRequest
{
    [Required]
    public int ServiceId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public int DurationMinutes { get; set; }
    
    [MaxLength(1000)]
    public string? IncludedItems { get; set; }
    
    public int SortOrder { get; set; } = 0;
}

public class ServiceListRequest
{
    public ServiceType? Type { get; set; }
    public bool? IsActive { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// Response DTOs
public class ServiceResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceType Type { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? HourlyRate { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string? Requirements { get; set; }
    public string? Restrictions { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<ServicePackageResponse> ServicePackages { get; set; } = new();
}

public class ServicePackageResponse
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public string? IncludedItems { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ServiceSummaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceType Type { get; set; }
    public decimal BasePrice { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}

public class ServicePriceResponse
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int? ServicePackageId { get; set; }
    public string? ServicePackageName { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? HourlyRate { get; set; }
    public int DurationMinutes { get; set; }
    public decimal CalculatedPrice { get; set; }
    public string PricingMethod { get; set; } = string.Empty; // "Fixed", "Hourly", "Package"
    public string? Breakdown { get; set; } // Calculated price breakdown details
}

// Admin-specific DTOs for enhanced service management
public class ServiceStatisticsResponse
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public ServiceType Type { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
    public int ActivePackages { get; set; }
    public DateTime LastBookingDate { get; set; }
    public bool IsActive { get; set; }
}

public class ServiceAnalyticsRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ServiceType? Type { get; set; }
    public bool? IsActive { get; set; }
}

public class BulkServiceStatusRequest
{
    [Required]
    public List<int> ServiceIds { get; set; } = new();
    
    [Required]
    public bool IsActive { get; set; }
}

public class ServiceSearchRequest : ServiceListRequest
{
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public decimal? MinRevenue { get; set; }
    public int? MinBookings { get; set; }
    public string? SortBy { get; set; } // "name", "created", "bookings", "revenue"
    public string? SortDirection { get; set; } = "asc"; // "asc" or "desc"
}
