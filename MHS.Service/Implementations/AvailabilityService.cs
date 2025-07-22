using MHS.Repository.Interfaces;
using MHS.Repository.Models;
using MHS.Service.Interfaces;
using MHS.Service.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MHS.Service.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AvailabilityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AppResponse<List<TimeSlotDto>>> GetAvailableTimeSlotsAsync(DateTime date, int? serviceId = null, int? staffId = null)
        {
            try
            {
                var availableSlots = new List<TimeSlotDto>();
                var dayOfWeek = (int)date.DayOfWeek;

                // Step 1: Check if business is open on this day
                var businessHours = await _unitOfWork.Repository<BusinessHours>()
                    .FindAsync(bh => bh.DayOfWeek == dayOfWeek && bh.IsActive);

                if (businessHours == null)
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetErrorResponse("BusinessHours", $"No business hours found for day {dayOfWeek} ({date.DayOfWeek})");
                }

                if (businessHours.IsClosed)
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetSuccessResponse(availableSlots, "Info", "Business is closed on this day");
                }

                // Step 2: Get service duration (if specific service selected)
                var serviceDuration = await GetServiceDurationAsync(serviceId);

                // Step 3: Get available staff for this day
                var availableStaff = await GetAvailableStaffAsync(date, staffId);

                if (!availableStaff.Any())
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetErrorResponse("Staff", $"No staff available on day {dayOfWeek}. Found {availableStaff.Count} staff members.");
                }

                // Step 4: Generate time slots
                var timeSlots = GenerateTimeSlots(businessHours, serviceDuration);

                if (!timeSlots.Any())
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetErrorResponse("TimeSlots", $"No time slots generated. Business hours: {businessHours.OpenTime} - {businessHours.CloseTime}, Duration: {serviceDuration} minutes");
                }

                // Step 5: Check each slot against staff availability and existing bookings
                foreach (var slot in timeSlots)
                {
                    var slotDateTime = date.Date.Add(slot.StartTime);

                    // Skip past time slots

                    var staffAvailableForSlot = await GetStaffAvailableForSlotAsync(
                        availableStaff, slotDateTime, slot.EndTime, serviceId);

                    if (staffAvailableForSlot.IsSucceeded && staffAvailableForSlot.Data?.Any() == true)
                    {
                        availableSlots.Add(new TimeSlotDto
                        {
                            StartTime = slot.StartTime,
                            EndTime = slot.EndTime,
                            DisplayTime = slot.StartTime.ToString(@"hh\:mm"),
                            AvailableStaff = staffAvailableForSlot.Data,
                            IsAvailable = true
                        });
                    }
                }

                var result = availableSlots.OrderBy(s => s.StartTime).ToList();
                
                if (!result.Any())
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetErrorResponse("NoSlots", $"No available slots found. Generated {timeSlots.Count} slots, but none are available. Date: {date}, Current time: {DateTime.Now}");
                }

                return new AppResponse<List<TimeSlotDto>>().SetSuccessResponse(result);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<TimeSlotDto>>()
                    .SetErrorResponse("Error", $"Failed to get available time slots: {ex.Message}");
            }
        }

        private async Task<int> GetServiceDurationAsync(int? serviceId)
        {
            if (serviceId.HasValue)
            {
                var service = await _unitOfWork.Repository<MHS.Repository.Models.Service>().GetEntityByIdAsync(serviceId.Value);
                return service?.EstimatedDurationMinutes ?? 60; // Default 60 minutes
            }
            return 60; // Default duration
        }

        private async Task<List<Staff>> GetAvailableStaffAsync(DateTime date, int? specificStaffId)
        {
            var dayOfWeek = (int)date.DayOfWeek;

            // First, get all staff to debug
            var allStaff = await _unitOfWork.Repository<Staff>().ListAsync(
                includeProperties: q => q.Include(s => s.WorkSchedules).Include(s => s.User));

            var availableStaff = new List<Staff>();

            foreach (var staff in allStaff)
            {
                // Check if this staff member should be included based on specificStaffId filter
                if (specificStaffId.HasValue && staff.Id != specificStaffId.Value)
                {
                    continue;
                }

                // Check if staff has work schedule for this day
                var workSchedule = staff.WorkSchedules.FirstOrDefault(ws => ws.DayOfWeek == dayOfWeek && ws.IsActive);
                
                if (workSchedule != null)
                {
                    availableStaff.Add(staff);
                }
            }

            return availableStaff;
        }

        private List<(TimeSpan StartTime, TimeSpan EndTime)> GenerateTimeSlots(
            BusinessHours businessHours, int serviceDurationMinutes)
        {
            var slots = new List<(TimeSpan StartTime, TimeSpan EndTime)>();
            var slotDuration = TimeSpan.FromMinutes(serviceDurationMinutes);
            var current = businessHours.OpenTime;

            // Ensure we have valid business hours
            if (businessHours.CloseTime <= businessHours.OpenTime)
            {
                return slots; // Invalid business hours
            }

            while (current.Add(slotDuration) <= businessHours.CloseTime)
            {
                slots.Add((current, current.Add(slotDuration)));
                current = current.Add(TimeSpan.FromMinutes(30)); // 30-minute intervals
            }

            return slots;
        }

        public async Task<AppResponse<List<StaffAvailabilityDto>>> GetStaffAvailableForSlotAsync(
            List<Staff> staff, DateTime slotStart, TimeSpan slotEndTime, int? serviceId)
        {
            try
            {
                var availableStaff = new List<StaffAvailabilityDto>();
                var slotEnd = slotStart.Date.Add(slotEndTime);

                foreach (var staffMember in staff)
                {
                    var workSchedule = staffMember.WorkSchedules
                        .FirstOrDefault(ws => ws.DayOfWeek == (int)slotStart.DayOfWeek && ws.IsActive);
                    if (workSchedule == null ||
                        slotStart.TimeOfDay < workSchedule.StartTime ||
                        slotEnd.TimeOfDay > workSchedule.EndTime)
                    {
                        continue; // Staff not working during this slot
                    }

                    // Get all bookings for the staff member on the date and evaluate on client side
                    var bookingsOnDate = await _unitOfWork.Repository<Booking>()
                        .ListAsync(b => b.StaffId == staffMember.Id && b.ScheduledDate == slotStart.Date);

                    var hasConflictingBooking = bookingsOnDate.Any(b =>
                        (b.ScheduledTime <= slotStart.TimeOfDay &&
                         b.ScheduledTime.Add(TimeSpan.FromMinutes(60)) > slotStart.TimeOfDay) ||
                        (b.ScheduledTime < slotEnd.TimeOfDay &&
                         b.ScheduledTime >= slotStart.TimeOfDay));

                    if (!hasConflictingBooking)
                    {
                        var canPerformService = await CanStaffPerformServiceAsync(staffMember.Id, serviceId);
                        if (canPerformService)
                        {
                            availableStaff.Add(new StaffAvailabilityDto
                            {
                                StaffId = staffMember.Id,
                                StaffName = staffMember.User.FirstName,
                            });
                        }
                    }
                }
                return new AppResponse<List<StaffAvailabilityDto>>().SetSuccessResponse(availableStaff);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<StaffAvailabilityDto>>()
                    .SetErrorResponse("Error", $"Failed to get staff availability: {ex.Message}");
            }
        }
        private async Task<bool> CanStaffPerformServiceAsync(int staffId, int? serviceId)
        {
            if (!serviceId.HasValue)
                return true; // No specific service requirement

            // Implement skill-based matching if needed
            return true;
        }

        public async Task<AppResponse<List<StaffAvailabilityDto>>> GetAvailableStaffForSlotAsync(
            DateTime date, TimeSpan startTime, TimeSpan endTime, int? serviceId = null)
        {
            try
            {
                var dayOfWeek = (int)date.DayOfWeek;
                var slotDateTime = date.Date.Add(startTime);

                // Get available staff for this day
                var availableStaff = await GetAvailableStaffAsync(date, null);

                if (!availableStaff.Any())
                {
                    return new AppResponse<List<StaffAvailabilityDto>>()
                        .SetSuccessResponse(new List<StaffAvailabilityDto>(), "Info", "No staff available on this day");
                }

                // Get staff available for the specific slot
                var staffResponse = await GetStaffAvailableForSlotAsync(availableStaff, slotDateTime, endTime, serviceId);
                return staffResponse;
            }
            catch (Exception ex)
            {
                return new AppResponse<List<StaffAvailabilityDto>>()
                    .SetErrorResponse("Error", $"Failed to get available staff for slot: {ex.Message}");
            }
        }

        public async Task<AppResponse<List<TimeSlotDto>>> GetAvailableTimeSlotsForServiceAsync(DateTime date, int serviceId)
        {
            try
            {
                var availableSlots = new List<TimeSlotDto>();
                var dayOfWeek = (int)date.DayOfWeek;

                // Step 1: Check if business is open on this day
                var businessHours = await _unitOfWork.Repository<BusinessHours>()
                    .FindAsync(bh => bh.DayOfWeek == dayOfWeek && bh.IsActive);

                if (businessHours == null)
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetErrorResponse("BusinessHours", $"No business hours found for day {dayOfWeek} ({date.DayOfWeek})");
                }

                if (businessHours.IsClosed)
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetSuccessResponse(availableSlots, "Info", "Business is closed on this day");
                }

                // Step 2: Get service duration
                var service = await _unitOfWork.Repository<MHS.Repository.Models.Service>().GetEntityByIdAsync(serviceId);
                if (service == null)
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetErrorResponse("Service", $"Service with ID {serviceId} not found");
                }

                var serviceDuration = service.EstimatedDurationMinutes;

                // Step 3: Get all staff that can perform this service
                var allStaff = await _unitOfWork.Repository<Staff>().ListAsync(
                    includeProperties: q => q.Include(s => s.WorkSchedules).Include(s => s.StaffSkills).Include(s => s.User));

                //var availableStaff = allStaff.Where(s => 
                //    s.WorkSchedules.Any(ws => ws.DayOfWeek == dayOfWeek && ws.IsActive) &&
                //    s.StaffSkills.Any(ss => ss.ServiceId == serviceId && ss.IsActive)
                //).ToList();

                //if (!availableStaff.Any())
                //{
                //    return new AppResponse<List<TimeSlotDto>>()
                //        .SetErrorResponse("Staff", $"No staff available for service {serviceId} on day {dayOfWeek}");
                //}

                // Step 4: Generate time slots
                var timeSlots = GenerateTimeSlots(businessHours, serviceDuration);

                if (!timeSlots.Any())
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetErrorResponse("TimeSlots", $"No time slots generated. Business hours: {businessHours.OpenTime} - {businessHours.CloseTime}, Duration: {serviceDuration} minutes");
                }

                // Step 5: Check each slot against staff availability and existing bookings
                foreach (var slot in timeSlots)
                {
                    var slotDateTime = date.Date.Add(slot.StartTime);

                    // Skip past time slots
                    if (slotDateTime <= DateTime.Now)
                        continue;

                    //var staffAvailableForSlot = await GetStaffAvailableForSlotAsync(
                    //    availableStaff, slotDateTime, slot.EndTime, serviceId);

                    var staffAvailableForSlot = await GetAvailableStaffForSlotAsync(
                        date, slot.StartTime, slot.EndTime, serviceId);

                    if (staffAvailableForSlot.IsSucceeded && staffAvailableForSlot.Data?.Any() == true)
                    {
                        availableSlots.Add(new TimeSlotDto
                        {
                            StartTime = slot.StartTime,
                            EndTime = slot.EndTime,
                            DisplayTime = slot.StartTime.ToString(@"hh\:mm"),
                            AvailableStaff = staffAvailableForSlot.Data,
                            IsAvailable = true
                        });
                    }
                }

                var result = availableSlots.OrderBy(s => s.StartTime).ToList();
                
                if (!result.Any())
                {
                    return new AppResponse<List<TimeSlotDto>>()
                        .SetErrorResponse("NoSlots", $"No available slots found for service {serviceId}. Generated {timeSlots.Count} slots, but none are available. Date: {date}, Current time: {DateTime.Now}");
                }

                return new AppResponse<List<TimeSlotDto>>().SetSuccessResponse(result);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<TimeSlotDto>>()
                    .SetErrorResponse("Error", $"Failed to get available time slots for service: {ex.Message}");
            }
        }

        public async Task<AppResponse<List<StaffAvailabilityDto>>> GetAvailableStaffForServiceSlotAsync(
            DateTime date, TimeSpan startTime, TimeSpan endTime, int serviceId)
        {
            try
            {
                var dayOfWeek = (int)date.DayOfWeek;
                var slotDateTime = date.Date.Add(startTime);

                // Step 1: Check if business is open on this day
                var businessHours = await _unitOfWork.Repository<BusinessHours>()
                    .FindAsync(bh => bh.DayOfWeek == dayOfWeek && bh.IsActive);

                if (businessHours == null)
                {
                    return new AppResponse<List<StaffAvailabilityDto>>()
                        .SetErrorResponse("BusinessHours", $"No business hours found for day {dayOfWeek}");
                }

                if (businessHours.IsClosed)
                {
                    return new AppResponse<List<StaffAvailabilityDto>>()
                        .SetSuccessResponse(new List<StaffAvailabilityDto>(), "Info", "Business is closed on this day");
                }

                // Step 2: Get all staff that can perform this service
                var allStaff = await _unitOfWork.Repository<Staff>().ListAsync(
                    includeProperties: q => q.Include(s => s.WorkSchedules).Include(s => s.StaffSkills).Include(s => s.User));

                var availableStaff = allStaff.Where(s => 
                    s.WorkSchedules.Any(ws => ws.DayOfWeek == dayOfWeek && ws.IsActive) &&
                    s.StaffSkills.Any(ss => ss.ServiceId == serviceId && ss.IsActive)
                ).ToList();

                if (!availableStaff.Any())
                {
                    return new AppResponse<List<StaffAvailabilityDto>>()
                        .SetErrorResponse("Staff", $"No staff available for service {serviceId} on day {dayOfWeek}");
                }

                // Step 3: Get staff available for the specific slot
                var staffResponse = await GetStaffAvailableForSlotAsync(availableStaff, slotDateTime, endTime, serviceId);
                return staffResponse;
            }
            catch (Exception ex)
            {
                return new AppResponse<List<StaffAvailabilityDto>>()
                    .SetErrorResponse("Error", $"Failed to get available staff for service slot: {ex.Message}");
            }
        }

        public async Task<AppResponse<Dictionary<DateTime, List<TimeSlotDto>>>> GetAvailableSlotsForDateRangeAsync(
            DateTime startDate, DateTime endDate, int? serviceId = null, int? staffId = null)
        {
            try
            {
                var result = new Dictionary<DateTime, List<TimeSlotDto>>();

                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var slotsResponse = await GetAvailableTimeSlotsAsync(date, serviceId, staffId);
                    if (slotsResponse.IsSucceeded && slotsResponse.Data?.Any() == true)
                    {
                        result[date] = slotsResponse.Data;
                    }
                }

                return new AppResponse<Dictionary<DateTime, List<TimeSlotDto>>>().SetSuccessResponse(result);
            }
            catch (Exception ex)
            {
                return new AppResponse<Dictionary<DateTime, List<TimeSlotDto>>>()
                    .SetErrorResponse("Error", $"Failed to get available slots for date range: {ex.Message}");
            }
        }

        public async Task<AppResponse<TimeSlotDto>> GetNextAvailableSlotAsync(int? serviceId = null, int? staffId = null)
        {
            try
            {
                var startDate = DateTime.Now.Date;
                var endDate = startDate.AddDays(30);

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var slotsResponse = await GetAvailableTimeSlotsAsync(date, serviceId, staffId);
                    if (slotsResponse.IsSucceeded && slotsResponse.Data?.Any() == true)
                    {
                        var nextSlot = slotsResponse.Data.FirstOrDefault();
                        if (nextSlot != null)
                        {
                            nextSlot.Date = date;
                            return new AppResponse<TimeSlotDto>().SetSuccessResponse(nextSlot);
                        }
                    }
                }

                return new AppResponse<TimeSlotDto>()
                    .SetErrorResponse("Info", "No available slots found in the next 30 days");
            }
            catch (Exception ex)
            {
                return new AppResponse<TimeSlotDto>()
                    .SetErrorResponse("Error", $"Failed to get next available slot: {ex.Message}");
            }
        }
    }

    public class TimeSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DisplayTime { get; set; }
        public DateTime Date { get; set; }
        public bool IsAvailable { get; set; }
        public List<StaffAvailabilityDto> AvailableStaff { get; set; } = new();
    }

    public class StaffAvailabilityDto
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Position { get; set; }
    }

}
