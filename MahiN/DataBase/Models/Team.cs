using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفريق مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الفريق لا يمكن أن يتجاوز 100 حرف")]
        public string Name { get; set; } = string.Empty;

        // ربط الفريق بالبطولة
        [Required]
        [ForeignKey(nameof(Tournament))]
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        [MaxLength(50)]
        public string? GroupName { get; set; } // مثال: "Group A", "المجموعة 1"

        // حالة الدفع وتاريخ الإنشاء
        [Required]
        public bool IsPaid { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // أعضاء الفريق
        public ICollection<TeamMember> Members { get; set; } = new HashSet<TeamMember>();
    }
}
