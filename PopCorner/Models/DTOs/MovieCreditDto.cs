using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class MovieCreditDto
    {
        [Required]
        public Guid ArtistId { get; set; }

        [Required]
        public int CreditRoleId { get; set; }

        [Required]
        public string CharacterName { get; set; } = string.Empty;
        public int? Order { get; set; } = -1;
    }
}
