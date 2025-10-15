using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class Artist
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(2000)]
        public string? Bio { get; set; }

        [Url, MaxLength(500)]
        public string? AvatarUrl { get; set; }
        public ICollection<MovieCredit> Credits { get; set; } = new List<MovieCredit>();
    }
}
