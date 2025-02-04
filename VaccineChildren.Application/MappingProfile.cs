using AutoMapper;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
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
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.RoleName));
        CreateMap<VaccineReq, Vaccine>();
        CreateMap<Vaccine, VaccineRes>();
    }
}