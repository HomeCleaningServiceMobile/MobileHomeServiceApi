using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MHS.Service.Interfaces;
using MHS.Service.DTOs;
using MHS.Common.Enums;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceManagementService _serviceManagementService;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(IServiceManagementService serviceManagementService, ILogger<ServicesController> logger)
    {
        _serviceManagementService = serviceManagementService;
        _logger = logger;
    }

    /// <summary>
    /// Get all services with filtering and pagination
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetServices([FromQuery] ServiceListRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.GetServicesAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get service by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServiceById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.GetServiceByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new service (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.CreateServiceAsync(request, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetServiceById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update service (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.UpdateServiceAsync(id, request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete service (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteService(int id, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.DeleteServiceAsync(id, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Activate service (Admin only)
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ActivateService(int id, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.ActivateServiceAsync(id, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Deactivate service (Admin only)
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeactivateService(int id, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.DeactivateServiceAsync(id, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get services by type
    /// </summary>
    [HttpGet("by-type/{type}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServicesByType(ServiceType type, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.GetServicesByTypeAsync(type, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get popular services
    /// </summary>
    [HttpGet("popular")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularServices([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.GetPopularServicesAsync(limit, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Calculate service price
    /// </summary>
    [HttpPost("{id}/calculate-price")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateServicePrice(int id, [FromBody] CalculatePriceRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.CalculateServicePriceAsync(id, request.ServicePackageId, request.DurationMinutes, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // Service Package endpoints
    
    /// <summary>
    /// Get service packages by service ID
    /// </summary>
    [HttpGet("{serviceId}/packages")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServicePackagesByServiceId(int serviceId, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.GetServicePackagesByServiceIdAsync(serviceId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get service package by ID
    /// </summary>
    [HttpGet("packages/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServicePackageById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.GetServicePackageByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create service package (Admin only)
    /// </summary>
    [HttpPost("packages")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateServicePackage([FromBody] CreateServicePackageRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.CreateServicePackageAsync(request, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetServicePackageById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update service package (Admin only)
    /// </summary>
    [HttpPut("packages/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateServicePackage(int id, [FromBody] CreateServicePackageRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.UpdateServicePackageAsync(id, request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete service package (Admin only)
    /// </summary>
    [HttpDelete("packages/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteServicePackage(int id, CancellationToken cancellationToken = default)
    {
        var result = await _serviceManagementService.DeleteServicePackageAsync(id, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// Additional DTO for price calculation
public class CalculatePriceRequest
{
    public int? ServicePackageId { get; set; }
    public int DurationMinutes { get; set; }
} 