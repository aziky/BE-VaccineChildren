using System.Security.Cryptography;
using System.Text;

namespace VaccineChildren.Application.Services.Impl;

public class RsaService
{
    private static RSA _rsa;
    private readonly string _keyPath = "rsa_key.xml"; // đường dẫn để lưu key

    public RsaService()
    {
        if (_rsa == null)
        {
            _rsa = RSA.Create();
            if (File.Exists(_keyPath))
            {
                // Load existing key
                string keyXml = File.ReadAllText(_keyPath);
                _rsa.FromXmlString(keyXml);
            }
            else
            {
                // Generate and save new key
                string keyXml = _rsa.ToXmlString(includePrivateParameters: true);
                File.WriteAllText(_keyPath, keyXml);
            }
        }
    }

    public string Encrypt(string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] encryptedBytes = _rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string encryptedData)
    {
        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch
        {
            return null;
        }
    }
}