using AutoMapper;
using MHS.Repository.Models;
using MHS.Service.DTOs;

namespace MHS.Service.AutoMapper
{
    public class ServiceMapping : Profile
    {
        public ServiceMapping()
        {
            // Entity -> DTO: Trả về dữ liệu
            CreateMap<MHS.Repository.Models.Service, ServiceResponse>();
            CreateMap<ServicePackage, ServicePackageResponse>();
            CreateMap<MHS.Repository.Models.Service, ServiceSummaryResponse>();
            


            // DTO -> Entity: Nhận dữ liệu
            CreateMap<CreateServiceRequest, MHS.Repository.Models.Service>();
            CreateMap<UpdateServiceRequest, MHS.Repository.Models.Service>();
            CreateMap<CreateServicePackageRequest, ServicePackage>();

            //Custom Mapping
            CreateMap<ServicePackage, ServicePriceResponse>()
            .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
            .ForMember(dest => dest.ServicePackageId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ServicePackageName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.Service.BasePrice))
            .ForMember(dest => dest.HourlyRate, opt => opt.MapFrom(src => src.Service.HourlyRate))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.DurationMinutes))
            .ForMember(dest => dest.CalculatedPrice, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.PricingMethod, opt => opt.MapFrom(src => "Package"))
            .ForMember(dest => dest.Breakdown, opt => opt.MapFrom(src => $"Fixed package price: ${src.Price}"));

        }
    }
} 