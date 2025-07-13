using MHS.Common.DTOs;
using MHS.Repository.Interfaces;
using MHS.Repository.Models;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MHS.Service.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="customerRepository"></param>
        /// <param name="logger"></param>
        public CustomerService(
            IUnitOfWork unitOfWork,
            IGenericRepository<Customer> customerRepository,
            ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<AppResponse<string>> DeleteCustomerByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Deleting customer for {userId}", userId);

                var customer = await _customerRepository.FindAsync(query => query.UserId == userId);
                if (customer == null)
                {
                    return new AppResponse<string>().SetErrorResponse(nameof(userId), "Customer not found");
                }

                _ = _customerRepository.Delete(customer);
                _ = await _unitOfWork.CompleteAsync();

                return new AppResponse<string>().SetSuccessResponse(userId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer for {userId}", userId);
                return new AppResponse<string>()
                    .SetErrorResponse("Error", "An error occurred while deleting customer");
            }
        }

        public async Task<AppResponse<GetCustomerProfileResponse>> GetCustomerProfileByUserIdAsync(int userId)
        {
            var customers = await _customerRepository.ListAsync(
                filter: query => query.UserId == userId,
                includeProperties: query => query.Include(x => x.User));
            if (!customers.Any())
            {
                return new AppResponse<GetCustomerProfileResponse>().SetErrorResponse(nameof(userId), "Customer not found");
            }

            var response = customers.Select(customer => new GetCustomerProfileResponse
            {
                UserId = userId,
                DateOfBirth = customer.User.DateOfBirth,
                EmergencyContactName = customer.User.EmergencyContactName,
                EmergencyContactPhone = customer.User?.EmergencyContactPhone,
                FirstName = customer.User.FirstName,
                LastName = customer.User.LastName,
                Gender = customer.User.Gender,
                ProfileImageUrl = customer.User.ProfileImageUrl,
                CustomerAddresses = customer.Addresses.Select(address => new CustomerAddressResponse
                {
                    Id = address.Id,
                    City = address.City,
                    District = address.District,
                    FullAddress = address.FullAddress,
                    IsDefault = address.IsDefault,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude,
                    PostalCode = address.PostalCode,
                    Province = address.Province,
                    SpecialInstructions = address.SpecialInstructions,
                    Street = address.Street,
                    Title = address.Title
                }).ToList(),
            }).FirstOrDefault();

            return new AppResponse<GetCustomerProfileResponse>().SetSuccessResponse(response ?? new());
        }

        public async Task<AppResponse<List<GetCustomerProfileResponse>>> GetCustomerProfilesWithPaginationAsync(PaginationRequest request)
        {
            var response = new AppResponse<List<GetCustomerProfileResponse>>();
            var result = await _customerRepository.ListAsyncWithPaginated(
                includeProperties: query => query.Include(x => x.User).Include(x => x.Addresses),
                pagination: request);

            if (result.TotalCount == 0)
            {
                _ = response.SetErrorResponse("Customers", "Not found any customers");
                return response;
            }

            _ = response.SetSuccessResponse(response.Data ?? []);
            _ = response.SetPagination(pageNumber: result.PageNumber,
                pageSize: result.PageSize,
                totalCount: result.TotalCount);

            return response;
        }

        public async Task<AppResponse<string>> UpdateCustomerProfileAsync(int userId, UpdateCustomerProfileRequest request)
        {
            try
            {
                _logger.LogInformation("Updating customer for {userId}", userId);

                var customer = (await _customerRepository.ListAsync(
                    filter: query => query.UserId == userId,
                    includeProperties: query => query.Include(x => x.User)
                                                     .Include(x => x.Addresses))
                ).FirstOrDefault();

                if (customer == null)
                {
                    return new AppResponse<string>().SetErrorResponse(nameof(userId), "Customer not found");
                }

                customer.User.FirstName = request.FirstName;
                customer.User.LastName = request.LastName;
                customer.User.ProfileImageUrl = request.ProfileImageUrl;
                customer.User.DateOfBirth = request.DateOfBirth;
                customer.User.Gender = request.Gender;
                customer.User.EmergencyContactPhone = request.EmergencyContactPhone;
                customer.User.EmergencyContactName = request.EmergencyContactName;

                var deletedAddresses = customer.Addresses.ToDictionary(x => x.Id);

                if (request.UpdateCustomerAddresses != null)
                {
                    foreach (var address in request.UpdateCustomerAddresses)
                    {
                        if (deletedAddresses.ContainsKey(address.Id))
                        {
                            _ = deletedAddresses.TryGetValue(address.Id, out var addressInDb);
                            UpdateAddress(addressInDb, address);
                        }
                        else if (address.Id == 0)
                        {
                            var addedAddress = new CustomerAddress
                            {
                                Title = address.Title,
                                Street = address.Street,
                                City = address.City,
                                Province = address.Province,
                                PostalCode = address.PostalCode,
                                Latitude = address.Latitude,
                                Longitude = address.Longitude,
                                IsDefault = address.IsDefault,
                                SpecialInstructions = address.SpecialInstructions,
                                FullAddress = string.Join(", ", [address.Street, address.City, address.Province]),
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                            };

                            customer.Addresses.Add(addedAddress);
                        }
                    }
                }

                foreach (var address in customer.Addresses.ToList())
                {
                    if (!deletedAddresses.ContainsKey(address.Id))
                    {
                        _ = customer.Addresses.Remove(address);
                    }
                }

                customer.UpdatedAt = DateTime.UtcNow;
                _ = await _unitOfWork.CompleteAsync();

                return new AppResponse<string>().SetSuccessResponse(userId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer for {userId}", userId);
                return new AppResponse<string>().SetErrorResponse("Error", $"An error occurred while updating customer: {ex.Message}");
            }
        }

        private void UpdateAddress(CustomerAddress addressInDb, UpdateCustomerAddressRequest address)
        {
            addressInDb.Title = address.Title;
            addressInDb.Street = address.Street;
            addressInDb.City = address.City;
            addressInDb.Province = address.Province;
            addressInDb.PostalCode = address.PostalCode;
            addressInDb.Latitude = address.Latitude;
            addressInDb.Longitude = address.Longitude;
            addressInDb.IsDefault = address.IsDefault;
            addressInDb.SpecialInstructions = address.SpecialInstructions;
            addressInDb.FullAddress = string.Join(", ", [address.Street, address.City, address.Province]);
            addressInDb.UpdatedAt = DateTime.UtcNow;
        }

    }
}
