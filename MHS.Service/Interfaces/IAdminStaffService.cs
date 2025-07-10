using MHS.Service.DTOs;

namespace MHS.Service.Interfaces;

public interface IAdminStaffService
{
    Task<AppResponse<PaginatedResponse<AdminStaffListResponse>>> GetAllStaffAsync(AdminStaffSearchRequest request);
    Task<AppResponse<AdminStaffDetailResponse>> GetStaffByIdAsync(int staffId);
    Task<AppResponse<AdminStaffDetailResponse>> CreateStaffAsync(AdminCreateStaffRequest request);
    Task<AppResponse<AdminStaffDetailResponse>> UpdateStaffAsync(int staffId, AdminUpdateStaffRequest request);
    Task<AppResponse<bool>> ChangeStaffStatusAsync(int staffId, StaffStatusChangeRequest request);
    Task<AppResponse<bool>> DeleteStaffAsync(int staffId);
    
    // Staff Skills Management
    Task<AppResponse<StaffSkillResponse>> AddStaffSkillAsync(int staffId, StaffSkillManagementRequest request);
    Task<AppResponse<StaffSkillResponse>> UpdateStaffSkillAsync(int staffId, int skillId, StaffSkillManagementRequest request);
    Task<AppResponse<bool>> RemoveStaffSkillAsync(int staffId, int skillId);
    
    // Staff Reports and Analytics
    Task<AppResponse<List<AdminStaffListResponse>>> GetTopPerformingStaffAsync(int count = 10);
    Task<AppResponse<object>> GetStaffStatisticsAsync();
    Task<AppResponse<List<AdminStaffListResponse>>> GetInactiveStaffAsync(int daysInactive = 30);
}
