namespace PopCorner.Services.Interfaces
{
    public interface IAuthService
    {
        // OTP
        string CreateOtp(string email);

        string HashOtp(string otp);

        Task<bool> SaveOtp(string key, string hashOtp);

        Task<bool> VerifyOtp(string hashOtp, string targetOtp);

        Task<bool> RemoveOtp(string key);

        string? GetHashOtp(string key);
    }
}
