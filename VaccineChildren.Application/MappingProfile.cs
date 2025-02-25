using System.Globalization;
using AutoMapper;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Requests;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.DTOs.Responses;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserReq, User>();
        CreateMap<User, UserRes>();
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<StaffReq, Staff>();
        CreateMap<Staff, StaffRes>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address));

        // Ánh xạ VaccineReq sang Vaccine
        CreateMap<VaccineReq, Vaccine>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => 
                MappingHelpers.SerializeDescription(src.Description)));

        // Ánh xạ Vaccine sang VaccineRes
        CreateMap<Vaccine, VaccineRes>()
            .ForMember(dest => dest.VaccineId, opt => opt.MapFrom(src => src.VaccineId.ToString()))
            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => 
                src.VaccineManufactures.FirstOrDefault() != null ? src.VaccineManufactures.FirstOrDefault().Manufacturer : null))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => 
                src.VaccineManufactures.FirstOrDefault() != null ? src.VaccineManufactures.FirstOrDefault().Price : 0m))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => 
                MappingHelpers.DeserializeDescription(src.Description)));

        CreateMap<ManufacturerReq, Manufacturer>();
        CreateMap<Manufacturer, ManufacturerRes>();

        CreateMap<CreateChildReq, Child>()
            .ForMember(dest => dest.Dob, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToLower()));

        CreateMap<PackageReq, Package>();
        CreateMap<Package, PackageRes>()
            .ForMember(dest => dest.Vaccines, opt => opt.MapFrom(src => src.Vaccines));
        CreateMap<BatchReq, Batch>();
        CreateMap<Batch, BatchRes>();

        CreateMap<VaccineManufacture, VaccineRes>()
            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Manufacturer))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price ?? 0))
            .ForMember(dest => dest.VaccineId, opt => opt.MapFrom(src => src.VaccineId.ToString()))
            .ForMember(dest => dest.VaccineName, opt => opt.MapFrom(src => src.Vaccine.VaccineName));

        CreateMap<User, GetUserRes>()
            .ForMember(dest => dest.ListChildRes, opt => opt.MapFrom(src => src.Children));

        CreateMap<Child, GetChildRes>();
    }
}