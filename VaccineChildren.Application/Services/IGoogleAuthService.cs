using VaccineChildren.Application.DTOs.Request;

namespace VaccineChildren.Application.Services;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> VerifyGoogleTokenAsync(string idToken);
}