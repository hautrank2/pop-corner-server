using PopCorner.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class MovieReaction
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required] public Guid MovieId { get; set; }
        [Required] public Guid UserId { get; set; }

        [Required] public ReactionType ReactionType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Movie Movie { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
