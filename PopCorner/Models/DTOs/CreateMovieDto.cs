using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class CreateMovieDto
    {
        [Required] public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public double Rating { get; set; } = 0;
        public string? PosterUrl { get; set; }
    }
}
