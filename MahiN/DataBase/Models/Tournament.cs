using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class Tournament
    {
        [Key]
        public int Id { get; set; }

        // ربط البطولة بالملعب (Playground)
        [Required]
        [ForeignKey(nameof(Playground))]
        public int PlaygroundId { get; set; }
        public Playground Playground { get; set; } = null!;

        // بيانات البطولة
        [Required(ErrorMessage = "اسم البطولة مطلوب")]
        [MaxLength(150, ErrorMessage = "اسم البطولة لا يمكن أن يتجاوز 150 حرف")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف لا يمكن أن يتجاوز 500 حرف")]
        public string? Description { get; set; }

        [Required]
        [EnumDataType(typeof(TournamentStatus))]
        public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100000, ErrorMessage = "رسوم الاشتراك يجب أن تكون قيمة موجبة")]
        public decimal RegistrationFee { get; set; }

        // التواريخ
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        // شروط أفراد الفريق
        [Required]
        [Range(1, 30, ErrorMessage = "الحد الأدنى يجب أن يكون على الأقل لاعب واحد")]
        public int MinPlayersPerTeam { get; set; }

        [Required]
        [Range(1, 30, ErrorMessage = "الحد الأقصى يجب أن يكون قيماً منطقية")]
        public int MaxPlayersPerTeam { get; set; }

        [Required]
        [EnumDataType(typeof(TournamentType))]
        public TournamentType Type { get; set; } // نوع البطولة

        // العلاقات
        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();
        public ICollection<TournamentMatch> Matches { get; set; } = new HashSet<TournamentMatch>();
    }
}
