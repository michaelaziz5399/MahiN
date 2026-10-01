using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.TeamDtos
{
    public class AddTeamMemberDto
    {
        [Required(ErrorMessage = "معرف الفريق مطلوب")]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "معرف اللاعب مطلوب")]
        public string UserId { get; set; } = string.Empty;
    }
}
