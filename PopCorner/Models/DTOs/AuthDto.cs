using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class EditPasswordDto
    {
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    public class ForgotPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }

    public class VerifiOtp
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Otp { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Otp { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
