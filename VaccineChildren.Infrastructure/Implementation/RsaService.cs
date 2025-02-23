using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VaccineChildren.Application.Services.Impl;

public class RsaService : IRsaService, IDisposable
{
    private readonly RSA _rsa;
    private readonly string _privateKeyPath;
    private readonly string _publicKeyPath;
    
    public RsaService(string privateKeyPath = "private_key.rsa", string publicKeyPath = "public_key.rsa")
    {
        _privateKeyPath = privateKeyPath;
        _publicKeyPath = publicKeyPath;
        _rsa = RSA.Create();
        LoadExistingKey();
    }

    private void LoadExistingKey()
    {
        try
        {
            byte[] privateKeyBytes = File.ReadAllBytes(_privateKeyPath);
            _rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load RSA key.", ex);
        }
    }

    public string Encrypt(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentNullException(nameof(data));

        try
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = _rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedBytes);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Encryption failed.", ex);
        }
    }

    public string Decrypt(string encryptedData)
    {
        if (string.IsNullOrEmpty(encryptedData))
            throw new ArgumentNullException(nameof(encryptedData));

        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid Base64 string.", nameof(encryptedData), ex);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Decryption failed.", ex);
        }
    }

    public SecurityKey GetRsaSecurityKey()
    {
        return new RsaSecurityKey(_rsa);
    }

    public SigningCredentials GetSigningCredentials()
    {
        var securityKey = GetRsaSecurityKey();
        return new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
    }

    public void Dispose()
    {
        _rsa?.Dispose();
    }
}