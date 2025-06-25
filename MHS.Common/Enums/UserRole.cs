namespace MHS.Common.Enums;

public enum UserRole
{
    Customer = 1,
    Staff = 2,
    Admin = 3,
    Manager = 4,
    System = 5
}

public enum UserStatus
{
    Inactive = 0,
    Active = 1,
    Suspended = 2,
    Banned = 3
}

public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    AutoAssigned = 2,
    PendingSchedule = 3,
    InProgress = 4,
    Completed = 5,
    Cancelled = 6,
    Rejected = 7
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3,
    Cancelled = 4
}

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    DebitCard = 3,
    BankTransfer = 4,
    EWallet = 5,
    QRCode = 6
}

public enum ServiceType
{
    HouseCleaning = 1,
    Cooking = 2,
    Laundry = 3,
    Ironing = 4,
    Gardening = 5,
    Babysitting = 6,
    ElderCare = 7,
    PetCare = 8,
    GeneralMaintenance = 9
}

public enum WorkScheduleStatus
{
    Available = 1,
    Assigned = 2,
    Busy = 3,
    OffDuty = 4
}

public enum NotificationType
{
    BookingConfirmed = 1,
    BookingCancelled = 2,
    BookingCompleted = 3,
    PaymentReceived = 4,
    PaymentFailed = 5,
    StaffAssigned = 6,
    WorkStarted = 7,
    WorkCompleted = 8,
    RatingReceived = 9,
    SystemAlert = 10
} 