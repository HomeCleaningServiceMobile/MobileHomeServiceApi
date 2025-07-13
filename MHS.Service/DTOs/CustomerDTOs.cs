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

// CRUD Customer Profile
public class UpdateCustomerProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public List<UpdateCustomerAddressRequest> UpdateCustomerAddresses { get; set; }
}

public class UpdateCustomerAddressRequest
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

public class GetCustomerProfileResponse
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public List<CustomerAddressResponse> CustomerAddresses { get; set; }
}