namespace Core.Interfaces.Services;

public interface IEncryptionService
{
    string Encrypt(string plainText, string key);
    string Decrypt(string encrypted, string key);
}