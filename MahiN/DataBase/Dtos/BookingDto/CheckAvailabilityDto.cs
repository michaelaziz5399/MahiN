namespace MahiN.DataBase.Dtos.BookingDto
{
    public class CheckAvailabilityDto
    {
        public List<int> PlaygroundIds { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? ExcludeBookingId { get; set; }
    }
}
