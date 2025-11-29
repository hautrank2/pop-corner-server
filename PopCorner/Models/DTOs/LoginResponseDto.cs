namespace PopCorner.Models.DTOs
{
    public class LoginResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string? AvatarUrl { get; set; }

        public string Role { get; set; } = "";

        public string Token { get; set; } = "";
    }
}
