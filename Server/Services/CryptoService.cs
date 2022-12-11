using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.Server.Services;

public interface ICryptoService
{
    (string hashedPassword, string salt) HashSha512(string password, string? salt = null);
    (string hashedPassword, string salt) HashHmac(string password, byte[]? key = null);
    string AesEncryptToHexString(string toEncrypt, string password);
    string AesDecryptToString(string hexString, string password);
    string GenerateSalt(int saltLength);
    byte[] HexStringToBytes(string hexString);
}

public class CryptoService : ICryptoService
{
    private readonly AppSettings _appSettings;

    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();
    private const int AesBlockByteSize = 128 / 8;

    public CryptoService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public (string hashedPassword, string salt) HashSha512(string password, string? salt = null)
    {
        salt ??= GenerateSalt(128);
        password = _appSettings.PasswordPepper + salt + password;
        var bytes = Encoding.UTF8.GetBytes(password);
        var sha512 = SHA512.HashData(bytes);
        return (Convert.ToHexString(sha512), salt);
    }

    public (string hashedPassword, string salt) HashHmac(string password, byte[]? key = null)
    {
        using var hmac = key is null ? new HMACSHA512() : new HMACSHA512(key);
        key = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (Convert.ToHexString(hash), Convert.ToHexString(key));
    }

    public string AesEncryptToHexString(string toEncrypt, string password)
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

    public string AesDecryptToString(string hexString, string password)
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

    public string GenerateSalt(int saltLength)
    {
        if (saltLength < 1)
        {
            throw new ArgumentException("Argument must be a positive number", nameof(saltLength));
        }

        var salt = new byte[saltLength / 2];
        Random.GetBytes(salt);
        return Convert.ToHexString(salt);
    }

    public byte[] HexStringToBytes(string hexString)
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