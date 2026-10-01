using MahiN.DataBase.Dtos.PlaygroundDtos;

namespace MahiN.DataBase.Dtos.BookingDto
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // البيانات المالية
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal RemainingAmount => TotalAmount - AmountPaid;


        // الحالات (مُحولة إلى string لسهولة القراءة في الفرونت إند)
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;

        // التواريخ
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }


        // معلومات من أنشأ الحجز
        public string CreatedById { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;

        // معلومات من اعتمد الحجز
        public string? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }

        public List<PlaygroundBasicDto> Playgrounds { get; set; } = new();
        public bool IsExpired { get; set; }
    }
}
