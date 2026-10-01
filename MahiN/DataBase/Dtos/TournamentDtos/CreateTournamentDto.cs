using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.TournamentDtos
{
    public class CreateTournamentDto
    {
        [Required(ErrorMessage = "الملعب مطلوب")]
        public int PlaygroundId { get; set; }

        [Required(ErrorMessage = "اسم البطولة مطلوب")]
        [StringLength(150, ErrorMessage = "اسم البطولة لا يتجاوز 150 حرف")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "الوصف لا يتجاوز 500 حرف")]
        public string? Description { get; set; }

        [Range(0, 100000, ErrorMessage = "رسوم الاشتراك يجب أن تكون قيمة موجبة")]
        public decimal RegistrationFee { get; set; }

        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        public DateOnly EndDate { get; set; }

        [Range(1, 30, ErrorMessage = "الحد الأدنى للاعبين بين 1 و 30")]
        public int MinPlayersPerTeam { get; set; }

        [Range(1, 30, ErrorMessage = "الحد الأقصى للاعبين بين 1 و 30")]
        public int MaxPlayersPerTeam { get; set; }
    }
}
