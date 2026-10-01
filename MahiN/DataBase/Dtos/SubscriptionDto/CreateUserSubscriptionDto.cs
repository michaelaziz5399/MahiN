using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MahiN.DataBase.Dtos.SubscriptionDto
{
    public class CreateUserSubscriptionDto
    {
        [Required(ErrorMessage = "برجاء اضافه مستخدم")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "برجاء اضافه خطه الاشتراك")]
        public int SubscriptionPlanId { get; set; }

        [Required(ErrorMessage = "برجاء اضافه قيمه الاشتراك")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePaid { get; set; }
    }
}
