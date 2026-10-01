using AutoMapper;
using MahiN.DataBase.Data;
using MahiN.DataBase.Dtos.TournamentDtos;
using MahiN.DataBase.Enums;
using MahiN.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.TournamentService
{
    public class TournamentRepository(ApplicationDbContext _dbContext , IMapper _mapper) : ITournamentRepository
    {
        // 1. جلب جميع البطولات مع الفلترة
        public async Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync(int? playgroundId = null, string? status = null)
        {
            var query = _dbContext.Tournaments
                .Include(t => t.Playground)
                .Include(t => t.Teams)
                .AsQueryable();

            if (playgroundId.HasValue)
            {
                query = query.Where(t => t.PlaygroundId == playgroundId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TournamentStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(t => t.Status == parsedStatus);
            }

            var tournaments = await query
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
        }

        // 2. جلب تفاصيل بطولة واحدة مع الفرق والمباريات
        public async Task<TournamentDetailsDto?> GetTournamentByIdAsync(int id)
        {
            var tournament = await _dbContext.Tournaments
                .Include(t => t.Playground)
                .Include(t => t.Teams)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.HomeTeam)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.AwayTeam)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null) return null;

            return _mapper.Map<TournamentDetailsDto>(tournament);
        }

        // 3. إنشاء بطولة جديدة
        public async Task<TournamentDto> CreateTournamentAsync(CreateTournamentDto dto)
        {
            // التحقق من صحة التواريخ
            if (dto.EndDate < dto.StartDate)
            {
                throw new InvalidOperationException("تاريخ النهاية لا يمكن أن يكون قبل تاريخ البداية.");
            }

            // التحقق من منطقية عدد اللاعبين
            if (dto.MaxPlayersPerTeam < dto.MinPlayersPerTeam)
            {
                throw new InvalidOperationException("الحد الأقصى للاعبين لا يمكن أن يكون أقل من الحد الأدنى.");
            }

            // التحقق من وجود الملعب
            var playgroundExists = await _dbContext.Playgrounds.AnyAsync(p => p.Id == dto.PlaygroundId);
            if (!playgroundExists)
            {
                throw new KeyNotFoundException("الملعب المحدد غير موجود.");
            }

            var tournament = _mapper.Map<Tournament>(dto);

            await _dbContext.Tournaments.AddAsync(tournament);
            await _dbContext.SaveChangesAsync();

            // إعادة تحميل البيانات لربط اسم الملعب
            var createdTournament = await _dbContext.Tournaments
                .Include(t => t.Playground)
                .Include(t => t.Teams)
                .FirstOrDefaultAsync(t => t.Id == tournament.Id);

            return _mapper.Map<TournamentDto>(createdTournament);
        }

        // 4. تعديل بطولة
        public async Task<TournamentDto?> UpdateTournamentAsync(int id, CreateTournamentDto dto)
        {
            var tournament = await _dbContext.Tournaments
                .Include(t => t.Playground)
                .Include(t => t.Teams)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null) return null;

            if (dto.EndDate < dto.StartDate)
            {
                throw new InvalidOperationException("تاريخ النهاية لا يمكن أن يكون قبل تاريخ البداية.");
            }

            if (dto.MaxPlayersPerTeam < dto.MinPlayersPerTeam)
            {
                throw new InvalidOperationException("الحد الأقصى للاعبين لا يمكن أن يكون أقل من الحد الأدنى.");
            }

            _mapper.Map(dto, tournament);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<TournamentDto>(tournament);
        }

        // 5. تحديث حالة البطولة
        public async Task<bool> UpdateTournamentStatusAsync(int id, UpdateTournamentStatusDto dto)
        {
            var tournament = await _dbContext.Tournaments.FindAsync(id);
            if (tournament == null) return false;

            tournament.Status = dto.Status;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // 6. حذف بطولة
        public async Task<bool> DeleteTournamentAsync(int id)
        {
            var tournament = await _dbContext.Tournaments
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null) return false;

            // منع الحذف إذا كان للبطولة مباريات قد بدأت بالفعل
            if (tournament.Matches.Any(m => m.IsCompleted))
            {
                throw new InvalidOperationException("لا يمكن حذف بطولة تحوي مباريات تم لعبها بالفعل.");
            }

            _dbContext.Tournaments.Remove(tournament);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
