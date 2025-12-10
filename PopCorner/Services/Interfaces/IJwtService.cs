using PopCorner.Models.Common;

namespace PopCorner.Services.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(Guid userId, string email, string role);
        Guid? ParseToken(string token);
        TokenPayload? Validate(string token);
    }

}
