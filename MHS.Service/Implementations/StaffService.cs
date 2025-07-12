using MHS.Common.DTOs;
using MHS.Repository.Interfaces;
using MHS.Repository.Models;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MHS.Service.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Staff> _staffRepository;
        private readonly ILogger<StaffService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="logger"></param>
        public StaffService(IUnitOfWork unitOfWork, ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _staffRepository = _unitOfWork.Repository<Staff>();
        }

        public async Task<AppResponse<string>> DeleteStaffByEmployeeIdAsync(string employeeId)
        {
            try
            {
                _logger.LogInformation("Deleting staff for {employeeId}", employeeId);

                var staff = await _staffRepository.FindAsync(query => query.EmployeeId == employeeId);
                if (staff == null)
                {
                    return new AppResponse<string>().SetErrorResponse(nameof(employeeId), "Staff not found");
                }

                _ = _staffRepository.Delete(staff);
                _ = await _unitOfWork.CompleteAsync();

                return new AppResponse<string>().SetSuccessResponse(employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff for {employeeId}", employeeId);
                return new AppResponse<string>()
                    .SetErrorResponse("Error", "An error occurred while deleting staff");
            }
        }

        public async Task<AppResponse<List<GetStaffProfileResponse>>> GetAllStaffProfilesAsync(PaginationRequest request)
        {
            var response = new AppResponse<List<GetStaffProfileResponse>>();
            var result = await _staffRepository.ListAsyncWithPaginated(pagination: request);
            if (result.TotalCount == 0)
            {
                _ = response.SetErrorResponse("Staffs", "Not found any staffs");
                return response;
            }

            _ = response.SetSuccessResponse(response.Data ?? []);
            _ = response.SetPagination(pageNumber: result.PageNumber,
                pageSize: result.PageSize,
                totalCount: result.TotalCount);

            return response;
        }

        public async Task<AppResponse<GetStaffProfileResponse>> GetStaffProfileByEmloyeeIdAsync(string emloyeeId)
        {
            var staffs = await _staffRepository.ListAsync(
                filter: query => query.EmployeeId == emloyeeId,
                includeProperties: query => query.Include(x => x.User));
            if (!staffs.Any())
            {
                return new AppResponse<GetStaffProfileResponse>().SetErrorResponse(nameof(emloyeeId), "Staff not found");
            }

            var response = staffs.Select(staff => new GetStaffProfileResponse
            {
                FirstName = staff.User.FirstName,
                LastName = staff.User.LastName,
                ProfileImageUrl = staff.User.ProfileImageUrl,
                DateOfBirth = staff.User.DateOfBirth,
                Gender = staff.User.Gender,
                EmergencyContactPhone = staff.User.EmergencyContactPhone,
                EmergencyContactName = staff.User.EmergencyContactName,
                Skills = staff.Skills,
                Bio = staff.Bio,
            }).FirstOrDefault();

            return new AppResponse<GetStaffProfileResponse>().SetSuccessResponse(response ?? new());
        }

        public async Task<AppResponse<string>> UpdateStaffProfileAsync(int userId, UpdateStaffProfileRequest request)
        {
            try
            {
                _logger.LogInformation("Deleting staff for userId {userId}", userId);

                var staff = (await _staffRepository
                    .ListAsync(
                    filter: query => query.UserId == userId,
                    includeProperties: query => query.Include(x => x.User))
                    ).FirstOrDefault();

                if (staff == null)
                {
                    return new AppResponse<string>().SetErrorResponse(nameof(userId), "Staff not found");
                }

                staff.User.FirstName = request.FirstName;
                staff.User.LastName = request.LastName;
                staff.User.ProfileImageUrl = request.ProfileImageUrl;
                staff.User.DateOfBirth = request.DateOfBirth;
                staff.User.Gender = request.Gender;
                staff.User.EmergencyContactPhone = request.EmergencyContactPhone;
                staff.User.EmergencyContactName = request.EmergencyContactName;
                staff.Skills = request.Skills;
                staff.Bio = request.Bio;
                staff.UpdatedAt = DateTime.UtcNow;

                _staffRepository.Update(staff);
                _ = await _unitOfWork.CompleteAsync();

                return new AppResponse<string>().SetSuccessResponse(string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff for userId {userId}", userId);
                return new AppResponse<string>()
                    .SetErrorResponse("Error", "An error occurred while updating staff");
            }
        }
    }
}
