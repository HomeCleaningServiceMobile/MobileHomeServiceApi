using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MHS.Common.DTOs;
using MHS.Common.Enums;
using MHS.Repository.Interfaces;
using MHS.Repository.Models;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MHS.Service.Implementations
{
    public class ServiceManagementService : IServiceManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<MHS.Repository.Models.Service> _serviceRepository;
        private readonly IGenericRepository<ServicePackage> _servicePackageRepository;
        private readonly IMapper _mapper;
        
        public ServiceManagementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _serviceRepository = _unitOfWork.Repository<MHS.Repository.Models.Service>();
            _servicePackageRepository = _unitOfWork.Repository<ServicePackage>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AppResponse<string>> ActivateServiceAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = await _serviceRepository.GetEntityByIdAsync(id);
                if (service == null)
                    return new AppResponse<string>().SetErrorResponse("Error", "Service not found.");
                    
                service.IsActive = true;
                _serviceRepository.Update(service);
                await _unitOfWork.CompleteAsync(cancellationToken);
                
                return new AppResponse<string>().SetSuccessResponse(service.Id.ToString(), "Success", "Service activated successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<string>().SetErrorResponse("Error", $"Error activating service: {ex.Message}");
            }
        }

        public async Task<AppResponse<ServicePriceResponse>> CalculateServicePriceAsync(int serviceId, int? servicePackageId, int durationMinutes, CancellationToken cancellationToken = default)
        {
            try
            {
                if (durationMinutes <= 0)
                {
                    return new AppResponse<ServicePriceResponse>().SetErrorResponse("Error", "Duration must be greater than zero.");
                }

                if (serviceId <= 0)
                {
                    return new AppResponse<ServicePriceResponse>().SetErrorResponse("Error", "Invalid service ID.");
                }

                var service = await _serviceRepository.GetEntityByIdAsync(serviceId);
                if (service == null || service.IsDeleted || !service.IsActive)
                {
                    return new AppResponse<ServicePriceResponse>().SetErrorResponse("Error", "Service not found.");
                }

                ServicePackage servicePackage = null;
                if (servicePackageId.HasValue && servicePackageId.Value > 0)
                {
                    servicePackage = await _servicePackageRepository.GetEntityByIdAsync(servicePackageId.Value);
                    if (servicePackage == null)
                    {
                        return new AppResponse<ServicePriceResponse>().SetErrorResponse("Error", "Service package not found.");
                    }
                    
                    var response = _mapper.Map<ServicePriceResponse>(servicePackage);
                    return new AppResponse<ServicePriceResponse>().SetSuccessResponse(response, "Success", "Price calculated successfully.");
                } 
                else
                {
                    var response = _mapper.Map<ServicePriceResponse>(service);
                    response.DurationMinutes = durationMinutes;
                    
                    if (service.HourlyRate.HasValue)
                    {
                        var hours = durationMinutes / 60.0m;
                        response.CalculatedPrice = service.HourlyRate.Value * hours;
                        response.PricingMethod = "Hourly";
                        response.Breakdown = $"Hourly rate: ${service.HourlyRate}/hour × {hours:F2} hours = ${response.CalculatedPrice:F2}";
                    } 
                    else
                    {
                        response.CalculatedPrice = service.BasePrice;
                        response.PricingMethod = "Fixed";
                        response.Breakdown = $"Base price: ${service.BasePrice}";
                    }
                    
                    return new AppResponse<ServicePriceResponse>().SetSuccessResponse(response, "Success", "Price calculated successfully.");
                }
            }
            catch (Exception ex)
            {
                return new AppResponse<ServicePriceResponse>().SetErrorResponse("Error", $"Error calculating service price: {ex.Message}");
            }
        }

        public async Task<AppResponse<ServiceResponse>> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = _mapper.Map<MHS.Repository.Models.Service>(request);

                await _serviceRepository.AddAsync(service);
                await _unitOfWork.CompleteAsync(cancellationToken);

                var response = _mapper.Map<ServiceResponse>(service);

                return new AppResponse<ServiceResponse>().SetSuccessResponse(response, "Success", "Service created successfully");
            } 
            catch (Exception ex) 
            {
                return new AppResponse<ServiceResponse>().SetErrorResponse("Error", $"Error creating service: {ex.Message}");
            }
        }

        public async Task<AppResponse<ServicePackageResponse>> CreateServicePackageAsync(CreateServicePackageRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = await _serviceRepository.GetEntityByIdAsync(request.ServiceId);
                if (service == null)
                {
                    return new AppResponse<ServicePackageResponse>().SetErrorResponse("Error", "Service not found.");
                }

                var servicePackage = _mapper.Map<ServicePackage>(request);
                servicePackage.IsActive = true;

                await _servicePackageRepository.AddAsync(servicePackage);
                await _unitOfWork.CompleteAsync(cancellationToken);
                
                var response = _mapper.Map<ServicePackageResponse>(servicePackage);
                return new AppResponse<ServicePackageResponse>().SetSuccessResponse(response, "Success", "Service package created successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<ServicePackageResponse>().SetErrorResponse("Error", $"Error creating service package: {ex.Message}");
            }
        }

        public async Task<AppResponse<string>> DeactivateServiceAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = await _serviceRepository.GetEntityByIdAsync(id);
                if (service == null)
                {
                    return new AppResponse<string>().SetErrorResponse("Error", "Service not found.");
                }

                service.IsActive = false;
                _serviceRepository.Update(service);
                await _unitOfWork.CompleteAsync(cancellationToken);

                return new AppResponse<string>().SetSuccessResponse(service.Id.ToString(), "Success", "Service deactivated successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<string>().SetErrorResponse("Error", $"Error deactivating service: {ex.Message}");
            }
        }

        public async Task<AppResponse<string>> DeleteServiceAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = await _serviceRepository.GetEntityByIdAsync(id);
                if (service == null)
                {
                    return new AppResponse<string>().SetErrorResponse("Error", "Service not found.");
                }

                service.IsDeleted = true;
                _serviceRepository.Update(service);
                await _unitOfWork.CompleteAsync(cancellationToken);

                return new AppResponse<string>().SetSuccessResponse(service.Id.ToString(), "Success", "Service deleted successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<string>().SetErrorResponse("Error", $"Error deleting service: {ex.Message}");
            }
        }

        public async Task<AppResponse<string>> DeleteServicePackageAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var servicePackage = await _servicePackageRepository.GetEntityByIdAsync(id);
                if (servicePackage == null)
                {
                    return new AppResponse<string>().SetErrorResponse("Error", "Service package not found.");
                }

                _servicePackageRepository.Delete(servicePackage);
                await _unitOfWork.CompleteAsync(cancellationToken);

                return new AppResponse<string>().SetSuccessResponse(servicePackage.Id.ToString(), "Success", "Service package deleted successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<string>().SetErrorResponse("Error", $"Error deleting service package: {ex.Message}");
            }
        }

        public async Task<AppResponse<List<ServiceResponse>>> GetPopularServicesAsync(int limit = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var services = await _serviceRepository.ListAsync(
                    filter: s => !s.IsDeleted && s.IsActive,
                    orderBy: q => q.OrderByDescending(s => s.CreatedAt)
                );
                var popularServices = services.Take(limit).ToList();

                var response = _mapper.Map<List<ServiceResponse>>(popularServices);
                return new AppResponse<List<ServiceResponse>>().SetSuccessResponse(response, "Success", "Popular services retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<List<ServiceResponse>>().SetErrorResponse("Error", $"Error retrieving popular services: {ex.Message}");
            }
        }

        public async Task<AppResponse<ServiceResponse>> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = await _serviceRepository.GetEntityByIdAsync(id);
                if (service == null || service.IsDeleted)
                {
                    return new AppResponse<ServiceResponse>().SetErrorResponse("Error", "Service not found.");
                }

                var response = _mapper.Map<ServiceResponse>(service);
                return new AppResponse<ServiceResponse>().SetSuccessResponse(response, "Success", "Service retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<ServiceResponse>().SetErrorResponse("Error", $"Error retrieving service: {ex.Message}");
            }
        }

        public async Task<AppResponse<ServicePackageResponse>> GetServicePackageByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var servicePackage = await _servicePackageRepository.GetEntityByIdAsync(id);
                if (servicePackage == null)
                {
                    return new AppResponse<ServicePackageResponse>().SetErrorResponse("Error", "Service package not found.");
                }

                var response = _mapper.Map<ServicePackageResponse>(servicePackage);
                return new AppResponse<ServicePackageResponse>().SetSuccessResponse(response, "Success", "Service package retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<ServicePackageResponse>().SetErrorResponse("Error", $"Error retrieving service package: {ex.Message}");
            }
        }

        public async Task<AppResponse<List<ServicePackageResponse>>> GetServicePackagesByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var servicePackages = await _servicePackageRepository.ListAsync(
                    filter: sp => sp.ServiceId == serviceId && sp.IsActive,
                    orderBy: q => q.OrderBy(sp => sp.SortOrder)
                );

                var response = _mapper.Map<List<ServicePackageResponse>>(servicePackages);
                return new AppResponse<List<ServicePackageResponse>>().SetSuccessResponse(response, "Success", "Service packages retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<List<ServicePackageResponse>>().SetErrorResponse("Error", $"Error retrieving service packages: {ex.Message}");
            }
        }

        public async Task<AppResponse<PaginatedList<ServiceSummaryResponse>>> GetServicesAsync(ServiceListRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Build dynamic filter
                Expression<Func<MHS.Repository.Models.Service, bool>> filter = s =>
                    !s.IsDeleted &&
                    (request.Type == null || s.Type == request.Type) &&
                    (request.IsActive == null || s.IsActive == request.IsActive) &&
                    (request.MinPrice == null || s.BasePrice >= request.MinPrice) &&
                    (request.MaxPrice == null || s.BasePrice <= request.MaxPrice) &&
                    (string.IsNullOrEmpty(request.SearchTerm) || s.Name.Contains(request.SearchTerm) || s.Description.Contains(request.SearchTerm));

                // Get all filtered services first to count
                var allServices = await _serviceRepository.ListAsync(filter: filter);
                var totalCount = allServices.Count();

                // Apply pagination
                var items = allServices
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var summaryItems = _mapper.Map<List<ServiceSummaryResponse>>(items);
                var paginatedResult = PaginatedList<ServiceSummaryResponse>.Create(summaryItems, request.PageNumber, request.PageSize, totalCount);

                return new AppResponse<PaginatedList<ServiceSummaryResponse>>().SetSuccessResponse(paginatedResult, "Success", "Services retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<PaginatedList<ServiceSummaryResponse>>().SetErrorResponse("Error", $"Error retrieving services: {ex.Message}");
            }
        }

        public async Task<AppResponse<List<ServiceResponse>>> GetServicesByTypeAsync(ServiceType type, CancellationToken cancellationToken = default)
        {
            try
            {
                var services = await _serviceRepository.ListAsync(
                    filter: s => s.Type == type && !s.IsDeleted && s.IsActive,
                    orderBy: q => q.OrderBy(s => s.Name)
                );

                var response = _mapper.Map<List<ServiceResponse>>(services);
                return new AppResponse<List<ServiceResponse>>().SetSuccessResponse(response, "Success", "Services retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<List<ServiceResponse>>().SetErrorResponse("Error", $"Error retrieving services by type: {ex.Message}");
            }
        }

        public async Task<AppResponse<ServiceResponse>> UpdateServiceAsync(int id, UpdateServiceRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = await _serviceRepository.GetEntityByIdAsync(id);
                if (service == null || service.IsDeleted)
                {
                    return new AppResponse<ServiceResponse>().SetErrorResponse("Error", "Service not found.");
                }

                _mapper.Map(request, service);
                _serviceRepository.Update(service);
                await _unitOfWork.CompleteAsync(cancellationToken);

                var response = _mapper.Map<ServiceResponse>(service);
                return new AppResponse<ServiceResponse>().SetSuccessResponse(response, "Success", "Service updated successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<ServiceResponse>().SetErrorResponse("Error", $"Error updating service: {ex.Message}");
            }
        }

        public async Task<AppResponse<ServicePackageResponse>> UpdateServicePackageAsync(int id, CreateServicePackageRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var servicePackage = await _servicePackageRepository.GetEntityByIdAsync(id);
                if (servicePackage == null)
                {
                    return new AppResponse<ServicePackageResponse>().SetErrorResponse("Error", "Service package not found.");
                }

                _mapper.Map(request, servicePackage);
                _servicePackageRepository.Update(servicePackage);
                await _unitOfWork.CompleteAsync(cancellationToken);

                var response = _mapper.Map<ServicePackageResponse>(servicePackage);
                return new AppResponse<ServicePackageResponse>().SetSuccessResponse(response, "Success", "Service package updated successfully.");
            }
            catch (Exception ex)
            {
                return new AppResponse<ServicePackageResponse>().SetErrorResponse("Error", $"Error updating service package: {ex.Message}");
            }
        }
    }
}
