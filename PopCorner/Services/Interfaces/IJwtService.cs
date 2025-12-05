namespace PopCorner.Services.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(Guid userId, string email, string role);
        Guid? ParseToken(string token);

    }
}
