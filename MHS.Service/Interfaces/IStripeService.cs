using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHS.Service.DTOs;

namespace MHS.Service.Interfaces
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntent(decimal ammount, string currency, string description, CancellationToken cancellationToken = default);
        Task<AppResponse<PaymentConfirmationResponse>> ConfirmPaymentAndDeductBalance(string paymentIntentId, int customerId, int bookingId, CancellationToken cancellationToken = default);
    }
}
