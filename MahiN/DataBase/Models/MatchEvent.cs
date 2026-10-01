using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class MatchEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Match))]
        public int MatchId { get; set; }

        public TournamentMatch Match { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Player))]
        public string PlayerId { get; set; } = string.Empty;

        public AppUser Player { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Team))]
        public int TeamId { get; set; }

        public Team Team { get; set; } = null!;

        [Required]
        [EnumDataType(typeof(MatchEventType))]
        public MatchEventType EventType { get; set; }

        [Required]
        [Range(1, 130, ErrorMessage = "الدقيقة يجب أن تكون بين 1 و 130")]
        public int Minute { get; set; }
    }
}
