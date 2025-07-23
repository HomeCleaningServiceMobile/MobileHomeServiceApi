using MHS.Service.DTOs;
using MHS.Common.Enums;

namespace MHS.Service.Interfaces;

public interface IBookingService
{
    // Customer booking operations
    Task<AppResponse<BookingResponse>> CreateBookingAsync(int customerId, CreateBookingRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<BookingResponse>> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppResponse<List<BookingSummaryResponse>>> GetBookingsAsync(BookingListRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<BookingResponse>> UpdateBookingAsync(int id, UpdateBookingRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> CancelBookingAsync(int id, string reason, CancellationToken cancellationToken = default);
    
    // Enhanced customer booking operations
    Task<AppResponse<List<BookingSummaryResponse>>> GetCustomerBookingsAsync(CustomerBookingListRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<CustomerBookingHistoryResponse>> GetCustomerBookingHistoryAsync(int customerId, CancellationToken cancellationToken = default);
    Task<AppResponse<List<BookingSummaryResponse>>> GetCustomerUpcomingBookingsAsync(int customerId, int days = 30, CancellationToken cancellationToken = default);
    
    // Staff operations
    Task<AppResponse<string>> RespondToBookingAsync(int userId, StaffResponseRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> CheckInAsync(int userId, CheckInRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> CheckOutAsync(int userId, CheckOutRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<List<BookingSummaryResponse>>> GetStaffBookingsAsync(int userId, BookingListRequest request, CancellationToken cancellationToken = default);
    
    // Auto-assignment system
    Task<AppResponse<string>> AutoAssignStaffAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<AppResponse<List<StaffResponse>>> FindAvailableStaffAsync(int serviceId, DateTime scheduledDate, TimeSpan scheduledTime, decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
    
    // Admin operations
    Task<AppResponse<List<BookingSummaryResponse>>> GetAllBookingsAsync(BookingListRequest request, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> ManualAssignStaffAsync(int bookingId, int staffId, CancellationToken cancellationToken = default);
    Task<AppResponse<string>> ForceCompleteBookingAsync(int id, string reason, CancellationToken cancellationToken = default);
    
    // Booking flow management
    Task<AppResponse<string>> ConfirmBookingCompletionAsync(int bookingId, int customerId, CancellationToken cancellationToken = default);
    Task<AppResponse<List<DateTime>>> GetAvailableTimeSlotsAsync(int serviceId, DateTime date, decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
} 