namespace MHS.Service.DTOs;

public class MonthlyRevenueDTO
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}

public class UserSummaryDTO
{
    public int TotalCustomers { get; set; }
    public int TotalStaff { get; set; }
    public int ActiveStaff { get; set; }
}

public class TopServiceRevenueDTO
{
    public string ServiceName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int BookingCount { get; set; }
}