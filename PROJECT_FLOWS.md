# Mobile Home Service API - Project Flows Documentation

## 📋 Table of Contents
1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Core Business Flows](#core-business-flows)
4. [User Journey Flows](#user-journey-flows)
5. [API Endpoints Overview](#api-endpoints-overview)
6. [Business Rules & Status Lifecycle](#business-rules--status-lifecycle)
7. [Technical Features](#technical-features)
8. [Database Schema Overview](#database-schema-overview)

---

## 🎯 Project Overview

### **Purpose**
The Mobile Home Service API is a comprehensive backend system designed to facilitate on-demand home services. It serves as the backend for a mobile application that connects customers needing home services with qualified service providers.

### **Main Objectives**
- **Service Marketplace**: Platform for customers to book various home services
- **Staff Management**: System for service providers to manage their work and availability
- **Business Operations**: Administrative tools for managing the entire service ecosystem
- **Payment Processing**: Handle transactions and financial operations
- **Quality Assurance**: Review and rating system for service quality

### **Service Types Available**
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

### **Layered Architecture**
```
┌─────────────────────────────────────────┐
│           API Layer                     │
│  (MobileHomeServiceApi)                 │
│  - Controllers                          │
│  - Authentication                       │
│  - API Documentation                    │
└─────────────────────────────────────────┘
                    │
┌─────────────────────────────────────────┐
│         Service Layer                   │
│  (MHS.Service)                          │
│  - Business Logic                       │
│  - DTOs                                 │
│  - Service Implementations              │
└─────────────────────────────────────────┘
                    │
┌─────────────────────────────────────────┐
│       Repository Layer                  │
│  (MHS.Repository)                       │
│  - Data Access                          │
│  - Models                               │
│  - Database Context                     │
└─────────────────────────────────────────┘
                    │
┌─────────────────────────────────────────┐
│        Common Layer                     │
│  (MHS.Common)                           │
│  - Shared Enums                         │
│  - Constants                            │
│  - Utilities                            │
└─────────────────────────────────────────┘
```

---

## 🔄 Core Business Flows

### **1. User Registration & Authentication Flow**
```mermaid
graph TD
    A[User Registration] --> B[Role Selection]
    B --> C{Customer/Staff/Admin}
    C -->|Customer| D[Customer Profile Setup]
    C -->|Staff| E[Staff Profile Setup]
    C -->|Admin| F[Admin Profile Setup]
    D --> G[Email/Phone Verification]
    E --> G
    F --> G
    G --> H[Account Activation]
    H --> I[Login Process]
    I --> J[JWT Token Generation]
    J --> K[Authentication Complete]
```

### **2. Service Management Flow**
```mermaid
graph TD
    A[Admin Creates Service] --> B[Define Service Details]
    B --> C[Set Service Type]
    C --> D[Configure Pricing]
    D --> E[Set Duration & Requirements]
    E --> F[Create Service Packages]
    F --> G[Set Availability]
    G --> H[Activate Service]
    H --> I[Service Available for Booking]
```

**Detailed Steps:**
1. **Service Creation**: Admin accesses service management panel
2. **Basic Information**:
   - Enter service name and description
   - Select service type (HouseCleaning, Cooking, etc.)
   - Upload service image/icon
3. **Pricing Configuration**:
   - Set base price for the service
   - Configure hourly rate (if applicable)
   - Define estimated duration in minutes
4. **Requirements & Restrictions**:
   - List equipment/materials needed
   - Specify any restrictions or limitations
   - Set special requirements for customers
5. **Service Package Creation**:
   - Create Basic package with standard features
   - Create Premium package with additional services
   - Set different pricing and duration for each package
6. **Availability Settings**:
   - Define service areas/regions
   - Set operational hours
   - Configure staff requirements
7. **Activation**: 
   - Review all configurations
   - Activate service for customer booking
   - Service appears in mobile app catalog

### **3. Customer Booking Flow**
```mermaid
graph TD
    A[Customer Browse Services] --> B[Select Service]
    B --> C[Choose Service Package]
    C --> D[Select Date & Time]
    D --> E[Provide Service Address]
    E --> F[Add GPS Coordinates]
    F --> G[Special Instructions]
    G --> H[Create Booking]
    H --> I[Calculate Total Amount]
    I --> J[Generate Booking Number]
    J --> K[Save Booking]
    K --> L[Trigger Auto-Assignment]
    L --> M[Booking Created Successfully]
```

### **4. Staff Assignment Flow**
```mermaid
graph TD
    A[New Booking Created] --> B[Auto-Assignment System]
    B --> C[Find Available Staff]
    C --> D{Staff Available?}
    D -->|Yes| E[Send Assignment Notification]
    D -->|No| F[Manual Assignment Required]
    E --> G[Staff Response Required]
    G --> H{Staff Response}
    H -->|Accept| I[Booking Confirmed]
    H -->|Decline| J[Find Next Available Staff]
    J --> C
    F --> K[Admin Manual Assignment]
    K --> I
    I --> L[Customer Notification]
```

**Detailed Steps:**
1. **Trigger Auto-Assignment**: System activates when booking status = Pending
2. **Staff Filtering Criteria**:
   - Match required service skills
   - Check availability on booking date/time
   - Verify geographic proximity to service location
   - Consider staff rating and experience level
3. **Priority Algorithm**:
   - Distance from service location (closest first)
   - Staff rating and customer reviews
   - Number of completed jobs
   - Last assignment time (load balancing)
4. **Notification Process**:
   - Send push notification to selected staff
   - Include booking details and location
   - Set response deadline (typically 15-30 minutes)
   - Update booking status to "AutoAssigned"
5. **Staff Response Handling**:
   - **Accept**: Update booking to "Confirmed", notify customer
   - **Decline**: Log reason, find next available staff
   - **No Response**: Auto-decline after deadline, find next staff
6. **Fallback to Manual Assignment**:
   - If no staff available, notify admin
   - Admin can manually assign or reschedule
   - System sends manual assignment notification
7. **Confirmation**:
   - Update booking status and timestamps
   - Send confirmation to customer with staff details
   - Add booking to staff's schedule

### **5. Service Execution Flow**
```mermaid
graph TD
    A[Staff Assigned] --> B[Staff Check-In]
    B --> C[Service Execution]
    C --> D[Upload Progress Images]
    D --> E[Staff Check-Out]
    E --> F[Service Completion]
    F --> G[Customer Confirmation]
    G --> H[Payment Processing]
    H --> I[Review & Rating]
    I --> J[Booking Closure]
```

**Detailed Steps:**
1. **Pre-Service Preparation**:
   - Staff receives confirmed booking notification
   - Review service requirements and customer instructions
   - Prepare necessary equipment and materials
   - Travel to customer location
2. **Check-In Process**:
   - Staff arrives at service location
   - Verify GPS location matches booking address
   - Take "arrival" photo for documentation
   - Update booking status to "InProgress"
   - Record actual start time
3. **Service Execution**:
   - Review service requirements with customer
   - Clarify any special instructions
   - Begin service work according to package specifications
   - Follow safety protocols and quality standards
4. **Progress Documentation**:
   - Take before/during/after photos
   - Upload images to booking record
   - Add notes about work performed
   - Document any issues or changes
5. **Service Completion**:
   - Complete all tasks in service package
   - Clean up work area
   - Review completed work with customer
   - Address any customer concerns
6. **Check-Out Process**:
   - Take final completion photos
   - Record actual end time
   - Get customer approval/signature (if required)
   - Update booking status to "Completed"
7. **Post-Service**:
   - System calculates final amount
   - Generate service completion report
   - Trigger payment processing
   - Send completion notification to customer

### **6. Payment & Completion Flow**
```mermaid
graph TD
    A[Service Completed] --> B[Calculate Final Amount]
    B --> C[Payment Processing]
    C --> D{Payment Success?}
    D -->|Yes| E[Payment Confirmation]
    D -->|No| F[Payment Retry/Manual]
    E --> G[Generate Invoice]
    F --> C
    G --> H[Customer Review]
    H --> I[Staff Rating]
    I --> J[Booking Closure]
```

**Detailed Steps:**
1. **Amount Calculation**:
   - Start with base service package price
   - Add any extra time charges (if service exceeded estimated duration)
   - Apply any additional service fees
   - Calculate platform commission
   - Apply discounts or promotions (if applicable)
2. **Payment Method Selection**:
   - Present customer with saved payment methods
   - Allow selection of new payment method
   - Support: Cash, Card, Bank Transfer, E-Wallet, QR Code
3. **Payment Processing**:
   - **Cash**: Staff collects payment, marks as paid
   - **Electronic**: Process through payment gateway
   - Generate unique transaction ID
   - Update payment status in real-time
4. **Payment Verification**:
   - **Success**: Confirm payment received
   - **Failure**: Retry mechanism with different method
   - **Pending**: Monitor gateway response
5. **Invoice Generation**:
   - Create detailed invoice with service breakdown
   - Include staff details and service completion photos
   - Generate PDF invoice for customer records
   - Send via email and in-app notification
6. **Review & Rating Process**:
   - Prompt customer to rate service (1-5 stars)
   - Collect detailed ratings (Quality, Timeliness, Professionalism, Communication)
   - Allow written feedback and comments
   - Optional: Upload photos of completed work
7. **Staff Compensation**:
   - Calculate staff payment (service fee - platform commission)
   - Update staff earnings and completed job count
   - Process payment to staff account
8. **Booking Finalization**:
   - Archive booking as completed
   - Update customer loyalty points
   - Send completion summary to all parties
   - Generate analytics data for reporting

---

## 👥 User Journey Flows

### **Customer Journey**
```mermaid
graph TD
    A[Registration] --> B[Browse Services]
    B --> C[Service Selection]
    C --> D[Booking Creation]
    D --> E[Staff Assignment Notification]
    E --> F[Service Tracking]
    F --> G[Service Execution]
    G --> H[Payment Processing]
    H --> I[Review & Rating]
    I --> J[Booking History]
```

**Detailed Steps:**
1. **Registration**: Create account with email/phone verification
2. **Service Discovery**: Browse available services by type and location
3. **Booking Process**: 
   - Select service and optional packages
   - Choose date/time slots
   - Provide detailed address with GPS coordinates
   - Add special instructions
4. **Staff Assignment**: System auto-assigns qualified staff
5. **Service Tracking**: Monitor booking status in real-time
6. **Payment**: Process payment after service completion
7. **Feedback**: Rate and review the service provider

### **Staff Journey**
```mermaid
graph TD
    A[Registration & Verification] --> B[Profile Setup]
    B --> C[Skill Certification]
    C --> D[Schedule Management]
    D --> E[Receive Booking Notifications]
    E --> F[Accept/Decline Bookings]
    F --> G[Service Execution]
    G --> H[Check-In/Check-Out]
    H --> I[Service Documentation]
    I --> J[Payment Receipt]
```

**Detailed Steps:**
1. **Onboarding**: Registration with skill verification
2. **Profile Setup**: Add services they can provide
3. **Schedule Management**: Set availability and work hours
4. **Booking Management**: 
   - Receive auto-assignment notifications
   - Accept/decline booking requests
   - View upcoming schedules
5. **Service Execution**:
   - Check-in at customer location
   - Perform required services
   - Document work with photos
   - Check-out upon completion
6. **Payment**: Receive compensation for completed services

### **Admin Journey**
```mermaid
graph TD
    A[System Management] --> B[Service Management]
    B --> C[Staff Management]
    C --> D[Booking Oversight]
    D --> E[Manual Assignments]
    E --> F[Payment Management]
    F --> G[Analytics & Reports]
    G --> H[System Monitoring]
```

**Detailed Steps:**
1. **Service Management**: Create/update/deactivate services
2. **Staff Management**: Onboard staff, verify skills, monitor performance
3. **Booking Oversight**: Monitor all bookings, resolve issues
4. **Manual Assignment**: Assign staff when auto-assignment fails
5. **Payment Management**: Handle payments, refunds, disputes
6. **System Analytics**: Generate reports and monitor KPIs

---

## 🛠️ API Endpoints Overview

### **Authentication Endpoints**
- `POST /api/users/login` - User login
- `POST /api/users/logout` - User logout
- `POST /api/users/refresh-token` - Refresh JWT token
- `POST /api/users/send-email-verification` - Send email verification
- `POST /api/users/verify-otp` - Verify OTP code

### **User Management Endpoints**
- `POST /api/users` - Create new user
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user profile
- `DELETE /api/users/{id}` - Delete user
- `POST /api/users/change-password` - Change password
- `POST /api/users/forgot-password` - Forgot password
- `POST /api/users/reset-password` - Reset password

### **Service Management Endpoints**
- `GET /api/services` - Get all services (with filtering)
- `GET /api/services/{id}` - Get service by ID
- `POST /api/services` - Create new service (Admin)
- `PUT /api/services/{id}` - Update service (Admin)
- `DELETE /api/services/{id}` - Delete service (Admin)
- `GET /api/services/by-type/{type}` - Get services by type
- `GET /api/services/popular` - Get popular services
- `POST /api/services/{id}/calculate-price` - Calculate service price

### **Booking Management Endpoints**
- `POST /api/bookings` - Create new booking
- `GET /api/bookings/{id}` - Get booking by ID
- `GET /api/bookings` - Get bookings (with filtering)
- `PUT /api/bookings/{id}` - Update booking
- `POST /api/bookings/{id}/cancel` - Cancel booking
- `POST /api/bookings/respond` - Staff respond to booking
- `POST /api/bookings/check-in` - Staff check-in
- `POST /api/bookings/check-out` - Staff check-out
- `GET /api/bookings/available-slots` - Get available time slots
- `POST /api/bookings/{id}/auto-assign` - Auto-assign staff (Admin)
- `POST /api/bookings/{id}/manual-assign/{staffId}` - Manual assign staff (Admin)

---

## 📊 Business Rules & Status Lifecycle

### **Booking Status Lifecycle**
```mermaid
graph TD
    A[Pending] --> B[Auto-Assigned]
    B --> C{Staff Response}
    C -->|Accept| D[Confirmed]
    C -->|Decline| A
    D --> E[In-Progress]
    E --> F[Completed]
    A --> G[Cancelled]
    B --> G
    D --> G
    A --> H[Rejected]
```

### **User Roles & Permissions**
| Role | Permissions |
|------|-------------|
| **Customer** | Book services, make payments, leave reviews, manage profile |
| **Staff** | Accept bookings, execute services, manage schedule, upload completion photos |
| **Admin** | Full system management, create services, manage staff, force complete bookings |
| **Manager** | Operational oversight, staff assignment, service management |
| **System** | Automated processes, notifications, assignments |

### **Payment Methods Supported**
- Cash
- Credit/Debit Cards
- Bank Transfer
- E-Wallet
- QR Code payments

---

## 🔧 Technical Features

### **Authentication & Security**
- JWT-based authentication with role-based authorization
- ASP.NET Identity for user management
- Password policies and email verification
- Refresh token mechanism

### **Database & ORM**
- Entity Framework Core with SQL Server
- Repository pattern with Unit of Work
- Soft delete implementation
- Audit fields (CreatedAt, UpdatedAt)

### **API Documentation**
- Swagger/OpenAPI integration
- Comprehensive endpoint documentation
- Request/response examples

### **Logging & Monitoring**
- Serilog for structured logging
- Comprehensive error handling
- Request/response logging

### **Performance & Scalability**
- Memory caching for frequently accessed data
- Pagination for large datasets
- Async/await patterns throughout

### **Real-time Features**
- **Geo-location**: GPS-based staff assignment and tracking
- **Notifications**: Real-time updates for booking status changes
- **Availability**: Dynamic time slot management
- **Auto-assignment**: Intelligent staff matching algorithm

---

## 🗄️ Database Schema Overview

### **Core Entities**
```
Users (Identity)
├── Customers
├── Staff
└── Admins

Services
├── ServicePackages
└── StaffSkills

Bookings
├── BookingImages
├── Payments
├── Reviews
└── StaffReports

Notifications
WorkSchedules
CustomerAddresses
CustomerPaymentMethods
```

### **Key Relationships**
- **One-to-One**: User ↔ Customer/Staff/Admin
- **One-to-Many**: Service → ServicePackages
- **Many-to-Many**: Staff ↔ Services (via StaffSkills)
- **One-to-Many**: Booking → BookingImages
- **One-to-One**: Booking ↔ Payment
- **One-to-One**: Booking ↔ Review

---

## 🚀 Getting Started

### **Prerequisites**
- .NET 6 or later
- SQL Server
- Visual Studio or VS Code

### **Configuration**
1. Update `appsettings.json` with your database connection string
2. Configure JWT settings
3. Set up email/SMS providers for notifications

### **Running the Application**
```bash
# Build the solution
dotnet build

# Run database migrations
dotnet ef database update

# Start the application
dotnet run --project MobileHomeServiceApi
```

### **API Documentation**
Access Swagger UI at: `https://localhost:5001/swagger`

---

## 📈 Business Model

The system operates as a **service marketplace** where:
- Customers pay for services through the platform
- Staff receive payment for completed services
- The platform takes a commission from each transaction
- Quality is maintained through reviews and ratings

### **Revenue Streams**
1. **Commission**: Percentage of each transaction
2. **Subscription**: Premium features for staff
3. **Advertising**: Promoted services
4. **Service Fees**: Booking and payment processing fees

---

## 🔮 Future Enhancements

### **Planned Features**
- **Real-time Chat**: Communication between customers and staff
- **Video Calls**: Remote consultation capabilities
- **AI Scheduling**: Machine learning for optimal staff assignment
- **IoT Integration**: Smart home device integration
- **Multi-language Support**: Localization for different markets
- **Advanced Analytics**: Business intelligence dashboard
- **Mobile Push Notifications**: Real-time mobile alerts
- **Loyalty Program**: Customer retention features

### **Scalability Considerations**
- **Microservices Architecture**: Break down into smaller services
- **Event-Driven Architecture**: Implement event sourcing
- **Caching Strategy**: Redis for distributed caching
- **Load Balancing**: Multiple API instances
- **Database Sharding**: Horizontal scaling

---

*This documentation provides a comprehensive overview of the Mobile Home Service API project flows and architecture. For technical implementation details, refer to the source code and inline documentation.* 