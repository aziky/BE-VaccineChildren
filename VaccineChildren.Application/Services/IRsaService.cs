using Microsoft.IdentityModel.Tokens;

public interface IRsaService
{
    string Encrypt(string data);
    string Decrypt(string encryptedData);
    SecurityKey GetRsaSecurityKey();
    SigningCredentials GetSigningCredentials();
}