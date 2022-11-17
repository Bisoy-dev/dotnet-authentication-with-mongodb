using System.Text;
using System.Security.Cryptography;
namespace LearnHttpContext.Helpers;

public class PasswordHelper
{
    public static void Hash(string password, out byte[] passHashed, out byte[] passSalt)
    {
        using var hmac = new HMACSHA512();
        passSalt = hmac.Key;
        passHashed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }


    public static bool Verify(string password, byte[] hashed, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(hashed);
    }
}