using MHS.Repository.Models;
using MHS.Service.Implementations;
using MHS.Service.DTOs;

namespace MHS.Service.Interfaces
{
    public interface IAvailabilityService
    {
        Task<AppResponse<Dictionary<DateTime, List<TimeSlotDto>>>> GetAvailableSlotsForDateRangeAsync(DateTime startDate, DateTime endDate, int? serviceId = null, int? staffId = null);
        Task<AppResponse<List<TimeSlotDto>>> GetAvailableTimeSlotsAsync(DateTime date, int? serviceId = null, int? staffId = null);
        Task<AppResponse<TimeSlotDto>> GetNextAvailableSlotAsync(int? serviceId = null, int? staffId = null);
        Task<AppResponse<List<StaffAvailabilityDto>>> GetStaffAvailableForSlotAsync(
            List<Staff> staff, DateTime slotStart, TimeSpan slotEndTime, int? serviceId);
        Task<AppResponse<List<StaffAvailabilityDto>>> GetAvailableStaffForSlotAsync(
            DateTime date, TimeSpan startTime, TimeSpan endTime, int? serviceId = null);
        Task<AppResponse<List<TimeSlotDto>>> GetAvailableTimeSlotsForServiceAsync(DateTime date, int serviceId);
        Task<AppResponse<List<StaffAvailabilityDto>>> GetAvailableStaffForServiceSlotAsync(
            DateTime date, TimeSpan startTime, TimeSpan endTime, int serviceId);
    }
}