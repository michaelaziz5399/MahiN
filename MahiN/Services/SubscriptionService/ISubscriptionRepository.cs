using MahiN.DataBase.Dtos.SubscriptionDto;
using MahiN.DataBase.Enums;
using Microsoft.AspNetCore.Identity;

namespace MahiN.Services.SubscriptionService
{
    public interface ISubscriptionRepository
    {
        // ==================== 1.(Subscription Plans) ====================

        Task<IEnumerable<SubscriptionPlanResponseDto>> GetAllPlansAsync();
        Task<SubscriptionPlanResponseDto?> GetPlanByIdAsync(int planId);
        Task<SubscriptionPlanResponseDto> CreatePlanAsync(CreateUpdateSubscriptionPlanDto model);
        Task<SubscriptionPlanResponseDto?> UpdatePlanAsync(int planId, CreateUpdateSubscriptionPlanDto model);
        Task<bool> TogglePlanStatusAsync(int planId);
        Task<IEnumerable<SubscriptionPlanResponseDto>> GetPlansByAgeGroupAsync(AgeGroup ageGroup);
        Task<IdentityResult> DeletePlanAsync(int PlanId);

        // ==================== 2. (User Subscriptions) ====================

        // إنشاء اشتراك جديد لمستخدم
        Task<UserSubscriptionResponseDto?> SubscribeUserAsync(CreateUserSubscriptionDto model);

        // إلغاء/إيقاف اشتراك حالي لمستخدم
        Task<bool> CancelUserSubscriptionAsync(int userSubscriptionId);

        // جلب الاشتراك الحالي والفعّال لمستخدم معين
        Task<UserSubscriptionResponseDto?> GetActiveSubscriptionByUserIdAsync(string userId);

        // جلب سجل كافة الاشتراكات (السابقة والحالية) لمستخدم معين
        Task<IEnumerable<UserSubscriptionResponseDto>> GetUserSubscriptionHistoryAsync(string userId);

        // جلب قائمة بكل اشتراكات النظام (للأدمن مع إمكانية الفلترة)
        Task<IEnumerable<UserSubscriptionResponseDto>> GetAllUserSubscriptionsAsync(bool? activeOnly = null);
    }
}
