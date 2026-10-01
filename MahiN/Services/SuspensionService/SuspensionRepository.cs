using AutoMapper;
using MahiN.DataBase.Data;
using MahiN.DataBase.Dtos.SuspensionDtos;
using MahiN.DataBase.Enums;
using MahiN.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.SuspensionService
{
    public class SuspensionRepository(ApplicationDbContext _dbContext, IMapper _mapper) : ISuspensionRepository
    {
        // 1. إضافة عقوبة جديدة
        public async Task<PlayerSuspensionDto> CreateSuspensionAsync(CreateSuspensionDto dto)
        {
            var playerExists = await _dbContext.Users.AnyAsync(u => u.Id == dto.PlayerId);
            if (!playerExists)
            {
                throw new KeyNotFoundException("اللاعب المحدد غير موجود.");
            }

            // التحقق المنطقي حسب نوع الـ Scope
            if (dto.Scope == SuspensionScope.CurrentTournamentOnly && !dto.TournamentId.HasValue)
            {
                throw new InvalidOperationException("يجب تحديد البطولة عند اختيار إيقاف البطولة الحالية فقط.");
            }

            if (dto.Scope == SuspensionScope.TimePeriod && !dto.EndDate.HasValue)
            {
                throw new InvalidOperationException("يجب تحديد تاريخ النهاية عند اختيار إيقاف لفترة زمنية.");
            }

            if (dto.Scope == SuspensionScope.NumberOfTournaments && dto.NumberOfTournaments <= 0)
            {
                throw new InvalidOperationException("يجب تحديد عدد بطولات أكبر من صفر.");
            }

            var suspension = _mapper.Map<PlayerSuspension>(dto);

            await _dbContext.PlayerSuspensions.AddAsync(suspension);
            await _dbContext.SaveChangesAsync();

            var createdSuspension = await _dbContext.PlayerSuspensions
                .Include(s => s.Player)
                .Include(s => s.Tournament)
                .FirstOrDefaultAsync(s => s.Id == suspension.Id);

            return _mapper.Map<PlayerSuspensionDto>(createdSuspension);
        }

        // 2. إلغاء عقوبة يدوياً (فك الحظر)
        public async Task<bool> DeactivateSuspensionAsync(int suspensionId)
        {
            var suspension = await _dbContext.PlayerSuspensions.FindAsync(suspensionId);
            if (suspension == null) return false;

            suspension.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // 3. جلب جميع العقوبات النشطة للاعب
        public async Task<IEnumerable<PlayerSuspensionDto>> GetActiveSuspensionsByPlayerIdAsync(string playerId)
        {
            var activeSuspensions = await _dbContext.PlayerSuspensions
                .Include(s => s.Player)
                .Include(s => s.Tournament)
                .Where(s => s.PlayerId == playerId && s.IsActive)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PlayerSuspensionDto>>(activeSuspensions);
        }

        // 4. جلب السجل التاريخي لعقوبات اللاعب
        public async Task<IEnumerable<PlayerSuspensionDto>> GetPlayerSuspensionHistoryAsync(string playerId)
        {
            var history = await _dbContext.PlayerSuspensions
                .Include(s => s.Player)
                .Include(s => s.Tournament)
                .Where(s => s.PlayerId == playerId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PlayerSuspensionDto>>(history);
        }

        // 5. فحص هل اللاعب محروم من المشاركة في بطولة معينة
        public async Task<bool> IsPlayerSuspendedFromTournamentsAsync(string playerId, int targetTournamentId)
        {
            var activeSuspensions = await _dbContext.PlayerSuspensions
                .Where(s => s.PlayerId == playerId && s.IsActive &&
                      (s.Restriction == RestrictedAction.TournamentsOnly ||
                       s.Restriction == RestrictedAction.AllClubActivities ||
                       s.Restriction == RestrictedAction.FullAccountBlock))
                .ToListAsync();

            if (!activeSuspensions.Any()) return false;

            var now = DateTime.UtcNow;

            foreach (var suspension in activeSuspensions)
            {
                // أ. حظر كامل للحساب أو أنشطة النادي أو عقوبة دائمة
                if (suspension.Restriction == RestrictedAction.FullAccountBlock ||
                    suspension.Scope == SuspensionScope.Permanent)
                {
                    return true;
                }

                // ب. إيقاف في البطولة الحالية التي حدثت فيها المخالفة فقط
                if (suspension.Scope == SuspensionScope.CurrentTournamentOnly &&
                    suspension.TournamentId.HasValue &&
                    suspension.TournamentId.Value == targetTournamentId)
                {
                    return true;
                }

                // ج. إيقاف بزمن محدد (مثلاً شهر)
                if (suspension.Scope == SuspensionScope.TimePeriod &&
                    suspension.EndDate.HasValue &&
                    suspension.EndDate > now)
                {
                    return true;
                }

                // د. إيقاف بعدد البطولات (مثلاً محروم من 2 أو 3 بطولات)
                if (suspension.Scope == SuspensionScope.NumberOfTournaments && suspension.TournamentId.HasValue)
                {
                    var tournamentsCount = await _dbContext.Tournaments
                        .CountAsync(t => t.Id >= suspension.TournamentId.Value && t.Id <= targetTournamentId);

                    if (tournamentsCount <= suspension.NumberOfTournaments)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // 6. فحص هل اللاعب محروم من حجز الملاعب العادية
        public async Task<bool> IsPlayerSuspendedFromBookingAsync(string playerId)
        {
            var now = DateTime.UtcNow;

            return await _dbContext.PlayerSuspensions
                .AnyAsync(s => s.PlayerId == playerId && s.IsActive &&
                    (s.Restriction == RestrictedAction.FreePlaygroundBooking ||
                     s.Restriction == RestrictedAction.AllClubActivities ||
                     s.Restriction == RestrictedAction.FullAccountBlock) &&
                    (s.Scope == SuspensionScope.Permanent || (s.Scope == SuspensionScope.TimePeriod && s.EndDate.HasValue && s.EndDate > now)));
        }

        // 7. فحص الحظر الكامل للحساب
        public async Task<bool> IsAccountFullyBlockedAsync(string playerId)
        {
            return await _dbContext.PlayerSuspensions
                .AnyAsync(s => s.PlayerId == playerId && s.IsActive && s.Restriction == RestrictedAction.FullAccountBlock);
        }

        // 8. إغلاق العقوبات المنتهية تلقائياً
        public async Task<int> AutoCloseExpiredSuspensionsAsync()
        {
            var now = DateTime.UtcNow;

            // 1. جلب العقوبات الزمنية المنتهية
            var expiredTimeSuspensions = await _dbContext.PlayerSuspensions
                .Where(s => s.IsActive &&
                            s.Scope == SuspensionScope.TimePeriod &&
                            s.EndDate.HasValue &&
                            s.EndDate.Value <= now)
                .ToListAsync();

            // 2. جلب عقوبات بعدد البطولات التي استوفت عدد البطولات
            var expiredTournamentSuspensions = await _dbContext.PlayerSuspensions
                .Where(s => s.IsActive &&
                            s.Scope == SuspensionScope.NumberOfTournaments &&
                            s.TournamentId.HasValue)
                .ToListAsync();

            var closedCount = 0;

            foreach (var suspension in expiredTimeSuspensions)
            {
                suspension.IsActive = false;
                closedCount++;
            }

            foreach (var suspension in expiredTournamentSuspensions)
            {
                // حساب عدد البطولات التي أنشئت أو لعبت بدءاً من بطولة المخالفة حتى الآن
                var tournamentsPlayed = await _dbContext.Tournaments
                    .CountAsync(t => t.Id >= suspension.TournamentId!.Value);

                if (tournamentsPlayed > suspension.NumberOfTournaments)
                {
                    suspension.IsActive = false;
                    closedCount++;
                }
            }

            if (closedCount > 0)
            {
                await _dbContext.SaveChangesAsync();
            }

            return closedCount;
        }
    }
}
