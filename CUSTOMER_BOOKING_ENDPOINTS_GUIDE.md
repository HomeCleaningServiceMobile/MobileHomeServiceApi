# Customer Booking Endpoints Guide

This guide documents all customer booking-related API endpoints for viewing and managing bookings in the Mobile Home Service API.

## 🔐 Authentication

All customer booking endpoints require JWT authentication with `Customer` role (unless specified otherwise).

**Authorization Header:**
```
Authorization: Bearer <your-jwt-token>
```

## 🌐 Base URL

```
https://your-api-domain.com/api/booking
```

---

## 📋 Customer Booking Endpoints Overview

### 📖 Viewing Bookings
- [Get My Bookings (Enhanced)](#get-my-bookings-enhanced)
- [Get My Booking History](#get-my-booking-history)
- [Get My Upcoming Bookings](#get-my-upcoming-bookings)
- [Get Single Booking Details](#get-single-booking-details)

### ✏️ Managing Bookings
- [Create New Booking](#create-new-booking)
- [Update My Booking](#update-my-booking)
- [Cancel My Booking](#cancel-my-booking)
- [Confirm Booking Completion](#confirm-booking-completion)

---

## 📖 Viewing Bookings

### Get My Bookings (Enhanced)

Retrieves customer's bookings with advanced filtering, sorting, and search capabilities.

**Endpoint:** `GET /api/booking/my-bookings`

**Authorization:** `Customer`

**Query Parameters:**
- `status` (optional): Filter by booking status
  - `Pending`, `Confirmed`, `InProgress`, `Completed`, `Cancelled`
- `startDate` (optional): Filter bookings from this date (YYYY-MM-DD)
- `endDate` (optional): Filter bookings to this date (YYYY-MM-DD)
- `serviceId` (optional): Filter by specific service ID
- `serviceName` (optional): Filter by service name (partial match)
- `searchTerm` (optional): Search in booking number, service name, or address
- `sortBy` (optional): Sort field
  - `ScheduledDate`, `CreatedDate`, `BookingNumber`, `ServiceName`, `Status`, `TotalPrice`
- `sortDirection` (optional): `Ascending` or `Descending` (default: Descending)
- `pageNumber` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 10, max: 50)

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Customer bookings retrieved successfully",
  "data": [
    {
      "id": 1,
      "bookingNumber": "BK-20240115-001",
      "status": "Confirmed",
      "scheduledDate": "2024-01-20T00:00:00",
      "scheduledTime": "09:00:00",
      "serviceId": 1,
      "serviceName": "Plumbing Service",
      "serviceAddress": "123 Main St, City",
      "totalPrice": 150.00,
      "estimatedDuration": "02:00:00",
      "staff": {
        "id": 1,
        "name": "John Doe",
        "phoneNumber": "+1234567890",
        "rating": 4.8
      },
      "createdAt": "2024-01-15T10:30:00",
      "canCancel": true,
      "canReschedule": true
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

**Example Requests:**

```bash
# Get all bookings (most recent first)
curl -X GET "https://your-api-domain.com/api/booking/my-bookings" \
  -H "Authorization: Bearer <your-jwt-token>"

# Get pending bookings only
curl -X GET "https://your-api-domain.com/api/booking/my-bookings?status=Pending" \
  -H "Authorization: Bearer <your-jwt-token>"

# Search for plumbing services
curl -X GET "https://your-api-domain.com/api/booking/my-bookings?searchTerm=plumbing" \
  -H "Authorization: Bearer <your-jwt-token>"

# Get bookings from last month, sorted by price
curl -X GET "https://your-api-domain.com/api/booking/my-bookings?startDate=2024-01-01&endDate=2024-01-31&sortBy=TotalPrice&sortDirection=Descending" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

### Get My Booking History

Retrieves customer's booking history with statistics and insights.

**Endpoint:** `GET /api/booking/my-bookings/history`

**Authorization:** `Customer`

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Booking history retrieved successfully",
  "data": {
    "recentBookings": [
      {
        "id": 1,
        "bookingNumber": "BK-20240115-001",
        "status": "Completed",
        "scheduledDate": "2024-01-15T00:00:00",
        "serviceName": "Plumbing Service",
        "totalPrice": 150.00,
        "rating": 5
      }
    ],
    "statistics": {
      "totalBookings": 25,
      "completedBookings": 20,
      "cancelledBookings": 3,
      "pendingBookings": 2,
      "totalSpent": 2750.00,
      "averageRating": 4.6,
      "bookingsThisMonth": 3,
      "bookingsThisYear": 25
    },
    "topServices": [
      {
        "serviceId": 1,
        "serviceName": "Plumbing Service",
        "bookingCount": 8,
        "totalSpent": 1200.00,
        "lastUsed": "2024-01-15T00:00:00"
      },
      {
        "serviceId": 2,
        "serviceName": "Electrical Service",
        "bookingCount": 5,
        "totalSpent": 750.00,
        "lastUsed": "2024-01-10T00:00:00"
      }
    ]
  }
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/booking/my-bookings/history" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

### Get My Upcoming Bookings

Retrieves customer's upcoming bookings within a specified time period.

**Endpoint:** `GET /api/booking/my-bookings/upcoming`

**Authorization:** `Customer`

**Query Parameters:**
- `days` (optional): Number of days to look ahead (default: 30, max: 365)

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Upcoming bookings retrieved successfully",
  "data": [
    {
      "id": 2,
      "bookingNumber": "BK-20240120-001",
      "status": "Confirmed",
      "scheduledDate": "2024-01-20T00:00:00",
      "scheduledTime": "14:00:00",
      "serviceName": "HVAC Maintenance",
      "serviceAddress": "456 Oak Ave, City",
      "totalPrice": 200.00,
      "staff": {
        "id": 2,
        "name": "Jane Smith",
        "phoneNumber": "+1234567891"
      },
      "daysUntilService": 5,
      "canCancel": true,
      "canReschedule": true,
      "reminderSent": false
    }
  ]
}
```

**Example Requests:**
```bash
# Get bookings for next 30 days
curl -X GET "https://your-api-domain.com/api/booking/my-bookings/upcoming" \
  -H "Authorization: Bearer <your-jwt-token>"

# Get bookings for next 7 days
curl -X GET "https://your-api-domain.com/api/booking/my-bookings/upcoming?days=7" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

### Get Single Booking Details

Retrieves detailed information about a specific booking.

**Endpoint:** `GET /api/booking/{id}`

**Authorization:** `Customer` (can only view own bookings)

**Path Parameters:**
- `id` (required): Booking ID

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Booking details retrieved successfully",
  "data": {
    "id": 1,
    "bookingNumber": "BK-20240115-001",
    "status": "Completed",
    "scheduledDate": "2024-01-15T00:00:00",
    "scheduledTime": "09:00:00",
    "actualStartTime": "09:05:00",
    "actualEndTime": "11:15:00",
    "service": {
      "id": 1,
      "name": "Plumbing Service",
      "description": "Professional plumbing repairs and maintenance"
    },
    "servicePackage": {
      "id": 1,
      "name": "Basic Plumbing Package",
      "duration": "02:00:00",
      "price": 150.00
    },
    "customer": {
      "id": 1,
      "name": "John Customer",
      "phoneNumber": "+1234567890"
    },
    "staff": {
      "id": 1,
      "name": "John Doe",
      "phoneNumber": "+1234567890",
      "rating": 4.8,
      "profileImageUrl": "https://example.com/staff/1.jpg"
    },
    "serviceAddress": "123 Main St, City",
    "specialInstructions": "Please call before arriving",
    "totalPrice": 150.00,
    "paymentMethod": "CreditCard",
    "paymentStatus": "Completed",
    "completionNotes": "Service completed successfully",
    "rating": 5,
    "review": "Excellent service, very professional",
    "createdAt": "2024-01-10T10:30:00",
    "timeline": [
      {
        "status": "Created",
        "timestamp": "2024-01-10T10:30:00",
        "description": "Booking created"
      },
      {
        "status": "Confirmed",
        "timestamp": "2024-01-12T14:20:00",
        "description": "Staff assigned and confirmed"
      },
      {
        "status": "InProgress",
        "timestamp": "2024-01-15T09:05:00",
        "description": "Staff checked in"
      },
      {
        "status": "Completed",
        "timestamp": "2024-01-15T11:15:00",
        "description": "Service completed"
      }
    ]
  }
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/booking/1" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

## ✏️ Managing Bookings

### Create New Booking

Creates a new service booking.

**Endpoint:** `POST /api/booking`

**Authorization:** `Customer`

**Request Body:**
```json
{
  "serviceId": 1,
  "servicePackageId": 1,
  "scheduledDate": "2024-01-20",
  "scheduledTime": "09:00:00",
  "serviceAddress": "123 Main St, City",
  "addressLatitude": 40.7128,
  "addressLongitude": -74.0060,
  "specialInstructions": "Please call before arriving",
  "preferredPaymentMethod": "CreditCard"
}
```

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Booking created successfully",
  "data": {
    "id": 1,
    "bookingNumber": "BK-20240115-001",
    "status": "Pending",
    "estimatedTotal": 150.00
  }
}
```

---

### Update My Booking

Updates an existing booking (only allowed before staff assignment).

**Endpoint:** `PUT /api/booking/{id}`

**Authorization:** `Customer` (can only update own bookings)

**Path Parameters:**
- `id` (required): Booking ID

**Request Body:**
```json
{
  "scheduledDate": "2024-01-21",
  "scheduledTime": "10:00:00",
  "serviceAddress": "456 Oak Ave, City",
  "addressLatitude": 40.7589,
  "addressLongitude": -73.9851,
  "specialInstructions": "Updated instructions"
}
```

---

### Cancel My Booking

Cancels an existing booking.

**Endpoint:** `POST /api/booking/{id}/cancel`

**Authorization:** `Customer` (can only cancel own bookings)

**Path Parameters:**
- `id` (required): Booking ID

**Request Body:**
```json
{
  "reason": "Change of plans"
}
```

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Booking cancelled successfully",
  "data": {
    "bookingId": 1,
    "refundAmount": 0.00,
    "cancellationFee": 25.00
  }
}
```

---

## 📊 Booking Status Explanations

| Status | Description | Customer Actions Available |
|--------|-------------|---------------------------|
| `Pending` | Booking created, awaiting staff assignment | Update, Cancel |
| `Confirmed` | Staff assigned and confirmed | Cancel (with fee), Reschedule |
| `InProgress` | Staff has checked in, service in progress | None |
| `Completed` | Service completed | Rate & Review |
| `Cancelled` | Booking cancelled | None |

---

## 🔍 Search and Filter Examples

### Advanced Search Examples:

```bash
# Search by booking number
curl -X GET "https://your-api-domain.com/api/booking/my-bookings?searchTerm=BK-20240115" \
  -H "Authorization: Bearer <your-jwt-token>"

# Find all electrical services this year
curl -X GET "https://your-api-domain.com/api/booking/my-bookings?serviceName=electrical&startDate=2024-01-01" \
  -H "Authorization: Bearer <your-jwt-token>"

# Get expensive bookings (over $200) sorted by price
curl -X GET "https://your-api-domain.com/api/booking/my-bookings?sortBy=TotalPrice&sortDirection=Descending" \
  -H "Authorization: Bearer <your-jwt-token>"
```

### Filter Combinations:

```bash
# Completed bookings from last 3 months, sorted by date
curl -X GET "https://your-api-domain.com/api/booking/my-bookings?status=Completed&startDate=2023-10-01&sortBy=ScheduledDate&sortDirection=Descending" \
  -H "Authorization: Bearer <your-jwt-token>"

# All cancelled bookings with pagination
curl -X GET "https://your-api-domain.com/api/booking/my-bookings?status=Cancelled&pageNumber=1&pageSize=20" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

## ⚠️ Error Responses

### Common Error Response Format
```json
{
  "isSucceeded": false,
  "message": "Error message",
  "data": null,
  "errors": [
    {
      "field": "fieldName",
      "message": "Validation error message"
    }
  ]
}
```

### Common Error Scenarios

#### Permission Denied
```json
{
  "isSucceeded": false,
  "message": "You don't have permission to view this booking",
  "data": null
}
```

#### Booking Not Found
```json
{
  "isSucceeded": false,
  "message": "Booking not found",
  "data": null
}
```

#### Invalid Date Range
```json
{
  "isSucceeded": false,
  "message": "End date must be after start date",
  "data": null
}
```

---

## 📱 Mobile App Integration Tips

### 1. **Pagination Strategy**
```javascript
// Implement infinite scroll for better UX
const loadMoreBookings = async (pageNumber) => {
  const response = await fetch(`/api/booking/my-bookings?pageNumber=${pageNumber}&pageSize=20`);
  // Append to existing list
};
```

### 2. **Real-time Updates**
```javascript
// Use WebSocket or polling for status updates
const checkBookingUpdates = () => {
  // Poll every 30 seconds for active bookings
  setInterval(fetchUpcomingBookings, 30000);
};
```

### 3. **Caching Strategy**
```javascript
// Cache completed bookings, refresh active ones
const cacheCompletedBookings = (bookings) => {
  const completed = bookings.filter(b => b.status === 'Completed');
  localStorage.setItem('completedBookings', JSON.stringify(completed));
};
```

---

## 🚀 Best Practices

### 1. **Performance Optimization**
- Use pagination for large booking lists
- Implement caching for historical data
- Use specific date ranges to limit data

### 2. **User Experience**
- Show loading states during API calls
- Implement pull-to-refresh for mobile
- Use optimistic updates for better responsiveness

### 3. **Data Freshness**
- Refresh upcoming bookings more frequently
- Cache completed bookings for longer periods
- Show last updated timestamp

### 4. **Error Handling**
- Provide clear error messages
- Implement retry mechanisms
- Handle network connectivity issues

---

## 📞 Support

For additional support or questions about customer booking endpoints:
- **API Documentation**: [Main API Documentation]
- **Development Team**: Contact the backend development team
- **Bug Reports**: Submit issues through the project management system

---

**Last Updated:** January 2024  
**API Version:** v1.0 