using MahiN.DataBase.Enums;

namespace MahiN.DataBase.Dtos.BookingDto
{
    public class UpdateBookingStatusDto
    {
        public BookingStatus Status { get; set; }
        public string ApprovedById { get; set; } = string.Empty;
    }
}
