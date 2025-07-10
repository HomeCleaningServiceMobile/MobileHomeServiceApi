using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MHS.Common.Enums;
using MHS.Repository.Interfaces;
using MHS.Repository.Models;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace MHS.Service.Implementations;

public class AdminStaffService : IAdminStaffService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminStaffService> _logger;

    public AdminStaffService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        ILogger<AdminStaffService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<AppResponse<PaginatedResponse<AdminStaffListResponse>>> GetAllStaffAsync(AdminStaffSearchRequest request)
    {
        try
        {
            // Build filter expression
            Expression<Func<Staff, bool>>? filter = null;
            if (!string.IsNullOrEmpty(request.SearchTerm) || !string.IsNullOrEmpty(request.EmployeeId) || 
                request.IsAvailable.HasValue || request.Status.HasValue || request.IsDeleted.HasValue ||
                request.HireDateFrom.HasValue || request.HireDateTo.HasValue ||
                request.MinRating.HasValue || request.MaxRating.HasValue ||
                !string.IsNullOrEmpty(request.Skills))
            {
                filter = s => 
                    (string.IsNullOrEmpty(request.SearchTerm) || 
                     s.User.FirstName.Contains(request.SearchTerm) ||
                     s.User.LastName.Contains(request.SearchTerm) ||
                     s.User.Email.Contains(request.SearchTerm) ||
                     s.EmployeeId.Contains(request.SearchTerm)) &&
                    (string.IsNullOrEmpty(request.EmployeeId) || s.EmployeeId == request.EmployeeId) &&
                    (!request.IsAvailable.HasValue || s.IsAvailable == request.IsAvailable.Value) &&
                    (!request.Status.HasValue || s.User.Status == request.Status.Value) &&
                    (!request.IsDeleted.HasValue || s.IsDeleted == request.IsDeleted.Value) &&
                    (!request.HireDateFrom.HasValue || s.HireDate >= request.HireDateFrom.Value) &&
                    (!request.HireDateTo.HasValue || s.HireDate <= request.HireDateTo.Value) &&
                    (!request.MinRating.HasValue || s.AverageRating >= request.MinRating.Value) &&
                    (!request.MaxRating.HasValue || s.AverageRating <= request.MaxRating.Value) &&
                    (string.IsNullOrEmpty(request.Skills) || (s.Skills != null && s.Skills.Contains(request.Skills)));
            }

            // Build include expression
            Func<IQueryable<Staff>, IIncludableQueryable<Staff, object>> includeProperties = q => q
                .Include(s => s.User)
                .Include(s => s.StaffSkills)
                .ThenInclude(ss => ss.Service);

            // Build order expression
            Func<IQueryable<Staff>, IOrderedQueryable<Staff>>? orderBy = request.SortBy?.ToLower() switch
            {
                "firstname" => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.User.FirstName) : 
                    q => q.OrderBy(s => s.User.FirstName),
                "lastname" => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.User.LastName) : 
                    q => q.OrderBy(s => s.User.LastName),
                "email" => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.User.Email) : 
                    q => q.OrderBy(s => s.User.Email),
                "employeeid" => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.EmployeeId) : 
                    q => q.OrderBy(s => s.EmployeeId),
                "hiredate" => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.HireDate) : 
                    q => q.OrderBy(s => s.HireDate),
                "rating" => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.AverageRating) : 
                    q => q.OrderBy(s => s.AverageRating),
                "completedjobs" => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.TotalCompletedJobs) : 
                    q => q.OrderBy(s => s.TotalCompletedJobs),
                "status" => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.User.Status) : 
                    q => q.OrderBy(s => s.User.Status),
                _ => request.SortDescending ? 
                    q => q.OrderByDescending(s => s.CreatedAt) : 
                    q => q.OrderBy(s => s.CreatedAt)
            };

            var staffList = await _unitOfWork.Repository<Staff>().ListAsyncWithPaginated(
                filter, orderBy, includeProperties, request);

            var staffResponses = staffList.Items.Select(staff => new AdminStaffListResponse
            {
                Id = staff.Id,
                UserId = staff.UserId,
                EmployeeId = staff.EmployeeId,
                FullName = $"{staff.User.FirstName ?? ""} {staff.User.LastName ?? ""}".Trim(),
                Email = staff.User.Email ?? "",
                PhoneNumber = staff.User.PhoneNumber,
                HireDate = staff.HireDate,
                Skills = staff.Skills,
                HourlyRate = staff.HourlyRate,
                AverageRating = staff.AverageRating,
                TotalCompletedJobs = staff.TotalCompletedJobs,
                IsAvailable = staff.IsAvailable,
                Status = staff.User.Status,
                IsDeleted = staff.IsDeleted,
                LastActiveAt = staff.LastActiveAt,
                ServiceRadiusKm = staff.ServiceRadiusKm,
                CertificationImageUrl = staff.CertificationImageUrl,
                IdCardImageUrl = staff.IdCardImageUrl,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt
            }).ToList();

            var paginatedResponse = new PaginatedResponse<AdminStaffListResponse>
            {
                Data = staffResponses,
                TotalCount = staffList.TotalCount,
                PageNumber = staffList.PageNumber,
                PageSize = staffList.PageSize
            };

            return new AppResponse<PaginatedResponse<AdminStaffListResponse>>()
                .SetSuccessResponse(paginatedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff list");
            return new AppResponse<PaginatedResponse<AdminStaffListResponse>>().SetErrorResponse("Error", "Failed to retrieve staff list");
        }
    }

    public async Task<AppResponse<AdminStaffDetailResponse>> GetStaffByIdAsync(int staffId)
    {
        try
        {
            var staff = await _unitOfWork.Repository<Staff>().ListAsync(
                s => s.Id == staffId,
                null,
                q => q.Include(s => s.User)
                      .Include(s => s.StaffSkills)
                      .ThenInclude(ss => ss.Service)
                      .Include(s => s.Bookings.Take(10))
                      .ThenInclude(b => b.Customer)
                      .ThenInclude(c => c.User));

            var staffEntity = staff.FirstOrDefault();
            if (staffEntity == null)
            {
                return new AppResponse<AdminStaffDetailResponse>()
                    .SetErrorResponse("NotFound", "Staff not found");
            }

            var response = new AdminStaffDetailResponse
            {
                Id = staffEntity.Id,
                UserId = staffEntity.UserId,
                EmployeeId = staffEntity.EmployeeId,
                FullName = $"{staffEntity.User.FirstName ?? ""} {staffEntity.User.LastName ?? ""}".Trim(),
                Email = staffEntity.User.Email ?? "",
                PhoneNumber = staffEntity.User.PhoneNumber,
                HireDate = staffEntity.HireDate,
                Skills = staffEntity.Skills,
                Bio = staffEntity.Bio,
                HourlyRate = staffEntity.HourlyRate,
                AverageRating = staffEntity.AverageRating,
                TotalCompletedJobs = staffEntity.TotalCompletedJobs,
                IsAvailable = staffEntity.IsAvailable,
                Status = staffEntity.User.Status,
                IsDeleted = staffEntity.IsDeleted,
                LastActiveAt = staffEntity.LastActiveAt,
                ServiceRadiusKm = staffEntity.ServiceRadiusKm,
                CurrentLatitude = staffEntity.CurrentLatitude,
                CurrentLongitude = staffEntity.CurrentLongitude,
                FullAddress = staffEntity.FullAddress,
                Street = staffEntity.Street,
                District = staffEntity.District,
                City = staffEntity.City,
                Province = staffEntity.Province,
                PostalCode = staffEntity.PostalCode,
                CertificationImageUrl = staffEntity.CertificationImageUrl,
                IdCardImageUrl = staffEntity.IdCardImageUrl,
                CreatedAt = staffEntity.CreatedAt,
                UpdatedAt = staffEntity.UpdatedAt,
                User = _mapper.Map<UserResponse>(staffEntity.User),
                StaffSkills = _mapper.Map<List<StaffSkillResponse>>(staffEntity.StaffSkills),
                RecentBookings = _mapper.Map<List<BookingResponse>>(staffEntity.Bookings.Take(10))
            };

            return new AppResponse<AdminStaffDetailResponse>()
                .SetSuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff detail for ID {StaffId}", staffId);
            return new AppResponse<AdminStaffDetailResponse>()
                .SetErrorResponse("Error", "Failed to retrieve staff details");
        }
    }

    public async Task<AppResponse<AdminStaffDetailResponse>> CreateStaffAsync(AdminCreateStaffRequest request)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AppResponse<AdminStaffDetailResponse>()
                    .SetErrorResponse("Email", "Email already exists");
            }

            // Check if employee ID already exists
            var existingStaffList = await _unitOfWork.Repository<Staff>()
                .ListAsync(s => s.EmployeeId == request.EmployeeId && !s.IsDeleted);

            if (existingStaffList.Any())
            {
                return new AppResponse<AdminStaffDetailResponse>()
                    .SetErrorResponse("EmployeeId", "Employee ID already exists");
            }

            // Create user
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Staff,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var createUserResult = await _userManager.CreateAsync(user, request.Password);
            if (!createUserResult.Succeeded)
            {
                return new AppResponse<AdminStaffDetailResponse>()
                    .SetErrorResponse("User", string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
            }

            // Add to Staff role
            await _userManager.AddToRoleAsync(user, UserRole.Staff.ToString());

            // Create staff profile
            var staff = new Staff
            {
                UserId = user.Id,
                EmployeeId = request.EmployeeId,
                HireDate = request.HireDate,
                Skills = request.Skills,
                Bio = request.Bio,
                HourlyRate = request.HourlyRate,
                ServiceRadiusKm = request.ServiceRadiusKm,
                FullAddress = request.FullAddress,
                Street = request.Street,
                District = request.District,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                CertificationImageUrl = request.CertificationImageUrl,
                IdCardImageUrl = request.IdCardImageUrl,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Staff>().AddAsync(staff);
            await _unitOfWork.CompleteAsync();

            return await GetStaffByIdAsync(staff.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff");
            return new AppResponse<AdminStaffDetailResponse>()
                .SetErrorResponse("Error", "Failed to create staff");
        }
    }

    public async Task<AppResponse<AdminStaffDetailResponse>> UpdateStaffAsync(int staffId, AdminUpdateStaffRequest request)
    {
        try
        {
            var staffList = await _unitOfWork.Repository<Staff>()
                .ListAsync(s => s.Id == staffId, null, 
                    q => q.Include(s => s.User));

            var staff = staffList.FirstOrDefault();
            if (staff == null)
            {
                return new AppResponse<AdminStaffDetailResponse>()
                    .SetErrorResponse("NotFound", "Staff not found");
            }

            // Update user information
            if (!string.IsNullOrEmpty(request.FirstName))
                staff.User.FirstName = request.FirstName;
            
            if (!string.IsNullOrEmpty(request.LastName))
                staff.User.LastName = request.LastName;
            
            if (!string.IsNullOrEmpty(request.Email))
                staff.User.Email = staff.User.UserName = request.Email;
            
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                staff.User.PhoneNumber = request.PhoneNumber;

            // Update staff information
            if (!string.IsNullOrEmpty(request.EmployeeId))
            {
                // Check if new employee ID already exists
                var existingStaffList = await _unitOfWork.Repository<Staff>()
                    .ListAsync(s => s.EmployeeId == request.EmployeeId && s.Id != staffId);

                if (existingStaffList.Any())
                {
                    return new AppResponse<AdminStaffDetailResponse>()
                        .SetErrorResponse("EmployeeId", "Employee ID already exists");
                }
                
                staff.EmployeeId = request.EmployeeId;
            }

            if (request.HireDate.HasValue)
                staff.HireDate = request.HireDate.Value;
            
            if (request.Skills != null)
                staff.Skills = request.Skills;
            
            if (request.Bio != null)
                staff.Bio = request.Bio;
            
            if (request.HourlyRate.HasValue)
                staff.HourlyRate = request.HourlyRate.Value;
            
            if (request.IsAvailable.HasValue)
                staff.IsAvailable = request.IsAvailable.Value;
            
            if (request.ServiceRadiusKm.HasValue)
                staff.ServiceRadiusKm = request.ServiceRadiusKm.Value;
            
            if (request.FullAddress != null)
                staff.FullAddress = request.FullAddress;
            
            if (request.Street != null)
                staff.Street = request.Street;
            
            if (request.District != null)
                staff.District = request.District;
            
            if (request.City != null)
                staff.City = request.City;
            
            if (request.Province != null)
                staff.Province = request.Province;
            
            if (request.PostalCode != null)
                staff.PostalCode = request.PostalCode;
            
            if (request.CertificationImageUrl != null)
                staff.CertificationImageUrl = request.CertificationImageUrl;
            
            if (request.IdCardImageUrl != null)
                staff.IdCardImageUrl = request.IdCardImageUrl;

            staff.UpdatedAt = DateTime.UtcNow;
            staff.User.UpdatedAt = DateTime.UtcNow;

            var updateUserResult = await _userManager.UpdateAsync(staff.User);
            if (!updateUserResult.Succeeded)
            {
                return new AppResponse<AdminStaffDetailResponse>()
                    .SetErrorResponse("User", string.Join(", ", updateUserResult.Errors.Select(e => e.Description)));
            }

            _unitOfWork.Repository<Staff>().Update(staff);
            await _unitOfWork.CompleteAsync();

            return await GetStaffByIdAsync(staffId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff with ID {StaffId}", staffId);
            return new AppResponse<AdminStaffDetailResponse>()
                .SetErrorResponse("Error", "Failed to update staff");
        }
    }

    public async Task<AppResponse<bool>> ChangeStaffStatusAsync(int staffId, StaffStatusChangeRequest request)
    {
        try
        {
            var staffList = await _unitOfWork.Repository<Staff>()
                .ListAsync(s => s.Id == staffId && !s.IsDeleted, null,
                    q => q.Include(s => s.User));

            var staff = staffList.FirstOrDefault();
            if (staff == null)
            {
                return new AppResponse<bool>()
                    .SetErrorResponse("NotFound", "Staff not found");
            }

            staff.User.Status = request.Status;
            staff.User.UpdatedAt = DateTime.UtcNow;

            // Make staff unavailable when status is not Active
            if (request.Status != UserStatus.Active)
            {
                staff.IsAvailable = false;
            }

            var updateResult = await _userManager.UpdateAsync(staff.User);
            if (!updateResult.Succeeded)
            {
                return new AppResponse<bool>()
                    .SetErrorResponse("User", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }

            _unitOfWork.Repository<Staff>().Update(staff);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>()
                .SetSuccessResponse(true, "Status", $"Staff status changed to {request.Status} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing staff status with ID {StaffId}", staffId);
            return new AppResponse<bool>()
                .SetErrorResponse("Error", "Failed to update staff status");
        }
    }

    public async Task<AppResponse<bool>> DeleteStaffAsync(int staffId)
    {
        try
        {
            var staffList = await _unitOfWork.Repository<Staff>()
                .ListAsync(s => s.Id == staffId && !s.IsDeleted, null,
                    q => q.Include(s => s.User).Include(s => s.Bookings));

            var staff = staffList.FirstOrDefault();
            if (staff == null)
            {
                return new AppResponse<bool>()
                    .SetErrorResponse("NotFound", "Staff not found");
            }

            // Check if staff has active bookings
            var hasActiveBookings = staff.Bookings.Any(b => 
                b.Status == BookingStatus.Pending || 
                b.Status == BookingStatus.Confirmed || 
                b.Status == BookingStatus.InProgress);

            if (hasActiveBookings)
            {
                return new AppResponse<bool>()
                    .SetErrorResponse("Validation", "Cannot delete staff with active bookings");
            }

            // Soft delete - set IsDeleted flag and deactivate
            staff.IsDeleted = true;
            staff.DeletedAt = DateTime.UtcNow;
            staff.IsAvailable = false;
            staff.User.Status = UserStatus.Inactive;
            staff.User.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(staff.User);
            _unitOfWork.Repository<Staff>().Update(staff);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>()
                .SetSuccessResponse(true, "Success", "Staff deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff with ID {StaffId}", staffId);
            return new AppResponse<bool>()
                .SetErrorResponse("Error", "Failed to delete staff");
        }
    }

    public async Task<AppResponse<StaffSkillResponse>> AddStaffSkillAsync(int staffId, StaffSkillManagementRequest request)
    {
        try
        {
            var staffList = await _unitOfWork.Repository<Staff>()
                .ListAsync(s => s.Id == staffId);

            if (!staffList.Any())
            {
                return new AppResponse<StaffSkillResponse>()
                    .SetErrorResponse("NotFound", "Staff not found");
            }

            // Check if skill already exists for this staff
            var existingSkillList = await _unitOfWork.Repository<StaffSkill>()
                .ListAsync(ss => ss.StaffId == staffId && ss.ServiceId == request.ServiceId);

            if (existingSkillList.Any())
            {
                return new AppResponse<StaffSkillResponse>()
                    .SetErrorResponse("Validation", "Staff already has this skill");
            }

            var staffSkill = new StaffSkill
            {
                StaffId = staffId,
                ServiceId = request.ServiceId,
                SkillLevel = request.SkillLevel,
                IsActive = request.IsActive,
                CertifiedAt = request.CertifiedAt,
                CertificationUrl = request.CertificationUrl,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<StaffSkill>().AddAsync(staffSkill);
            await _unitOfWork.CompleteAsync();

            var skillWithService = await _unitOfWork.Repository<StaffSkill>()
                .ListAsync(ss => ss.Id == staffSkill.Id, null,
                    q => q.Include(ss => ss.Service));

            return new AppResponse<StaffSkillResponse>()
                .SetSuccessResponse(_mapper.Map<StaffSkillResponse>(skillWithService.FirstOrDefault()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding skill to staff {StaffId}", staffId);
            return new AppResponse<StaffSkillResponse>()
                .SetErrorResponse("Error", "Failed to add staff skill");
        }
    }

    public async Task<AppResponse<StaffSkillResponse>> UpdateStaffSkillAsync(int staffId, int skillId, StaffSkillManagementRequest request)
    {
        try
        {
            var staffSkillList = await _unitOfWork.Repository<StaffSkill>()
                .ListAsync(ss => ss.Id == skillId && ss.StaffId == staffId, null,
                    q => q.Include(ss => ss.Service));

            var staffSkill = staffSkillList.FirstOrDefault();
            if (staffSkill == null)
            {
                return new AppResponse<StaffSkillResponse>()
                    .SetErrorResponse("NotFound", "Staff skill not found");
            }

            staffSkill.ServiceId = request.ServiceId;
            staffSkill.SkillLevel = request.SkillLevel;
            staffSkill.IsActive = request.IsActive;
            staffSkill.CertifiedAt = request.CertifiedAt;
            staffSkill.CertificationUrl = request.CertificationUrl;
            staffSkill.Notes = request.Notes;
            staffSkill.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<StaffSkill>().Update(staffSkill);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<StaffSkillResponse>()
                .SetSuccessResponse(_mapper.Map<StaffSkillResponse>(staffSkill));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff skill {SkillId} for staff {StaffId}", skillId, staffId);
            return new AppResponse<StaffSkillResponse>()
                .SetErrorResponse("Error", "Failed to update staff skill");
        }
    }

    public async Task<AppResponse<bool>> RemoveStaffSkillAsync(int staffId, int skillId)
    {
        try
        {
            var staffSkillList = await _unitOfWork.Repository<StaffSkill>()
                .ListAsync(ss => ss.Id == skillId && ss.StaffId == staffId);

            var staffSkill = staffSkillList.FirstOrDefault();
            if (staffSkill == null)
            {
                return new AppResponse<bool>()
                    .SetErrorResponse("NotFound", "Staff skill not found");
            }

            _unitOfWork.Repository<StaffSkill>().Delete(staffSkill);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>()
                .SetSuccessResponse(true, "Success", "Staff skill removed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing staff skill {SkillId} for staff {StaffId}", skillId, staffId);
            return new AppResponse<bool>()
                .SetErrorResponse("Error", "Failed to remove staff skill");
        }
    }

    public async Task<AppResponse<List<AdminStaffListResponse>>> GetTopPerformingStaffAsync(int count = 10)
    {
        try
        {
            var topStaff = await _unitOfWork.Repository<Staff>()
                .ListAsync(
                    s => !s.IsDeleted && s.User.Status == UserStatus.Active && s.IsAvailable,
                    q => q.OrderByDescending(s => s.AverageRating).ThenByDescending(s => s.TotalCompletedJobs),
                    q => q.Include(s => s.User));

            var limitedStaff = topStaff.Take(count).ToList();

            var response = limitedStaff.Select(staff => new AdminStaffListResponse
            {
                Id = staff.Id,
                UserId = staff.UserId,
                EmployeeId = staff.EmployeeId,
                FullName = $"{staff.User.FirstName ?? ""} {staff.User.LastName ?? ""}".Trim(),
                Email = staff.User.Email ?? "",
                PhoneNumber = staff.User.PhoneNumber,
                HireDate = staff.HireDate,
                Skills = staff.Skills,
                HourlyRate = staff.HourlyRate,
                AverageRating = staff.AverageRating,
                TotalCompletedJobs = staff.TotalCompletedJobs,
                IsAvailable = staff.IsAvailable,
                Status = staff.User.Status,
                IsDeleted = staff.IsDeleted,
                LastActiveAt = staff.LastActiveAt,
                ServiceRadiusKm = staff.ServiceRadiusKm,
                CertificationImageUrl = staff.CertificationImageUrl,
                IdCardImageUrl = staff.IdCardImageUrl,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt
            }).ToList();

            return new AppResponse<List<AdminStaffListResponse>>()
                .SetSuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top performing staff");
            return new AppResponse<List<AdminStaffListResponse>>()
                .SetErrorResponse("Error", "Failed to retrieve top performing staff");
        }
    }

    public async Task<AppResponse<object>> GetStaffStatisticsAsync()
    {
        try
        {
            var allStaff = await _unitOfWork.Repository<Staff>()
                .ListAsync(null, null, q => q.Include(s => s.User));

            var totalStaff = allStaff.Count;
            var activeStaff = allStaff.Count(s => !s.IsDeleted);
            var availableStaff = allStaff.Count(s => !s.IsDeleted && s.IsAvailable);
            var deletedStaff = allStaff.Count(s => s.IsDeleted);
            
            var activeNonDeletedStaff = allStaff.Where(s => !s.IsDeleted && s.User.Status == UserStatus.Active).ToList();
            var averageRating = activeNonDeletedStaff.Any() ? 
                activeNonDeletedStaff.Average(s => (double)s.AverageRating) : 0;

            // Status breakdown
            var statusBreakdown = allStaff
                .Where(s => !s.IsDeleted)
                .GroupBy(s => s.User.Status)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            var stats = new
            {
                TotalStaff = totalStaff,
                ActiveStaff = activeStaff,
                AvailableStaff = availableStaff,
                DeletedStaff = deletedStaff,
                AverageRating = Math.Round(averageRating, 2),
                StatusBreakdown = statusBreakdown
            };

            return new AppResponse<object>()
                .SetSuccessResponse(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff statistics");
            return new AppResponse<object>()
                .SetErrorResponse("Error", "Failed to retrieve staff statistics");
        }
    }

    public async Task<AppResponse<List<AdminStaffListResponse>>> GetInactiveStaffAsync(int daysInactive = 30)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysInactive);
            
            var inactiveStaff = await _unitOfWork.Repository<Staff>()
                .ListAsync(
                    s => !s.IsDeleted && s.User.Status == UserStatus.Active && 
                         (s.LastActiveAt == null || s.LastActiveAt < cutoffDate),
                    q => q.OrderBy(s => s.LastActiveAt),
                    q => q.Include(s => s.User));

            var response = inactiveStaff.Select(staff => new AdminStaffListResponse
            {
                Id = staff.Id,
                UserId = staff.UserId,
                EmployeeId = staff.EmployeeId,
                FullName = $"{staff.User.FirstName ?? ""} {staff.User.LastName ?? ""}".Trim(),
                Email = staff.User.Email ?? "",
                PhoneNumber = staff.User.PhoneNumber,
                HireDate = staff.HireDate,
                Skills = staff.Skills,
                HourlyRate = staff.HourlyRate,
                AverageRating = staff.AverageRating,
                TotalCompletedJobs = staff.TotalCompletedJobs,
                IsAvailable = staff.IsAvailable,
                Status = staff.User.Status,
                IsDeleted = staff.IsDeleted,
                LastActiveAt = staff.LastActiveAt,
                ServiceRadiusKm = staff.ServiceRadiusKm,
                CertificationImageUrl = staff.CertificationImageUrl,
                IdCardImageUrl = staff.IdCardImageUrl,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt
            }).ToList();

            return new AppResponse<List<AdminStaffListResponse>>()
                .SetSuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inactive staff");
            return new AppResponse<List<AdminStaffListResponse>>()
                .SetErrorResponse("Error", "Failed to retrieve inactive staff");
        }
    }
}
