using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class TournamentMatch
    {
        [Key]
        public int Id { get; set; }

        // ربط المباراة بالبطولة
        [Required]
        [ForeignKey(nameof(Tournament))]
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        // الفرق المنافسة (قابل للـ null للأدوار المستقبلية)
        [ForeignKey(nameof(HomeTeam))]
        public int? HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; }

        [ForeignKey(nameof(AwayTeam))]
        public int? AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; }

        // النصوص التوضيحية قبل معرفة المتأهل (مثل: "1st Group A")
        public string? HomePlaceholder { get; set; }
        public string? AwayPlaceholder { get; set; }

        // اسم المجموعة (جديد)
        [MaxLength(50)]
        public string? GroupName { get; set; } // مثال: "Group A", "المجموعة 1"

        // الدور الحقيقي للمباراة (مجموعات، ربع نهائي، نهائي...)
        public TournamentStage Stage { get; set; }

        [Required]
        public DateTime MatchDate { get; set; }

        // الأهداف والنتيجة
        [Range(0, 100, ErrorMessage = "عدد الأهداف لا يمكن أن يكون بالسالب")]
        public int? HomeScore { get; set; }

        [Range(0, 100, ErrorMessage = "عدد الأهداف لا يمكن أن يكون بالسالب")]
        public int? AwayScore { get; set; }

        // تفاصيل التمديد وركلات الترجيح (جديد)
        public bool IsExtraTime { get; set; } = false;
        public bool IsPenaltyShootout { get; set; } = false;

        [Range(0, 100, ErrorMessage = "عدد ركلات الترجيح لا يمكن أن يكون بالسالب")]
        public int? HomePenaltyScore { get; set; }

        [Range(0, 100, ErrorMessage = "عدد ركلات الترجيح لا يمكن أن يكون بالسالب")]
        public int? AwayPenaltyScore { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        // الفائز بالمباراة
        [ForeignKey(nameof(WinnerTeam))]
        public int? WinnerTeamId { get; set; }
        public Team? WinnerTeam { get; set; }

        // ربط المباراة بالمباراة التالية في الشجرة الإقصائية (Bracket)
        [ForeignKey(nameof(NextMatch))]
        public int? NextMatchId { get; set; }
        public TournamentMatch? NextMatch { get; set; }

        // الأحداث الخاصة بالمباراة (أهداف، كروت...)
        public ICollection<MatchEvent> Events { get; set; } = new HashSet<MatchEvent>();
    }
}