namespace PopCorner.Models.Common
{
    public class TokenPayload
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = default!;
        public string Role { get; set; } = "User";
    }
}
