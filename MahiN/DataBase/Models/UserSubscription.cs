using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class UserSubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }

        public int SubscriptionPlanId { get; set; }
        [ForeignKey(nameof(SubscriptionPlanId))]
        public SubscriptionPlan? SubscriptionPlan { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePaid { get; set; }
        public bool IsActive { get; set; }

    }
}
