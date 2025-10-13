using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class Movie
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public double Rating { get; set; } = 0;
        public string? PosterUrl { get; set; }
    }
}
