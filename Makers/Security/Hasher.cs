using Makers.Utilities;
using System.Security.Cryptography;
using System.Text;


namespace Makers.Security;

public static class Hasher
{
    public static void GetHashedPassword(string providedPassword, out string hashedPassword, out string salt)
    {
        byte[] saltBytes = new byte[20];

        RandomNumberGenerator.Create().GetNonZeroBytes(saltBytes);

        salt = Convert.ToBase64String(saltBytes);

        using Rfc2898DeriveBytes hasher = new(providedPassword, saltBytes, 10000);

        hashedPassword = Convert.ToBase64String(hasher.GetBytes(20));
    }

    public static void CheckHashedPassword(string providedPassword, string providedSalt, out string hashedPassword)
    {
        using Rfc2898DeriveBytes hasher = new(providedPassword, providedSalt.Base64Decode(), 10000);

        hashedPassword = Convert.ToBase64String(hasher.GetBytes(20));
    }

    public static string GetSHA512(byte[] data)
    {
        using SHA512 sha = SHA512.Create();

        StringBuilder sb = new();

        data = sha.ComputeHash(data);

        foreach (byte by in data)
        {
            sb.Append(by.ToString("x2"));
        }

        return sb.ToString();
    }
}