using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class MovieActor
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required] public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required] public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [Range(1, 10)]
        public int Score { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
