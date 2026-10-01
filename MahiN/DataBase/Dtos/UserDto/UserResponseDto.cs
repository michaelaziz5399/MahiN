using MahiN.DataBase.Dtos.SubscriptionDto;

namespace MahiN.DataBase.Dtos.UserDto
{
    public class UserResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = "";
        public string? NfcUrl { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string AgeGroup { get; set; } = string.Empty;

        // الأدوار الخاصة بالمستخدم
        public string? Role { get; set; } 

        // تفاصيل الاشتراك الحالي إن وجد
        public SubscriptionResposDto? ActiveSubscription { get; set; }
    }
}
