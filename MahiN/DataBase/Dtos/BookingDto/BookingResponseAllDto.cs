
namespace MahiN.DataBase.Dtos.BookingDto
{
    public class BookingResponseAllDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal RemainingAmount => TotalAmount - AmountPaid;
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
    }
}
