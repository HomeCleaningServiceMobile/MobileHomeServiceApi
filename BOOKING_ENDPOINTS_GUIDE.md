# Mobile Home Service API - Booking Endpoints Guide

This guide covers all booking endpoints available in the Mobile Home Service API.

## Base URL
```
https://your-api-domain.com/api/booking
```

## Common Headers
```
Content-Type: application/json
Authorization: Bearer {your-jwt-token}
```

**Note:** All booking endpoints require authentication 🔒

---

## Booking Status Reference

| Status | Value | Description |
|--------|-------|-------------|
| `Pending` | 0 | Initial booking state, waiting for confirmation |
| `Confirmed` | 1 | Booking confirmed by system |
| `AutoAssigned` | 2 | Staff automatically assigned |
| `PendingSchedule` | 3 | Waiting for staff to accept/reject |
| `InProgress` | 4 | Service currently being performed |
| `Completed` | 5 | Service finished successfully |
| `Cancelled` | 6 | Booking cancelled |
| `Rejected` | 7 | Booking rejected by staff |

## Payment Methods

| Method | Value | Description |
|--------|-------|-------------|
| `Cash` | 1 | Cash payment |
| `CreditCard` | 2 | Credit card payment |
| `DebitCard` | 3 | Debit card payment |
| `BankTransfer` | 4 | Bank transfer |
| `EWallet` | 5 | E-wallet payment |
| `QRCode` | 6 | QR code payment |

---

## 1. Create Booking (Customer)
**POST** `/api/booking`

🔒 **Customer Role Required**

Create a new service booking.

### Request Body
```json
{
  "serviceId": 1,
  "servicePackageId": 2,
  "scheduledDate": "2024-02-15T00:00:00Z",
  "scheduledTime": "14:30:00",
  "serviceAddress": "123 Main St, District 1, Ho Chi Minh City",
  "addressLatitude": 10.762622,
  "addressLongitude": 106.660172,
  "specialInstructions": "Please bring cleaning supplies. Access via main entrance.",
  "preferredPaymentMethod": 1
}
```

### Response (Success - 201)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Booking created successfully"]
  },
  "data": {
    "id": 123,
    "bookingNumber": "BK-2024-001-123",
    "status": 0,
    "scheduledDate": "2024-02-15T00:00:00Z",
    "scheduledTime": "14:30:00",
    "estimatedDurationMinutes": 120,
    "totalAmount": 50.00,
    "finalAmount": null,
    "serviceAddress": "123 Main St, District 1, Ho Chi Minh City",
    "specialInstructions": "Please bring cleaning supplies. Access via main entrance.",
    "notes": null,
    "startedAt": null,
    "completedAt": null,
    "cancelledAt": null,
    "cancellationReason": null,
    "createdAt": "2024-01-15T10:30:00Z",
    "customer": {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "email": "customer@example.com",
      "phoneNumber": "+1234567890"
    },
    "service": {
      "id": 1,
      "name": "House Cleaning",
      "description": "Professional house cleaning service",
      "basePrice": 50.00,
      "type": 1
    },
    "servicePackage": {
      "id": 2,
      "name": "Deep Cleaning Package",
      "description": "Complete deep cleaning service",
      "price": 80.00,
      "durationMinutes": 180
    },
    "staff": null,
    "payment": null,
    "review": null,
    "bookingImages": []
  }
}
```

---

## 2. Get Booking by ID
**GET** `/api/booking/{id}`

🔒 **All Roles** (with permissions)

Get detailed information about a specific booking.

### Path Parameters
- `id` (integer): Booking ID

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "data": {
    "id": 123,
    "bookingNumber": "BK-2024-001-123",
    "status": 4,
    "scheduledDate": "2024-02-15T00:00:00Z",
    "scheduledTime": "14:30:00",
    "estimatedDurationMinutes": 120,
    "totalAmount": 50.00,
    "finalAmount": 55.00,
    "serviceAddress": "123 Main St, District 1, Ho Chi Minh City",
    "specialInstructions": "Please bring cleaning supplies.",
    "notes": "Customer was very satisfied with service",
    "startedAt": "2024-02-15T14:35:00Z",
    "completedAt": null,
    "cancelledAt": null,
    "cancellationReason": null,
    "createdAt": "2024-01-15T10:30:00Z",
    "customer": {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "email": "customer@example.com",
      "phoneNumber": "+1234567890"
    },
    "service": {
      "id": 1,
      "name": "House Cleaning",
      "description": "Professional house cleaning service",
      "basePrice": 50.00,
      "type": 1
    },
    "staff": {
      "id": 5,
      "firstName": "Jane",
      "lastName": "Smith",
      "email": "staff@example.com",
      "phoneNumber": "+1234567891",
      "hourlyRate": 25.00,
      "rating": 4.8
    },
    "payment": {
      "id": 78,
      "amount": 55.00,
      "paymentMethod": 1,
      "status": 1,
      "paidAt": "2024-02-15T16:45:00Z"
    },
    "review": null,
    "bookingImages": []
  }
}
```

