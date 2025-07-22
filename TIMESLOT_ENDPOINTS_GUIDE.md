# TimeSlot Service API Endpoints Guide

## Overview
The TimeSlot service provides endpoints for managing and retrieving available time slots for bookings, staff availability, and scheduling information.

## Base URL
```
https://your-api-domain.com/api/timeslot
```

## Authentication
All endpoints require authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Response Format
All endpoints return responses in the following format:
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {
    "Info": ["Success message"]
  },
  "data": {
    // Response data here
  },
  "pagination": null
}
```

---

## Endpoints

### 1. Get Available Time Slots
Retrieves available time slots for a specific date.

**Endpoint:** `GET /api/timeslot/available-slots`

**Query Parameters:**
- `date` (required): Date in YYYY-MM-DD format
- `serviceId` (optional): ID of the specific service to filter by
- `staffId` (optional): ID of the specific staff member to filter by

**Example Request:**
```
GET /api/timeslot/available-slots?date=2024-01-20&serviceId=1
```

**Example Response:**
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {},
  "data": [
    {
      "startTime": "09:00:00",
      "endTime": "10:00:00",
      "displayTime": "09:00",
      "date": "2024-01-20T00:00:00.000Z",
      "isAvailable": true,
      "availableStaff": [
        {
          "staffId": 1,
          "staffName": "John Doe",
          "position": "Cleaner"
        },
        {
          "staffId": 2,
          "staffName": "Jane Smith",
          "position": "Cleaner"
        }
      ]
    },
    {
      "startTime": "10:00:00",
      "endTime": "11:00:00",
      "displayTime": "10:00",
      "date": "2024-01-20T00:00:00.000Z",
      "isAvailable": true,
      "availableStaff": [
        {
          "staffId": 1,
          "staffName": "John Doe",
          "position": "Cleaner"
        }
      ]
    }
  ]
}
```

**Error Response (Past Date):**
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {
    "Validation": ["Cannot retrieve time slots for a past date."]
  },
  "data": null
}
```

**Error Response (Business Closed):**
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {
    "Info": ["Business is closed on this day"]
  },
  "data": []
}
```

---

### 2. Get Available Staff for Time Slot
Retrieves available staff members for a specific time slot on a given date.

**Endpoint:** `GET /api/timeslot/available-staff-for-slot`

**Query Parameters:**
- `date` (required): Date in YYYY-MM-DD format
- `startTime` (required): Start time in HH:mm format (e.g., "09:00")
- `endTime` (required): End time in HH:mm format (e.g., "10:00")
- `serviceId` (optional): ID of the specific service to filter by

**Example Request:**
```
GET /api/timeslot/available-staff-for-slot?date=2024-01-20&startTime=09:00&endTime=10:00&serviceId=1
```

**Example Response:**
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {},
  "data": [
    {
      "staffId": 1,
      "staffName": "John Doe",
      "position": "Cleaner"
    },
    {
      "staffId": 2,
      "staffName": "Jane Smith",
      "position": "Cleaner"
    }
  ]
}
```

**Error Response (Invalid Time Format):**
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {
    "Validation": ["Invalid start time format. Use HH:mm (e.g., 09:00)."]
  },
  "data": null
}
```

**Error Response (Invalid Time Range):**
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {
    "Validation": ["Start time must be earlier than end time."]
  },
  "data": null
}
```

---

### 3. Get Available Slots for Date Range
Retrieves available time slots for a range of dates.

**Endpoint:** `GET /api/timeslot/available-slots-range`

**Query Parameters:**
- `startDate` (required): Start date in YYYY-MM-DD format
- `endDate` (required): End date in YYYY-MM-DD format
- `serviceId` (optional): ID of the specific service to filter by
- `staffId` (optional): ID of the specific staff member to filter by

**Example Request:**
```
GET /api/timeslot/available-slots-range?startDate=2024-01-20&endDate=2024-01-25&serviceId=1
```

**Example Response:**
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {},
  "data": {
    "2024-01-20T00:00:00.000Z": [
      {
        "startTime": "09:00:00",
        "endTime": "10:00:00",
        "displayTime": "09:00",
        "date": "2024-01-20T00:00:00.000Z",
        "isAvailable": true,
        "availableStaff": [
          {
            "staffId": 1,
            "staffName": "John Doe",
            "position": "Cleaner"
          }
        ]
      }
    ],
    "2024-01-21T00:00:00.000Z": [
      {
        "startTime": "10:00:00",
        "endTime": "11:00:00",
        "displayTime": "10:00",
        "date": "2024-01-21T00:00:00.000Z",
        "isAvailable": true,
        "availableStaff": [
          {
            "staffId": 2,
            "staffName": "Jane Smith",
            "position": "Cleaner"
          }
        ]
      }
    ]
  }
}
```

