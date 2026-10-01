using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.MatchDtos
{
    public class CreateTournamentMatchDto
    {
        [Required(ErrorMessage = "معرف البطولة مطلوب")]
        public int TournamentId { get; set; }

        public int? HomeTeamId { get; set; }
        public int? AwayTeamId { get; set; }

        public string? HomePlaceholder { get; set; }
        public string? AwayPlaceholder { get; set; }

        [MaxLength(50)]
        public string? GroupName { get; set; }

        public TournamentStage Stage { get; set; } = TournamentStage.GroupStage;

        [Required(ErrorMessage = "تاريخ ووقت المباراة مطلوب")]
        public DateTime MatchDate { get; set; }

        public int? NextMatchId { get; set; }
    }
}
