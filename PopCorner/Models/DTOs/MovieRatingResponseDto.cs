using PopCorner.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class MovieRatingResponseDto
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required] public Guid UserId { get; set; }
        public UserDto User { get; set; } = null!;

        [Range(0, 10)] public decimal Score { get; set; }  // map HasPrecision(3,1)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
