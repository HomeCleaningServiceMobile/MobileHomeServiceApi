using MHS.Common.Enums;

namespace MHS.Service.DTOs;

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