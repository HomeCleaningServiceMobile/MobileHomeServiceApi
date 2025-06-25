using MHS.Service.DTOs;

namespace MHS.Service.Interfaces;

public interface IBookingService
{
    // Customer booking operations
    Task<ApiResponse<BookingResponse>> CreateBookingAsync(int customerId, CreateBookingRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<BookingResponse>> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<PaginatedResponse<BookingSummaryResponse>>> GetBookingsAsync(BookingListRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<BookingResponse>> UpdateBookingAsync(int id, UpdateBookingRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> CancelBookingAsync(int id, string reason, CancellationToken cancellationToken = default);
    
    // Staff operations
    Task<ApiResponse<string>> RespondToBookingAsync(int staffId, StaffResponseRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> CheckInAsync(int staffId, CheckInRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> CheckOutAsync(int staffId, CheckOutRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<PaginatedResponse<BookingSummaryResponse>>> GetStaffBookingsAsync(int staffId, BookingListRequest request, CancellationToken cancellationToken = default);
    
    // Auto-assignment system
    Task<ApiResponse<string>> AutoAssignStaffAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<StaffResponse>>> FindAvailableStaffAsync(int serviceId, DateTime scheduledDate, TimeSpan scheduledTime, decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
    
    // Admin operations
    Task<ApiResponse<string>> ManualAssignStaffAsync(int bookingId, int staffId, CancellationToken cancellationToken = default);
    Task<ApiResponse<PaginatedResponse<BookingSummaryResponse>>> GetAllBookingsAsync(BookingListRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> ForceCompleteBookingAsync(int id, string reason, CancellationToken cancellationToken = default);
    
    // Booking flow management
    Task<ApiResponse<string>> ConfirmBookingCompletionAsync(int bookingId, int customerId, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<DateTime>>> GetAvailableTimeSlotsAsync(int serviceId, DateTime date, decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
} 