using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class Rating
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required] public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required] public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [Range(0, 10)] public decimal Score { get; set; }  // map HasPrecision(3,1)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
