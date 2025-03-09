using System.Security.Authentication;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;

namespace VaccineChildren.Application.Services.Impl;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GoogleAuthService> _logger;
    private readonly string _googleApiUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=";

    public GoogleAuthService(HttpClient httpClient, ILogger<GoogleAuthService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GoogleUserInfo> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_googleApiUrl}{idToken}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Google token");
            throw new AuthenticationException("Failed to verify Google token", ex);
        }
    }
}