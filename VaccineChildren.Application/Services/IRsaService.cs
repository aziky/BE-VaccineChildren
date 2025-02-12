public interface IRsaService
{
    string Encrypt(string data);
    string Decrypt(string encryptedData);
    // string GenerateJwtToken(string userId, string role, int expirationHours = 1);
}