**Error Response (Invalid Date Range):**
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {
    "Validation": ["End date must be after start date."]
  },
  "data": null
}
```

---

### 4. Get Next Available Slot
Retrieves the next available time slot within the next 30 days.

**Endpoint:** `GET /api/timeslot/next-available-slot`

**Query Parameters:**
- `serviceId` (optional): ID of the specific service to filter by
- `staffId` (optional): ID of the specific staff member to filter by

**Example Request:**
```
GET /api/timeslot/next-available-slot?serviceId=1
```

**Example Response:**
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {},
  "data": {
    "startTime": "14:00:00",
    "endTime": "15:00:00",
    "displayTime": "14:00",
    "date": "2024-01-16T00:00:00.000Z",
    "isAvailable": true,
    "availableStaff": [
      {
        "staffId": 1,
        "staffName": "John Doe",
        "position": "Cleaner"
      }
    ]
  }
}
```

**Error Response (No Available Slots):**
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00.000Z",
  "messages": {
    "Info": ["No available slots found in the next 30 days"]
  },
  "data": null
}
```

---

## Data Models

### TimeSlotDto
```json
{
  "startTime": "09:00:00",
  "endTime": "10:00:00",
  "displayTime": "09:00",
  "date": "2024-01-20T00:00:00.000Z",
  "isAvailable": true,
  "availableStaff": [
    {
      "staffId": 1,
      "staffName": "John Doe",
      "position": "Cleaner"
    }
  ]
}
```

### StaffAvailabilityDto
```json
{
  "staffId": 1,
  "staffName": "John Doe",
  "position": "Cleaner"
}
```

---

## Business Logic

### Time Slot Generation
- Time slots are generated based on business hours for each day
- Default slot duration is 60 minutes (configurable per service)
- Slots are created at 30-minute intervals
- Past time slots are automatically filtered out

### Staff Availability
- Staff availability is checked against their work schedules
- Conflicts with existing bookings are considered
- Staff must be available for the entire duration of the slot
- Service-specific skills are validated (if applicable)

### Business Hours
- Business hours are checked for each day
- Days marked as "closed" return no available slots
- Time slots are only generated within business hours

---

## Error Codes and Messages

### Validation Errors
- **Past Date**: "Cannot retrieve time slots for a past date."
- **Invalid Time Format**: "Invalid start time format. Use HH:mm (e.g., 09:00)."
- **Invalid Time Range**: "Start time must be earlier than end time."
- **Invalid Date Range**: "End date must be after start date."

### Business Logic Messages
- **Business Closed**: "Business is closed on this day"
- **No Staff Available**: "No staff available on this day"
- **No Slots Available**: "No available slots found in the next 30 days"

### System Errors
- **Service Error**: "Failed to get available time slots: [error message]"
- **Staff Error**: "Failed to get staff availability: [error message]"

---

## Usage Examples

### Frontend Integration Example (JavaScript)
```javascript
// Get available slots for today
async function getAvailableSlots(date, serviceId) {
  try {
    const response = await fetch(`/api/timeslot/available-slots?date=${date}&serviceId=${serviceId}`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const result = await response.json();
    
    if (result.isSucceeded) {
      return result.data;
    } else {
      console.error('Error:', result.messages);
      return [];
    }
  } catch (error) {
    console.error('Network error:', error);
    return [];
  }
}

// Get next available slot
async function getNextAvailableSlot(serviceId) {
  try {
    const response = await fetch(`/api/timeslot/next-available-slot?serviceId=${serviceId}`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const result = await response.json();
    
    if (result.isSucceeded) {
      return result.data;
    } else {
      console.error('Error:', result.messages);
      return null;
    }
  } catch (error) {
    console.error('Network error:', error);
    return null;
  }
}
```

### Mobile App Integration Example (C#)
```csharp
public class TimeSlotService
{
    private readonly HttpClient _httpClient;
    
    public TimeSlotService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<TimeSlotDto>> GetAvailableSlotsAsync(DateTime date, int? serviceId = null)
    {
        var queryParams = new List<string>
        {
            $"date={date:yyyy-MM-dd}"
        };
        
        if (serviceId.HasValue)
            queryParams.Add($"serviceId={serviceId}");
            
        var url = $"/api/timeslot/available-slots?{string.Join("&", queryParams)}";
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AppResponse<List<TimeSlotDto>>>();
            return result?.IsSucceeded == true ? result.Data : new List<TimeSlotDto>();
        }
        
        return new List<TimeSlotDto>();
    }
}
```

---

## Rate Limiting
- Standard rate limiting applies to all endpoints
- Recommended: Maximum 100 requests per minute per user

## Caching
- Time slot data can be cached for up to 5 minutes
- Staff availability data should not be cached for more than 1 minute
- Business hours data can be cached for up to 24 hours

---

## Version History
- **v1.0**: Initial release with basic time slot functionality
- **v1.1**: Added date range and next available slot endpoints
- **v1.2**: Improved error handling with AppResponse format 