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

        public Guid UserId
        {
            get
            {
                // 1. Ưu tiên đọc từ Items (AuthHandler đã set)
                if (HttpContext.Items.TryGetValue("UserId", out var raw)
                    && raw is Guid idFromItems)
                {
                    return idFromItems;
                }

                // 2. Fallback: lấy từ claim
                var idClaim =
                    HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    HttpContext.User?.FindFirst("sub")?.Value;

                if (Guid.TryParse(idClaim, out var idFromClaim))
                {
                    return idFromClaim;
                }

                // 3. Không có user → ném lỗi
                throw new InvalidOperationException("No authenticated user id found in current context.");
            }
        }

        public string? Email =>
            HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? Role =>
            HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value ?? "User";

        public User? User
        {
            get
            {
                if (HttpContext.Items.TryGetValue("User", out var raw)
                    && raw is User u)
                {
                    return u;
                }

                // Nếu muốn lazy-load từ DB thì inject IUserRepository vào
                // và dùng UserId để load. Còn không, cứ để null.
                return null;
            }
        }
    }
}
