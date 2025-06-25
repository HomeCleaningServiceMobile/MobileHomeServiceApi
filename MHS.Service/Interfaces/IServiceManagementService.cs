using MHS.Service.DTOs;

namespace MHS.Service.Interfaces;

public interface IServiceManagementService
{
    // Service operations
    Task<ApiResponse<ServiceResponse>> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<ServiceResponse>> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<PaginatedResponse<ServiceSummaryResponse>>> GetServicesAsync(ServiceListRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<ServiceResponse>> UpdateServiceAsync(int id, UpdateServiceRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> DeleteServiceAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> ActivateServiceAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> DeactivateServiceAsync(int id, CancellationToken cancellationToken = default);
    
    // Service package operations
    Task<ApiResponse<ServicePackageResponse>> CreateServicePackageAsync(CreateServicePackageRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<ServicePackageResponse>> GetServicePackageByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<ServicePackageResponse>>> GetServicePackagesByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default);
    Task<ApiResponse<ServicePackageResponse>> UpdateServicePackageAsync(int id, CreateServicePackageRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> DeleteServicePackageAsync(int id, CancellationToken cancellationToken = default);
    
    // Pricing operations
    Task<ApiResponse<decimal>> CalculateServicePriceAsync(int serviceId, int? servicePackageId, int durationMinutes, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<ServiceResponse>>> GetServicesByTypeAsync(Common.Enums.ServiceType type, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<ServiceResponse>>> GetPopularServicesAsync(int limit = 10, CancellationToken cancellationToken = default);
} 