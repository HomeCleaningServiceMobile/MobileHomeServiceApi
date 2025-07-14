using MHS.Service.DTOs;

namespace MHS.Service.Interfaces;

public interface IAdminDashboardService
{
    Task<List<MonthlyRevenueDTO>> GetMonthlyRevenueAsync();
    Task<UserSummaryDTO> GetNumberOfCustomerAndStaffAsync();
    Task<List<TopServiceRevenueDTO>> GetTop5ServicesAsync();
}