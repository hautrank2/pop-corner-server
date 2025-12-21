namespace PopCorner.Services.Interfaces
{
    public interface IAuthService
    {
        // OTP
        Task<string> CreateOtpAsync(string email);

        Task<string> HashOtp(string otp);

        Task<bool> SaveOtp(string key, string hashOtp);

        Task<bool> VerifyOtp(string hashOtp, string targetOtp);

        Task<bool> RemoveOtp(string key);

        Task<string?> GetHashOtp(string key);
    }
}
