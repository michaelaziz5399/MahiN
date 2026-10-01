using MahiN.DataBase.Dtos.SuspensionDtos;

namespace MahiN.Services.SuspensionService
{
    public interface ISuspensionRepository
    {
        // 1. إضافة عقوبة جديدة
        Task<PlayerSuspensionDto> CreateSuspensionAsync(CreateSuspensionDto dto);

        // 2. إلغاء عقوبة يدوياً (فك الحظر)
        Task<bool> DeactivateSuspensionAsync(int suspensionId);

        // 3. جلب جميع العقوبات النشطة للاعب
        Task<IEnumerable<PlayerSuspensionDto>> GetActiveSuspensionsByPlayerIdAsync(string playerId);

        // 4. جلب السجل التاريخي الكامل لعقوبات لاعب
        Task<IEnumerable<PlayerSuspensionDto>> GetPlayerSuspensionHistoryAsync(string playerId);

        // 5. فحص صلاحية المشاركة في بطولة محددة
        Task<bool> IsPlayerSuspendedFromTournamentsAsync(string playerId, int targetTournamentId);

        // 6. فحص صلاحية حجز الملعب العادي
        Task<bool> IsPlayerSuspendedFromBookingAsync(string playerId);

        // 7. فحص ما إذا كان الحساب حظراً كلياً
        Task<bool> IsAccountFullyBlockedAsync(string playerId);

        // 8. إغلاق العقوبات المنتهية تلقائياً
        Task<int> AutoCloseExpiredSuspensionsAsync();
    }
}
