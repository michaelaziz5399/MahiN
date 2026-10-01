using MahiN.DataBase.Enums;

namespace MahiN.DataBase.Dtos.MatchDtos
{
    public class TournamentMatchDto
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }

        public int? HomeTeamId { get; set; }
        public string? HomeTeamName { get; set; }
        public string? HomePlaceholder { get; set; }

        public int? AwayTeamId { get; set; }
        public string? AwayTeamName { get; set; }
        public string? AwayPlaceholder { get; set; }

        public string? GroupName { get; set; }

        public TournamentStage Stage { get; set; }
        public string StageName { get; set; } = string.Empty;

        public DateTime MatchDate { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public bool IsExtraTime { get; set; }
        public bool IsPenaltyShootout { get; set; }
        public int? HomePenaltyScore { get; set; }
        public int? AwayPenaltyScore { get; set; }

        public bool IsCompleted { get; set; }

        public int? WinnerTeamId { get; set; }
        public string? WinnerTeamName { get; set; }

        public int? NextMatchId { get; set; }
    }
}
