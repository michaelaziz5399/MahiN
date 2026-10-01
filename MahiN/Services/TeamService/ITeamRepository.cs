using MahiN.DataBase.Dtos.TeamDtos;

namespace MahiN.Services.TeamService
{
    public interface ITeamRepository
    {
        // 1. إنشاء فريق جديد (ويصبح المنشئ هو الكابتن تلقائياً)
        Task<TeamDto> CreateTeamAsync(CreateTeamDto dto, string captainUserId);

        // 2. إضافة لاعب إلى الفريق (مع فحص العقوبات والأماكن الشاغرة)
        Task<TeamMemberDto> AddTeamMemberAsync(AddTeamMemberDto dto);

        // 3. حذف لاعب من الفريق
        Task<bool> RemoveTeamMemberAsync(int teamId, string userId);

        // 4. جلب بيانات فريق محدد مع أعضائه
        Task<TeamDto?> GetTeamByIdAsync(int teamId);

        // 5. جلب جميع فرق بطولة معينة
        Task<IEnumerable<TeamDto>> GetTeamsByTournamentIdAsync(int tournamentId);

        // 6. تحديث حالة دفع رسوم الاشتراك للفريق
        Task<bool> UpdateTeamPaymentStatusAsync(int teamId, bool isPaid);

        // 7. حذف الفريق بالكامل
        Task<bool> DeleteTeamAsync(int teamId);
    }
}