---

## 3. Get Bookings (with Filters)
**GET** `/api/booking`

🔒 **All Roles** (role-based filtering applied)

Get a list of bookings with filtering and pagination.

### Query Parameters
- `status` (integer, optional): Filter by booking status
- `startDate` (datetime, optional): Filter bookings from this date
- `endDate` (datetime, optional): Filter bookings until this date
- `customerId` (integer, optional): Filter by customer ID (Admin only)
- `staffId` (integer, optional): Filter by staff ID (Admin only)
- `pageNumber` (integer, default: 1): Page number
- `pageSize` (integer, default: 10): Items per page

### Example Request
```
GET /api/booking?status=1&startDate=2024-02-01&endDate=2024-02-28&pageNumber=1&pageSize=10
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "data": {
    "items": [
      {
        "id": 123,
        "bookingNumber": "BK-2024-001-123",
        "status": 1,
        "scheduledDate": "2024-02-15T00:00:00Z",
        "scheduledTime": "14:30:00",
        "totalAmount": 50.00,
        "serviceAddress": "123 Main St",
        "customer": {
          "id": 1,
          "firstName": "John",
          "lastName": "Doe"
        },
        "service": {
          "id": 1,
          "name": "House Cleaning"
        },
        "staff": {
          "id": 5,
          "firstName": "Jane",
          "lastName": "Smith"
        }
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

---

## 4. Update Booking (Customer/Admin)
**PUT** `/api/booking/{id}`

🔒 **Customer/Admin Roles** (customers can only update their own bookings)

Update booking details.

### Path Parameters
- `id` (integer): Booking ID

### Request Body
```json
{
  "scheduledDate": "2024-02-16T00:00:00Z",
  "scheduledTime": "15:00:00",
  "serviceAddress": "456 Updated St, District 2, Ho Chi Minh City",
  "addressLatitude": 10.762622,
  "addressLongitude": 106.660172,
  "specialInstructions": "Updated instructions: Use side entrance"
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Booking updated successfully"]
  },
  "data": {
    "id": 123,
    "bookingNumber": "BK-2024-001-123",
    "status": 1,
    "scheduledDate": "2024-02-16T00:00:00Z",
    "scheduledTime": "15:00:00",
    "serviceAddress": "456 Updated St, District 2, Ho Chi Minh City",
    "specialInstructions": "Updated instructions: Use side entrance"
  }
}
```

---

## 5. Cancel Booking (Customer/Admin)
**POST** `/api/booking/{id}/cancel`

🔒 **Customer/Admin Roles** (customers can only cancel their own bookings)

Cancel a booking.

### Path Parameters
- `id` (integer): Booking ID

### Request Body
```json
{
  "reason": "Customer requested cancellation due to scheduling conflict"
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Booking cancelled successfully"]
  },
  "data": {
    "id": 123,
    "status": 6,
    "cancelledAt": "2024-01-15T10:30:00Z",
    "cancellationReason": "Customer requested cancellation due to scheduling conflict"
  }
}
```

---

## 6. Staff Respond to Booking (Staff)
**POST** `/api/booking/{id}/respond`

🔒 **Staff Role Required**

Staff accepts or rejects a booking assignment.

### Path Parameters
- `id` (integer): Booking ID

### Request Body
```json
{
  "accept": true,
  "declineReason": null
}
```

### Request Body (Decline)
```json
{
  "accept": false,
  "declineReason": "Not available at the scheduled time"
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Booking response recorded successfully"]
  },
  "data": {
    "id": 123,
    "status": 1,
    "staff": {
      "id": 5,
      "firstName": "Jane",
      "lastName": "Smith"
    }
  }
}
```

---

## 7. Staff Check-In (Staff)
**POST** `/api/booking/{id}/checkin`

🔒 **Staff Role Required**

Staff checks in to start the service.

### Path Parameters
- `id` (integer): Booking ID

### Request Body
```json
{
  "currentLatitude": 10.762622,
  "currentLongitude": 106.660172,
  "notes": "Arrived at customer location, ready to start service"
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Check-in successful"]
  },
  "data": {
    "id": 123,
    "status": 4,
    "startedAt": "2024-01-15T10:30:00Z",
    "notes": "Arrived at customer location, ready to start service"
  }
}
```

---

## 8. Staff Check-Out (Staff)
**POST** `/api/booking/{id}/checkout`

🔒 **Staff Role Required**

Staff checks out after completing the service.

### Path Parameters
- `id` (integer): Booking ID

### Request Body
```json
{
  "completionNotes": "Service completed successfully. All areas cleaned as requested.",
  "completionImageUrls": [
    "https://example.com/before-image.jpg",
    "https://example.com/after-image1.jpg",
    "https://example.com/after-image2.jpg"
  ]
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Check-out successful"]
  },
  "data": {
    "id": 123,
    "status": 5,
    "completedAt": "2024-01-15T12:30:00Z",
    "notes": "Service completed successfully. All areas cleaned as requested.",
    "bookingImages": [
      {
        "id": 1,
        "imageUrl": "https://example.com/before-image.jpg",
        "imageType": "before",
        "description": "Before cleaning",
        "takenAt": "2024-01-15T10:30:00Z",
        "takenBy": "Jane Smith"
      }
    ]
  }
}
```

---

## 9. Auto-Assign Staff (Admin)
**POST** `/api/booking/{id}/auto-assign`

🔒 **Admin Role Required**

Automatically assign available staff to a booking.

### Path Parameters
- `id` (integer): Booking ID

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Staff assigned successfully"]
  },
  "data": {
    "id": 123,
    "status": 2,
    "staff": {
      "id": 5,
      "firstName": "Jane",
      "lastName": "Smith",
      "phoneNumber": "+1234567891",
      "hourlyRate": 25.00,
      "rating": 4.8
    }
  }
}
```

