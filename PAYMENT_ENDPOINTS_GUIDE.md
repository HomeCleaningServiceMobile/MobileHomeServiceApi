# Mobile Home Service API - Payment Endpoints Guide

This guide covers all payment endpoints available in the Mobile Home Service API, supporting both VNPay and Stripe payment providers.

## Base URL
```
https://your-api-domain.com/api/payment
```

## Common Headers
```
Content-Type: application/json
Authorization: Bearer {your-jwt-token} (for protected endpoints)
```

---

## VNPay Payment Endpoints

### 1. Create VNPay Payment URL
**POST** `/api/payment/vnpay/create`

Creates a VNPay payment URL for the user to complete payment.

#### Request Body
```json
{
  "amount": 50.00,
  "bookingId": "123",
  "orderInfo": "Payment for booking #123",
  "returnUrl": "https://your-app.com/payment/success",
  "ipAddress": "127.0.0.1"
}
```

#### Response (Success - 200)
```json
{
  "success": true,
  "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=1250000&vnp_Command=pay&...",
  "transactionId": "",
  "provider": "VNPay",
  "message": ""
}
```

#### Response (Error - 400)
```json
{
  "success": false,
  "paymentUrl": "",
  "transactionId": "",
  "provider": "VNPay",
  "message": "Invalid request parameters"
}
```

#### Response (Error - 500)
```json
{
  "success": false,
  "paymentUrl": "",
  "transactionId": "",
  "provider": "VNPay",
  "message": "Internal server error"
}
```

---

### 2. Confirm VNPay Payment and Deduct Balance
**POST** `/api/payment/vnpay/confirm?customerId={customerId}&bookingId={bookingId}`

Validates the VNPay callback, confirms payment, and deducts the amount from customer balance.

#### Query Parameters
- `customerId` (int, required): ID of the customer
- `bookingId` (int, required): ID of the booking

#### Request Body (VNPay Callback Data)
```json
{
  "vnp_Amount": "1250000",
  "vnp_BankCode": "NCB",
  "vnp_BankTranNo": "VNP14024944",
  "vnp_CardType": "ATM",
  "vnp_OrderInfo": "123",
  "vnp_PayDate": "20240115103000",
  "vnp_ResponseCode": "00",
  "vnp_TmnCode": "DEMO123",
  "vnp_TransactionNo": "14024944",
  "vnp_TransactionStatus": "00",
  "vnp_TxnRef": "638412345678901234",
  "vnp_SecureHash": "abc123def456..."
}
```

#### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["VNPay payment confirmed and balance deducted successfully"]
  },
  "data": {
    "isConfirmed": true,
    "transactionId": "14024944",
    "amountDeducted": 50.00,
    "remainingBalance": 150.00,
    "bookingId": 123,
    "bookingStatus": "Confirmed",
    "paymentProvider": "VNPay",
    "confirmedAt": "2024-01-15T10:30:00Z"
  }
}
```

#### Response (Error - 400)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Validation": ["Invalid VNPay callback signature"]
  }
}
```

#### Response (Error - 400 - Payment Failed)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Payment": ["VNPay payment failed with response code: 24"]
  }
}
```

#### Response (Error - 400 - Insufficient Balance)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Balance": ["Insufficient balance. Required: $50.00, Available: $30.00"]
  }
}
```

#### Response (Error - 400 - Customer Not Found)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Customer": ["Customer not found"]
  }
}
```

---

## Stripe Payment Endpoints

### 1. Create Stripe Payment Intent
**POST** `/api/payment/stripe/create`

Creates a Stripe payment intent for the user to complete payment.

#### Request Body
```json
{
  "amount": 5000,
  "currency": "usd",
  "description": "Payment for booking #123"
}
```

#### Response (Success - 200)
```json
{
  "client_secret": "pi_3ABC123def456_secret_XYZ789"
}
```

#### Response (Error - 500)
```json
{
  "success": false,
  "sessionId": "",
  "paymentUrl": "",
  "message": "Internal server error",
  "provider": "Stripe"
}
```

---

### 2. Confirm Stripe Payment and Deduct Balance
**POST** `/api/payment/stripe/confirm?paymentIntentId={paymentIntentId}&customerId={customerId}&bookingId={bookingId}`

Confirms the Stripe payment and deducts the amount from customer balance.

#### Query Parameters
- `paymentIntentId` (string, required): Stripe payment intent ID
- `customerId` (int, required): ID of the customer
- `bookingId` (int, required): ID of the booking

#### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Stripe payment confirmed and balance deducted successfully"]
  },
  "data": {
    "isConfirmed": true,
    "transactionId": "pi_3ABC123def456",
    "amountDeducted": 50.00,
    "remainingBalance": 150.00,
    "bookingId": 123,
    "bookingStatus": "Confirmed",
    "paymentProvider": "Stripe",
    "confirmedAt": "2024-01-15T10:30:00Z"
  }
}
```

#### Response (Error - 400 - Payment Not Successful)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Payment": ["Stripe payment not successful. Status: requires_payment_method"]
  }
}
```

#### Response (Error - 400 - Customer Not Found)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Customer": ["Customer not found"]
  }
}
```

#### Response (Error - 400 - Insufficient Balance)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Balance": ["Insufficient balance. Required: $50.00, Available: $30.00"]
  }
}
```

---

## Error Response Categories

### Validation Errors
- **VNPay**: Invalid callback signature
- **Stripe**: Invalid payment intent ID

### Payment Errors
- **VNPay**: Payment failed with specific response codes
- **Stripe**: Payment not in successful state

### Customer Errors
- Customer not found in the system

### Balance Errors
- Insufficient customer balance for payment amount

### System Errors
- Internal server errors with generic error messages

---

## Payment Flow

### VNPay Flow
1. **Frontend** → `POST /vnpay/create` → Get payment URL
2. **User** → Complete payment on VNPay
3. **VNPay** → Returns to your app with callback data
4. **Frontend** → `POST /vnpay/confirm` → Validate + Process + Deduct balance

### Stripe Flow
1. **Frontend** → `POST /stripe/create` → Get payment intent
2. **User** → Complete payment with Stripe Elements
3. **Frontend** → `POST /stripe/confirm` → Validate + Process + Deduct balance

---

## Data Models

### PaymentConfirmationResponse
```json
{
  "isConfirmed": true,
  "transactionId": "string",
  "amountDeducted": 0.00,
  "remainingBalance": 0.00,
  "bookingId": 0,
  "bookingStatus": "string",
  "paymentProvider": "string",
  "confirmedAt": "2024-01-15T10:30:00Z"
}
```

### AppResponse Structure
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "key": ["array of messages"]
  },
  "data": {
    // Response data object
  }
}
```

---

## Currency Handling

### VNPay
- Accepts USD amounts in the request
- Automatically converts to VND using current exchange rates
- Processes payment in VND
- Deducts equivalent USD amount from customer balance

### Stripe
- Processes payments in USD
- Direct USD amount deduction from customer balance

---

## Status Codes

- **200**: Success
- **400**: Bad Request (validation errors, payment failures, insufficient balance)
- **500**: Internal Server Error

---

## Testing

### VNPay Test Card Numbers
- **Successful Payment**: Use VNPay sandbox test cards
- **Failed Payment**: Use invalid card numbers

### Stripe Test Card Numbers
- **Successful Payment**: `4242424242424242`
- **Failed Payment**: `4000000000000002`

---

## Security Notes

- All payment confirmations validate signatures/payment status
- Customer balance is only deducted after successful payment validation
- Booking status is updated to "Confirmed" only after successful payment processing
- All endpoints include proper error handling and logging 