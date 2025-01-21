
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IUserService
{
    Task RegisterUser(UserReq userReq);
    Task<UserRes> Login(UserReq userReq);
}