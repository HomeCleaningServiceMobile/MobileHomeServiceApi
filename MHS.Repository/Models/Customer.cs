using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHS.Repository.Models;

public class Customer : BaseEntity
{
    [Required]
    public int UserId { get; set; }
    
    [MaxLength(500)]
    public string? PreferredServices { get; set; }
    
    [MaxLength(500)]
    public string? SpecialInstructions { get; set; }
    
    public decimal TotalSpent { get; set; } = 0;
    
    public int TotalBookings { get; set; } = 0;
    
    public decimal AverageRating { get; set; } = 0;
    public decimal Balance { get; set; } = 0;
    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;
    
    public virtual ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public virtual ICollection<CustomerPaymentMethod> PaymentMethods { get; set; } = new List<CustomerPaymentMethod>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
} 