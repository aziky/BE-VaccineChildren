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
    }
}