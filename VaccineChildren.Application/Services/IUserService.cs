
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IUserService
{
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest registerRequest);
    Task<UserRes> Login(UserReq userReq);
}