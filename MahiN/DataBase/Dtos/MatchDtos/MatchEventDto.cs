namespace MahiN.DataBase.Dtos.MatchDtos
{
    public class MatchEventDto
    {
        public int Id { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public int Minute { get; set; }
    }
}
