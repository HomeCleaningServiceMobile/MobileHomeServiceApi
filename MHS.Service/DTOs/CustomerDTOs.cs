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