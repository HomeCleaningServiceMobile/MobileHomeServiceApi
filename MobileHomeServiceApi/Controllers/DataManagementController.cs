using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MHS.Common.Enums;
using MHS.Repository.Data;
using MHS.Repository.Models;
using MHS.Service.DTOs;
using AutoMapper;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataManagementController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<DataManagementController> _logger;
    private readonly IMapper _mapper;

    public DataManagementController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<DataManagementController> logger,
        IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Seed all data at once
    /// </summary>
    [HttpPost("seed-all")]
    public async Task<IActionResult> SeedAllData()
    {
        try
        {
            await SeedRolesAsync();
            await SeedServicesAsync();
            await SeedUsersAsync();
            await SeedCustomerAddressesAsync();
            await SeedStaffSkillsAsync();
            await SeedBusinessHoursAsync();
            await SeedStandardWorkSchedulesAsync();
            await SeedBookingsAsync();
            await SeedPaymentsAsync();
            await SeedReviewsAsync();
            await SeedNotificationsAsync();

            return Ok(new AppResponse<string>()
                .SetSuccessResponse("All data seeded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding all data");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while seeding data"));
        }
    }

    /// <summary>
    /// Seed roles
    /// </summary>
    [HttpPost("seed-roles")]
    public async Task<IActionResult> SeedRoles()
    {
        try
        {
            await SeedRolesAsync();
            return Ok(new AppResponse<string>()
                .SetSuccessResponse("Roles seeded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding roles");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while seeding roles"));
        }
    }

    /// <summary>
    /// Seed services and service packages
    /// </summary>
    [HttpPost("seed-services")]
    public async Task<IActionResult> SeedServices()
    {
        try
        {
            await SeedServicesAsync();
            return Ok(new AppResponse<string>()
                .SetSuccessResponse("Services seeded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding services");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while seeding services"));
        }
    }

    /// <summary>
    /// Seed users (Admin, Staff, Customer)
    /// </summary>
    [HttpPost("seed-users")]
    public async Task<IActionResult> SeedUsers()
    {
        try
        {
            await SeedUsersAsync();
            return Ok(new AppResponse<string>()
                .SetSuccessResponse("Users seeded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding users");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while seeding users"));
        }
    }

    /// <summary>
    /// Seed bookings with related data
    /// </summary>
    [HttpPost("seed-bookings")]
    public async Task<IActionResult> SeedBookings()
    {
        try
        {
            await SeedBookingsAsync();
            return Ok(new AppResponse<string>()
                .SetSuccessResponse("Bookings seeded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding bookings");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while seeding bookings"));
        }
    }

    /// <summary>
    /// Seed business hours
    /// </summary>
    [HttpPost("seed-business-hours")]
    public async Task<IActionResult> SeedBusinessHours()
    {
        try
        {
            await SeedBusinessHoursAsync();
            return Ok(new AppResponse<string>()
                .SetSuccessResponse("Business hours seeded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding business hours");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while seeding business hours"));
        }
    }

    /// <summary>
    /// Seed work schedules for staff
    /// </summary>
    [HttpPost("seed-work-schedules")]
    public async Task<IActionResult> SeedWorkSchedules()
    {
        try
        {
            await SeedStandardWorkSchedulesAsync();
            return Ok(new AppResponse<string>()
                .SetSuccessResponse("Work schedules seeded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding work schedules");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while seeding work schedules"));
        }
    }

    /// <summary>
    /// Get current business hours
    /// </summary>
    [HttpGet("business-hours")]
    public async Task<IActionResult> GetBusinessHours()
    {
        try
        {
            var businessHours = await _context.BusinessHours
                .OrderBy(bh => bh.DayOfWeek)
                .ToListAsync();

            var businessHoursResponse = _mapper.Map<List<BusinessHoursResponse>>(businessHours);

            return Ok(new AppResponse<List<BusinessHoursResponse>>()
                .SetSuccessResponse(businessHoursResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving business hours");
            return StatusCode(500, new AppResponse<List<BusinessHoursResponse>>()
                .SetErrorResponse("Error", "An error occurred while retrieving business hours"));
        }
    }

    /// <summary>
    /// Get work schedules for all staff
    /// </summary>
    [HttpGet("work-schedules")]
    public async Task<IActionResult> GetWorkSchedules()
    {
        try
        {
            var workSchedules = await _context.WorkSchedules
                .Include(ws => ws.Staff)
                .ThenInclude(s => s.User)
                .OrderBy(ws => ws.StaffId)
                .ThenBy(ws => ws.DayOfWeek)
                .ToListAsync();

            var workSchedulesResponse = _mapper.Map<List<WorkScheduleResponse>>(workSchedules);

            return Ok(new AppResponse<List<WorkScheduleResponse>>()
                .SetSuccessResponse(workSchedulesResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work schedules");
            return StatusCode(500, new AppResponse<List<WorkScheduleResponse>>()
                .SetErrorResponse("Error", "An error occurred while retrieving work schedules"));
        }
    }

    /// <summary>
    /// Get work schedules for a specific staff member
    /// </summary>
    [HttpGet("work-schedules/staff/{staffId}")]
    public async Task<IActionResult> GetWorkSchedulesByStaff(int staffId)
    {
        try
        {
            var workSchedules = await _context.WorkSchedules
                .Include(ws => ws.Staff)
                .ThenInclude(s => s.User)
                .Where(ws => ws.StaffId == staffId)
                .OrderBy(ws => ws.DayOfWeek)
                .ToListAsync();

            if (!workSchedules.Any())
            {
                return NotFound(new AppResponse<List<WorkScheduleResponse>>()
                    .SetErrorResponse("NotFound", $"No work schedules found for staff ID {staffId}"));
            }

            var workSchedulesResponse = _mapper.Map<List<WorkScheduleResponse>>(workSchedules);

            return Ok(new AppResponse<List<WorkScheduleResponse>>()
                .SetSuccessResponse(workSchedulesResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work schedules for staff {StaffId}", staffId);
            return StatusCode(500, new AppResponse<List<WorkScheduleResponse>>()
                .SetErrorResponse("Error", "An error occurred while retrieving work schedules"));
        }
    }

    /// <summary>
    /// Get business hours for a specific day
    /// </summary>
    [HttpGet("business-hours/day/{dayOfWeek}")]
    public async Task<IActionResult> GetBusinessHoursByDay(int dayOfWeek)
    {
        try
        {
            if (dayOfWeek < 0 || dayOfWeek > 6)
            {
                return BadRequest(new AppResponse<BusinessHoursResponse>()
                    .SetErrorResponse("Validation", "Day of week must be between 0 (Sunday) and 6 (Saturday)"));
            }

            var businessHours = await _context.BusinessHours
                .FirstOrDefaultAsync(bh => bh.DayOfWeek == dayOfWeek);

            if (businessHours == null)
            {
                return NotFound(new AppResponse<BusinessHoursResponse>()
                    .SetErrorResponse("NotFound", $"No business hours found for day {dayOfWeek}"));
            }

            var businessHoursResponse = _mapper.Map<BusinessHoursResponse>(businessHours);

            return Ok(new AppResponse<BusinessHoursResponse>()
                .SetSuccessResponse(businessHoursResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving business hours for day {DayOfWeek}", dayOfWeek);
            return StatusCode(500, new AppResponse<BusinessHoursResponse>()
                .SetErrorResponse("Error", "An error occurred while retrieving business hours"));
        }
    }

    /// <summary>
    /// Clear all data
    /// </summary>
    [HttpDelete("clear-all")]
    public async Task<IActionResult> ClearAllData()
    {
        try
        {
            // Clear in order to avoid foreign key constraints
            _context.BookingImages.RemoveRange(_context.BookingImages);
            _context.Reviews.RemoveRange(_context.Reviews);
            _context.Payments.RemoveRange(_context.Payments);
            _context.Bookings.RemoveRange(_context.Bookings);
            _context.WorkSchedules.RemoveRange(_context.WorkSchedules);
            _context.BusinessHours.RemoveRange(_context.BusinessHours);
            _context.StaffSkills.RemoveRange(_context.StaffSkills);
            _context.CustomerAddresses.RemoveRange(_context.CustomerAddresses);
            _context.CustomerPaymentMethods.RemoveRange(_context.CustomerPaymentMethods);
            _context.Notifications.RemoveRange(_context.Notifications);
            _context.ServicePackages.RemoveRange(_context.ServicePackages);
            _context.Services.RemoveRange(_context.Services);
            _context.Admins.RemoveRange(_context.Admins);
            _context.Staffs.RemoveRange(_context.Staffs);
            _context.Customers.RemoveRange(_context.Customers);
            _context.Users.RemoveRange(_context.Users);
            _context.StaffReports.RemoveRange(_context.StaffReports);

            await _context.SaveChangesAsync();

            return Ok(new AppResponse<string>()
                .SetSuccessResponse("All data cleared successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all data");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while clearing data"));
        }
    }

    #region Private Methods

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Admin", "Staff", "Customer", "Manager", "System" };
        
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }
    }

    private async Task SeedServicesAsync()
    {
        if (!await _context.Services.AnyAsync())
        {
            var services = new List<Service>
            {
                new Service
                {
                    Name = "House Cleaning",
                    Description = "Professional house cleaning service including dusting, vacuuming, mopping, and sanitizing",
                    Type = ServiceType.HouseCleaning,
                    BasePrice = 50.00m,
                    HourlyRate = 25.00m,
                    EstimatedDurationMinutes = 180,
                    ImageUrl = "https://example.com/images/house-cleaning.jpg",
                    IsActive = true,
                    Requirements = "Access to cleaning supplies, water, and electricity",
                    Restrictions = "No access to hazardous materials"
                },
                new Service
                {
                    Name = "Cooking Service",
                    Description = "Professional cooking service for daily meals, special occasions, and meal prep",
                    Type = ServiceType.Cooking,
                    BasePrice = 40.00m,
                    HourlyRate = 20.00m,
                    EstimatedDurationMinutes = 120,
                    ImageUrl = "https://example.com/images/cooking.jpg",
                    IsActive = true,
                    Requirements = "Kitchen access, cooking ingredients, and utensils",
                    Restrictions = "No cooking with raw meat if allergic"
                },
                new Service
                {
                    Name = "Laundry Service",
                    Description = "Complete laundry service including washing, drying, and folding",
                    Type = ServiceType.Laundry,
                    BasePrice = 20.00m,
                    HourlyRate = 15.00m,
                    EstimatedDurationMinutes = 60,
                    ImageUrl = "https://example.com/images/laundry.jpg",
                    IsActive = true,
                    Requirements = "Washing machine, dryer, and detergent",
                    Restrictions = "No expensive or delicate items without approval"
                },
                new Service
                {
                    Name = "Ironing Service",
                    Description = "Professional ironing service for clothing and linens",
                    Type = ServiceType.Ironing,
                    BasePrice = 15.00m,
                    HourlyRate = 10.00m,
                    EstimatedDurationMinutes = 60,
                    ImageUrl = "https://example.com/images/ironing.jpg",
                    IsActive = true,
                    Requirements = "Iron and ironing board",
                    Restrictions = "No silk or delicate fabrics"
                },
                new Service
                {
                    Name = "Gardening Service",
                    Description = "Garden maintenance including planting, weeding, and watering",
                    Type = ServiceType.Gardening,
                    BasePrice = 35.00m,
                    HourlyRate = 18.00m,
                    EstimatedDurationMinutes = 150,
                    ImageUrl = "https://example.com/images/gardening.jpg",
                    IsActive = true,
                    Requirements = "Garden tools and water access",
                    Restrictions = "Weather dependent"
                },
                new Service
                {
                    Name = "Babysitting Service",
                    Description = "Professional childcare service for children of all ages",
                    Type = ServiceType.Babysitting,
                    BasePrice = 30.00m,
                    HourlyRate = 12.00m,
                    EstimatedDurationMinutes = 240,
                    ImageUrl = "https://example.com/images/babysitting.jpg",
                    IsActive = true,
                    Requirements = "Emergency contact information",
                    Restrictions = "Children must be at least 6 months old"
                },
                new Service
                {
                    Name = "Elder Care Service",
                    Description = "Compassionate care for elderly including companionship and basic assistance",
                    Type = ServiceType.ElderCare,
                    BasePrice = 45.00m,
                    HourlyRate = 20.00m,
                    EstimatedDurationMinutes = 180,
                    ImageUrl = "https://example.com/images/eldercare.jpg",
                    IsActive = true,
                    Requirements = "Medical information if applicable",
                    Restrictions = "No medical procedures"
                },
                new Service
                {
                    Name = "Pet Care Service",
                    Description = "Pet care including feeding, walking, and basic grooming",
                    Type = ServiceType.PetCare,
                    BasePrice = 25.00m,
                    HourlyRate = 15.00m,
                    EstimatedDurationMinutes = 90,
                    ImageUrl = "https://example.com/images/petcare.jpg",
                    IsActive = true,
                    Requirements = "Pet food and supplies",
                    Restrictions = "No aggressive animals"
                },
                new Service
                {
                    Name = "General Maintenance",
                    Description = "Basic home maintenance and repair services",
                    Type = ServiceType.GeneralMaintenance,
                    BasePrice = 40.00m,
                    HourlyRate = 25.00m,
                    EstimatedDurationMinutes = 120,
                    ImageUrl = "https://example.com/images/maintenance.jpg",
                    IsActive = true,
                    Requirements = "Access to tools and materials",
                    Restrictions = "No electrical or plumbing work"
                }
            };

            _context.Services.AddRange(services);
            await _context.SaveChangesAsync();

            // Add service packages
            var servicePackages = new List<ServicePackage>();
            foreach (var service in services)
            {
                servicePackages.Add(new ServicePackage
                {
                    ServiceId = service.Id,
                    Name = $"{service.Name} - Basic Package",
                    Description = $"Basic package for {service.Name.ToLower()}",
                    Price = service.BasePrice,
                    DurationMinutes = service.EstimatedDurationMinutes,
                    IsActive = true
                });

                servicePackages.Add(new ServicePackage
                {
                    ServiceId = service.Id,
                    Name = $"{service.Name} - Premium Package",
                    Description = $"Premium package for {service.Name.ToLower()} with additional features",
                    Price = service.BasePrice * 1.5m,
                    DurationMinutes = service.EstimatedDurationMinutes + 60,
                    IsActive = true
                });
            }

            _context.ServicePackages.AddRange(servicePackages);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedUsersAsync()
    {
        // Admin user
        if (await _userManager.FindByEmailAsync("admin@example.com") == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
                PhoneNumber = "0123456789",
                PhoneNumberConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(adminUser, "123456");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                
                var admin = new Admin
                {
                    UserId = adminUser.Id,
                    EmployeeId = "ADM001",
                    Position = "System Administrator",
                    Department = "IT",
                    HireDate = DateTime.UtcNow.AddMonths(-6),
                };

                _context.Admins.Add(admin);
            }
        }

        // Staff users
        var staffMembers = new[]
        {
            new { Email = "staff1@example.com", FirstName = "John", LastName = "Doe", EmployeeId = "STF001" },
            new { Email = "staff2@example.com", FirstName = "Jane", LastName = "Smith", EmployeeId = "STF002" },
            new { Email = "staff3@example.com", FirstName = "Mike", LastName = "Johnson", EmployeeId = "STF003" }
        };

        foreach (var staffMember in staffMembers)
        {
            if (await _userManager.FindByEmailAsync(staffMember.Email) == null)
            {
                var staffUser = new ApplicationUser
                {
                    UserName = staffMember.Email,
                    Email = staffMember.Email,
                    EmailConfirmed = true,
                    PhoneNumber = "0123456789",
                    PhoneNumberConfirmed = true,
                    FirstName = staffMember.FirstName,
                    LastName = staffMember.LastName,
                    Role = UserRole.Staff,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(staffUser, "123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(staffUser, "Staff");
                    
                    var staff = new Staff
                    {
                        UserId = staffUser.Id,
                        EmployeeId = staffMember.EmployeeId,
                        HireDate = DateTime.UtcNow.AddMonths(-3),
                        IsAvailable = true,
                    };

                    _context.Staffs.Add(staff);
                }
            }
        }

        // Customer users
        var customers = new[]
        {
            new { Email = "customer1@example.com", FirstName = "Alice", LastName = "Brown" },
            new { Email = "customer2@example.com", FirstName = "Bob", LastName = "Wilson" },
            new { Email = "customer3@example.com", FirstName = "Carol", LastName = "Davis" }
        };

        foreach (var customer in customers)
        {
            if (await _userManager.FindByEmailAsync(customer.Email) == null)
            {
                var customerUser = new ApplicationUser
                {
                    UserName = customer.Email,
                    Email = customer.Email,
                    EmailConfirmed = true,
                    PhoneNumber = "0123456789",
                    PhoneNumberConfirmed = true,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(customerUser, "123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(customerUser, "Customer");
                    
                    var customerProfile = new Customer
                    {
                        UserId = customerUser.Id,
                        TotalBookings = 0,
                    };

                    _context.Customers.Add(customerProfile);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedCustomerAddressesAsync()
    {
        if (!await _context.CustomerAddresses.AnyAsync())
        {
            var customers = await _context.Customers.ToListAsync();
            var addresses = new List<CustomerAddress>();

            foreach (var customer in customers)
            {
                addresses.Add(new CustomerAddress
                {
                    Title = "Home",
                    CustomerId = customer.Id,
                    FullAddress = $"123 Home Street, Customer {customer.Id}",
                    District = "District 1",
                    Province = "Ho Chi Minh City",
                    Latitude = 10.8231m,
                    Longitude = 106.6297m,
                    IsDefault = true
                });

                addresses.Add(new CustomerAddress
                {
                    CustomerId = customer.Id,
                    Title = "Office",
                    FullAddress = $"456 Office Street, Customer {customer.Id}",
                    District = "District 2",
                    Province = "Ho Chi Minh City",
                    Latitude = 10.8231m,
                    Longitude = 106.6297m,
                    IsDefault = false,
                });
            }

            _context.CustomerAddresses.AddRange(addresses);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedStaffSkillsAsync()
    {
        if (!await _context.StaffSkills.AnyAsync())
        {
            var staffs = await _context.Staffs.ToListAsync();
            var services = await _context.Services.ToListAsync();
            var skills = new List<StaffSkill>();

            foreach (var staff in staffs)
            {
                // Each staff member gets 3-4 random skills
                var randomServices = services.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
                foreach (var service in randomServices)
                {
                    skills.Add(new StaffSkill
                    {
                        StaffId = staff.Id,
                        ServiceId = service.Id,
                        SkillLevel = new Random().Next(1, 6), // 1-5 skill level
                        IsActive = true,
                        CertifiedAt = DateTime.UtcNow.AddMonths(-new Random().Next(1, 24))
                    });
                }
            }

            _context.StaffSkills.AddRange(skills);
            await _context.SaveChangesAsync();
        }
    }


    private async Task SeedBusinessHoursAsync()
    {
        if (!await _context.BusinessHours.AnyAsync())
        {
            var businessHours = new List<BusinessHours>
            {
                new BusinessHours { DayOfWeek = 1, OpenTime = new TimeSpan(8, 0, 0), CloseTime = new TimeSpan(18, 0, 0), IsClosed = false, IsActive = true }, // Monday
                new BusinessHours { DayOfWeek = 2, OpenTime = new TimeSpan(8, 0, 0), CloseTime = new TimeSpan(18, 0, 0), IsClosed = false, IsActive = true }, // Tuesday
                new BusinessHours { DayOfWeek = 3, OpenTime = new TimeSpan(8, 0, 0), CloseTime = new TimeSpan(18, 0, 0), IsClosed = false, IsActive = true }, // Wednesday
                new BusinessHours { DayOfWeek = 4, OpenTime = new TimeSpan(8, 0, 0), CloseTime = new TimeSpan(18, 0, 0), IsClosed = false, IsActive = true }, // Thursday
                new BusinessHours { DayOfWeek = 5, OpenTime = new TimeSpan(8, 0, 0), CloseTime = new TimeSpan(18, 0, 0), IsClosed = false, IsActive = true }, // Friday
                new BusinessHours { DayOfWeek = 6, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(17, 0, 0), IsClosed = false, IsActive = true }, // Saturday
                new BusinessHours { DayOfWeek = 0, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(18, 0, 0), IsClosed = true, IsActive = true }   // Sunday (Closed)
            };

            _context.BusinessHours.AddRange(businessHours);
            await _context.SaveChangesAsync();
        }
    }
    private async Task SeedStandardWorkSchedulesAsync()
    {
        if (!await _context.WorkSchedules.AnyAsync())
        {
            var staffs = await _context.Staffs.ToListAsync();
            var schedules = new List<WorkSchedule>();
            var random = new Random();

            foreach (var staff in staffs)
            {
                var staffIndex = staffs.IndexOf(staff);
                
                int[] workingDays = staffIndex  switch
                {
                    0 => new[] { 1, 2, 3, 4, 5 },
                    1 => new[] { 1, 2, 3, 4, 5, 6 }, 
                    _ => new[] { 2, 3, 4, 5, 6 } 
                };

                foreach (var dayOfWeek in workingDays)
                {
                    var startHour = 8 + (staffIndex % 2); // 8 or 9 AM
                    var endHour = 17 + (staffIndex % 2); // 5 or 6 PM
                    
                    // Saturday has shorter hours
                    if (dayOfWeek == 6)
                    {
                        startHour = 9;
                        endHour = 16;
                    }

                    schedules.Add(new WorkSchedule
                    {
                        StaffId = staff.Id,
                        DayOfWeek = dayOfWeek,
                        StartTime = new TimeSpan(startHour, 0, 0),
                        EndTime = new TimeSpan(endHour, 0, 0),
                        IsActive = true,
                    });
                }
            }

            _context.WorkSchedules.AddRange(schedules);
            await _context.SaveChangesAsync();
        }
    }
    private async Task SeedBookingsAsync()
    {
        if (!await _context.Bookings.AnyAsync())
        {
            var customers = await _context.Customers.ToListAsync();
            var staffs = await _context.Staffs.ToListAsync();
            var services = await _context.Services.ToListAsync();
            var servicePackages = await _context.ServicePackages.ToListAsync();
            var bookings = new List<Booking>();

            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                var customer = customers[random.Next(customers.Count)];
                var staff = staffs[random.Next(staffs.Count)];
                var service = services[random.Next(services.Count)];
                var servicePackage = servicePackages.Where(sp => sp.ServiceId == service.Id).FirstOrDefault();
                
                var scheduledDate = DateTime.UtcNow.Date.AddDays(random.Next(1, 30));
                var scheduledTime = TimeSpan.FromHours(8 + random.Next(0, 9)); // 8 AM to 5 PM
                var estimatedDuration = service.EstimatedDurationMinutes;
                
                // Determine status and set related timestamps
                var status = (BookingStatus)random.Next(0, 8);
                DateTime? startedAt = null;
                DateTime? completedAt = null;
                DateTime? cancelledAt = null;
                DateTime? staffAcceptedAt = null;
                DateTime? staffResponseDeadline = null;

                switch (status)
                {
                    case BookingStatus.Confirmed:
                    case BookingStatus.AutoAssigned:
                        staffAcceptedAt = scheduledDate.AddDays(-1);
                        staffResponseDeadline = scheduledDate.AddDays(-2);
                        break;
                    case BookingStatus.InProgress:
                        staffAcceptedAt = scheduledDate.AddDays(-1);
                        startedAt = scheduledDate.Add(scheduledTime);
                        break;
                    case BookingStatus.Completed:
                        staffAcceptedAt = scheduledDate.AddDays(-1);
                        startedAt = scheduledDate.Add(scheduledTime);
                        completedAt = startedAt?.AddMinutes(estimatedDuration);
                        break;
                    case BookingStatus.Cancelled:
                        cancelledAt = scheduledDate.AddDays(-1);
                        break;
                }

                var booking = new Booking
                {
                    BookingNumber = $"BK{DateTime.Now.Ticks.ToString().Substring(10)}{i:D3}",
                    CustomerId = customer.Id,
                    StaffId = staff.Id,
                    ServiceId = service.Id,
                    ServicePackageId = servicePackage?.Id,
                    Status = status,
                    ScheduledDate = scheduledDate,
                    ScheduledTime = scheduledTime,
                    EstimatedDurationMinutes = estimatedDuration,
                    TotalAmount = servicePackage?.Price ?? service.BasePrice,
                    FinalAmount = servicePackage?.Price ?? service.BasePrice,
                    ServiceAddress = $"123 Service Address {i + 1}, Ward {i % 3 + 1}, District {i % 5 + 1}, Ho Chi Minh City",
                    AddressLatitude = 10.8231m + (decimal)(random.NextDouble() * 0.1 - 0.05), // Random coordinates around HCM
                    AddressLongitude = 106.6297m + (decimal)(random.NextDouble() * 0.1 - 0.05),
                    SpecialInstructions = i % 3 == 0 ? $"Special instructions for booking {i + 1}" : null,
                    Notes = $"Booking notes for booking {i + 1}",
                    
                    // Timestamps based on status
                    StartedAt = startedAt,
                    CompletedAt = completedAt,
                    CancelledAt = cancelledAt,
                    CancellationReason = status == BookingStatus.Cancelled ? "Customer requested cancellation" : null,
                    
                    // Staff response fields
                    StaffResponseDeadline = staffResponseDeadline,
                    StaffAcceptedAt = staffAcceptedAt,
                    StaffDeclinedAt = null,
                    StaffDeclineReason = null
                };

                bookings.Add(booking);
            }

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedPaymentsAsync()
    {
        if (!await _context.Payments.AnyAsync())
        {
            var bookings = await _context.Bookings.ToListAsync();
            var payments = new List<Payment>();

            foreach (var booking in bookings)
            {
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    PaymentNumber = $"PAY{DateTime.Now.Ticks.ToString().Substring(10)}{booking.Id:D3}",
                    Amount = booking.TotalAmount,
                    Method = (PaymentMethod)new Random().Next(1, 7),
                    Status = (PaymentStatus)new Random().Next(0, 5),
                    PaidAt = booking.ScheduledDate.AddDays(-1),
                    TransactionId = $"TXN{Guid.NewGuid().ToString()[..8]}",
                    GatewayTransactionId = $"GTW{Guid.NewGuid().ToString()[..8]}",
                    GatewayName = "Test Gateway",
                    RefundAmount = 0,
                    Notes = $"Payment for booking {booking.BookingNumber}"
                };

                payments.Add(payment);
            }

            _context.Payments.AddRange(payments);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedReviewsAsync()
    {
        if (!await _context.Reviews.AnyAsync())
        {
            var completedBookings = await _context.Bookings
                .Where(b => b.Status == BookingStatus.Completed)
                .ToListAsync();

            var reviews = new List<Review>();

            foreach (var booking in completedBookings.Take(5)) // Only create reviews for first 5 completed bookings
            {
                var review = new Review
                {
                    BookingId = booking.Id,
                    CustomerId = booking.CustomerId,
                    StaffId = booking.StaffId.Value, // Cast nullable int to int
                    Rating = new Random().Next(3, 6), // 3-5 stars
                    Comment = $"Great service! The staff was professional and efficient for booking {booking.BookingNumber}",
                    QualityRating = new Random().Next(3, 6),
                    TimelinessRating = new Random().Next(3, 6),
                    ProfessionalismRating = new Random().Next(3, 6),
                    CommunicationRating = new Random().Next(3, 6),
                    IsPublic = true,
                    IsReported = false
                };

                reviews.Add(review);
            }

            _context.Reviews.AddRange(reviews);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedNotificationsAsync()
    {
        if (!await _context.Notifications.AnyAsync())
        {
            var users = await _context.Users.Take(5).ToListAsync();
            var notifications = new List<Notification>();

            foreach (var user in users)
            {
                for (int i = 0; i < 3; i++)
                {
                    var notification = new Notification
                    {
                        UserId = user.Id,
                        Title = $"Test Notification {i + 1}",
                        Message = $"This is a test notification message {i + 1} for user {user.FirstName}",
                        Type = (NotificationType)new Random().Next(1, 11),
                        IsRead = i == 0, // Make first notification read
                        ReadAt = i == 0 ? DateTime.UtcNow.AddHours(-1) : null,
                        IsSent = true,
                        SentAt = DateTime.UtcNow.AddMinutes(-new Random().Next(1, 60))
                    };

                    notifications.Add(notification);
                }
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();
        }
    }

    #endregion
} 