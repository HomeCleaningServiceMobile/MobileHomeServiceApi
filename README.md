# Mobile Home Service API

A comprehensive backend system for a home service company, built with .NET 8. This API powers mobile and web applications for booking, managing, and executing home services with employed staff.

---

## 🎯 Project Overview

**Purpose:**  
The Mobile Home Service API is designed for a home service company to manage operations, staff, bookings, and payments. It serves as the backend for mobile and web applications.

**Main Objectives:**
- Service Management: Customers book cleaning, cooking, maintenance, and more.
- Staff Management: Company staff schedules, assignments, and profiles.
- Business Operations: Admin tools for operations, payments, and analytics.
- Payment Processing: Handle customer payments directly to the company.
- Quality Assurance: Ratings and reviews for service quality.

**Service Types Available:**
- House Cleaning
- Cooking
- Laundry & Ironing
- Gardening
- Babysitting
- Elder Care
- Pet Care
- General Maintenance

---

## 🏗️ System Architecture

Layered architecture:
- **API Layer:** Controllers, authentication, documentation.
- **Service Layer:** Business logic, DTOs, service implementations.
- **Repository Layer:** Data access, models, database context.
- **Common Layer:** Shared enums, constants, utilities.

---

## 🔑 Authentication

- JWT-based authentication for all protected endpoints.
- Role-based access: Customer, Staff, Admin, Manager.
- Refresh token mechanism for session renewal.

---

## 📋 Core Features

- User Registration & Login: Customer, Staff, Admin flows.
- Service Discovery & Booking: Browse, filter, and book services/packages.
- Staff Assignment: Auto/manual assignment based on skills, location, and availability.
- Booking Management: Real-time status, notifications, check-in/out, and history.
- Payment Processing: Cash, card, bank transfer, e-wallet, QR code.
- Review & Rating: Customer feedback and staff performance tracking.

---

## 🛠️ Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Configuration

1. Update `appsettings.json` with your database connection string.
2. Configure JWT settings.
3. Set up email/SMS providers for notifications.

### API Documentation

Access Swagger UI at: `https://localhost:5001/swagger`

---

## 📚 API Endpoint Guides

- [Authentication Endpoints](AUTH_ENDPOINTS_GUIDE.md)
- [Staff Endpoints](STAFF_ENDPOINTS_GUIDE.md)
- [Project Flows & Architecture](PROJECT_FLOWS.md)

---

## ⚠️ Error Handling & Rate Limiting

- Standardized error responses with status codes and messages.
- Rate limits: 100 requests/min (general), 5 requests/min (auth), 10 requests/min (profile updates).

---

## 📝 Best Practices

- Always include JWT in the Authorization header.
- Handle token expiration and refresh gracefully.
- Use pagination for list endpoints.
- Implement retry logic for rate-limited endpoints.

---

## 🗄️ Database Schema Overview

**Core Entities:**
- Users (Identity): Customers, Staff, Admins
- Services, ServicePackages, StaffSkills
- Bookings, BookingImages, Payments, Reviews, StaffReports
- Notifications, WorkSchedules, CustomerAddresses, CustomerPaymentMethods

**Key Relationships:**
- One-to-One: User ↔ Customer/Staff/Admin
- One-to-Many: Service → ServicePackages
- Many-to-Many: Staff ↔ Services (via StaffSkills)
- One-to-Many: Booking → BookingImages
- One-to-One: Booking ↔ Payment
- One-to-One: Booking ↔ Review

---

## 📞 Support

For API documentation, bug reports, or feature requests, contact the backend development team or use the project management system.

---

## License

This project is proprietary and intended for internal use by the home service company.

---

**Last Updated:** September 2025  
**API Version:** v1.0
