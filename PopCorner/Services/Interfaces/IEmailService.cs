namespace PopCorner.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
        Task SendOtpAsync(string to, string otp);
    }
}