---

## 10. Manual Assign Staff (Admin)
**POST** `/api/booking/{id}/assign`

🔒 **Admin Role Required**

Manually assign a specific staff member to a booking.

### Path Parameters
- `id` (integer): Booking ID

### Request Body
```json
{
  "staffId": 5
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Staff assigned successfully"]
  },
  "data": {
    "id": 123,
    "status": 2,
    "staff": {
      "id": 5,
      "firstName": "Jane",
      "lastName": "Smith",
      "phoneNumber": "+1234567891"
    }
  }
}
```

---

## 11. Force Complete Booking (Admin)
**POST** `/api/booking/{id}/force-complete`

🔒 **Admin Role Required**

Force complete a booking (emergency situations).

### Path Parameters
- `id` (integer): Booking ID

### Request Body
```json
{
  "reason": "Emergency completion - customer confirmed service was satisfactory"
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Booking force completed successfully"]
  },
  "data": {
    "id": 123,
    "status": 5,
    "completedAt": "2024-01-15T10:30:00Z",
    "notes": "Emergency completion - customer confirmed service was satisfactory"
  }
}
```

---

## 12. Customer Confirm Completion (Customer)
**POST** `/api/booking/{id}/confirm`

🔒 **Customer Role Required**

Customer confirms that the service was completed satisfactorily.

