using MHS.Service.DTOs;

namespace MHS.Service.Interfaces
{
    public interface IVNPayService
    {
        VNPayResponse CreatePaymentUrl(VNPayRequest body);
        bool ValidateCallback(VNPayCallbackRequest callback);
        Task<AppResponse<PaymentConfirmationResponse>> ConfirmPaymentAndDeductBalance(VNPayCallbackRequest callback, int customerId, int bookingId, CancellationToken cancellationToken = default);
    }
} 