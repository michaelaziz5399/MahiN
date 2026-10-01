using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.BookingDto
{
    public class UpdatePaymentDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ الجديد يجب أن يكون أكبر من صفر")]
        public decimal NewPaymentAmount { get; set; }
    }
}
