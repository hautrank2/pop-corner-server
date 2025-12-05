using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class MovieRateDto
    {
        [Range(0, 10)] public decimal Score { get; set; }
    }
}
