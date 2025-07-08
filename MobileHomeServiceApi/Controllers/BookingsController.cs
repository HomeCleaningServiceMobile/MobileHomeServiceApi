using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MHS.Service.Interfaces;
using MHS.Service.DTOs;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new booking
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        // Get customer ID from claims (assuming Customer role)
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Invalid user ID");
        }

        // TODO: Get customer ID from user ID
        var customerId = userId; // This should be mapped to actual customer ID

        var result = await _bookingService.CreateBookingAsync(customerId, request, cancellationToken);
        return result.IsSucceeded ? CreatedAtAction(nameof(GetBookingById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    /// <summary>
    /// Get booking by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.GetBookingByIdAsync(id, cancellationToken);
        return result.IsSucceeded ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get bookings with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] BookingListRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.GetBookingsAsync(request, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Update booking details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.UpdateBookingAsync(id, request, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Cancel a booking
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(int id, [FromBody] string reason, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.CancelBookingAsync(id, reason, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Staff responds to booking assignment
    /// </summary>
    [HttpPost("respond")]
    public async Task<IActionResult> RespondToBooking([FromBody] StaffResponseRequest request, CancellationToken cancellationToken = default)
    {
        // Get staff ID from claims (assuming Staff role)
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Invalid user ID");
        }

        // TODO: Get staff ID from user ID
        var staffId = userId; // This should be mapped to actual staff ID

        var result = await _bookingService.RespondToBookingAsync(staffId, request, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Staff check-in to start work
    /// </summary>
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request, CancellationToken cancellationToken = default)
    {
        // Get staff ID from claims
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Invalid user ID");
        }

        var staffId = userId; // This should be mapped to actual staff ID

        var result = await _bookingService.CheckInAsync(staffId, request, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Staff check-out to complete work
    /// </summary>
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request, CancellationToken cancellationToken = default)
    {
        // Get staff ID from claims
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Invalid user ID");
        }

        var staffId = userId; // This should be mapped to actual staff ID

        var result = await _bookingService.CheckOutAsync(staffId, request, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get available time slots for a service
    /// </summary>
    [HttpGet("available-slots")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableTimeSlots([FromQuery] int serviceId, [FromQuery] DateTime date, [FromQuery] decimal latitude, [FromQuery] decimal longitude, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.GetAvailableTimeSlotsAsync(serviceId, date, latitude, longitude, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Find available staff for a service
    /// </summary>
    [HttpGet("available-staff")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> FindAvailableStaff([FromQuery] int serviceId, [FromQuery] DateTime scheduledDate, [FromQuery] TimeSpan scheduledTime, [FromQuery] decimal latitude, [FromQuery] decimal longitude, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.FindAvailableStaffAsync(serviceId, scheduledDate, scheduledTime, latitude, longitude, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Auto-assign staff to a booking (Admin only)
    /// </summary>
    [HttpPost("{id}/auto-assign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AutoAssignStaff(int id, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.AutoAssignStaffAsync(id, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Manually assign staff to a booking (Admin only)
    /// </summary>
    [HttpPost("{id}/manual-assign/{staffId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ManualAssignStaff(int id, int staffId, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.ManualAssignStaffAsync(id, staffId, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Confirm booking completion (Customer)
    /// </summary>
    [HttpPost("{id}/confirm-completion")]
    public async Task<IActionResult> ConfirmBookingCompletion(int id, CancellationToken cancellationToken = default)
    {
        // Get customer ID from claims
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("Invalid user ID");
        }

        var customerId = userId; // This should be mapped to actual customer ID

        var result = await _bookingService.ConfirmBookingCompletionAsync(id, customerId, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Force complete booking (Admin only)
    /// </summary>
    [HttpPost("{id}/force-complete")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ForceCompleteBooking(int id, [FromBody] string reason, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.ForceCompleteBookingAsync(id, reason, cancellationToken);
        return result.IsSucceeded ? Ok(result) : BadRequest(result);
    }
} 