using AutoMapper;
using MahiN.DataBase.Data;
using MahiN.DataBase.Dtos.MatchDtos;
using MahiN.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.MatchService
{
    public class MatchRepository(ApplicationDbContext _dbContext, IMapper _mapper) : IMatchRepository
    {
        // 1. إنشاء مباراة
        public async Task<TournamentMatchDto> CreateMatchAsync(CreateTournamentMatchDto dto)
        {
            if (dto.HomeTeamId == dto.AwayTeamId)
            {
                throw new InvalidOperationException("لا يمكن للمنافسة أن تكون بين الفريق ونفسه.");
            }

            var tournamentExists = await _dbContext.Tournaments.AnyAsync(t => t.Id == dto.TournamentId);
            if (!tournamentExists)
            {
                throw new KeyNotFoundException("البطولة غير موجودة.");
            }

            var homeTeamExists = await _dbContext.Teams.AnyAsync(t => t.Id == dto.HomeTeamId && t.TournamentId == dto.TournamentId);
            var awayTeamExists = await _dbContext.Teams.AnyAsync(t => t.Id == dto.AwayTeamId && t.TournamentId == dto.TournamentId);

            if (!homeTeamExists || !awayTeamExists)
            {
                throw new InvalidOperationException("كلا الفريقين يجب أن يكونا مسجلين في نفس البطولة.");
            }

            var match = _mapper.Map<TournamentMatch>(dto);

            await _dbContext.TournamentMatches.AddAsync(match);
            await _dbContext.SaveChangesAsync();

            return await GetMatchByIdAsync(match.Id)
                   ?? throw new InvalidOperationException("حدث خطأ أثناء استرجاع بيانات المباراة.");
        }

        // 2. تحديث نتيجة المباراة
        public async Task<TournamentMatchDto?> UpdateMatchResultAsync(int matchId, UpdateMatchResultDto dto)
        {
            var match = await _dbContext.TournamentMatches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null) return null;

            // استخدام AutoMapper لتأثير التحديث
            _mapper.Map(dto, match);

            // تحديد الفائز تلقائياً إن لم يمرر، وإلا التأكد من صحة الفائز الممرر
            if (dto.HomeScore > dto.AwayScore)
            {
                match.WinnerTeamId = match.HomeTeamId;
            }
            else if (dto.AwayScore > dto.HomeScore)
            {
                match.WinnerTeamId = match.AwayTeamId;
            }
            else
            {
                match.WinnerTeamId = dto.WinnerTeamId; // تعادل أو فوز بركلات الترجيح
            }

            await _dbContext.SaveChangesAsync();
            return await GetMatchByIdAsync(match.Id);
        }

        // 3. إضافة حدث للمباراة
        public async Task<MatchEventDto> AddMatchEventAsync(CreateMatchEventDto dto)
        {
            var match = await _dbContext.TournamentMatches.FirstOrDefaultAsync(m => m.Id == dto.MatchId);
            if (match == null)
            {
                throw new KeyNotFoundException("المباراة غير موجودة.");
            }

            if (dto.TeamId != match.HomeTeamId && dto.TeamId != match.AwayTeamId)
            {
                throw new InvalidOperationException("الفريق المحدد ليس طرفاً في هذه المباراة.");
            }

            var isPlayerInTeam = await _dbContext.TeamMembers.AnyAsync(tm => tm.TeamId == dto.TeamId && tm.UserId == dto.PlayerId);
            if (!isPlayerInTeam)
            {
                throw new InvalidOperationException("اللاعب المحدد لا ينتمي إلى هذا الفريق.");
            }

            var matchEvent = _mapper.Map<MatchEvent>(dto);

            await _dbContext.MatchEvents.AddAsync(matchEvent);
            await _dbContext.SaveChangesAsync();

            // إعادة الكائن محملاً بالبيانات للتحويل الصحيح للـ DTO
            var loadedEvent = await _dbContext.MatchEvents
                .Include(e => e.Player)
                .Include(e => e.Team)
                .FirstAsync(e => e.Id == matchEvent.Id);

            return _mapper.Map<MatchEventDto>(loadedEvent);
        }

        // 4. حذف حدث
        public async Task<bool> DeleteMatchEventAsync(int eventId)
        {
            var matchEvent = await _dbContext.MatchEvents.FindAsync(eventId);
            if (matchEvent == null) return false;

            _dbContext.MatchEvents.Remove(matchEvent);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // 5. جلب مباراة بمعرفها
        public async Task<TournamentMatchDto?> GetMatchByIdAsync(int matchId)
        {
            var match = await _dbContext.TournamentMatches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.WinnerTeam)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null) return null;

            return _mapper.Map<TournamentMatchDto>(match);
        }

        // 6. جلب مباريات البطولة
        public async Task<IEnumerable<TournamentMatchDto>> GetMatchesByTournamentIdAsync(int tournamentId)
        {
            var matches = await _dbContext.TournamentMatches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.WinnerTeam)
                .Where(m => m.TournamentId == tournamentId)
                .OrderBy(m => m.MatchDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TournamentMatchDto>>(matches);
        }

        // 7. جلب أحداث المباراة
        public async Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(int matchId)
        {
            var events = await _dbContext.MatchEvents
                .Include(e => e.Player)
                .Include(e => e.Team)
                .Where(e => e.MatchId == matchId)
                .OrderBy(e => e.Minute)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MatchEventDto>>(events);
        }

        // 8. حذف مباراة
        public async Task<bool> DeleteMatchAsync(int matchId)
        {
            var match = await _dbContext.TournamentMatches.FindAsync(matchId);
            if (match == null) return false;

            _dbContext.TournamentMatches.Remove(match);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
