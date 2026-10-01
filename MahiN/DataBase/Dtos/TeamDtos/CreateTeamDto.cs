using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.TeamDtos
{
    public class CreateTeamDto
    {
        [Required(ErrorMessage = "اسم الفريق مطلوب")]
        [StringLength(100, ErrorMessage = "اسم الفريق لا يتجاوز 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "معرف البطولة مطلوب")]
        public int TournamentId { get; set; }
    }
}
