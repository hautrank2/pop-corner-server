using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PopCorner.Services.Interfaces;        
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Services
{
    public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "CustomToken";

        private readonly IJwtService jwtService;
        private readonly IUserRepository userRepository;

        public AuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IJwtService jwtService,
            IUserRepository userRepo
        ) : base(options, logger, encoder, clock)
        {
            this.jwtService = jwtService;
            userRepository = userRepo;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 1. Lấy Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                // Không có token → để NoResult, để các scheme khác (nếu có) xử lý
                return AuthenticateResult.NoResult();
            }

            var token = authHeader["Bearer ".Length..].Trim();

            // 2. Validate token bằng JwtService
            var payload = jwtService.Validate(token);
            Console.WriteLine($"[AuthHandler] After jwtService.Validate: payload null? {payload == null}");

            if (payload == null)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            // Optional: cross-check user trong DB
            var user = await userRepository.GetByIdAsync(payload.UserId);

            Console.WriteLine($"user: {user}");
            if (user == null)
            {
                return AuthenticateResult.Fail("User not found or inactive");
            }

            // 3. Tạo claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            Console.WriteLine($"{claims.Count} claims");
            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return AuthenticateResult.Success(ticket);
        }
    }
}
