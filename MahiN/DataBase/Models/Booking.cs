using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        public ICollection<BookingPlayground> BookingPlaygrounds { get; set; } = new HashSet<BookingPlayground>();

        // 🎯 1. إجمالي سعر الحجز الكلي (مثلاً: 500 جنيه)
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // 🎯 2. المبلغ المدفوع فعلياً حتى الآن (العربون أو المبلغ كاملاً)
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; } = 0;

        // 🎯 3. خاصية محسوبة (Calculated Property) لمعرفة المتبقي تلقائياً
        public decimal RemainingAmount => TotalAmount - AmountPaid;

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public string? ApprovedById { get; set; }
        [ForeignKey(nameof(ApprovedById))]
        public AppUser? ApprovedBy { get; set; }

        public string CreatedById { get; set; } = string.Empty;
        [ForeignKey(nameof(CreatedById))]
        public AppUser? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
