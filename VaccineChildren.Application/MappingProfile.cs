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
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId is auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<StaffReq, Staff>();
        CreateMap<Staff, StaffRes>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address));
        CreateMap<VaccineReq, Vaccine>();
        CreateMap<Vaccine, VaccineRes>()
            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.VaccineManufactures.FirstOrDefault().Manufacturer))
            .ForMember(dest => dest.Description, opt => opt.Ignore()) // Ignore automatic mapping for description
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.VaccineManufactures.Sum(vm => vm.Price ?? 0))) // Sum up the prices from VaccineManufactures
            .AfterMap((src, dest) =>
            {
                if (!string.IsNullOrEmpty(src.Description))
                {
                    dest.Description = System.Text.Json.JsonSerializer.Deserialize<DTOs.Response.DescriptionDetail>(src.Description);
                }
            });
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
        
        // Correct mapping from VaccineManufacture to VaccineRes
        CreateMap<VaccineManufacture, VaccineRes>()
            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Manufacturer)) // Map Manufacturer from VaccineManufacture
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price ?? 0)) // Map Price, defaulting to 0 if null
            .ForMember(dest => dest.VaccineId, opt => opt.MapFrom(src => src.VaccineId)) // Map VaccineId if necessary
            .ForMember(dest => dest.VaccineName, opt => opt.MapFrom(src => src.Vaccine.VaccineName)); // Assuming Vaccine has a VaccineName property


        CreateMap<User, GetUserRes>()
            .ForMember(dest => dest.ListChildRes, opt => opt.MapFrom(src => src.Children));

        CreateMap<Child, GetChildRes>()
            .ForMember(dest => dest.VaccinatedInformation, opt => opt.MapFrom(src => src.Schedules));

        CreateMap<Schedule, GetChildRes.VaccinatedInfor>();
    }
}
 