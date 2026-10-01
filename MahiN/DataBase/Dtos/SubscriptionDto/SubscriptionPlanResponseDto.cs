using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Dtos.SubscriptionDto
{
    public class SubscriptionPlanResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public AgeGroup TargetAgeGroup { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
