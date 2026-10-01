namespace MahiN.DataBase.Models
{
    public class BookingPlayground
    {
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public int PlaygroundId { get; set; }
        public Playground? Playground { get; set; }
    }
}
