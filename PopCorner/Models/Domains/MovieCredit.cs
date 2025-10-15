using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class MovieCredit
    {
        [Required] public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [Required] public Guid ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;

        [Required] public int CreditRoleId { get; set; }       
        public CreditRole CreditRole { get; set; } = null!;

        [MaxLength(150)] public string? CharacterName { get; set; }
        public int? Order { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}
