using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MHS.Service.Interfaces;
using MHS.Service.DTOs;
using MHS.Repository.Interfaces;
using MHS.Repository.Models;
using MHS.Common.Enums;
using MHS.Common.DTOs;
using System.Linq.Expressions;
using ServiceEntity = MHS.Repository.Models.Service;

namespace MHS.Service.Implementations;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AppResponse<BookingResponse>> CreateBookingAsync(int customerId, CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating booking for customer {CustomerId}", customerId);

            // Validate customer exists
            var customer = await _unitOfWork.Repository<Customer>().GetEntityByIdAsync(customerId);
            if (customer == null)
            {
                return new AppResponse<BookingResponse>()
                    .SetErrorResponse("Customer", "Customer not found");
            }

            // Validate service exists
            var service = await _unitOfWork.Repository<ServiceEntity>().GetEntityByIdAsync(request.ServiceId);
            if (service == null || !service.IsActive)
            {
                return new AppResponse<BookingResponse>()
                    .SetErrorResponse("Service", "Service not found or inactive");
            }

            // Validate service package if provided
            ServicePackage? servicePackage = null;
            if (request.ServicePackageId.HasValue)
            {
                servicePackage = await _unitOfWork.Repository<ServicePackage>().GetEntityByIdAsync(request.ServicePackageId.Value);
                if (servicePackage == null || servicePackage.ServiceId != request.ServiceId)
                {
                    return new AppResponse<BookingResponse>()
                        .SetErrorResponse("ServicePackage", "Invalid service package");
                }
            }

            // Calculate total amount
            var totalAmount = CalculateBookingAmount(service, servicePackage);

            // Generate booking number
            var bookingNumber = await GenerateBookingNumberAsync();

            // Create booking entity
            var booking = new Booking
            {
                CustomerId = customerId,
                ServiceId = request.ServiceId,
                ServicePackageId = request.ServicePackageId,
                BookingNumber = bookingNumber,
                Status = BookingStatus.Pending,
                ScheduledDate = request.ScheduledDate,
                ScheduledTime = request.ScheduledTime,
                EstimatedDurationMinutes = servicePackage?.DurationMinutes ?? service.EstimatedDurationMinutes,
                TotalAmount = totalAmount,
                ServiceAddress = request.ServiceAddress,
                AddressLatitude = request.AddressLatitude,
                AddressLongitude = request.AddressLongitude,
                SpecialInstructions = request.SpecialInstructions,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Booking>().AddAsync(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            // Auto-assign staff
            await AutoAssignStaffAsync(booking.Id, cancellationToken);

            // Reload booking with related entities
            var createdBooking = await GetBookingWithRelatedEntitiesAsync(booking.Id);
            var response = _mapper.Map<BookingResponse>(createdBooking);

            _logger.LogInformation("Booking created successfully with ID {BookingId}", booking.Id);

            return new AppResponse<BookingResponse>()
                .SetSuccessResponse(response, "Success", "Booking created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for customer {CustomerId}", customerId);
            return new AppResponse<BookingResponse>()
                .SetErrorResponse("Error", "An error occurred while creating the booking");
        }
    }

    public async Task<AppResponse<BookingResponse>> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await GetBookingWithRelatedEntitiesAsync(id);
            if (booking == null)
            {
                return new AppResponse<BookingResponse>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            var response = _mapper.Map<BookingResponse>(booking);
            return new AppResponse<BookingResponse>()
                .SetSuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking {BookingId}", id);
            return new AppResponse<BookingResponse>()
                .SetErrorResponse("Error", "An error occurred while retrieving the booking");
        }
    }

    public async Task<AppResponse<List<BookingSummaryResponse>>> GetBookingsAsync(BookingListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            Expression<Func<Booking, bool>> filter = booking => true;

            if (request.Status.HasValue)
                filter = filter.And(b => b.Status == request.Status.Value);

            if (request.StartDate.HasValue)
                filter = filter.And(b => b.ScheduledDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                filter = filter.And(b => b.ScheduledDate <= request.EndDate.Value);

            if (request.CustomerId.HasValue)
                filter = filter.And(b => b.CustomerId == request.CustomerId.Value);

            if (request.StaffId.HasValue)
                filter = filter.And(b => b.StaffId == request.StaffId.Value);

            var pagination = new PaginationRequest
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var bookings = await _unitOfWork.Repository<Booking>().ListAsyncWithPaginated(
                filter: filter,
                orderBy: q => q.OrderByDescending(b => b.CreatedAt),
                includeProperties: q => q.Include(b => b.Customer).ThenInclude(c => c.User)
                                        .Include(b => b.Service)
                                        .Include(b => b.Staff).ThenInclude(s => s.User)
                                        .Include(b => b.ServicePackage),
                pagination: pagination,
                cancellationToken: cancellationToken
            );

            var items = _mapper.Map<List<BookingSummaryResponse>>(bookings.Items);

            return new AppResponse<List<BookingSummaryResponse>>()
                .SetSuccessResponse(items)
                .SetPagination(bookings.PageNumber, bookings.PageSize, bookings.TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            return new AppResponse<List<BookingSummaryResponse>>()
                .SetErrorResponse("Error", "An error occurred while retrieving bookings");
        }
    }

    public async Task<AppResponse<BookingResponse>> UpdateBookingAsync(int id, UpdateBookingRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(id);
            if (booking == null)
            {
                return new AppResponse<BookingResponse>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            // Only allow updates for pending bookings
            if (booking.Status != BookingStatus.Pending)
            {
                return new AppResponse<BookingResponse>()
                    .SetErrorResponse("Status", "Only pending bookings can be updated");
            }

            // Update booking details
            if (request.ScheduledDate.HasValue)
                booking.ScheduledDate = request.ScheduledDate.Value;

            if (request.ScheduledTime.HasValue)
                booking.ScheduledTime = request.ScheduledTime.Value;

            if (!string.IsNullOrEmpty(request.ServiceAddress))
                booking.ServiceAddress = request.ServiceAddress;

            if (request.AddressLatitude.HasValue)
                booking.AddressLatitude = request.AddressLatitude.Value;

            if (request.AddressLongitude.HasValue)
                booking.AddressLongitude = request.AddressLongitude.Value;

            if (!string.IsNullOrEmpty(request.SpecialInstructions))
                booking.SpecialInstructions = request.SpecialInstructions;

            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var updatedBooking = await GetBookingWithRelatedEntitiesAsync(id);
            var response = _mapper.Map<BookingResponse>(updatedBooking);

            return new AppResponse<BookingResponse>()
                .SetSuccessResponse(response, "Success", "Booking updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking {BookingId}", id);
            return new AppResponse<BookingResponse>()
                .SetErrorResponse("Error", "An error occurred while updating the booking");
        }
    }

    public async Task<AppResponse<string>> CancelBookingAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(id);
            if (booking == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            // Only allow cancellation for specific statuses
            if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Status", "Cannot cancel this booking");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancellationReason = reason;
            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Booking cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while cancelling the booking");
        }
    }

    public async Task<AppResponse<string>> RespondToBookingAsync(int staffId, StaffResponseRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(request.BookingId);
            if (booking == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            if (booking.StaffId != staffId)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Authorization", "You are not assigned to this booking");
            }

            if (booking.Status != BookingStatus.AutoAssigned)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Status", "Invalid booking status for response");
            }

            if (request.Accept)
            {
                booking.Status = BookingStatus.Confirmed;
                booking.StaffAcceptedAt = DateTime.UtcNow;
            }
            else
            {
                booking.Status = BookingStatus.Pending;
                booking.StaffId = null;
                booking.StaffDeclinedAt = DateTime.UtcNow;
                booking.StaffDeclineReason = request.DeclineReason;
            }

            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var message = request.Accept ? "Booking accepted successfully" : "Booking declined successfully";
            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to booking {BookingId}", request.BookingId);
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while responding to the booking");
        }
    }

    public async Task<AppResponse<string>> CheckInAsync(int staffId, CheckInRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(request.BookingId);
            if (booking == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            if (booking.StaffId != staffId)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Authorization", "You are not assigned to this booking");
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Status", "Invalid booking status for check-in");
            }

            booking.Status = BookingStatus.InProgress;
            booking.StartedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.Notes))
                booking.Notes = request.Notes;

            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Check-in successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking in to booking {BookingId}", request.BookingId);
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while checking in");
        }
    }

    public async Task<AppResponse<string>> CheckOutAsync(int staffId, CheckOutRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(request.BookingId);
            if (booking == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            if (booking.StaffId != staffId)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Authorization", "You are not assigned to this booking");
            }

            if (booking.Status != BookingStatus.InProgress)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Status", "Invalid booking status for check-out");
            }

            booking.Status = BookingStatus.Completed;
            booking.CompletedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.CompletionNotes))
                booking.Notes = string.IsNullOrEmpty(booking.Notes) ? request.CompletionNotes : $"{booking.Notes}\n\nCompletion Notes: {request.CompletionNotes}";

            _unitOfWork.Repository<Booking>().Update(booking);

            // Add completion images if provided
            if (request.CompletionImageUrls != null && request.CompletionImageUrls.Any())
            {
                var bookingImages = request.CompletionImageUrls.Select(url => new BookingImage
                {
                    BookingId = booking.Id,
                    ImageUrl = url,
                    ImageType = "completion",
                    TakenAt = DateTime.UtcNow,
                    TakenBy = "Staff",
                    CreatedAt = DateTime.UtcNow
                });

                await _unitOfWork.Repository<BookingImage>().AddRangeAsync(bookingImages);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Check-out successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out from booking {BookingId}", request.BookingId);
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while checking out");
        }
    }

    public async Task<AppResponse<string>> AutoAssignStaffAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().FindAsync(b => b.Id == bookingId);
            if (booking == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            // Find available staff
            var availableStaff = await FindAvailableStaffAsync(
                booking.ServiceId, 
                booking.ScheduledDate, 
                booking.ScheduledTime, 
                booking.AddressLatitude, 
                booking.AddressLongitude, 
                cancellationToken);

            if (!availableStaff.IsSucceeded || availableStaff.Data == null || !availableStaff.Data.Any())
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Assignment", "No available staff found for auto-assignment");
            }

            // Assign to the first available staff (you can implement more sophisticated logic here)
            var selectedStaff = availableStaff.Data.First();
            booking.StaffId = selectedStaff.Id;
            booking.Status = BookingStatus.AutoAssigned;
            booking.StaffResponseDeadline = DateTime.UtcNow.AddHours(2); // 2 hours to respond
            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Staff auto-assigned successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error auto-assigning staff to booking {BookingId}", bookingId);
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while auto-assigning staff");
        }
    }

    public async Task<AppResponse<List<StaffResponse>>> FindAvailableStaffAsync(int serviceId, DateTime scheduledDate, TimeSpan scheduledTime, decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find staff with the required skill for the service
            var staffWithSkills = await _unitOfWork.Repository<StaffSkill>().ListAsync(
                filter: ss => ss.ServiceId == serviceId && ss.IsActive,
                includeProperties: q => q.Include(ss => ss.Staff).ThenInclude(s => s.User)
            );

            var availableStaff = new List<Staff>();

            foreach (var staffSkill in staffWithSkills)
            {
                var staff = staffSkill.Staff;
                
                // Check if staff is available (not banned, active, etc.)
                if (staff.User.Status != UserStatus.Active || !staff.IsAvailable)
                    continue;

                // Check if staff is within service radius (simplified distance check)
                if (staff.CurrentLatitude.HasValue && staff.CurrentLongitude.HasValue)
                {
                    var distance = CalculateDistance(
                        (double)latitude, (double)longitude,
                        (double)staff.CurrentLatitude.Value, (double)staff.CurrentLongitude.Value);
                    
                    if (distance > staff.ServiceRadiusKm)
                        continue;
                }

                // Check if staff has no conflicting bookings
                var hasConflict = await _unitOfWork.Repository<Booking>().ExistsAsync(b =>
                    b.StaffId == staff.Id &&
                    b.ScheduledDate == scheduledDate &&
                    b.Status != BookingStatus.Cancelled &&
                    b.Status != BookingStatus.Completed);

                if (!hasConflict)
                {
                    availableStaff.Add(staff);
                }
            }

            var response = _mapper.Map<List<StaffResponse>>(availableStaff);
            return new AppResponse<List<StaffResponse>>()
                .SetSuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding available staff for service {ServiceId}", serviceId);
            return new AppResponse<List<StaffResponse>>()
                .SetErrorResponse("Error", "An error occurred while finding available staff");
        }
    }

    public async Task<AppResponse<string>> ManualAssignStaffAsync(int bookingId, int staffId, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(bookingId);
            if (booking == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            var staff = await _unitOfWork.Repository<Staff>().GetEntityByIdAsync(staffId);
            if (staff == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Staff", "Staff not found");
            }

            // Check if staff has the required skill
            var hasSkill = await _unitOfWork.Repository<StaffSkill>().ExistsAsync(ss =>
                ss.StaffId == staffId && ss.ServiceId == booking.ServiceId && ss.IsActive);

            if (!hasSkill)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Skill", "Staff does not have the required skill for this service");
            }

            booking.StaffId = staffId;
            booking.Status = BookingStatus.Confirmed;
            booking.StaffAcceptedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Staff assigned successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error manually assigning staff {StaffId} to booking {BookingId}", staffId, bookingId);
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while assigning staff");
        }
    }

    public async Task<AppResponse<List<BookingSummaryResponse>>> GetAllBookingsAsync(BookingListRequest request, CancellationToken cancellationToken = default)
    {
        return await GetBookingsAsync(request, cancellationToken);
    }

    public async Task<AppResponse<string>> ForceCompleteBookingAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(id);
            if (booking == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            booking.Status = BookingStatus.Completed;
            booking.CompletedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;
            booking.Notes = string.IsNullOrEmpty(booking.Notes) ? $"Force completed: {reason}" : $"{booking.Notes}\n\nForce completed: {reason}";

            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Booking force completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error force completing booking {BookingId}", id);
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while force completing the booking");
        }
    }

    public async Task<AppResponse<string>> ConfirmBookingCompletionAsync(int bookingId, int customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(bookingId);
            if (booking == null)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Booking", "Booking not found");
            }

            if (booking.CustomerId != customerId)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Authorization", "You are not authorized to confirm this booking");
            }

            if (booking.Status != BookingStatus.Completed)
            {
                return new AppResponse<string>()
                    .SetErrorResponse("Status", "Booking is not completed yet");
            }

            // Set final amount to total amount if not already set
            if (!booking.FinalAmount.HasValue)
            {
                booking.FinalAmount = booking.TotalAmount;
            }

            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new AppResponse<string>()
                .SetSuccessResponse("Success", "Success", "Booking completion confirmed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming booking completion {BookingId}", bookingId);
            return new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while confirming booking completion");
        }
    }

    public async Task<AppResponse<List<DateTime>>> GetAvailableTimeSlotsAsync(int serviceId, DateTime date, decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find available staff for the service
            var availableStaff = await FindAvailableStaffAsync(serviceId, date, TimeSpan.Zero, latitude, longitude, cancellationToken);
            
            if (!availableStaff.IsSucceeded || availableStaff.Data == null || !availableStaff.Data.Any())
            {
                return new AppResponse<List<DateTime>>()
                    .SetSuccessResponse(new List<DateTime>(), "Info", "No available time slots found");
            }

            // Generate time slots (simplified - every hour from 8 AM to 6 PM)
            var timeSlots = new List<DateTime>();
            for (int hour = 8; hour <= 18; hour++)
            {
                var timeSlot = date.Date.AddHours(hour);
                
                // Check if any staff is available at this time
                var hasAvailableStaff = true; // Simplified check
                
                if (hasAvailableStaff)
                {
                    timeSlots.Add(timeSlot);
                }
            }

            return new AppResponse<List<DateTime>>()
                .SetSuccessResponse(timeSlots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available time slots for service {ServiceId}", serviceId);
            return new AppResponse<List<DateTime>>()
                .SetErrorResponse("Error", "An error occurred while getting available time slots");
        }
    }

    public async Task<AppResponse<List<BookingSummaryResponse>>> GetStaffBookingsAsync(int staffId, BookingListRequest request, CancellationToken cancellationToken = default)
    {
        // Set the staff ID filter
        request.StaffId = staffId;
        return await GetBookingsAsync(request, cancellationToken);
    }

    // Helper methods
    private async Task<string> GenerateBookingNumberAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var lastBookingNumber = await _unitOfWork.Repository<Booking>()
            .ListAsync(filter: b => b.BookingNumber.StartsWith($"BK{today}"), orderBy: null);

        var sequence = lastBookingNumber.Count() + 1;
        return $"BK{today}{sequence:D4}";
    }

    private decimal CalculateBookingAmount(ServiceEntity service, ServicePackage? servicePackage)
    {
        if (servicePackage != null)
        {
            return servicePackage.Price;
        }
        return service.BasePrice;
    }

    private async Task<Booking?> GetBookingWithRelatedEntitiesAsync(int bookingId)
    {
        return await _unitOfWork.Repository<Booking>().FindAsync(b => b.Id == bookingId);
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Simplified distance calculation (Haversine formula)
        const double R = 6371; // Earth's radius in kilometers
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}

// Extension method for combining expressions
public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(left, parameter),
            Expression.Invoke(right, parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
} 