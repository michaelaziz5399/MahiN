using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.PlaygroundDtos
{
    public class CreatePlaygroundDto
    {
        [Required(ErrorMessage = "اسم الملعب مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الملعب لا يتجاوز 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف لا يتجاوز 500 حرف")]
        public string? Description { get; set; }

        // لرفع صورة الملعب
        public IFormFile? Photo { get; set; }

        [Required(ErrorMessage = "نوع الملعب مطلوب")]
        public PlaygroundType PlaygroundType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "السعة يجب أن تكون شخص واحد على الأقل")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "سعر الساعة مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "سعر الساعة يجب أن يكون رقم موجب")]
        public decimal HourlyRate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
