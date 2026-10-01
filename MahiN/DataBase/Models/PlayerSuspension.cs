using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class PlayerSuspension
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PlayerId { get; set; } = string.Empty;

        [ForeignKey(nameof(PlayerId))]
        public AppUser Player { get; set; } = null!;

        // سبب ونوع المخالفة
        public SuspensionCategory Category { get; set; } // ملعب / مدارس أحد / اشتراكات

        [Required(ErrorMessage = "سبب العقوبة مطلوب")]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        // ما هو الشيء المحروم منه؟
        public RestrictedAction Restriction { get; set; } // حرمان من دورة / حرمان من حجز ملعب / حرمان كلي

        // البطولة المرتبطة (إن وجدت)
        public int? TournamentId { get; set; }

        [ForeignKey(nameof(TournamentId))]
        public Tournament? Tournament { get; set; }

        // المدة ونطاق العقوبة
        public SuspensionScope Scope { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public int NumberOfTournaments { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }
}
