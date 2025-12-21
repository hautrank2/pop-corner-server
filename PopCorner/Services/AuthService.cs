using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using PopCorner.Helpers;
using PopCorner.Services.Interfaces;
using System.Security.Cryptography;

namespace PopCorner.Services
{
    public class AuthService : IAuthService
    {
        private readonly TimeSpan expiration = TimeSpan.FromSeconds(60 * 5); // 5 minutes
        private readonly IMemoryCache cache;

        public AuthService(IMemoryCache cache)
        {
           this.cache = cache;
        }

        public async Task<string> CreateOtpAsync(string email)
        {
            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            return otp;
        }

        public async Task<string> HashOtp(string otp)
        {
            var hash = PasswordHelper.Hash(otp);
            return hash;
        }

        public async Task<bool> SaveOtp(string key, string hashOtp)
        {
            cache.Set(key, hashOtp, expiration);
            return true;
        }

        public async Task<string?> GetHashOtp(string key)
        {
            var result = cache.Get(key);
            if (result == null) 
            { 
                return null; 
            }
            return result.ToString();
        }

        public async Task<bool> VerifyOtp(string hashOtp, string targetOtp)
        {
            var isOk = PasswordHelper.Verify(hashOtp, targetOtp);

            return isOk;
        }

        public async Task<bool> RemoveOtp(string key)
        {
            cache.Remove(key);
            return true;
        }
    }
}
