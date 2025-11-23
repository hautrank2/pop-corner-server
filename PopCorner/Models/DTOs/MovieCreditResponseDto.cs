using PopCorner.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class MovieCreditResponseDto
    {
        [Required] public Guid MovieId { get; set; }

        [Required] public Guid ArtistId { get; set; }
        public MovieCreditArtist Artist { get; set; } = null!;

        [Required] public int CreditRoleId { get; set; }
        public MovieCreditRoleDto CreditRole { get; set; } = null!;

        [MaxLength(150)] public string? CharacterName { get; set; }
        public int? Order { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
