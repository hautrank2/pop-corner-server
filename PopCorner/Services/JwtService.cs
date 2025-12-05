using Microsoft.IdentityModel.Tokens;
using PopCorner.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PopCorner.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly TokenValidationParameters _tokenParams;

        public JwtService(IConfiguration config)
        {
            _config = config;

            var jwt = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwt["Key"]);

            _tokenParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwt["Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Không delay 5p mặc định
            };
        }

        public string GenerateToken(Guid userId, string email, string role)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
        };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid? ParseToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, _tokenParams, out var validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                var sub = jwtToken!.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                return Guid.Parse(sub);
            }
            catch
            {
                return null;
            }
        }
    }
}
