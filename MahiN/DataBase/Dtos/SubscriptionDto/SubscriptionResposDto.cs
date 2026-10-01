namespace MahiN.DataBase.Dtos.SubscriptionDto
{
    public class SubscriptionResposDto
    {
        public int Id { get; set; }
        public int SubscriptionPlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
