namespace MahiN.DataBase.Dtos.SubscriptionDto
{
    public class UserSubscriptionResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserPhoneNumber { get; set; } = string.Empty;

        public int SubscriptionPlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
