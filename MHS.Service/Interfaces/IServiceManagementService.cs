using MHS.Common.DTOs;
using MHS.Service.DTOs;

namespace MHS.Service.Interfaces;

public interface IServiceManagementService
{
    // Service operations
    Task<AppResponse<ServiceResponse>> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<ServiceResponse>> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppResponse<PaginatedList<ServiceSummaryResponse>>> GetServicesAsync(ServiceListRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<ServiceResponse>> UpdateServiceAsync(int id, UpdateServiceRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> DeleteServiceAsync(int id, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> ActivateServiceAsync(int id, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> DeactivateServiceAsync(int id, CancellationToken cancellationToken = default);
    
    // Service package operations
    Task<AppResponse<ServicePackageResponse>> CreateServicePackageAsync(CreateServicePackageRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<ServicePackageResponse>> GetServicePackageByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppResponse<List<ServicePackageResponse>>> GetServicePackagesByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default);
    Task<AppResponse<ServicePackageResponse>> UpdateServicePackageAsync(int id, CreateServicePackageRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> DeleteServicePackageAsync(int id, CancellationToken cancellationToken = default);
    
    // Pricing operations
    Task<AppResponse<ServicePriceResponse>> CalculateServicePriceAsync(int serviceId, int? servicePackageId, int durationMinutes, CancellationToken cancellationToken = default);
    Task<AppResponse<List<ServiceResponse>>> GetServicesByTypeAsync(Common.Enums.ServiceType type, CancellationToken cancellationToken = default);
    Task<AppResponse<List<ServiceResponse>>> GetPopularServicesAsync(int limit = 10, CancellationToken cancellationToken = default);
} 