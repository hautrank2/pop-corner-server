using PopCorner.Models.Domains;
using PopCorner.Services.Interfaces;
using System.Security.Claims;

namespace PopCorner.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext HttpContext =>
            _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HttpContext.");

        public bool IsAuthenticated =>
            HttpContext.User?.Identity?.IsAuthenticated ?? false;

        public User User
        {
            get
            {
                // Logic hiện tại: lấy từ HttpContext.Items["User"]
                if (HttpContext.Items["User"] is User userFromItem)
                {
                    return userFromItem;
                }

                // Nếu sau này bạn đổi sang lấy từ Claims:
                var userIdClaim = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? HttpContext.User?.FindFirst("sub")?.Value;

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    // TODO: nếu cần có thể inject IUserRepository để load từ DB
                    // _userRepository.GetByIdAsync(userId) ...
                    throw new InvalidOperationException(
                        "User entity is not loaded. Implement loading logic here.");
                }

                throw new InvalidOperationException("Current user not found.");
            }
        }

        public Guid UserId => User.Id;
    }
}
