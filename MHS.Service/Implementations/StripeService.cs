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
using MHS.Service.DTOs;


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

        public async Task<AppResponse<PaymentConfirmationResponse>> ConfirmPaymentAndDeductBalance(string paymentIntentId, int customerId, int bookingId, CancellationToken cancellationToken = default)
        {
            var response = new AppResponse<PaymentConfirmationResponse>();

            try
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

                if (paymentIntent.Status != "succeeded")
                {
                    return response.SetErrorResponse("Payment", $"Stripe payment not successful. Status: {paymentIntent.Status}");
                }

                var customer = await _customerRepository.GetEntityByIdAsync(customerId);
                if (customer == null)
                {
                    return response.SetErrorResponse("Customer", "Customer not found");
                }

                decimal amount = paymentIntent.AmountReceived / 100m;

                if (customer.Balance < amount)
                {
                    return response.SetErrorResponse("Balance", $"Insufficient balance. Required: ${amount:F2}, Available: ${customer.Balance:F2}");
                }

                customer.Balance -= amount;
                customer.TotalSpent += amount;
                _customerRepository.Update(customer);

                var payment = await _paymentRepository.FindAsync(p => p.BookingId == bookingId);
                if (payment != null) 
                {
                    payment.Status = MHS.Common.Enums.PaymentStatus.Paid;
                    payment.PaidAt = DateTime.Now; //Change from UtcNow to Now 
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

                var confirmationData = new PaymentConfirmationResponse
                {
                    IsConfirmed = true,
                    TransactionId = paymentIntent.Id,
                    AmountDeducted = amount,
                    RemainingBalance = customer.Balance,
                    BookingId = bookingId,
                    BookingStatus = booking?.Status.ToString() ?? "Unknown",
                    PaymentProvider = "Stripe",
                    ConfirmedAt = DateTime.Now
                };

                return response.SetSuccessResponse(confirmationData, "Success", "Stripe payment confirmed and balance deducted successfully");
            }
            catch (Exception ex)
            {
                return response.SetErrorResponse("Error", $"An error occurred while processing payment: {ex.Message}");
            }
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