### Path Parameters
- `id` (integer): Booking ID

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Booking completion confirmed"]
  },
  "data": {
    "id": 123,
    "status": 5,
    "completedAt": "2024-01-15T12:30:00Z"
  }
}
```

---

## 13. Get Available Time Slots (Customer)
**GET** `/api/booking/available-slots`

🔒 **Customer Role Required**

Get available time slots for a service on a specific date.

### Query Parameters
- `serviceId` (integer, required): Service ID
- `date` (datetime, required): Date to check availability
- `latitude` (decimal, required): Service location latitude
- `longitude` (decimal, required): Service location longitude

### Example Request
```
GET /api/booking/available-slots?serviceId=1&date=2024-02-15&latitude=10.762622&longitude=106.660172
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "data": {
    "availableSlots": [
      {
        "time": "09:00:00",
        "available": true,
        "staffCount": 3
      },
      {
        "time": "10:00:00",
        "available": true,
        "staffCount": 2
      },
      {
        "time": "11:00:00",
        "available": false,
        "staffCount": 0
      },
      {
        "time": "14:00:00",
        "available": true,
        "staffCount": 4
      }
    ]
  }
}
```

---

## 14. Find Available Staff (Admin)
**GET** `/api/booking/available-staff`

🔒 **Admin Role Required**

Find available staff for a specific service and time.

### Query Parameters
- `serviceId` (integer, required): Service ID
- `scheduledDate` (datetime, required): Scheduled date
- `scheduledTime` (time, required): Scheduled time
- `latitude` (decimal, required): Service location latitude
- `longitude` (decimal, required): Service location longitude

### Example Request
```
GET /api/booking/available-staff?serviceId=1&scheduledDate=2024-02-15&scheduledTime=14:30:00&latitude=10.762622&longitude=106.660172
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "data": {
    "availableStaff": [
      {
        "id": 5,
        "firstName": "Jane",
        "lastName": "Smith",
        "phoneNumber": "+1234567891",
        "hourlyRate": 25.00,
        "rating": 4.8,
        "experienceYears": 5,
        "distanceKm": 2.5,
        "skills": ["House Cleaning", "Deep Cleaning"]
      },
      {
        "id": 7,
        "firstName": "Bob",
        "lastName": "Johnson",
        "phoneNumber": "+1234567892",
        "hourlyRate": 30.00,
        "rating": 4.9,
        "experienceYears": 8,
        "distanceKm": 3.2,
        "skills": ["House Cleaning", "Window Cleaning"]
      }
    ]
  }
}
```

---

## 15. Get All Bookings (Admin)
**GET** `/api/booking/all`

🔒 **Admin Role Required**

Get all bookings in the system with filters and pagination.

### Query Parameters
Same as `/api/booking` but without role-based filtering.

### Response (Success - 200)
Same format as `/api/booking` but includes all bookings across all customers and staff.

---

## 16. Get Staff Bookings (Staff/Admin)
**GET** `/api/booking/staff/{staffId}`

🔒 **Staff/Admin Roles** (staff can only view their own bookings)

Get bookings for a specific staff member.

### Path Parameters
- `staffId` (integer): Staff ID

### Query Parameters
Same as `/api/booking` but filtered for the specific staff member.

### Response (Success - 200)
Same format as `/api/booking` but filtered for the specific staff member.

---

## Error Responses

### Common Error Status Codes

- **400 Bad Request** - Invalid request data or validation errors
- **401 Unauthorized** - Missing or invalid authentication token
- **403 Forbidden** - User doesn't have permission for this resource
- **404 Not Found** - Booking not found
- **500 Internal Server Error** - Server error

### Error Response Format
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Error": ["Detailed error message"]
  }
}
```

---

## Booking Workflow Examples

### Example 1: Complete Customer Booking Flow

1. **Customer checks available slots:**
```bash
curl -X GET "https://your-api-domain.com/api/booking/available-slots?serviceId=1&date=2024-02-15&latitude=10.762622&longitude=106.660172" \
  -H "Authorization: Bearer {customer-token}"
```

2. **Customer creates booking:**
```bash
curl -X POST "https://your-api-domain.com/api/booking" \
  -H "Authorization: Bearer {customer-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "serviceId": 1,
    "scheduledDate": "2024-02-15T00:00:00Z",
    "scheduledTime": "14:30:00",
    "serviceAddress": "123 Main St",
    "addressLatitude": 10.762622,
    "addressLongitude": 106.660172,
    "preferredPaymentMethod": 1
  }'
```

3. **Admin assigns staff:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/123/auto-assign" \
  -H "Authorization: Bearer {admin-token}"
```

4. **Staff accepts booking:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/123/respond" \
  -H "Authorization: Bearer {staff-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "accept": true
  }'
```

5. **Staff checks in:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/123/checkin" \
  -H "Authorization: Bearer {staff-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "currentLatitude": 10.762622,
    "currentLongitude": 106.660172,
    "notes": "Arrived and ready to start"
  }'
```

6. **Staff checks out:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/123/checkout" \
  -H "Authorization: Bearer {staff-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "completionNotes": "Service completed successfully"
  }'
```

7. **Customer confirms completion:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/123/confirm" \
  -H "Authorization: Bearer {customer-token}"
```

### Example 2: Get Customer's Bookings
```bash
curl -X GET "https://your-api-domain.com/api/booking?status=1&pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {customer-token}"
```

### Example 3: Staff View Their Bookings
```bash
curl -X GET "https://your-api-domain.com/api/booking?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {staff-token}"
```

---

## Notes

- All endpoints require authentication with JWT tokens
- Role-based access control is strictly enforced
- Customers can only access their own bookings
- Staff can only access bookings assigned to them
- Admins have full access to all bookings
- Booking status transitions follow a specific workflow
- Location data (latitude/longitude) is used for staff assignment and availability
- Time slots are based on staff availability and existing bookings
- All timestamps are in UTC format
- Images uploaded during service completion are stored and accessible via URLs 