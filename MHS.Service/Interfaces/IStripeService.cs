using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHS.Service.Interfaces
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntent(decimal ammount, string currency, string description, CancellationToken cancellationToken = default);
        Task<bool> ConfirmPaymentAndDeductBalance(string paymentIntentId, int customerId, int bookingId, CancellationToken cancellationToken = default);
    }
}
