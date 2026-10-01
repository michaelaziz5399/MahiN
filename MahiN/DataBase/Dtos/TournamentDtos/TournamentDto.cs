namespace MahiN.DataBase.Dtos.TournamentDtos
{
    public class TournamentDto
    {
        public int Id { get; set; }
        public int PlaygroundId { get; set; }
        public string PlaygroundName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal RegistrationFee { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TeamsCount { get; set; }
    }
}
