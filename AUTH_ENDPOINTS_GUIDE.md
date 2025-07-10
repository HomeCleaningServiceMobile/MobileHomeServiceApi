# Mobile Home Service API - Authentication Endpoints Guide

This guide covers all authentication endpoints available in the Mobile Home Service API.

## Base URL
```
https://your-api-domain.com/api/auth
```

## Common Headers
```
Content-Type: application/json
Authorization: Bearer {your-jwt-token} (for protected endpoints)
```

---

## 1. User Login
**POST** `/api/auth/login`

Authenticate a user and receive JWT tokens.

### Request Body
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Login successful"]
  },
  "data": {
    "user": {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "email": "user@example.com",
      "phoneNumber": "+1234567890",
      "role": "Customer",
      "status": "Active",
      "profileImageUrl": null,
      "address": "123 Main St",
      "ward": "Ward 1",
      "district": "District 1",
      "province": "Ho Chi Minh City",
      "country": "Vietnam",
      "latitude": 10.762622,
      "longitude": 106.660172,
      "dateOfBirth": "1990-01-15T00:00:00Z",
      "gender": "Male",
      "emergencyContactName": "Jane Doe",
      "emergencyContactPhone": "+1234567891",
      "lastLoginAt": "2024-01-15T10:30:00Z",
      "emailVerifiedAt": "2024-01-01T00:00:00Z",
      "phoneVerifiedAt": "2024-01-01T00:00:00Z",
      "createdAt": "2024-01-01T00:00:00Z"
    },
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2024-01-15T12:30:00Z"
  }
}
```

### Response (Error - 400)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Error": ["Invalid email or password"]
  }
}
```

---

## 2. Customer Registration
**POST** `/api/auth/register/customer`

Register a new customer account.

### Request Body
```json
{
  "fullName": "John Doe",
  "email": "customer@example.com",
  "phoneNumber": "+1234567890",
  "password": "password123",
  "dateOfBirth": "1990-01-15T00:00:00Z",
  "gender": "Male",
  "address": "123 Main St",
  "ward": "Ward 1",
  "district": "District 1",
  "province": "Ho Chi Minh City",
  "country": "Vietnam",
  "latitude": 10.762622,
  "longitude": 106.660172,
  "emergencyContactName": "Jane Doe",
  "emergencyContactPhone": "+1234567891"
}
```

### Response (Success - 201)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Customer registered successfully"]
  },
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 2,
      "firstName": "John",
      "lastName": "Doe",
      "email": "customer@example.com",
      "phoneNumber": "+1234567890",
      "role": "Customer",
      "status": "Active",
      "createdAt": "2024-01-15T10:30:00Z"
    },
    "expiresAt": "2024-01-15T12:30:00Z"
  }
}
```

---

## 3. Staff Registration
**POST** `/api/auth/register/staff`

Register a new staff account.

### Request Body
```json
{
  "fullName": "Jane Smith",
  "email": "staff@example.com",
  "phoneNumber": "+1234567890",
  "password": "password123",
  "dateOfBirth": "1985-06-20T00:00:00Z",
  "gender": "Female",
  "address": "456 Staff St",
  "ward": "Ward 2",
  "district": "District 2",
  "province": "Ho Chi Minh City",
  "emergencyContactName": "John Smith",
  "emergencyContactPhone": "+1234567892",
  "experienceYears": 5,
  "hourlyRate": 25.00,
  "bio": "Experienced home service professional",
  "certifications": ["Certified Cleaner", "Licensed Electrician"],
  "skills": ["Cleaning", "Electrical Work"],
  "availableFrom": "08:00",
  "availableTo": "18:00",
  "workingDays": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
}
```

### Response (Success - 201)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Staff registered successfully"]
  },
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 3,
      "firstName": "Jane",
      "lastName": "Smith",
      "email": "staff@example.com",
      "phoneNumber": "+1234567890",
      "role": "Staff",
      "status": "Active",
      "createdAt": "2024-01-15T10:30:00Z"
    },
    "expiresAt": "2024-01-15T12:30:00Z"
  }
}
```

---

## 4. Get User Profile
**GET** `/api/auth/profile`

🔒 **Requires Authentication**

Get the current authenticated user's profile information.

