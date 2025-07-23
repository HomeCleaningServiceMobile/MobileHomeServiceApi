using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MHS.Service.Interfaces;
using MHS.Service.DTOs;
using System.Security.Claims;
using MHS.Service.Implementations;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IMapboxService _mapboxService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IMapboxService mapboxService, IBookingService bookingService, ILogger<BookingController> logger)
    {
        _mapboxService = mapboxService;
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new booking
    /// </summary>
    /// <param name="request">Booking creation request</param>
    /// <returns>Created booking details</returns>
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _bookingService.CreateBookingAsync(customerId, request);

            if (result.IsSucceeded)
            {
                return CreatedAtAction(nameof(GetBooking), new { id = result.Data?.Id }, result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get booking by ID
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Booking details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        try
        {
            var result = await _bookingService.GetBookingByIdAsync(id);

            if (result.IsSucceeded)
            {
                // Check if user has permission to view this booking
                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();
                
                if (userRole == "Admin" || 
                    (userRole == "Customer" && result.Data?.Customer?.Id == currentUserId) ||
                    (userRole == "Staff" && result.Data?.Staff?.Id == currentUserId))
                {
                    return Ok(result);
                }
                
                return Forbid("You don't have permission to view this booking");
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get bookings with filtering and pagination
    /// </summary>
    /// <param name="request">Booking list request parameters</param>
    /// <returns>Paginated list of bookings</returns>
    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] BookingListRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            // Apply role-based filtering
            if (userRole == "Customer")
            {
                request.CustomerId = currentUserId;
            }
            else if (userRole == "Staff")
            {
                request.StaffId = currentUserId;
            }
            // Admin can see all bookings without filtering

            var result = await _bookingService.GetBookingsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customer's own bookings with enhanced filtering and sorting options
    /// </summary>
    /// <param name="request">Customer booking list request parameters</param>
    /// <returns>Paginated list of customer bookings</returns>
    [HttpGet("my-bookings")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyBookings([FromQuery] CustomerBookingListRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            request.CustomerId = customerId;

            var result = await _bookingService.GetCustomerBookingsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer bookings for user {UserId}", GetCurrentUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customer booking history with statistics
    /// </summary>
    /// <returns>Customer booking history with summary statistics</returns>
    [HttpGet("my-bookings/history")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyBookingHistory()
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _bookingService.GetCustomerBookingHistoryAsync(customerId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking history for customer {CustomerId}", GetCurrentUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get upcoming bookings for customer
    /// </summary>
    /// <param name="days">Number of days to look ahead (default: 30)</param>
    /// <returns>List of upcoming bookings</returns>
    [HttpGet("my-bookings/upcoming")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyUpcomingBookings([FromQuery] int days = 30)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _bookingService.GetCustomerUpcomingBookingsAsync(customerId, days);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming bookings for customer {CustomerId}", GetCurrentUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update booking details
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Update booking request</param>
    /// <returns>Updated booking details</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingRequest request)
    {
        try
        {
            // Check if customer is updating their own booking
            var userRole = GetCurrentUserRole();
            if (userRole == "Customer")
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking.IsSucceeded && booking.Data?.Customer?.Id != GetCurrentUserId())
                {
                    return Forbid("You can only update your own bookings");
                }
            }

            var result = await _bookingService.UpdateBookingAsync(id, request);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Cancel a booking
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Cancellation request</param>
    /// <returns>Cancellation result</returns>
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> CancelBooking(int id, [FromBody] CancelBookingRequest request)
    {
        try
        {
            // Check if customer is cancelling their own booking
            var userRole = GetCurrentUserRole();
            if (userRole == "Customer")
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking.IsSucceeded && booking.Data?.Customer?.Id != GetCurrentUserId())
                {
                    return Forbid("You can only cancel your own bookings");
                }
            }

            var result = await _bookingService.CancelBookingAsync(id, request.Reason);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Staff response to booking assignment
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Staff response request</param>
    /// <returns>Response result</returns>
    [HttpPost("{id}/respond")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> RespondToBooking(int id, [FromBody] StaffResponseRequest request)
    {
        try
        {
            var staffId = GetCurrentUserId();
            request.BookingId = id;
            
            var result = await _bookingService.RespondToBookingAsync(staffId, request);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Staff check-in to start service
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Check-in request</param>
    /// <returns>Check-in result</returns>
    [HttpPost("{id}/checkin")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> CheckIn(int id, [FromBody] CheckInRequest request)
    {
        try
        {
            var staffId = GetCurrentUserId();
            request.BookingId = id;
            
            var result = await _bookingService.CheckInAsync(staffId, request);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking in to booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Staff check-out to complete service
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Check-out request</param>
    /// <returns>Check-out result</returns>
    [HttpPost("{id}/checkout")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> CheckOut(int id, [FromBody] CheckOutRequest request)
    {
        try
        {
            var staffId = GetCurrentUserId();
            request.BookingId = id;
            
            var result = await _bookingService.CheckOutAsync(staffId, request);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out from booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Auto-assign staff to booking
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Assignment result</returns>
    [HttpPost("{id}/auto-assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AutoAssignStaff(int id)
    {
        try
        {
            var result = await _bookingService.AutoAssignStaffAsync(id);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error auto-assigning staff to booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Manually assign staff to booking
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Manual assignment request</param>
    /// <returns>Assignment result</returns>
    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ManualAssignStaff(int id, [FromBody] ManualAssignStaffRequest request)
    {
        try
        {
            var result = await _bookingService.ManualAssignStaffAsync(id, request.StaffId);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error manually assigning staff to booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Force complete booking (Admin only)
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Force completion request</param>
    /// <returns>Completion result</returns>
    [HttpPost("{id}/force-complete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ForceCompleteBooking(int id, [FromBody] ForceCompleteBookingRequest request)
    {
        try
        {
            var result = await _bookingService.ForceCompleteBookingAsync(id, request.Reason);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error force completing booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Customer confirm booking completion
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Confirmation result</returns>
    [HttpPost("{id}/confirm")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ConfirmBookingCompletion(int id)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _bookingService.ConfirmBookingCompletionAsync(id, customerId);

            if (result.IsSucceeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming booking completion {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get available time slots for a service
    /// </summary>
    /// <param name="serviceId">Service ID</param>
    /// <param name="date">Date to check availability</param>
    /// <param name="latitude">Service location latitude</param>
    /// <param name="longitude">Service location longitude</param>
    /// <returns>Available time slots</returns>
    [HttpGet("available-slots")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetAvailableTimeSlots(
        [FromQuery] int serviceId,
        [FromQuery] DateTime date,
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude)
    {
        try
        {
            var result = await _bookingService.GetAvailableTimeSlotsAsync(serviceId, date, latitude, longitude);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available time slots for service {ServiceId}", serviceId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Find available staff for a service
    /// </summary>
    /// <param name="serviceId">Service ID</param>
    /// <param name="scheduledDate">Scheduled date</param>
    /// <param name="scheduledTime">Scheduled time</param>
    /// <param name="latitude">Service location latitude</param>
    /// <param name="longitude">Service location longitude</param>
    /// <returns>Available staff list</returns>
    [HttpGet("available-staff")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> FindAvailableStaff(
        [FromQuery] int serviceId,
        [FromQuery] DateTime scheduledDate,
        [FromQuery] TimeSpan scheduledTime,
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude)
    {
        try
        {
            var result = await _bookingService.FindAvailableStaffAsync(serviceId, scheduledDate, scheduledTime, latitude, longitude);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding available staff for service {ServiceId}", serviceId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get all bookings (Admin only)
    /// </summary>
    /// <param name="request">Booking list request parameters</param>
    /// <returns>All bookings with pagination</returns>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllBookings([FromQuery] BookingListRequest request)
    {
        try
        {
            var result = await _bookingService.GetAllBookingsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get staff bookings (Staff role only)
    /// </summary>
    /// <param name="request">Booking list request parameters</param>
    /// <returns>Staff bookings with pagination</returns>
    [HttpGet("staff")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetStaffBookings([FromQuery] BookingListRequest request)
    {
        try
        {
            // Get user ID from claims (Staff role)
            var userId = GetCurrentUserId();

            var result = await _bookingService.GetStaffBookingsAsync(userId, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff bookings for current user");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get staff bookings
    /// </summary>
    /// <param name="staffId">Staff ID</param>
    /// <param name="request">Booking list request parameters</param>
    /// <returns>Staff bookings with pagination</returns>
    [HttpGet("staff/{staffId}")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<IActionResult> GetStaffBookings(int staffId, [FromQuery] BookingListRequest request)
    {
        try
        {
            var userRole = GetCurrentUserRole();
            var currentUserId = GetCurrentUserId();

            // Staff can only see their own bookings
            if (userRole == "Staff" && currentUserId != staffId)
            {
                return Forbid("You can only view your own bookings");
            }

            var result = await _bookingService.GetStaffBookingsAsync(staffId, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff bookings for staff {StaffId}", staffId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}/directions")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetDirections(int id)
    {
        try
        {
            var staffId = GetCurrentUserId();

            
            var bookingResult = await _bookingService.GetBookingByIdAsync(id);
            if (!bookingResult.IsSucceeded || bookingResult.Data == null)
                return NotFound("Booking not found");

           
            if (bookingResult.Data.Staff == null || bookingResult.Data.Staff.Id != staffId)
                return Forbid("You are not assigned to this booking");

            // L?y v? tr� staff (gi? s? c� trong bookingResult.Data.Staff)
            var staffLat = bookingResult.Data.Staff.CurrentLatitude;
            var staffLng = bookingResult.Data.Staff.CurrentLongitude;
            var customerLat = bookingResult.Data.AddressLatitude;
            var customerLng = bookingResult.Data.AddressLongitude;

            if (staffLat == null || staffLng == null)
                return BadRequest("Staff location not available");

            // G?i MapboxService
            var directions = await _mapboxService.GetDirectionsAsync(
                (double)staffLat, (double)staffLng,
                (double)customerLat, (double)customerLng
            );

            if (directions == null)
                return StatusCode(502, "Failed to get directions from Mapbox");

            return Ok(directions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting directions for booking {BookingId}", id);
            return StatusCode(500, "Internal server error");
        }
    }


    #region Helper Methods

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("User ID not found in token");
    }

    private string GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value ?? throw new UnauthorizedAccessException("User role not found in token");
    }

    #endregion
}

// Additional DTOs for controller-specific requests
public class CancelBookingRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class ManualAssignStaffRequest
{
    public int StaffId { get; set; }
}

public class ForceCompleteBookingRequest
{
    public string Reason { get; set; } = string.Empty;
} 