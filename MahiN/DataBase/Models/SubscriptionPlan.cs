using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public AgeGroup TargetAgeGroup { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new HashSet<UserSubscription>();
    }
}
