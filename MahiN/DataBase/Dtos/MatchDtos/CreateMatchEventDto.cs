using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.MatchDtos
{
    public class CreateMatchEventDto
    {
        [Required]
        public int MatchId { get; set; }

        [Required]
        public string PlayerId { get; set; } = string.Empty;

        [Required]
        public int TeamId { get; set; }

        [Required]
        public MatchEventType EventType { get; set; }

        [Range(1, 130)]
        public int Minute { get; set; }
    }
}
