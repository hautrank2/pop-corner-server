using PopCorner.Helpers;
using PopCorner.Repositories.Interfaces;
using PopCorner.Services.Interfaces;

namespace PopCorner.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IJwtService jwtService,
            IUserRepository userRepository)
        {
            // Lấy Authorization header
            var header = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            var token = header.Replace("Bearer ", "");

            // Parse token lấy userId
            var userId = jwtService.ParseToken(token);

            if (userId == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            // Tìm user trong DB
            var user = await userRepository.GetByIdAsync(userId.Value);

            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User not found.");
                return;
            }

            // Attach vào HttpContext
            context.Items["User"] = user;
            context.Items["UserId"] = user.Id;

            await _next(context);
        }
    }
}
