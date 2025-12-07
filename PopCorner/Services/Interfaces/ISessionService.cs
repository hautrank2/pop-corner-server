using PopCorner.Models.Domains;

namespace PopCorner.Services.Interfaces
{
    public interface ISessionService
    {
        bool IsAuthenticated { get; }
        Guid UserId { get; }
        User User { get; }
    }
}
