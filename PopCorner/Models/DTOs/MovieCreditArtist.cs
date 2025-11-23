namespace PopCorner.Models.DTOs
{
    public class MovieCreditArtist
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
