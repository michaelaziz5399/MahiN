using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.AuthDto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
