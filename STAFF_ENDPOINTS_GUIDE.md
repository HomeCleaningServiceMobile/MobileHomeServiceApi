# Staff Endpoints Guide

This comprehensive guide documents all staff-related API endpoints available in the Mobile Home Service API.

## 🔐 Authentication

All staff endpoints require JWT authentication. Most endpoints require either `Staff` or `Admin` role.

**Authorization Header:**
```
Authorization: Bearer <your-jwt-token>
```

## 🌐 Base URLs

- **Staff Management**: `/api/staff`
- **Time Slots & Availability**: `/api/timeslots` 
- **Booking Operations**: `/api/booking`
- **Work Schedules**: `/api/datamanagement`

---

## 📋 Endpoints Overview

### 👤 Core Staff Profile Management
- [Get All Staff Profiles](#get-all-staff-profiles)
- [Get Staff Profile by Employee ID](#get-staff-profile-by-employee-id)
- [Update Staff Profile](#update-staff-profile)
- [Delete Staff by Employee ID](#delete-staff-by-employee-id)

### 📅 Staff Availability & Time Slots
- [Get Available Staff for Time Slot](#get-available-staff-for-time-slot)
- [Get Available Staff for Service](#get-available-staff-for-service)
- [Get Available Time Slots (with staff filter)](#get-available-time-slots)

### 📋 Booking & Assignment Operations
- [Get Staff Bookings](#get-staff-bookings)
- [Find Available Staff for Service Location](#find-available-staff-for-service-location)
- [Auto-assign Staff to Booking](#auto-assign-staff-to-booking)
- [Manual Staff Assignment](#manual-staff-assignment)
- [Staff Response to Booking](#staff-response-to-booking)
- [Staff Check-in](#staff-check-in)
- [Staff Check-out](#staff-check-out)
- [Get Directions to Booking](#get-directions-to-booking)

### 🕒 Work Schedule Management
- [Get Work Schedules for Staff](#get-work-schedules-for-staff)
- [Get All Work Schedules](#get-all-work-schedules)

### 👥 Staff Registration & Authentication
- [Staff Registration](#staff-registration)

---

## 👤 Core Staff Profile Management

### Get All Staff Profiles

Retrieves a paginated list of all staff profiles.

**Endpoint:** `GET /api/staff`

**Authorization:** `Staff, Admin`

**Query Parameters:**
- `pageNumber` (optional): Page number (default: 1, min: 1)
- `pageSize` (optional): Items per page (default: 10, min: 1, max: 100)

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Staff profiles retrieved successfully",
  "data": [
    {
      "firstName": "John",
      "lastName": "Doe",
      "profileImageUrl": "https://example.com/image.jpg",
      "dateOfBirth": "1990-01-01T00:00:00",
      "gender": "Male",
      "emergencyContactName": "Jane Doe",
      "emergencyContactPhone": "+1234567890",
      "skills": "Plumbing, Electrical",
      "bio": "Experienced technician with 5+ years in home services"
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

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/staff?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

### Get Staff Profile by Employee ID

Retrieves a specific staff profile by their employee ID.

**Endpoint:** `GET /api/staff/{employeeId}`

**Authorization:** `Staff, Admin`

**Path Parameters:**
- `employeeId` (required): The employee ID of the staff member

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Staff profile retrieved successfully",
  "data": {
    "firstName": "John",
    "lastName": "Doe",
    "profileImageUrl": "https://example.com/image.jpg",
    "dateOfBirth": "1990-01-01T00:00:00",
    "gender": "Male",
    "emergencyContactName": "Jane Doe",
    "emergencyContactPhone": "+1234567890",
    "skills": "Plumbing, Electrical",
    "bio": "Experienced technician with 5+ years in home services"
  }
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/staff/STF001" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

### Update Staff Profile

Updates a staff member's profile information.

**Endpoint:** `PUT /api/staff/{userId}`

**Authorization:** `Staff, Admin`

**Path Parameters:**
- `userId` (required): The user ID of the staff member

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "profileImageUrl": "https://example.com/new-image.jpg",
  "dateOfBirth": "1990-01-01T00:00:00",
  "gender": "Male",
  "emergencyContactName": "Jane Doe",
  "emergencyContactPhone": "+1234567890",
  "skills": "Plumbing, Electrical, HVAC",
  "bio": "Updated bio with more experience"
}
```

**Validation Rules:**
- `firstName`: Required, max 100 characters
- `lastName`: Required, max 100 characters
- `profileImageUrl`: Optional, max 255 characters
- `gender`: Optional, max 20 characters
- `emergencyContactName`: Optional, max 100 characters
- `emergencyContactPhone`: Optional, max 20 characters
- `skills`: Optional, max 500 characters
- `bio`: Optional, max 1000 characters

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Staff profile updated successfully",
  "data": "Profile updated successfully"
}
```

**Example Request:**
```bash
curl -X PUT "https://your-api-domain.com/api/staff/123" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "skills": "Plumbing, Electrical, HVAC",
    "bio": "Updated bio with more experience"
  }'
```

---

### Delete Staff by Employee ID

Deletes a staff member by their employee ID.

**Endpoint:** `DELETE /api/staff/{employeeId}`

**Authorization:** `Staff, Admin`

**Path Parameters:**
- `employeeId` (required): The employee ID of the staff member to delete

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Staff deleted successfully",
  "data": "Staff with employee ID STF001 has been deleted"
}
```

**Example Request:**
```bash
curl -X DELETE "https://your-api-domain.com/api/staff/STF001" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

## 📅 Staff Availability & Time Slots

### Get Available Staff for Time Slot

Retrieves available staff for a specific time slot on a given date.

**Endpoint:** `GET /api/timeslots/available-staff-for-slot`

**Authorization:** Public

**Query Parameters:**
- `date` (required): The date to check for staff availability (format: YYYY-MM-DD)
- `startTime` (required): Start time of the slot (format: HH:mm)
- `endTime` (required): End time of the slot (format: HH:mm)
- `serviceId` (optional): ID of the service to filter staff by

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Available staff retrieved successfully",
  "data": [
    {
      "staffId": 1,
      "staffName": "John Doe",
      "employeeId": "STF001",
      "hourlyRate": 25.00,
      "averageRating": 4.5,
      "totalCompletedJobs": 150,
      "isAvailable": true,
      "serviceId": 1,
      "serviceName": "Plumbing"
    }
  ]
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/timeslots/available-staff-for-slot?date=2024-01-15&startTime=09:00&endTime=11:00&serviceId=1"
```

---

### Get Available Staff for Service

Retrieves available staff for a specific service and time slot on a given date.

**Endpoint:** `GET /api/timeslots/service-staff`

**Authorization:** Public

**Query Parameters:**
- `date` (required): The date to check for staff availability (format: YYYY-MM-DD)
- `startTime` (required): Start time of the slot (format: HH:mm)
- `endTime` (required): End time of the slot (format: HH:mm)
- `serviceId` (required): ID of the service to filter staff by

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Available staff for service retrieved successfully",
  "data": [
    {
      "staffId": 1,
      "staffName": "John Doe",
      "employeeId": "STF001",
      "hourlyRate": 25.00,
      "averageRating": 4.5,
      "totalCompletedJobs": 150,
      "isAvailable": true,
      "serviceId": 1,
      "serviceName": "Plumbing"
    }
  ]
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/timeslots/service-staff?date=2024-01-15&startTime=09:00&endTime=11:00&serviceId=1"
```

---

### Get Available Time Slots

Retrieves available time slots with optional staff and service filtering.

**Endpoint:** `GET /api/timeslots/available-slots`

**Authorization:** Public

**Query Parameters:**
- `date` (required): The date to check for availability (format: YYYY-MM-DD)
- `serviceId` (optional): ID of the service to filter by
- `staffId` (optional): ID of the staff member to filter by

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Available time slots retrieved successfully",
  "data": [
    {
      "startTime": "09:00:00",
      "endTime": "11:00:00",
      "date": "2024-01-15T00:00:00",
      "availableStaff": [
        {
          "staffId": 1,
          "staffName": "John Doe",
          "hourlyRate": 25.00
        }
      ]
    }
  ]
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/timeslots/available-slots?date=2024-01-15&serviceId=1&staffId=1"
```

---

## 📋 Booking & Assignment Operations

### Get Staff Bookings

Retrieves bookings assigned to a specific staff member.

**Endpoint:** `GET /api/booking/staff/{staffId}`

**Authorization:** `Staff, Admin`

**Path Parameters:**
- `staffId` (required): The ID of the staff member

**Query Parameters:**
- `pageNumber` (optional): Page number for pagination
- `pageSize` (optional): Items per page
- `status` (optional): Filter by booking status
- `startDate` (optional): Filter bookings from this date
- `endDate` (optional): Filter bookings to this date

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Staff bookings retrieved successfully",
  "data": [
    {
      "id": 1,
      "bookingNumber": "BK-20240115-001",
      "customerId": 1,
      "serviceId": 1,
      "staffId": 1,
      "scheduledDate": "2024-01-15T00:00:00",
      "scheduledTime": "09:00:00",
      "status": "Confirmed",
      "totalPrice": 150.00,
      "customerAddress": "123 Main St, City",
      "customerName": "Jane Smith",
      "serviceName": "Plumbing Service"
    }
  ]
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/booking/staff/1?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

### Find Available Staff for Service Location

Finds available staff for a specific service at a given location and time.

**Endpoint:** `GET /api/booking/available-staff`

**Authorization:** `Admin, Manager`

**Query Parameters:**
- `serviceId` (required): Service ID
- `scheduledDate` (required): Scheduled date (format: YYYY-MM-DD)
- `scheduledTime` (required): Scheduled time (format: HH:mm:ss)
- `latitude` (required): Service location latitude
- `longitude` (required): Service location longitude

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Available staff found successfully",
  "data": [
    {
      "staffId": 1,
      "staffName": "John Doe",
      "employeeId": "STF001",
      "distanceKm": 5.2,
      "hourlyRate": 25.00,
      "averageRating": 4.5,
      "isAvailable": true
    }
  ]
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/booking/available-staff?serviceId=1&scheduledDate=2024-01-15&scheduledTime=09:00:00&latitude=40.7128&longitude=-74.0060" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

### Auto-assign Staff to Booking

Automatically assigns the best available staff to a booking.

**Endpoint:** `POST /api/booking/{id}/auto-assign`

**Authorization:** `Admin, Manager`

**Path Parameters:**
- `id` (required): Booking ID

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Staff auto-assigned successfully",
  "data": {
    "bookingId": 1,
    "staffId": 1,
    "staffName": "John Doe",
    "assignedAt": "2024-01-15T10:30:00"
  }
}
```

**Example Request:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/1/auto-assign" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

### Manual Staff Assignment

Manually assigns a specific staff member to a booking.

**Endpoint:** `POST /api/booking/{id}/assign`

**Authorization:** `Admin`

**Path Parameters:**
- `id` (required): Booking ID

**Request Body:**
```json
{
  "staffId": 1
}
```

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Staff assigned successfully",
  "data": {
    "bookingId": 1,
    "staffId": 1,
    "staffName": "John Doe",
    "assignedAt": "2024-01-15T10:30:00"
  }
}
```

**Example Request:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/1/assign" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{"staffId": 1}'
```

---

### Staff Response to Booking

Allows staff to accept or decline a booking assignment.

**Endpoint:** `POST /api/booking/respond`

**Authorization:** `Staff, Admin`

**Request Body:**
```json
{
  "bookingId": 1,
  "isAccepted": true,
  "notes": "I can handle this booking on time"
}
```

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Booking response recorded successfully",
  "data": {
    "bookingId": 1,
    "staffResponse": "Accepted",
    "respondedAt": "2024-01-15T10:30:00"
  }
}
```

**Example Request:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/respond" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "bookingId": 1,
    "isAccepted": true,
    "notes": "I can handle this booking on time"
  }'
```

---

### Staff Check-in

Staff checks in when arriving at the service location.

**Endpoint:** `POST /api/booking/{id}/checkin`

**Authorization:** `Staff`

**Path Parameters:**
- `id` (required): Booking ID

**Request Body:**
```json
{
  "latitude": 40.7128,
  "longitude": -74.0060,
  "notes": "Arrived at location",
  "photos": ["https://example.com/checkin-photo.jpg"]
}
```

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Check-in successful",
  "data": {
    "bookingId": 1,
    "checkedInAt": "2024-01-15T09:00:00",
    "location": "40.7128, -74.0060"
  }
}
```

**Example Request:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/1/checkin" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "latitude": 40.7128,
    "longitude": -74.0060,
    "notes": "Arrived at location"
  }'
```

---

### Staff Check-out

Staff checks out when completing the service.

**Endpoint:** `POST /api/booking/{id}/checkout`

**Authorization:** `Staff`

**Path Parameters:**
- `id` (required): Booking ID

**Request Body:**
```json
{
  "serviceCompletedAt": "2024-01-15T11:00:00",
  "workDescription": "Service completed successfully",
  "beforePhotos": ["https://example.com/before.jpg"],
  "afterPhotos": ["https://example.com/after.jpg"],
  "additionalNotes": "Customer satisfied"
}
```

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Check-out successful",
  "data": {
    "bookingId": 1,
    "checkedOutAt": "2024-01-15T11:00:00",
    "totalDuration": "02:00:00",
    "serviceCompleted": true
  }
}
```

**Example Request:**
```bash
curl -X POST "https://your-api-domain.com/api/booking/1/checkout" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "serviceCompletedAt": "2024-01-15T11:00:00",
    "workDescription": "Service completed successfully",
    "additionalNotes": "Customer satisfied"
  }'
```

---

### Get Directions to Booking

Provides directions from staff location to booking location.

**Endpoint:** `GET /api/booking/{id}/directions`

**Authorization:** `Staff`

**Path Parameters:**
- `id` (required): Booking ID

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Directions retrieved successfully",
  "data": {
    "bookingId": 1,
    "customerAddress": "123 Main St, City, State",
    "distanceKm": 5.2,
    "estimatedTravelTime": "00:15:00",
    "directionsUrl": "https://maps.google.com/directions?..."
  }
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/booking/1/directions" \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

## 🕒 Work Schedule Management

### Get Work Schedules for Staff

Retrieves work schedules for a specific staff member.

**Endpoint:** `GET /api/datamanagement/work-schedules/staff/{staffId}`

**Authorization:** Public

**Path Parameters:**
- `staffId` (required): The ID of the staff member

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Work schedules retrieved successfully",
  "data": [
    {
      "id": 1,
      "staffId": 1,
      "dayOfWeek": 1,
      "startTime": "08:00:00",
      "endTime": "17:00:00",
      "isActive": true,
      "staff": {
        "id": 1,
        "userId": 123,
        "employeeId": "STF001",
        "firstName": "John",
        "lastName": "Doe"
      }
    }
  ]
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/datamanagement/work-schedules/staff/1"
```

---

### Get All Work Schedules

Retrieves work schedules for all staff members.

**Endpoint:** `GET /api/datamanagement/work-schedules`

**Authorization:** Public

**Response:**
```json
{
  "isSucceeded": true,
  "message": "All work schedules retrieved successfully",
  "data": [
    {
      "id": 1,
      "staffId": 1,
      "dayOfWeek": 1,
      "startTime": "08:00:00",
      "endTime": "17:00:00",
      "isActive": true,
      "staff": {
        "id": 1,
        "userId": 123,
        "employeeId": "STF001",
        "firstName": "John",
        "lastName": "Doe"
      }
    }
  ]
}
```

**Example Request:**
```bash
curl -X GET "https://your-api-domain.com/api/datamanagement/work-schedules"
```

---

## 👥 Staff Registration & Authentication

### Staff Registration

Registers a new staff member in the system.

**Endpoint:** `POST /api/auth/register/staff`

**Authorization:** Public

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "password": "SecurePassword123!",
  "employeeId": "STF001",
  "skills": "Plumbing, Electrical",
  "bio": "Experienced technician with 5+ years experience",
  "hourlyRate": 25.00,
  "serviceRadiusKm": 50
}
```

**Response:**
```json
{
  "isSucceeded": true,
  "message": "Staff registration successful",
  "data": {
    "userId": 123,
    "staffId": 1,
    "employeeId": "STF001",
    "email": "john.doe@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**Example Request:**
```bash
curl -X POST "https://your-api-domain.com/api/auth/register/staff" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890",
    "password": "SecurePassword123!",
    "employeeId": "STF001"
  }'
```

---

## 📊 Data Models

### StaffResponse
```json
{
  "id": 1,
  "userId": 123,
  "employeeId": "STF001",
  "hireDate": "2023-01-15T00:00:00",
  "skills": "Plumbing, Electrical",
  "bio": "Experienced technician",
  "hourlyRate": 25.00,
  "averageRating": 4.5,
  "totalCompletedJobs": 150,
  "isAvailable": true,
  "certificationImageUrl": "https://example.com/cert.jpg",
  "idCardImageUrl": "https://example.com/id.jpg",
  "lastActiveAt": "2024-01-15T10:30:00",
  "serviceRadiusKm": 50,
  "currentLatitude": 40.7128,
  "currentLongitude": -74.0060,
  "createdAt": "2023-01-15T00:00:00",
  "user": {
    "id": 123,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890"
  },
  "staffSkills": [
    {
      "id": 1,
      "staffId": 1,
      "serviceId": 1,
      "skillLevel": 5,
      "isActive": true,
      "certifiedAt": "2023-02-01T00:00:00",
      "certificationUrl": "https://example.com/cert.pdf",
      "notes": "Expert level",
      "service": {
        "id": 1,
        "name": "Plumbing",
        "description": "Plumbing services"
      }
    }
  ]
}
```

### StaffAvailabilityDto
```json
{
  "staffId": 1,
  "staffName": "John Doe",
  "employeeId": "STF001",
  "hourlyRate": 25.00,
  "averageRating": 4.5,
  "totalCompletedJobs": 150,
  "isAvailable": true,
  "serviceId": 1,
  "serviceName": "Plumbing",
  "distanceKm": 5.2
}
```

### UpdateStaffProfileRequest
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "profileImageUrl": "https://example.com/image.jpg",
  "dateOfBirth": "1990-01-01T00:00:00",
  "gender": "Male",
  "emergencyContactName": "Jane Doe",
  "emergencyContactPhone": "+1234567890",
  "skills": "Plumbing, Electrical, HVAC",
  "bio": "Experienced technician with 5+ years in home services"
}
```

### WorkScheduleResponse
```json
{
  "id": 1,
  "staffId": 1,
  "dayOfWeek": 1,
  "startTime": "08:00:00",
  "endTime": "17:00:00",
  "isActive": true,
  "staff": {
    "id": 1,
    "userId": 123,
    "employeeId": "STF001",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

### CheckInRequest
```json
{
  "bookingId": 1,
  "latitude": 40.7128,
  "longitude": -74.0060,
  "notes": "Arrived at location",
  "photos": ["https://example.com/photo.jpg"]
}
```

### CheckOutRequest
```json
{
  "bookingId": 1,
  "serviceCompletedAt": "2024-01-15T11:00:00",
  "workDescription": "Service completed successfully",
  "beforePhotos": ["https://example.com/before.jpg"],
  "afterPhotos": ["https://example.com/after.jpg"],
  "additionalNotes": "Customer satisfied"
}
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

### Common HTTP Status Codes
- `200 OK`: Request successful
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid request data or validation errors
- `401 Unauthorized`: Missing or invalid authentication token
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

### Common Error Scenarios

#### Authentication Errors
```json
{
  "isSucceeded": false,
  "message": "Invalid authentication token",
  "data": null
}
```

#### Permission Errors
```json
{
  "isSucceeded": false,
  "message": "You can only view your own bookings",
  "data": null
}
```

#### Validation Errors
```json
{
  "isSucceeded": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    {
      "field": "startTime",
      "message": "Start time must be earlier than end time"
    }
  ]
}
```

---

## 🚦 Rate Limiting

All endpoints are subject to rate limiting:
- **General endpoints**: 100 requests per minute per IP
- **Authentication endpoints**: 5 requests per minute per IP
- **Staff profile updates**: 10 requests per minute per authenticated user

Please implement appropriate retry logic with exponential backoff in your applications.

---

## 📝 Best Practices

### 1. Authentication
- Always include the JWT token in the Authorization header
- Handle token expiration gracefully
- Refresh tokens before they expire

### 2. Error Handling
- Check the `isSucceeded` field before processing data
- Display user-friendly error messages
- Log detailed error information for debugging

### 3. Pagination
- Use pagination for list endpoints to improve performance
- Cache results when appropriate
- Implement infinite scrolling for better UX

### 4. Real-time Updates
- Consider implementing WebSocket connections for real-time booking updates
- Refresh data periodically for critical information
- Use optimistic updates for better user experience

---

## 📞 Support

For additional support or questions about the staff endpoints:
- **API Documentation**: [Main API Documentation]
- **Development Team**: Contact the backend development team
- **Bug Reports**: Submit issues through the project management system
- **Feature Requests**: Discuss with the product team

---

**Last Updated:** January 2024  
**API Version:** v1.0 