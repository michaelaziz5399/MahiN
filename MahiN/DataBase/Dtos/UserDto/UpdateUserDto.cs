using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.UserDto
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "معرف المستخدم مطلوب")]
        public string Id { get; set; } = string.Empty;

        // جميع الخصائص الأخرى جعلناها Nullable ليمكن تعديل أي جزء منها بشكل اختياري
        [StringLength(100, MinimumLength = 15)]
        public string? FullName { get; set; }
        public IFormFile? PhotoUrl { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public AgeGroup? AgeGroup { get; set; }

        public Gender? Gender { get; set; }
    }
}
