using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MHS.Service.Interfaces;
using MHS.Service.DTOs;
using MHS.Common.Enums;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _adminDashboardService;
    private readonly ILogger<AdminDashboardController> _logger;

    public AdminDashboardController(
        IAdminDashboardService adminDashboardService,
        ILogger<AdminDashboardController> logger)
    {
        _adminDashboardService = adminDashboardService;
        _logger = logger;
    }

    [HttpGet("monthly-revenue")]
    public async Task<IActionResult> GetMonthlyRevenue()
    {
        try
        {
            _logger.LogInformation("Starting to retrieve monthly revenue data");
            
            var monthlyRevenue = await _adminDashboardService.GetMonthlyRevenueAsync();
            
            _logger.LogInformation("Successfully retrieved monthly revenue data with {Count} records", monthlyRevenue.Count);
            return Ok(new AppResponse<List<MonthlyRevenueDTO>>()
                .SetSuccessResponse(monthlyRevenue, "Message", "Monthly revenue data retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly revenue data: {Message}", ex.Message);
            return StatusCode(500, new AppResponse<List<MonthlyRevenueDTO>>()
                .SetErrorResponse("Error", $"An error occurred while retrieving monthly revenue data: {ex.Message}"));
        }
    }

    [HttpGet("top-services")]
    public async Task<IActionResult> GetTop5Services()
    {
        try
        {
            _logger.LogInformation("Starting to retrieve top 5 service revenue data");

            var topServices = await _adminDashboardService.GetTop5ServicesAsync();

            _logger.LogInformation("Successfully retrieved top service revenue data with {Count} records", topServices.Count);
            return Ok(new AppResponse<List<TopServiceRevenueDTO>>()
                .SetSuccessResponse(topServices, "Message", "Top service revenue data retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top service revenue data: {Message}", ex.Message);
            return StatusCode(500, new AppResponse<List<TopServiceRevenueDTO>>()
                .SetErrorResponse("Error", $"An error occurred while retrieving top service revenue data: {ex.Message}"));
        }
    }

    [HttpGet("user-summary")]
    public async Task<IActionResult> GetNumberOfCustomerAndStaff()
    {
        try
        {
            _logger.LogInformation("Starting to retrieve user summary data");

            var summary = await _adminDashboardService.GetNumberOfCustomerAndStaffAsync();

            _logger.LogInformation("Successfully retrieved user summary data");
            return Ok(new AppResponse<UserSummaryDTO>()
                .SetSuccessResponse(summary, "Message", "User summary data retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user summary data: {Message}", ex.Message);
            return StatusCode(500, new AppResponse<UserSummaryDTO>()
                .SetErrorResponse("Error", $"An error occurred while retrieving user summary data: {ex.Message}"));
        }
    }
}