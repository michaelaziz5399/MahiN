using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.RoleDto
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "اسم الدور مطلوب")]
        [StringLength(50, ErrorMessage = "اسم الدور يجب ألا يتجاوز 50 حرف")]
        public string Name { get; set; } = string.Empty;
    }
}