### Headers
```
Authorization: Bearer {your-jwt-token}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "data": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "user@example.com",
    "phoneNumber": "+1234567890",
    "role": "Customer",
    "status": "Active",
    "profileImageUrl": "https://example.com/profile.jpg",
    "address": "123 Main St",
    "ward": "Ward 1",
    "district": "District 1",
    "province": "Ho Chi Minh City",
    "country": "Vietnam",
    "latitude": 10.762622,
    "longitude": 106.660172,
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "gender": "Male",
    "emergencyContactName": "Jane Doe",
    "emergencyContactPhone": "+1234567891",
    "lastLoginAt": "2024-01-15T10:30:00Z",
    "emailVerifiedAt": "2024-01-01T00:00:00Z",
    "phoneVerifiedAt": "2024-01-01T00:00:00Z",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

---

## 5. Update User Profile
**PUT** `/api/auth/profile`

🔒 **Requires Authentication**

Update the current authenticated user's profile information.

### Headers
```
Authorization: Bearer {your-jwt-token}
```

### Request Body
```json
{
  "fullName": "John Updated Doe",
  "phoneNumber": "+1234567899",
  "address": "456 Updated St",
  "ward": "Ward 2",
  "district": "District 2",
  "province": "Ho Chi Minh City",
  "country": "Vietnam",
  "latitude": 10.762622,
  "longitude": 106.660172,
  "dateOfBirth": "1990-01-15T00:00:00Z",
  "gender": "Male",
  "emergencyContactName": "Jane Updated Doe",
  "emergencyContactPhone": "+1234567892"
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Profile updated successfully"]
  },
  "data": {
    "id": 1,
    "firstName": "John Updated",
    "lastName": "Doe",
    "email": "user@example.com",
    "phoneNumber": "+1234567899",
    "role": "Customer",
    "status": "Active"
  }
}
```

---

## 6. Change Password
**POST** `/api/auth/change-password`

🔒 **Requires Authentication**

Change the current authenticated user's password.

### Headers
```
Authorization: Bearer {your-jwt-token}
```

### Request Body
```json
{
  "currentPassword": "oldPassword123",
  "newPassword": "newPassword456"
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Password changed successfully"]
  }
}
```

### Response (Error - 400)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Error": ["Current password is incorrect"]
  }
}
```

---

## 7. Refresh Token
**POST** `/api/auth/refresh`

Refresh an expired JWT token using a refresh token.

### Request Body
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Token refreshed successfully"]
  },
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2024-01-15T14:30:00Z"
  }
}
```

### Response (Error - 400)
```json
{
  "isSucceeded": false,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Error": ["Invalid or expired refresh token"]
  }
}
```

---

## 8. Logout
**POST** `/api/auth/logout`

🔒 **Requires Authentication**

Logout the current authenticated user and invalidate tokens.

### Headers
```
Authorization: Bearer {your-jwt-token}
```

### Response (Success - 200)
```json
{
  "isSucceeded": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "messages": {
    "Success": ["Logged out successfully"]
  }
}
```

---

## Error Responses

### Common Error Status Codes

- **400 Bad Request** - Invalid request data or validation errors
- **401 Unauthorized** - Missing or invalid authentication token
- **404 Not Found** - User not found
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

## Usage Examples

### Example: Login and Access Protected Resource

1. **Login to get token:**
```bash
curl -X POST "https://your-api-domain.com/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'
```

2. **Use token to access profile:**
```bash
curl -X GET "https://your-api-domain.com/api/auth/profile" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Example: Register New Customer
```bash
curl -X POST "https://your-api-domain.com/api/auth/register/customer" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "John Doe",
    "email": "customer@example.com",
    "phoneNumber": "+1234567890",
    "password": "password123",
    "address": "123 Main St",
    "ward": "Ward 1",
    "district": "District 1",
    "province": "Ho Chi Minh City"
  }'
```

---

## Notes

- All timestamps are in UTC format
- Phone numbers should include country codes
- Passwords should be at least 3 characters (as per current configuration)
- Address fields support Vietnamese address format (Ward/District/Province)
- JWT tokens expire after 2 hours by default
- Refresh tokens have longer expiration times for token renewal 