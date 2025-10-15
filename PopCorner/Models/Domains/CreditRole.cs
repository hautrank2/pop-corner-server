﻿using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class CreditRole
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public ICollection<MovieCredit> Credits { get; set; } = new List<MovieCredit>();
    }
}
