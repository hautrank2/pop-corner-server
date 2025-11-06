using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class RemoveFileDto
    {
        [Required]
        public string Pathname { get; set; } = String.Empty;
        
    }
}
