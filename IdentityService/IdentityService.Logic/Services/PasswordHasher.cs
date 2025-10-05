using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using CoreLib.Interfaces;
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32));
        return $"{Convert.ToBase64String(salt)}.{hash}";
    }

    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32));
        return hash == parts[1];
    }
}