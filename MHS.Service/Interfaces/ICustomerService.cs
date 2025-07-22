using MHS.Common.DTOs;
using MHS.Service.DTOs;

namespace MHS.Service.Interfaces
{
    public interface ICustomerService
    {
        Task<AppResponse<string>> UpdateCustomerProfileAsync(int userId, UpdateCustomerProfileRequest request);
        Task<AppResponse<string>> DeleteCustomerByUserIdAsync(int userId);
        Task<AppResponse<List<GetCustomerProfileResponse>>> GetCustomerProfilesWithPaginationAsync(PaginationRequest request);
        Task<AppResponse<GetCustomerProfileResponse>> GetCustomerProfileByUserIdAsync(int userId);
    }
}
