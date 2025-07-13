using MHS.Common.DTOs;
using MHS.Service.DTOs;

namespace MHS.Service.Interfaces
{
    public interface IStaffService
    {
        Task<AppResponse<string>> UpdateStaffProfileAsync(int userId, UpdateStaffProfileRequest request);
        Task<AppResponse<string>> DeleteStaffByEmployeeIdAsync(string employeeId);
        Task<AppResponse<GetStaffProfileResponse>> GetStaffProfileByEmloyeeIdAsync(string emloyeeId);
        Task<AppResponse<List<GetStaffProfileResponse>>> GetAllStaffProfilesAsync(PaginationRequest request);
    }
}
