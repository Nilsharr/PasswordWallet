using System.Security.Cryptography;
using System.Text;
using Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Cryptography;

public class EncryptionService(ILogger<EncryptionService> logger) : IEncryptionService
{
    public string Encrypt(string plainText, string key)
    {
        logger.LogInformation("Encrypting plain text.");

        var byteKey = GetByteKey(key);

        using var aes = Aes.Create();

        var iv = aes.IV;
        using var encryptor = aes.CreateEncryptor(byteKey, iv);
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherTextBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainText.Length);

        var result = new byte[iv.Length + cipherTextBytes.Length];
        iv.CopyTo(result, 0);
        cipherTextBytes.CopyTo(result, iv.Length);

        return Convert.ToHexString(result);
    }

    public string Decrypt(string encrypted, string key)
    {
        logger.LogInformation("Decrypting encrypted text.");

        const int blockByteSize = 128 / 8;
        var encryptedData = HexStringToBytes(encrypted);
        var byteKey = GetByteKey(key);
        
        using var aes = Aes.Create();
        var iv = encryptedData.Take(blockByteSize).ToArray();
        var cipherText = encryptedData.Skip(blockByteSize).ToArray();
        
        using var encryptor = aes.CreateDecryptor(byteKey, iv);
        var decryptedBytes = encryptor
            .TransformFinalBlock(cipherText, 0, cipherText.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private static byte[] GetByteKey(string password) => MD5.HashData(Encoding.UTF8.GetBytes(password));

    private static byte[] HexStringToBytes(string hexString)
    {
        var bytes = new byte[hexString.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var currentHex = hexString.Substring(i * 2, 2);
            bytes[i] = Convert.ToByte(currentHex, 16);
        }

        return bytes;
    }
}