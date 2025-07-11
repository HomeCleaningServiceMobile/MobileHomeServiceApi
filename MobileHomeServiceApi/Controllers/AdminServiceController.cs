using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using MHS.Common.Enums;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/admin/services")]
[Authorize(Roles = "Admin")]
public class ServiceManagementController : ControllerBase
{
    private readonly IServiceManagementService _serviceManagementService;
    private readonly ILogger<ServiceManagementController> _logger;

    public ServiceManagementController(
        IServiceManagementService serviceManagementService,
        ILogger<ServiceManagementController> logger)
    {
        _serviceManagementService = serviceManagementService;
        _logger = logger;
    }

    /// <summary>
    /// Get all services with pagination and filtering
    /// </summary>
    /// <param name="request">Search and pagination parameters</param>
    /// <returns>Paginated list of services</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllServices([FromQuery] ServiceListRequest request)
    {
        try
        {
            var result = await _serviceManagementService.GetServicesAsync(request);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting services list");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving services list"));
        }
    }

    /// <summary>
    /// Get service by ID
    /// </summary>
    /// <param name="id">Service ID</param>
    /// <returns>Service details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceById(int id)
    {
        try
        {
            var result = await _serviceManagementService.GetServiceByIdAsync(id);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service {ServiceId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving service"));
        }
    }

    /// <summary>
    /// Create a new service
    /// </summary>
    /// <param name="request">Service creation data</param>
    /// <returns>Created service information</returns>
    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
    {
        try
        {
            var result = await _serviceManagementService.CreateServiceAsync(request);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetServiceById), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while creating service"));
        }
    }

    /// <summary>
    /// Update an existing service
    /// </summary>
    /// <param name="id">Service ID</param>
    /// <param name="request">Updated service data</param>
    /// <returns>Updated service information</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest request)
    {
        try
        {
            var result = await _serviceManagementService.UpdateServiceAsync(id, request);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating service {ServiceId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while updating service"));
        }
    }

    /// <summary>
    /// Delete a service
    /// </summary>
    /// <param name="id">Service ID</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        try
        {
            var result = await _serviceManagementService.DeleteServiceAsync(id);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting service {ServiceId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while deleting service"));
        }
    }

    /// <summary>
    /// Activate a service
    /// </summary>
    /// <param name="id">Service ID</param>
    /// <returns>Activation confirmation</returns>
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateService(int id)
    {
        try
        {
            var result = await _serviceManagementService.ActivateServiceAsync(id);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating service {ServiceId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while activating service"));
        }
    }

    /// <summary>
    /// Deactivate a service
    /// </summary>
    /// <param name="id">Service ID</param>
    /// <returns>Deactivation confirmation</returns>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateService(int id)
    {
        try
        {
            var result = await _serviceManagementService.DeactivateServiceAsync(id);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating service {ServiceId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while deactivating service"));
        }
    }

    /// <summary>
    /// Get services by type
    /// </summary>
    /// <param name="type">Service type</param>
    /// <returns>List of services by type</returns>
    [HttpGet("by-type/{type}")]
    public async Task<IActionResult> GetServicesByType(ServiceType type)
    {
        try
        {
            var result = await _serviceManagementService.GetServicesByTypeAsync(type);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting services by type {ServiceType}", type);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving services by type"));
        }
    }

    // Service Package Management APIs

    /// <summary>
    /// Get service packages by service ID
    /// </summary>
    /// <param name="serviceId">Service ID</param>
    /// <returns>List of service packages</returns>
    [HttpGet("{serviceId}/packages")]
    public async Task<IActionResult> GetServicePackages(int serviceId)
    {
        try
        {
            var result = await _serviceManagementService.GetServicePackagesByServiceIdAsync(serviceId);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service packages for service {ServiceId}", serviceId);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving service packages"));
        }
    }

    /// <summary>
    /// Get service package by ID
    /// </summary>
    /// <param name="serviceId">Service ID</param>
    /// <param name="packageId">Package ID</param>
    /// <returns>Service package details</returns>
    [HttpGet("{serviceId}/packages/{packageId}")]
    public async Task<IActionResult> GetServicePackageById(int serviceId, int packageId)
    {
        try
        {
            var result = await _serviceManagementService.GetServicePackageByIdAsync(packageId);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            // Verify the package belongs to the specified service
            if (result.Data?.ServiceId != serviceId)
            {
                return NotFound(new AppResponse<string>()
                    .SetErrorResponse("NotFound", "Service package not found for this service"));
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service package {PackageId} for service {ServiceId}", packageId, serviceId);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving service package"));
        }
    }

    /// <summary>
    /// Create a service package
    /// </summary>
    /// <param name="serviceId">Service ID</param>
    /// <param name="request">Package creation data</param>
    /// <returns>Created package information</returns>
    [HttpPost("{serviceId}/packages")]
    public async Task<IActionResult> CreateServicePackage(int serviceId, [FromBody] CreateServicePackageRequest request)
    {
        try
        {
            // Ensure the request matches the service ID in the route
            request.ServiceId = serviceId;
            
            var result = await _serviceManagementService.CreateServicePackageAsync(request);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return CreatedAtAction(nameof(GetServicePackageById), 
                new { serviceId, packageId = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service package for service {ServiceId}", serviceId);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while creating service package"));
        }
    }

    /// <summary>
    /// Update a service package
    /// </summary>
    /// <param name="serviceId">Service ID</param>
    /// <param name="packageId">Package ID</param>
    /// <param name="request">Updated package data</param>
    /// <returns>Updated package information</returns>
    [HttpPut("{serviceId}/packages/{packageId}")]
    public async Task<IActionResult> UpdateServicePackage(int serviceId, int packageId, [FromBody] CreateServicePackageRequest request)
    {
        try
        {
            // Ensure the request matches the service ID in the route
            request.ServiceId = serviceId;
            
            var result = await _serviceManagementService.UpdateServicePackageAsync(packageId, request);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating service package {PackageId} for service {ServiceId}", packageId, serviceId);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while updating service package"));
        }
    }

    /// <summary>
    /// Delete a service package
    /// </summary>
    /// <param name="serviceId">Service ID</param>
    /// <param name="packageId">Package ID</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{serviceId}/packages/{packageId}")]
    public async Task<IActionResult> DeleteServicePackage(int serviceId, int packageId)
    {
        try
        {
            var result = await _serviceManagementService.DeleteServicePackageAsync(packageId);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting service package {PackageId} for service {ServiceId}", packageId, serviceId);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while deleting service package"));
        }
    }

    /// <summary>
    /// Calculate service price
    /// </summary>
    /// <param name="serviceId">Service ID</param>
    /// <param name="packageId">Optional package ID</param>
    /// <param name="durationMinutes">Duration in minutes</param>
    /// <returns>Calculated price information</returns>
    [HttpGet("{serviceId}/calculate-price")]
    public async Task<IActionResult> CalculateServicePrice(int serviceId, [FromQuery] int? packageId, [FromQuery] int durationMinutes)
    {
        try
        {
            var result = await _serviceManagementService.CalculateServicePriceAsync(serviceId, packageId, durationMinutes);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating price for service {ServiceId}", serviceId);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while calculating service price"));
        }
    }

    /// <summary>
    /// Get popular services
    /// </summary>
    /// <param name="limit">Number of services to return</param>
    /// <returns>List of popular services</returns>
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularServices([FromQuery] int limit = 10)
    {
        try
        {
            var result = await _serviceManagementService.GetPopularServicesAsync(limit);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular services");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving popular services"));
        }
    }

    /// <summary>
    /// Get service statistics and analytics
    /// </summary>
    /// <param name="request">Analytics filter parameters</param>
    /// <returns>Service statistics data</returns>
    [HttpGet("analytics")]
    public async Task<IActionResult> GetServiceAnalytics([FromQuery] ServiceAnalyticsRequest request)
    {
        try
        {
            // This would require implementing in the service layer
            // For now, return a placeholder response indicating this endpoint is available
            var response = new AppResponse<string>()
                .SetSuccessResponse("Analytics endpoint ready - requires service layer implementation");
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service analytics");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving service analytics"));
        }
    }

    /// <summary>
    /// Bulk update service status (activate/deactivate multiple services)
    /// </summary>
    /// <param name="request">Bulk status update request</param>
    /// <returns>Bulk operation result</returns>
    [HttpPatch("bulk-status")]
    public async Task<IActionResult> BulkUpdateServiceStatus([FromBody] BulkServiceStatusRequest request)
    {
        try
        {
            var results = new List<string>();
            var errors = new List<string>();

            foreach (var serviceId in request.ServiceIds)
            {
                try
                {
                    var result = request.IsActive 
                        ? await _serviceManagementService.ActivateServiceAsync(serviceId)
                        : await _serviceManagementService.DeactivateServiceAsync(serviceId);

                    if (result.IsSucceeded)
                    {
                        results.Add($"Service {serviceId} {(request.IsActive ? "activated" : "deactivated")} successfully");
                    }
                    else
                    {
                        errors.Add($"Service {serviceId}: {string.Join(", ", result.Messages.Values)}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Service {serviceId}: {ex.Message}");
                    _logger.LogError(ex, "Error updating service {ServiceId}", serviceId);
                }
            }

            var response = new
            {
                SuccessCount = results.Count,
                ErrorCount = errors.Count,
                Results = results,
                Errors = errors,
                TotalProcessed = request.ServiceIds.Count
            };

            return Ok(new AppResponse<object>().SetSuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk service status update");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while updating service statuses"));
        }
    }

    /// <summary>
    /// Advanced service search with admin-specific filters
    /// </summary>
    /// <param name="request">Advanced search parameters</param>
    /// <returns>Filtered services with admin data</returns>
    [HttpPost("search")]
    public async Task<IActionResult> AdvancedServiceSearch([FromBody] ServiceSearchRequest request)
    {
        try
        {
            // Convert to base ServiceListRequest for now
            var baseRequest = new ServiceListRequest
            {
                Type = request.Type,
                IsActive = request.IsActive,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                SearchTerm = request.SearchTerm,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var result = await _serviceManagementService.GetServicesAsync(baseRequest);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            // Note: Advanced filtering (CreatedAfter, CreatedBefore, MinRevenue, etc.) 
            // would require service layer implementation
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in advanced service search");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while searching services"));
        }
    }

    /// <summary>
    /// Export services data to CSV
    /// </summary>
    /// <param name="request">Export filter parameters</param>
    /// <returns>CSV file with services data</returns>
    [HttpGet("export")]
    public async Task<IActionResult> ExportServices([FromQuery] ServiceListRequest request)
    {
        try
        {
            var result = await _serviceManagementService.GetServicesAsync(request);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            // Simple CSV generation (in production, use a proper CSV library)
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Id,Name,Type,BasePrice,IsActive,CreatedAt");
            
            if (result.Data?.Items != null)
            {
                foreach (var service in result.Data.Items)
                {
                    csv.AppendLine($"{service.Id},{service.Name},{service.Type},{service.BasePrice},{service.IsActive},");
                }
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"services_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting services");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while exporting services"));
        }
    }
}
