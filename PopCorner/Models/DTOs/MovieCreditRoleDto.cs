using PopCorner.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class MovieCreditRoleDto
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
