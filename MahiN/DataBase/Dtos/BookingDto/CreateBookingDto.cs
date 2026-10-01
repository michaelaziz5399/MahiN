using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.BookingDto
{
    public class CreateBookingDto : IValidatableObject
    {
        [Required(ErrorMessage = "عنوان الحجز مطلوب")]
        [MaxLength(150, ErrorMessage = "لا يمكن أن يتجاوز العنوان 150 حرفاً")]
        public string Title { get; set; } = string.Empty;

        public string CreatedById { get; set; } = string.Empty;

        [Required(ErrorMessage = "يجب تحديد ملعب واحد على الأقل")]
        [MinLength(1, ErrorMessage = "يجب اختيار ملعب واحد على الأقل")]
        public List<int> PlaygroundIds { get; set; } = new();

        [Required(ErrorMessage = "إجمالي المبلغ مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من 0")]
        public decimal TotalAmount { get; set; }

        // المبلغ المدفوع كعربون (اختياري عند الإنشاء، الافتراضي 0)
        [Range(0, double.MaxValue)]
        public decimal AmountPaid { get; set; }=0;

        [Required(ErrorMessage = "تاريخ ووقت البداية مطلوب")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "تاريخ ووقت النهاية مطلوب")]
        public DateTime EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate >= EndDate)
            {
                yield return new ValidationResult("تاريخ ووقت البداية يجب أن يكون قبل تاريخ ووقت النهاية"
                    , new[] { nameof(StartDate), nameof(EndDate) });
            }
            if (AmountPaid > TotalAmount)
            {
                yield return new ValidationResult("المبلغ المدفوع لا يمكن أن يكون أكبر من إجمالي المبلغ"
                    , new[] { nameof(AmountPaid), nameof(TotalAmount) });
            }
        }
    }
}
