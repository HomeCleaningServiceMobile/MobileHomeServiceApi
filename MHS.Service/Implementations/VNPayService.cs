using MHS.Repository.Interfaces;
using MHS.Service.Currency;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MHS.Service.Implementations
{
    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _configuration;
        private readonly string _vnpUrl;
        private readonly string _vnpReturnUrl;
        private readonly string _vnpTmnCode;
        private readonly string _vnpHashSecret;
        private readonly string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<MHS.Repository.Models.Customer> _customerRepository;
        private readonly IGenericRepository<MHS.Repository.Models.Payment> _paymentRepository;
        private readonly IGenericRepository<MHS.Repository.Models.Booking> _bookingRepository;

        public VNPayService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _vnpUrl = _configuration["VNPay:Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            _vnpReturnUrl = _configuration["VNPay:ReturnUrl"] ?? "http://localhost:5000/api/payment/vnpay/callback";
            _vnpTmnCode = _configuration["VNPay:TmnCode"] ?? "";
            _vnpHashSecret = _configuration["VNPay:HashSecret"] ?? "";
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _bookingRepository = _unitOfWork.Repository<MHS.Repository.Models.Booking>();
            _customerRepository = _unitOfWork.Repository<MHS.Repository.Models.Customer>();
            _paymentRepository = _unitOfWork.Repository<MHS.Repository.Models.Payment>();
        }

        public VNPayResponse CreatePaymentUrl(VNPayRequest body)
        {
            string returnUrl = "http://10.0.2.2:5096/api/payment/vnpay/callback";
            ExchangRate exchangRate = new ExchangRate();
            double exchangeRate = exchangRate.GetUsdToVndExchangeRateAsync().Result;
            var AmountInUsd = Convert.ToDouble(body.Amount, CultureInfo.InvariantCulture);

            double amountInVnd = Math.Round(exchangRate.ConvertUsdToVnd(AmountInUsd, exchangeRate));

            var vnPay = new VnPayLibrary();

            vnPay.AddRequestData("vnp_Amount", (amountInVnd * 100).ToString());
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", body.BookingId);
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnPay.AddRequestData("vnp_TmnCode", _vnpTmnCode);
            vnPay.AddRequestData("vnp_TxnRef", Guid.NewGuid().ToString());
            vnPay.AddRequestData("vnp_Version", "2.1.0");

            string paymentUrl = vnPay.CreateRequestUrl(vnp_Url, _vnpHashSecret);
            return new VNPayResponse { PaymentUrl = paymentUrl,Success = true };
        }

        public bool ValidateCallback(VNPayCallbackRequest callback)
        {
            try
            {
                var vnpParams = new Dictionary<string, string>();
                
                // Only add non-empty parameters for hash validation
                if (!string.IsNullOrEmpty(callback.vnp_Amount)) vnpParams.Add("vnp_Amount", callback.vnp_Amount);
                if (!string.IsNullOrEmpty(callback.vnp_BankCode)) vnpParams.Add("vnp_BankCode", callback.vnp_BankCode);
                if (!string.IsNullOrEmpty(callback.vnp_BankTranNo)) vnpParams.Add("vnp_BankTranNo", callback.vnp_BankTranNo);
                if (!string.IsNullOrEmpty(callback.vnp_CardType)) vnpParams.Add("vnp_CardType", callback.vnp_CardType);
                if (!string.IsNullOrEmpty(callback.vnp_OrderInfo)) vnpParams.Add("vnp_OrderInfo", callback.vnp_OrderInfo);
                if (!string.IsNullOrEmpty(callback.vnp_PayDate)) vnpParams.Add("vnp_PayDate", callback.vnp_PayDate);
                if (!string.IsNullOrEmpty(callback.vnp_ResponseCode)) vnpParams.Add("vnp_ResponseCode", callback.vnp_ResponseCode);
                if (!string.IsNullOrEmpty(callback.vnp_TmnCode)) vnpParams.Add("vnp_TmnCode", callback.vnp_TmnCode);
                if (!string.IsNullOrEmpty(callback.vnp_TransactionNo)) vnpParams.Add("vnp_TransactionNo", callback.vnp_TransactionNo);
                if (!string.IsNullOrEmpty(callback.vnp_TransactionStatus)) vnpParams.Add("vnp_TransactionStatus", callback.vnp_TransactionStatus);
                if (!string.IsNullOrEmpty(callback.vnp_TxnRef)) vnpParams.Add("vnp_TxnRef", callback.vnp_TxnRef);

                var queryString = BuildQueryString(vnpParams);
                var secureHash = CreateSecureHash(queryString, _vnpHashSecret);

                return secureHash.Equals(callback.vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }



        private string BuildQueryString(Dictionary<string, string> parameters)
        {
            var sortedParams = parameters
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .OrderBy(p => p.Key)
                .ToList();

            var queryString = string.Join("&", sortedParams.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
            return queryString;
        }

        private string CreateSecureHash(string data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                return Convert.ToHexString(hash).ToLower();
            }
        }

        public async Task<AppResponse<PaymentConfirmationResponse>> ConfirmPaymentAndDeductBalance(VNPayCallbackRequest callback, int customerId, int bookingId, CancellationToken cancellationToken = default)
        {
            var response = new AppResponse<PaymentConfirmationResponse>();

            if (!ValidateCallback(callback))
            {
                return response.SetErrorResponse("Validation", "Invalid VNPay callback signature");
            }

            if (callback.vnp_ResponseCode != "00" || callback.vnp_TransactionStatus != "00")
            {
                return response.SetErrorResponse("Payment", $"VNPay payment failed with response code: {callback.vnp_ResponseCode}");
            }

            try
            {
                var customer = await _customerRepository.GetEntityByIdAsync(customerId);
                if (customer == null)
                {
                    return response.SetErrorResponse("Customer", "Customer not found");
                }

                decimal amountInVnd = decimal.Parse(callback.vnp_Amount) / 100m; 
                
                ExchangRate exchangeRate = new ExchangRate();
                double exchangeRateValue = await exchangeRate.GetUsdToVndExchangeRateAsync();
                
                double amountInUsd = exchangeRate.ConvertVndToUsd(Convert.ToDouble(amountInVnd), exchangeRateValue);
                decimal amount = Convert.ToDecimal(amountInUsd);

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
                    payment.PaidAt = DateTime.Now;
                    payment.GatewayTransactionId = callback.vnp_TransactionNo;
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
                    TransactionId = callback.vnp_TransactionNo,
                    AmountDeducted = amount,
                    RemainingBalance = customer.Balance,
                    BookingId = bookingId,
                    BookingStatus = booking?.Status.ToString() ?? "Unknown",
                    PaymentProvider = "VNPay",
                    ConfirmedAt = DateTime.Now
                };

                return response.SetSuccessResponse(confirmationData, "Success", "VNPay payment confirmed and balance deducted successfully");
            }
            catch (Exception ex)
            {
                return response.SetErrorResponse("Error", $"An error occurred while processing payment: {ex.Message}");
            }
        }

    }
} 