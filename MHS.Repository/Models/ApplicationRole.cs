using Microsoft.AspNetCore.Identity;

namespace MHS.Repository.Models;

public class ApplicationRole : IdentityRole<int>
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
} 