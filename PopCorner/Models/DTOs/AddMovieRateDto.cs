using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class AddMovieRateDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(0, 10)]
        public float Score { get; set; }
    }
}
