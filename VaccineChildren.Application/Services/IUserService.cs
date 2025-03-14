
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IUserService
{
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest registerRequest);
    Task<UserRes> Login(UserReq userReq);
    Task<RegisterResponse> VerifyEmailAsync(string token, string email);
    Task<RegisterResponse> ResendVerificationEmailAsync(string email);
    Task CreateChildAsync(CreateChildReq request);
    Task<GetChildRes> GetChildByChildIdAsync(string childId);
    Task<GetUserRes> GetUserByUserIdAsync(string userId);
    Task<UserRes> LoginWithGoogleAsync(GoogleAuthRequest request);
    Task<RegisterResponse> ForgotPasswordAsync(string email);
    Task<RegisterResponse> VerifyResetTokenAsync(string token, string email);
    Task<RegisterResponse> ResetPasswordAsync(ResetPasswordRequest request);
}