using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using MHS.Service.Interfaces;
using MHS.Repository.Interfaces;
using MHS.Repository.Implementations;


namespace MHS.Service.Implementations
{
    public class StripeService : IStripeService
    {
        private readonly string _secretKey;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<MHS.Repository.Models.Customer> _customerRepository;
        private readonly IGenericRepository<MHS.Repository.Models.Payment> _paymentRepository;
        private readonly IGenericRepository<MHS.Repository.Models.Booking> _bookingRepository;
        public StripeService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _bookingRepository = _unitOfWork.Repository<MHS.Repository.Models.Booking>();
            _customerRepository = _unitOfWork.Repository<MHS.Repository.Models.Customer>();
            _paymentRepository = _unitOfWork.Repository<MHS.Repository.Models.Payment>();
            _secretKey = configuration["Stripe:SecretKey"] ?? throw new Exception("Stripe SecretKey not found");
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<bool> ConfirmPaymentAndDeductBalance(string paymentIntentId, int customerId, int bookingId, CancellationToken cancellationToken = default)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

            if (paymentIntent.Status == "succeeded")
            {
                var customer = await _customerRepository.GetEntityByIdAsync(customerId);
                if (customer == null)
                    throw new Exception("Customer not found");

                decimal amount = paymentIntent.AmountReceived / 100m;

                if (customer.Balance < amount)
                    throw new Exception("Insufficient balance");

                customer.Balance -= amount;
                customer.TotalSpent += amount;
                _customerRepository.Update(customer);

                var payment = await _paymentRepository.FindAsync(p => p.BookingId == bookingId);
                if (payment != null) 
                {
                    payment.Status = MHS.Common.Enums.PaymentStatus.Paid;
                    payment.PaidAt = DateTime.UtcNow;
                    payment.GatewayTransactionId = paymentIntent.Id;
                    _paymentRepository.Update(payment);
                }

                var booking = await _bookingRepository.GetEntityByIdAsync(bookingId); 
                if (booking != null)
                {
                    booking.Status = MHS.Common.Enums.BookingStatus.Confirmed;
                    _bookingRepository.Update(booking);
                }

                await _unitOfWork.CompleteAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<string> CreatePaymentIntent(decimal amount, string currency, string description, CancellationToken cancellationToken = default)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe dùng cent
                Currency = currency,
                Description = description,
                PaymentMethodTypes = new List<string> { "card" }
            };
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);
            return paymentIntent.ClientSecret;
        }
    }
}
