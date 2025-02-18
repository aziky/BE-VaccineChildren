using System;
using System.IO;
using System.Security.Cryptography;

public class Program
{
    public static void Main(string[] args)
    {
        // Step 1: Generate RSA Key Pair
        using (RSA rsa = RSA.Create(2048)) // Use 2048-bit key size
        {
            // Step 2: Get Private and Public Keys
            RSAParameters privateKeyParams = rsa.ExportParameters(true);
            RSAParameters publicKeyParams = rsa.ExportParameters(false);

            // Step 3: Encode the private key in PKCS8 format
            byte[] privateKeyBytes = rsa.ExportPkcs8PrivateKey(); // PKCS8 encoded private key
            string privateKeyString = Convert.ToBase64String(privateKeyBytes);
            
            // Step 4: Encode the public key in X.509 format (standard for public keys)
            byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo(); // X.509 encoded public key
            string publicKeyString = Convert.ToBase64String(publicKeyBytes);

            // Step 5: Save the private key to a file with .rsa extension (PKCS8 format)
            File.WriteAllBytes("privateKey.rsa", privateKeyBytes);

            // Step 6: Save the public key to a file (optional, for completeness)
            File.WriteAllBytes("publicKey.rsa", publicKeyBytes);

            // Optionally, print out the keys
            Console.WriteLine("Private Key (Base64 encoded): " + privateKeyString);
            Console.WriteLine("Public Key (Base64 encoded): " + publicKeyString);
        }
    }
}