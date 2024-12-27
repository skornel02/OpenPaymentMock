using System.Security.Cryptography;

namespace OpenPaymentMock.Server.Services;

public static class AccessKeyService
{
    public static string RandomAccessToken { 
        get
        {
            var secureBytes = RandomNumberGenerator.GetBytes(128);
            var encoded = Convert.ToBase64String(secureBytes);

            return encoded;
        } 
    }
}
