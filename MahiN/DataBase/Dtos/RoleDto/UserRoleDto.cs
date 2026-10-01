using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.RoleDto
{
    public class UserRoleDto
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم الدور مطلوب")]
        public string RoleId { get; set; } = string.Empty;
    }
}
