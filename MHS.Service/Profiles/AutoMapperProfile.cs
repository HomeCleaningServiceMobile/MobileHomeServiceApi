using AutoMapper;
using MHS.Repository.Models;
using MHS.Service.DTOs;

namespace MHS.Service.Profiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<ApplicationUser, UserResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

        // Customer mappings
        CreateMap<Customer, CustomerResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses));

        CreateMap<CustomerAddress, CustomerAddressResponse>();

        // Staff mappings
        CreateMap<Staff, StaffResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.StaffSkills, opt => opt.MapFrom(src => src.StaffSkills));

        CreateMap<StaffSkill, StaffSkillResponse>()
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));

        // Service mappings
        CreateMap<MHS.Repository.Models.Service, ServiceResponse>()
            .ForMember(dest => dest.ServicePackages, opt => opt.MapFrom(src => src.ServicePackages));

        CreateMap<ServicePackage, ServicePackageResponse>();

        // Booking mappings
        CreateMap<Booking, BookingResponse>()
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service))
            .ForMember(dest => dest.ServicePackage, opt => opt.MapFrom(src => src.ServicePackage))
            .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.Staff))
            .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
            .ForMember(dest => dest.Review, opt => opt.MapFrom(src => src.Review))
            .ForMember(dest => dest.BookingImages, opt => opt.MapFrom(src => src.BookingImages));

        CreateMap<Booking, BookingSummaryResponse>()
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service))
            .ForMember(dest => dest.ServicePackage, opt => opt.MapFrom(src => src.ServicePackage))
            .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.Staff))
            .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
            .ForMember(dest => dest.Review, opt => opt.MapFrom(src => src.Review))
            .ForMember(dest => dest.BookingImages, opt => opt.MapFrom(src => src.BookingImages));

        CreateMap<BookingImage, BookingImageResponse>();

        // Payment mappings
        CreateMap<Payment, PaymentResponse>();

        // Review mappings
        CreateMap<Review, ReviewResponse>()
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.Staff));

        // Notification mappings
        CreateMap<Notification, NotificationResponse>();

        // Work Schedule mappings
        CreateMap<WorkSchedule, WorkScheduleResponse>()
            .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.Staff))
            .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => src.DayName))
            .ForMember(dest => dest.WorkDuration, opt => opt.MapFrom(src => src.WorkDuration))
            .ForMember(dest => dest.WorkHours, opt => opt.MapFrom(src => src.WorkHours))
            .ForMember(dest => dest.IsWorkingNow, opt => opt.MapFrom(src => src.IsWorkingNow))
            .ForMember(dest => dest.ScheduleDisplay, opt => opt.MapFrom(src => src.ScheduleDisplay));

        // Business Hours mappings
        CreateMap<BusinessHours, BusinessHoursResponse>()
            .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => src.DayName))
            .ForMember(dest => dest.IsCurrentlyOpen, opt => opt.MapFrom(src => src.IsCurrentlyOpen))
            .ForMember(dest => dest.TimeDisplay, opt => opt.MapFrom(src => src.TimeDisplay));

        // Admin mappings
        CreateMap<Admin, AdminResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
    }
} 