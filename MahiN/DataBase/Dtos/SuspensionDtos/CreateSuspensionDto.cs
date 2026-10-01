using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.SuspensionDtos
{
    public class CreateSuspensionDto
    {
        [Required(ErrorMessage = "معرف اللاعب مطلوب")]
        public string PlayerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "مصدر المخالفة مطلوب")]
        public SuspensionCategory Category { get; set; }

        [Required(ErrorMessage = "نوع الحرمان مطلوب")]
        public RestrictedAction Restriction { get; set; }

        [Required(ErrorMessage = "سبب العقوبة مطلوب")]
        [MaxLength(500, ErrorMessage = "سبب العقوبة لا يمكن أن يتجاوز 500 حرف")]
        public string Reason { get; set; } = string.Empty;

        public int? TournamentId { get; set; }

        [Required(ErrorMessage = "نطاق زمن العقوبة مطلوب")]
        public SuspensionScope Scope { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(1, 20, ErrorMessage = "عدد البطولات يجب أن يكون بين 1 و 20")]
        public int NumberOfTournaments { get; set; } = 1;
    }
}
