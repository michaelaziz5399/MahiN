using MahiN.DataBase.Dtos.TournamentDtos;

namespace MahiN.Services.TournamentService
{
    public interface ITournamentRepository
    {
        // 1. الحصول على قائمة البطولات مع الفلترة حسب الحالة أو الملعب
        Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync(int? playgroundId = null, string? status = null);

        // 2. الحصول على تفاصيل بطولة واحدة شاملة الفرق والمباريات
        Task<TournamentDetailsDto?> GetTournamentByIdAsync(int id);

        // 3. إنشاء بطولة جديدة
        Task<TournamentDto> CreateTournamentAsync(CreateTournamentDto dto);

        // 4. تعديل بيانات بطولة
        Task<TournamentDto?> UpdateTournamentAsync(int id, CreateTournamentDto dto);

        // 5. تحديث حالة البطولة (Ongoing, Completed, Canceled, إلخ)
        Task<bool> UpdateTournamentStatusAsync(int id, UpdateTournamentStatusDto dto);

        // 6. حذف بطولة (في حال عدم وجود مباريات جارية)
        Task<bool> DeleteTournamentAsync(int id);
    }
}
