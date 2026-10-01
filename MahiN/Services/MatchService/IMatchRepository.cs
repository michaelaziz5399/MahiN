using MahiN.DataBase.Dtos.MatchDtos;

namespace MahiN.Services.MatchService
{
    public interface IMatchRepository
    {
        // 1. إنشاء مباراة جديدة
       Task<TournamentMatchDto> CreateMatchAsync(CreateTournamentMatchDto dto);

        // 2. تحديث نتيجة مباراة وتحديد الفائز
        Task<TournamentMatchDto?> UpdateMatchResultAsync(int matchId, UpdateMatchResultDto dto);

        // 3. تسجيل حدث داخل المباراة (هدف، كارت، إلخ)
        Task<MatchEventDto> AddMatchEventAsync(CreateMatchEventDto dto);

        // 4. حذف حدث من المباراة
        Task<bool> DeleteMatchEventAsync(int eventId);

        // 5. جلب بيانات مباراة معينة
        Task<TournamentMatchDto?> GetMatchByIdAsync(int matchId);

        // 6. جلب جميع مباريات بطولة محددة
        Task<IEnumerable<TournamentMatchDto>> GetMatchesByTournamentIdAsync(int tournamentId);

        // 7. جلب جميع أحداث مباراة محددة
        Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(int matchId);

        // 8. حذف مباراة
        Task<bool> DeleteMatchAsync(int matchId);
    }
}
