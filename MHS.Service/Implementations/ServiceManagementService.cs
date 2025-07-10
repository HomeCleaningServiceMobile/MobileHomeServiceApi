using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
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

        public async Task<ApiResponse<string>> ActivateServiceAsync(int id, CancellationToken cancellationToken = default)
        {
           try
            {
                var service = await _serviceRepository.GetEntityByIdAsync(id);
                if (service == null)
                    return new ApiResponse<string> { Success = false, Message = "Service not found." };
                service.IsActive = true;
                _serviceRepository.Update(service);
                await _unitOfWork.CompleteAsync(cancellationToken);
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Service activated successfully.",
                    Data = service.Id.ToString()
                };      
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> 
                { 
                    Success = false, 
                    Message = $"Error activating service: {ex.Message}", 
                    Errors = new List<string> { ex.Message } 
                };
            }
        }

        public async Task<ApiResponse<ServicePriceResponse>> CalculateServicePriceAsync(int serviceId, int? servicePackageId, int durationMinutes, CancellationToken cancellationToken = default)
        {
            try
            {
                if (durationMinutes <= 0)
                {
                    return new ApiResponse<ServicePriceResponse>
                    {
                        Success = false,
                        Message = "Duration must be greater than zero."
                    };
                }

                if (serviceId <= 0)
                {
                    return new ApiResponse<ServicePriceResponse>
                    {
                        Success = false,
                        Message = "Invalid service ID."
                    };
                }

                var service = await _serviceRepository.GetEntityByIdAsync(serviceId);
                if (service == null || service.IsDeleted || !service.IsActive )
                {
                    return new ApiResponse<ServicePriceResponse>
                    { 
                        Success = false,
                        Message = "Service not found."
                    };
                }

                ServicePackage servicePackage = null;
                if (servicePackageId.HasValue && servicePackageId.Value > 0)
                {
                    servicePackage = await _servicePackageRepository.GetEntityByIdAsync(servicePackageId.Value);
                    if (servicePackage == null /*|| servicePackage.IsCanceled || servicePackage.IsFaulted*/)
                    {
                        return new ApiResponse<ServicePriceResponse>
                        {
                            Success = false,
                            Message = "Service package not found."
                        };
                    }
                    //AutoMapper to map from ServicePackage to ServicePriceResponse
                    var response = _mapper.Map<ServicePriceResponse>(servicePackage);
                    return new ApiResponse<ServicePriceResponse>
                    {
                        Success = true,
                        Message = "Price calculated successfully.",
                        Data = response
                    };
                } 
                else
                {
                    //AutoMapper to map from Service to ServicePriceResponse
                    var response = _mapper.Map<ServicePriceResponse>(service);
                    response.DurationMinutes = durationMinutes;
                    

                    if (service.HourlyRate.HasValue)
                    {
                        var hours = durationMinutes / 60.0m;
                        response.CalculatedPrice = service.HourlyRate.Value * hours;
                        response.PricingMethod = "Hourly";
                        response.Breakdown = $"Hourly rate: ${service.HourlyRate}/hour × {hours:F2} hours = ${response.CalculatedPrice:F2}";
                    } else
                    {
                        //Use base price
                        response.CalculatedPrice = service.BasePrice;
                        response.PricingMethod = "Fixed";
                        response.Breakdown = $"Base price: ${service.BasePrice}";
                    }
                    return new ApiResponse<ServicePriceResponse>
                    {
                        Success = true,
                        Message = "Price calculated successfully.",
                        Data = response
                    };


                }

            }
            catch (Exception ex)
            {
                return new ApiResponse<ServicePriceResponse>
                {
                    Success = false,
                    Message = $"Error calculating service price: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<ServiceResponse>> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = _mapper.Map<MHS.Repository.Models.Service>(request);

                await _serviceRepository.AddAsync(service);
                await _unitOfWork.CompleteAsync(cancellationToken);

                var response = _mapper.Map<ServiceResponse>(service);

                return new ApiResponse<ServiceResponse>
                {
                    Success = true,
                    Message = "Service created successfully",
                    Data = response
                };

            } catch (Exception ex) {
                return new ApiResponse<ServiceResponse>
                {
                    Success = false,
                    Message = $"Error creating service: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<ServicePackageResponse>> CreateServicePackageAsync(CreateServicePackageRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Lấy service từ _serviceRepository thay vì _servicePackageRepository
                var service = await _serviceRepository.GetEntityByIdAsync(request.ServiceId);
                if (service == null)
                {
                    return new ApiResponse<ServicePackageResponse>
                    {
                        Success = false,
                        Message = "Service not found."
                    };
                }

                // AutoMapper map ServicePackage from CreateServicePackageRequest
                var servicePackage = _mapper.Map<ServicePackage>(request);
                servicePackage.IsActive = true; // set default value

                await _servicePackageRepository.AddAsync(servicePackage); // dùng AddAsync nếu có
                await _unitOfWork.CompleteAsync(cancellationToken);
                var response = _mapper.Map<ServicePackageResponse>(servicePackage);
                return new ApiResponse<ServicePackageResponse>
                {
                    Success = true,
                    Message = "Service package created successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ServicePackageResponse>
                {
                    Success = false,
                    Message = $"Error creating service package: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<string>> DeactivateServiceAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var service  = await _serviceRepository.GetEntityByIdAsync(id);
                if (service == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Service not found."
                    };
                }

                service.IsActive = false;
                _serviceRepository.Update(service);

                await _unitOfWork.CompleteAsync(cancellationToken);
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Service deactivated successfully.",
                    Data = service.Id.ToString()
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error deactivating service: {e.Message}",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteServiceAsync(int id, CancellationToken cancellationToken = default)
        {
            var service = await _serviceRepository.GetEntityByIdAsync(id);
            if (service == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Service not found."
                };
            }
            _serviceRepository.Delete(service);

            await _unitOfWork.CompleteAsync(cancellationToken);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Service deleted successfully.",
                Data = service.Id.ToString()
            };
        }

        public async Task<ApiResponse<string>> DeleteServicePackageAsync(int id, CancellationToken cancellationToken = default)
        {
            var servicePackage = await _servicePackageRepository.GetEntityByIdAsync(id);
            if (servicePackage == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Service package not found."
                };
            }
            _servicePackageRepository.Delete(servicePackage);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return new ApiResponse<string>
            {
                Success = true,
                Message = "Service package deleted successfully.",
                Data = servicePackage.Id.ToString()
            };
        }

        public async Task<ApiResponse<List<ServiceResponse>>> GetPopularServicesAsync(int limit = 10, CancellationToken cancellationToken = default)
        {
            var service =  await _serviceRepository.ListAsync(
                filter: s => s.IsActive,
                orderBy: q => q.OrderByDescending(s => s.CreatedAt),
                includeProperties: q => q.Include(s => s.ServicePackages)
            );

            var topService = service.Take(limit).ToList();

            var response = _mapper.Map<List<ServiceResponse>>(topService);

            return new ApiResponse<List<ServiceResponse>>
            {
                Success = true,
                Message = "Popular services retrieved successfully.",
                Data = response
            };

        }

        public async Task<ApiResponse<ServiceResponse>> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var service = await _serviceRepository.GetEntityByIdAsync(id);
            if (service == null)
            {
                return new ApiResponse<ServiceResponse>
                {
                    Success = false,
                    Message = "Service not found."
                };
            }
            var response = _mapper.Map<ServiceResponse>(service);
            return new ApiResponse<ServiceResponse>
            {
                Success = true,
                Message = "Service retrieved successfully.",
                Data = response
            };
        }

        public async Task<ApiResponse<ServicePackageResponse>> GetServicePackageByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var servicePackage = await _servicePackageRepository.GetEntityByIdAsync(id);
            if (servicePackage == null)
            {
                return new ApiResponse<ServicePackageResponse>
                {
                    Success = false,
                    Message = "Service package not found."
                };
            }
            var response = _mapper.Map<ServicePackageResponse>(servicePackage);
            return new ApiResponse<ServicePackageResponse>
            {
                Success = true,
                Message = "Service package retrieved successfully.",
                Data = response
            };
        }

        public async Task<ApiResponse<List<ServicePackageResponse>>> GetServicePackagesByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default)
        {
            var package = await _servicePackageRepository.ListAsync(
                filter: sp => sp.ServiceId == serviceId && sp.IsActive,
                orderBy: q => q.OrderBy(sp => sp.SortOrder)
            );

            if (package == null)
            {
                return new ApiResponse<List<ServicePackageResponse>>
                {
                    Success = false,
                    Message = "No service packages found for this service."
                };
            }

            var response = _mapper.Map<List<ServicePackageResponse>>(package);
            return new ApiResponse<List<ServicePackageResponse>>
            {
                Success = true,
                Message = "Service packages retrieved successfully.",
                Data = response
            };

        }

        public async Task<ApiResponse<PaginatedResponse<ServiceSummaryResponse>>> GetServicesAsync(ServiceListRequest request, CancellationToken cancellationToken = default)
        {
            // filter động
            Expression<Func<MHS.Repository.Models.Service, bool>> filter = s =>
             (request.Type == null || s.Type == request.Type) &&
             (request.IsActive == null || s.IsActive == request.IsActive) &&
             (request.MinPrice == null || s.BasePrice >= request.MinPrice) &&
             (request.MaxPrice == null || s.BasePrice <= request.MaxPrice) &&
             (string.IsNullOrEmpty(request.SearchTerm) || s.Name.Contains(request.SearchTerm));


            //PHÂN TRANG
            var pagination = new PaginationRequest
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var pagedList = await _serviceRepository.ListAsyncWithPaginated(
                filter: filter,
                orderBy: q => q.OrderBy(s => s.Name),
                includeProperties: q => q.Include(s => s.ServicePackages),
                pagination: pagination,
                cancellationToken: cancellationToken
            );

            var item = _mapper.Map<List<ServiceSummaryResponse>>(pagedList.Items);

            var response = new PaginatedResponse<ServiceSummaryResponse>
            {
                Items = item,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
                TotalPages = pagedList.TotalPages
            };

            return new ApiResponse<PaginatedResponse<ServiceSummaryResponse>>
            {
                Success = true,
                Message = "Services retrieved successfully.",
                Data = response
            };
        }


        public async Task<ApiResponse<List<ServiceResponse>>> GetServicesByTypeAsync(ServiceType type, CancellationToken cancellationToken = default)
        {
            var service = await _serviceRepository.ListAsync(
                filter: s => s.Type == type && s.IsActive,
                orderBy: q => q.OrderBy(s => s.Name),
                includeProperties: q => q.Include(s => s.ServicePackages)
            );
            if (service == null || !service.Any())
            {
                return new ApiResponse<List<ServiceResponse>>
                {
                    Success = false,
                    Message = "No services found for this type."
                };
            }
            var response = _mapper.Map<List<ServiceResponse>>(service);
            return new ApiResponse<List<ServiceResponse>>
            {
                Success = true,
                Message = "Services retrieved successfully.",
                Data = response
            };
        }

        public async Task<ApiResponse<ServiceResponse>> UpdateServiceAsync(int id, UpdateServiceRequest request, CancellationToken cancellationToken = default)
        {
            var service = await _serviceRepository.GetEntityByIdAsync(id);
            if (service == null)
            {
                return new ApiResponse<ServiceResponse>
                {
                    Success = false,
                    Message = "Service not found."
                };
            }

            //map tu request sang entity de cap nhat
            _mapper.Map(request, service);

            _serviceRepository.Update(service);
             _unitOfWork.CompleteAsync(cancellationToken);

            var response = _mapper.Map<ServiceResponse>(service);
            return new ApiResponse<ServiceResponse>
            {
                Success = true,
                Message = "Service updated successfully.",
                Data = response
            };
        }

        public async Task<ApiResponse<ServicePackageResponse>> UpdateServicePackageAsync(int id, CreateServicePackageRequest request, CancellationToken cancellationToken = default)
        {
            var package = await _servicePackageRepository.GetEntityByIdAsync(id);

            if (package == null)
            {
                return new ApiResponse<ServicePackageResponse>
                {
                    Success = false,
                    Message = "Service package not found."
                };
            }

            _mapper.Map(request, package);

            _servicePackageRepository.Update(package);

            await _unitOfWork.CompleteAsync(cancellationToken);

            var response = _mapper.Map<ServicePackageResponse>(package);

            return new ApiResponse<ServicePackageResponse>
            {
                Success = true,
                Message = "Service package updated successfully.",
                Data = response
            };
        }
    }
}
