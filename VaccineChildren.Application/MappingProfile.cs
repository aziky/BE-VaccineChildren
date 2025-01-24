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
        CreateMap<StaffReq, Staff>();
        CreateMap<Staff, StaffRes>()
            // Ánh xạ các trường từ User
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
            // Ánh xạ Role (lấy tên Role từ Role entity)
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.RoleId)) // Giả sử Role có thuộc tính Name
            // Nếu Status muốn ánh xạ từ Staff entity, thì không cần thay đổi
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); // Convert bool to string for Status
    }
}
