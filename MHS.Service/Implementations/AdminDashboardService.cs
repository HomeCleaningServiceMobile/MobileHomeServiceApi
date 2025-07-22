using MHS.Repository.Data;
using MHS.Repository.Interfaces;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using MHS.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace MHS.Service.Implementations;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MonthlyRevenueDTO>> GetMonthlyRevenueAsync()
    {
        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1).AddMonths(-5);
        var end   = now;

        var data = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Paid &&
                        p.PaidAt >= start && p.PaidAt <= end)
            .GroupBy(p => new { p.PaidAt!.Value.Year, p.PaidAt!.Value.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Revenue = g.Sum(p => p.Amount)
            })
            .ToListAsync();

        var result = Enumerable.Range(0, 6)
            .Select(i =>
            {
                var d = start.AddMonths(i);
                var key = data.FirstOrDefault(x => x.Year == d.Year && x.Month == d.Month);
                return new MonthlyRevenueDTO
                {
                    Month = $"{d:yyyy-MM}",
                    Revenue = key?.Revenue ?? 0
                };
            }).ToList();

        return result;
    }

    public async Task<List<TopServiceRevenueDTO>> GetTop5ServicesAsync()
    {
        var data = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Paid && p.PaidAt != null)
            .Include(p => p.Booking)
                .ThenInclude(b => b.Service)
            .GroupBy(p => new { p.Booking.ServiceId, p.Booking.Service.Name })
            .Select(g => new TopServiceRevenueDTO
            {
                ServiceName   = g.Key.Name,
                TotalRevenue  = g.Sum(p => p.Amount),
                BookingCount  = g.Count()
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(5)
            .ToListAsync();

        return data;
    }

    public async Task<UserSummaryDTO> GetNumberOfCustomerAndStaffAsync()
    {
        var totalCustomers = await _context.Users.CountAsync(u => u.Role == UserRole.Customer);
        var totalStaff = await _context.Users.CountAsync(u => u.Role == UserRole.Staff);
        var activeStaff = await _context.Users.CountAsync(u => u.Role == UserRole.Staff && u.Status == UserStatus.Active);

        return new UserSummaryDTO
        {
            TotalCustomers = totalCustomers,
            TotalStaff = totalStaff,
            ActiveStaff = activeStaff
        };
    }
}