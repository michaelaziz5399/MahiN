using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.SubscriptionDto
{
    public class CreateUpdateSubscriptionPlanDto
    {
        [Required(ErrorMessage = "اسم الخطة مطلوب")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, 100000, ErrorMessage = "يجب أن يكون السعر أكبر من 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "مدة الاشتراك بالأيام مطلوبة")]
        [Range(1, 365, ErrorMessage = "المدة يجب أن تكون بين 1 و 365 يوم")]
        public int DurationInDays { get; set; }

        [Required(ErrorMessage = "المرحلة العمرية مطلوبة")]
        public AgeGroup TargetAgeGroup { get; set; }
    }
}
