using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PopCorner.Models.Domains
{
    public class Artist
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, Column(TypeName = "date")]

        public DateTime? Birthday { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(2000)]
        public string? Bio { get; set; }

        [Url, MaxLength(500)]
        public string? AvatarUrl { get; set; }
        public ICollection<MovieCredit> Credits { get; set; } = new List<MovieCredit>();
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();

    }
}
