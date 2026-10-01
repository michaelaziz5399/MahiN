using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Dtos.TournamentDtos
{
    public class UpdateTournamentStatusDto
    {
        [Required(ErrorMessage = "حالة البطولة مطلوبة")]
        [EnumDataType(typeof(TournamentStatus), ErrorMessage = "حالة البطولة غير صالحة")]
        public TournamentStatus Status { get; set; }
    }
}
