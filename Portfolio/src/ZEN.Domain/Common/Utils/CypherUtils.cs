using System.Security.Cryptography;
using System.Text;

namespace ZEN.Domain.Common.Utils;

public static class CypherUtils
{
    public static string ToSHA256(this string src, string salt, int iterations = 10)
    {

        byte[] crypto = SHA256.HashData(Encoding.ASCII.GetBytes(src + salt));

        for (int i = 1; i < iterations; i++)
        {
            crypto = SHA256.HashData(crypto);
        }

        StringBuilder hash = new(crypto.Length * 2);
        foreach (byte theByte in crypto)
        {
            hash.Append(theByte.ToString("x2"));
        }

        return hash.ToString();
    }

    public static string GenerateHMACSHA256Signature(string data, string key)
    {
        using HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(key));
        byte[] hashBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(data));

        StringBuilder hash = new(hashBytes.Length * 2);
        foreach (byte theByte in hashBytes)
        {
            hash.Append(theByte.ToString("x2"));
        }

        return hash.ToString();
    }

    public static bool VerifyHMACSHA256Signature(string data, string key, string providedSignature)
    {
        // Tạo chữ ký từ dữ liệu và khóa
        string calculatedSignature = GenerateHMACSHA256Signature(data, key);

        // So sánh chữ ký đã cho với chữ ký được tính toán
        return calculatedSignature.Equals(providedSignature, StringComparison.OrdinalIgnoreCase);
    }

}