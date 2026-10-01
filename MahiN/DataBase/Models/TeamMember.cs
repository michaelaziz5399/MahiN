using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class TeamMember
    {
        [Key]
        public int Id { get; set; }

        // ربط العضو باللاعب (AppUser)
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        // ربط العضو بالفريق (Team)
        [Required]
        [ForeignKey(nameof(Team))]
        public int TeamId { get; set; }
        public Team? Team { get; set; }

        public bool IsCaptain { get; set; }
    }
}
