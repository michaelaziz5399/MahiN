using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.MatchDtos
{
    public class UpdateMatchResultDto
    {
        [Range(0, 100, ErrorMessage = "عدد الأهداف لا يمكن أن يكون بالسالب")]
        public int HomeScore { get; set; }

        [Range(0, 100, ErrorMessage = "عدد الأهداف لا يمكن أن يكون بالسالب")]
        public int AwayScore { get; set; }

        public bool IsExtraTime { get; set; } = false;
        public bool IsPenaltyShootout { get; set; } = false;

        [Range(0, 100, ErrorMessage = "عدد ركلات الترجيح لا يمكن أن يكون بالسالب")]
        public int? HomePenaltyScore { get; set; }

        [Range(0, 100, ErrorMessage = "عدد ركلات الترجيح لا يمكن أن يكون بالسالب")]
        public int? AwayPenaltyScore { get; set; }

        public int? WinnerTeamId { get; set; }
        public bool IsCompleted { get; set; } = true;
    }
}
