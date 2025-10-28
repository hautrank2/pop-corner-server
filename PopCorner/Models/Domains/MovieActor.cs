using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class MovieActor
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required] 
        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;

        [Required] 
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
