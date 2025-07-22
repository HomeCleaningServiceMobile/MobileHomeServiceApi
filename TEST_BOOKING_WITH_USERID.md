# Test Booking với UserId

## Cách sử dụng mới

### 1. Tạo booking với staff assignment
```json
POST /api/bookings
Authorization: Bearer {token}

{
  "serviceId": 2,
  "servicePackageId": 1,
  "scheduledDate": "2025-01-25T00:00:00.000Z",
  "scheduledTime": "14:30:00",
  "serviceAddress": "123 Đường ABC, Quận 1, TP.HCM",
  "addressLatitude": 10.762622,
  "addressLongitude": 106.660172,
  "specialInstructions": "Làm sạch kỹ phòng tắm",
  "preferredPaymentMethod": 1,
  "userId": 16
}
```

### 2. Tạo booking không assign staff (auto-assign)
```json
POST /api/bookings
Authorization: Bearer {token}

{
  "serviceId": 2,
  "servicePackageId": 1,
  "scheduledDate": "2025-01-25T00:00:00.000Z",
  "scheduledTime": "14:30:00",
  "serviceAddress": "123 Đường ABC, Quận 1, TP.HCM",
  "addressLatitude": 10.762622,
  "addressLongitude": 106.660172,
  "specialInstructions": "Làm sạch kỹ phòng tắm",
  "preferredPaymentMethod": 1
}
```

## Luồng xử lý

1. **Client gửi `userId`** (từ bảng AspNetUsers)
2. **Backend tìm staff** có `Staff.UserId = userId`
3. **Lấy `staff.Id`** và gán vào `booking.StaffId`
4. **Set status** = `Confirmed` nếu có staff, `Pending` nếu không

## Lưu ý

- `userId` phải tồn tại trong bảng `Staff` với `UserId = userId`
- Staff phải có `IsAvailable = true`
- Nếu không truyền `userId`, hệ thống sẽ auto-assign staff 