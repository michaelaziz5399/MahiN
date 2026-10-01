namespace MahiN.DataBase.Dtos.SuspensionDtos
{
    public class PlayerSuspensionDto
    {
        public int Id { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Restriction { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int? TournamentId { get; set; }
        public string? TournamentName { get; set; }
        public string Scope { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int NumberOfTournaments { get; set; }
        public bool IsActive { get; set; }
    }
}
