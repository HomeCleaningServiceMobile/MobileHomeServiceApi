using MHS.Service.Interfaces;
using MHS.Service.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileHomeServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly IAvailabilityService _timeSlotsService;

        public TimeSlotController(IAvailabilityService timeSlotsService)
        {
            _timeSlotsService = timeSlotsService;
        }

        [HttpGet("available-slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableSlots(
            [FromQuery] DateTime date,
            [FromQuery] int? serviceId = null,
            [FromQuery] int? staffId = null)
        {
            if (date.Date < DateTime.Now.Date)
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Cannot retrieve time slots for a past date."));
            }

            var response = await _timeSlotsService.GetAvailableTimeSlotsAsync(date, serviceId, staffId);
            
            if (response.IsSucceeded)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Gets available staff for a specific time slot on a given date.
        /// </summary>
        /// <param name="date">The date to check for staff availability (format: YYYY-MM-DD).</param>
        /// <param name="startTime">The start time of the slot (format: HH:mm).</param>
        /// <param name="endTime">The end time of the slot (format: HH:mm).</param>
        /// <param name="serviceId">Optional ID of the service to filter staff by.</param>
        /// <returns>A list of staff available for the specified time slot.</returns>
        /// <response code="200">Returns the list of available staff.</response>
        /// <response code="400">If the date is in the past or the time slot is invalid.</response>
        [HttpGet("available-staff-for-slot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableStaffForSlot(
            [FromQuery] DateTime date,
            [FromQuery] string startTime,
            [FromQuery] string endTime,
            [FromQuery] int? serviceId = null)
        {
            // Validate date
            if (date.Date < DateTime.Now.Date)
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Cannot retrieve staff availability for a past date."));
            }

            // Parse time strings (expected format: HH:mm)
            if (!TimeSpan.TryParse(startTime, out var startTimeSpan))
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Invalid start time format. Use HH:mm (e.g., 09:00)."));
            }

            if (!TimeSpan.TryParse(endTime, out var endTimeSpan))
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Invalid end time format. Use HH:mm (e.g., 09:30)."));
            }

            if (startTimeSpan >= endTimeSpan)
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Start time must be earlier than end time."));
            }

            // Get available staff for the slot
            var availableStaff = await _timeSlotsService.GetAvailableStaffForSlotAsync(date, startTimeSpan, endTimeSpan, serviceId);

            if (availableStaff.IsSucceeded)
            {
                return Ok(availableStaff);
            }
            else
            {
                return BadRequest(availableStaff);
            }
        }

        /// <summary>
        /// Gets available time slots for a date range.
        /// </summary>
        /// <param name="startDate">Start date of the range</param>
        /// <param name="endDate">End date of the range</param>
        /// <param name="serviceId">Optional service ID to filter by</param>
        /// <param name="staffId">Optional staff ID to filter by</param>
        /// <returns>Dictionary of dates and their available time slots</returns>
        [HttpGet("available-slots-range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableSlotsForDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? serviceId = null,
            [FromQuery] int? staffId = null)
        {
            if (startDate.Date < DateTime.Now.Date)
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Start date cannot be in the past."));
            }

            if (endDate < startDate)
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "End date must be after start date."));
            }

            var response = await _timeSlotsService.GetAvailableSlotsForDateRangeAsync(startDate, endDate, serviceId, staffId);
            
            if (response.IsSucceeded)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Gets the next available time slot.
        /// </summary>
        /// <param name="serviceId">Optional service ID to filter by</param>
        /// <param name="staffId">Optional staff ID to filter by</param>
        /// <returns>The next available time slot</returns>
        [HttpGet("next-available-slot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNextAvailableSlot(
            [FromQuery] int? serviceId = null,
            [FromQuery] int? staffId = null)
        {
            var response = await _timeSlotsService.GetNextAvailableSlotAsync(serviceId, staffId);
            
            if (response.IsSucceeded)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Debug endpoint to check data availability
        /// </summary>
        [HttpGet("debug")]
        public async Task<IActionResult> DebugData()
        {
            try
            {
                var today = DateTime.Now.Date;
                var dayOfWeek = (int)today.DayOfWeek;
                
                var debugInfo = new
                {
                    CurrentDate = today,
                    DayOfWeek = dayOfWeek,
                    DayName = today.DayOfWeek.ToString(),
                    CurrentTime = DateTime.Now,
                    TestDate = today.AddDays(1), // Tomorrow
                    TestDayOfWeek = (int)today.AddDays(1).DayOfWeek
                };

                return Ok(new AppResponse<object>().SetSuccessResponse(debugInfo));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppResponse<object>()
                    .SetErrorResponse("Error", $"Debug error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all available time slots for a specific service on a given date
        /// </summary>
        /// <param name="date">The date to check for availability (format: YYYY-MM-DD)</param>
        /// <param name="serviceId">ID of the service to check availability for</param>
        /// <returns>List of available time slots for the service</returns>
        [HttpGet("service-slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableSlotsForService(
            [FromQuery] DateTime date,
            [FromQuery] int serviceId)
        {
            //if (date.Date < DateTime.Now.Date)
            //{
            //    return BadRequest(new AppResponse<object>()
            //        .SetErrorResponse("Validation", "Cannot retrieve time slots for a past date."));
            //}

            var response = await _timeSlotsService.GetAvailableTimeSlotsForServiceAsync(date, serviceId);
            
            if (response.IsSucceeded)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Get all available staff for a specific service and time slot on a given date
        /// </summary>
        /// <param name="date">The date to check for staff availability (format: YYYY-MM-DD)</param>
        /// <param name="startTime">The start time of the slot (format: HH:mm)</param>
        /// <param name="endTime">The end time of the slot (format: HH:mm)</param>
        /// <param name="serviceId">ID of the service to filter staff by</param>
        /// <returns>List of available staff for the service and time slot</returns>
        [HttpGet("service-staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableStaffForService(
            [FromQuery] DateTime date,
            [FromQuery] string startTime,
            [FromQuery] string endTime,
            [FromQuery] int serviceId)
        {
            // Validate date
            if (date.Date < DateTime.Now.Date)
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Cannot retrieve staff availability for a past date."));
            }

            // Parse time strings (expected format: HH:mm)
            if (!TimeSpan.TryParse(startTime, out var startTimeSpan))
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Invalid start time format. Use HH:mm (e.g., 09:00)."));
            }

            if (!TimeSpan.TryParse(endTime, out var endTimeSpan))
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Invalid end time format. Use HH:mm (e.g., 09:30)."));
            }

            if (startTimeSpan >= endTimeSpan)
            {
                return BadRequest(new AppResponse<object>()
                    .SetErrorResponse("Validation", "Start time must be earlier than end time."));
            }

            // Get available staff for the service and slot
            var availableStaff = await _timeSlotsService.GetAvailableStaffForServiceSlotAsync(date, startTimeSpan, endTimeSpan, serviceId);

            if (availableStaff.IsSucceeded)
            {
                return Ok(availableStaff);
            }
            else
            {
                return BadRequest(availableStaff);
            }
        }
    }
}
