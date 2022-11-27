using System.Security.Cryptography;
using System.Text;

namespace PasswordWallet.Server.Utils;

public static class CryptoUtils
{
    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();
    private const int AesBlockByteSize = 128 / 8;

    public static string AesEncryptToHexString(string toEncrypt, string password)
    {
        var key = GetKey(password);

        using var aes = Aes.Create();
        var iv = aes.IV;
        using var encryptor = aes.CreateEncryptor(key, iv);
        var plainText = Encoding.UTF8.GetBytes(toEncrypt);
        var cipherText = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

        var result = new byte[iv.Length + cipherText.Length];
        iv.CopyTo(result, 0);
        cipherText.CopyTo(result, iv.Length);

        return Convert.ToHexString(result);
    }

    public static string AesDecryptToString(string hexString, string password)
    {
        var encryptedData = HexStringToBytes(hexString);
        var key = GetKey(password);

        using var aes = Aes.Create();
        var iv = encryptedData.Take(AesBlockByteSize).ToArray();
        var cipherText = encryptedData.Skip(AesBlockByteSize).ToArray();

        using var encryptor = aes.CreateDecryptor(key, iv);
        var decryptedBytes = encryptor
            .TransformFinalBlock(cipherText, 0, cipherText.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public static string GenerateSalt(int saltLength)
    {
        if (saltLength < 1)
        {
            throw new ArgumentException("Argument must be a positive number", nameof(saltLength));
        }

        var salt = new byte[saltLength];
        Random.GetBytes(salt);
        return Convert.ToHexString(salt);
    }

    public static byte[] HexStringToBytes(string hexString)
    {
        if (hexString is null)
        {
            throw new ArgumentNullException(nameof(hexString), "Argument cannot be null");
        }

        var bytes = new byte[hexString.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var currentHex = hexString.Substring(i * 2, 2);
            bytes[i] = Convert.ToByte(currentHex, 16);
        }

        return bytes;
    }

    private static byte[] GetKey(string password)
    {
        var keyBytes = Encoding.UTF8.GetBytes(password);
        using var md5 = MD5.Create();
        return md5.ComputeHash(keyBytes);
    }
}