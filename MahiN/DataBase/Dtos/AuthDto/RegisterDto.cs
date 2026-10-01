using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace MahiN.DataBase.Dtos.AuthDto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, MinimumLength = 15)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }
         // 🎯 استقبال الصورة كملف مرفوع
        public IFormFile? PhotoUrl { get; set; }
        public AgeGroup AgeGroup { get; set; }
    }
}
