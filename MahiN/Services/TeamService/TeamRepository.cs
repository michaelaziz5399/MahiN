using AutoMapper;
using MahiN.DataBase.Data;
using MahiN.DataBase.Dtos.TeamDtos;
using MahiN.DataBase.Models;
using MahiN.Services.SuspensionService;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.TeamService
{
    public class TeamRepository(
         ApplicationDbContext _dbContext,
         ISuspensionRepository _suspensionRepository) : ITeamRepository
    {
        // 1. إنشاء فريق جديد
        public async Task<TeamDto> CreateTeamAsync(CreateTeamDto dto, string captainUserId)
        {
            // أ. التحقق من وجود البطولة وهل ما زال باب التسجيل مفتوحاً
            var tournament = await _dbContext.Tournaments
                .Include(t => t.Teams)
                .FirstOrDefaultAsync(t => t.Id == dto.TournamentId);

            if (tournament == null)
            {
                throw new KeyNotFoundException("البطولة المحددة غير موجودة.");
            }

            // ب. فحص عقوبات الكابتن قبل إنشاء الفريق
            var isSuspended = await _suspensionRepository.IsPlayerSuspendedFromTournamentsAsync(captainUserId, dto.TournamentId);
            if (isSuspended)
            {
                throw new InvalidOperationException("لا يمكنك إنشاء فريق أو المشاركة، الحساب محروم حالياً من المشاركة في البطولات.");
            }

            // ج. فحص ما إذا كان الكابتن مشتركاً بالفعل في فريق آخر داخل نفس البطولة
            var isAlreadyRegistered = await _dbContext.TeamMembers
                .AnyAsync(tm => tm.UserId == captainUserId && tm.Team!.TournamentId == dto.TournamentId);

            if (isAlreadyRegistered)
            {
                throw new InvalidOperationException("أنت مسجل بالفعل في فريق آخر داخل هذه البطولة.");
            }

            // د. إنشاء الفريق وإضافة الكابتن كأول عضو
            var team = new Team
            {
                Name = dto.Name,
                TournamentId = dto.TournamentId,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow,
                Members = new List<TeamMember>
                {
                    new TeamMember
                    {
                        UserId = captainUserId,
                        IsCaptain = true
                    }
                }
            };

            await _dbContext.Teams.AddAsync(team);
            await _dbContext.SaveChangesAsync();

            return await GetTeamByIdAsync(team.Id)
                   ?? throw new InvalidOperationException("حدث خطأ أثناء استرجاع بيانات الفريق الجديد.");
        }

        // 2. إضافة لاعب إلى فريق
        public async Task<TeamMemberDto> AddTeamMemberAsync(AddTeamMemberDto dto)
        {
            // أ. التحقق من وجود الفريق والبطولة
            var team = await _dbContext.Teams
                .Include(t => t.Members)
                .Include(t => t.Tournament)
                .FirstOrDefaultAsync(t => t.Id == dto.TeamId);

            if (team == null)
            {
                throw new KeyNotFoundException("الفريق المحدد غير موجود.");
            }

            // ب. التحقق من السعة القصوى للاعبي الفريق
            if (team.Members.Count >= team.Tournament.MaxPlayersPerTeam)
            {
                throw new InvalidOperationException($"الفريق مكتمل بالفعل (الحد الأقصى {team.Tournament.MaxPlayersPerTeam} لاعبين).");
            }

            // ج. فحص وجود اللاعب
            var player = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (player == null)
            {
                throw new KeyNotFoundException("اللاعب المراد إضافته غير موجود.");
            }

            // د. فحص عقوبات اللاعب لمنع تسجيل الموقوفين
            var isSuspended = await _suspensionRepository.IsPlayerSuspendedFromTournamentsAsync(dto.UserId, team.TournamentId);
            if (isSuspended)
            {
                throw new InvalidOperationException("تعذر إضافة اللاعب، هذا اللاعب محروم حالياً من المشاركة في البطولات بسبب عقوبة نشطة.");
            }

            // هـ. فحص ما إذا كان اللاعب مسجلاً بالفعل في هذه البطولة (مع أي فريق)
            var isPlayerInTournament = await _dbContext.TeamMembers
                .AnyAsync(tm => tm.UserId == dto.UserId && tm.Team!.TournamentId == team.TournamentId);

            if (isPlayerInTournament)
            {
                throw new InvalidOperationException("هذا اللاعب مسجل بالفعل في فريق آخر ضمن هذه البطولة.");
            }

            // و. إضافة العضو الجديد
            var teamMember = new TeamMember
            {
                TeamId = dto.TeamId,
                UserId = dto.UserId,
                IsCaptain = false
            };

            await _dbContext.TeamMembers.AddAsync(teamMember);
            await _dbContext.SaveChangesAsync();

            return new TeamMemberDto
            {
                Id = teamMember.Id,
                UserId = player.Id,
                PlayerName = player.UserName ?? "لاعب",
                IsCaptain = false
            };
        }

        // 3. حذف لاعب من الفريق
        public async Task<bool> RemoveTeamMemberAsync(int teamId, string userId)
        {
            var member = await _dbContext.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (member == null) return false;

            if (member.IsCaptain)
            {
                throw new InvalidOperationException("لا يمكن حذف كابتن الفريق. يلزم حذف الفريق بالكامل أو نقل الكابتن أولاً.");
            }

            _dbContext.TeamMembers.Remove(member);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // 4. جلب فريق برقمه
        public async Task<TeamDto?> GetTeamByIdAsync(int teamId)
        {
            var team = await _dbContext.Teams
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null) return null;

            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                TournamentId = team.TournamentId,
                IsPaid = team.IsPaid,
                MembersCount = team.Members.Count,
                Members = team.Members.Select(m => new TeamMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    PlayerName = m.User?.UserName ?? "لاعب",
                    IsCaptain = m.IsCaptain
                }).ToList()
            };
        }

        // 5. جلب جميع فرق البطولة
        public async Task<IEnumerable<TeamDto>> GetTeamsByTournamentIdAsync(int tournamentId)
        {
            var teams = await _dbContext.Teams
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .Where(t => t.TournamentId == tournamentId)
                .ToListAsync();

            return teams.Select(t => new TeamDto
            {
                Id = t.Id,
                Name = t.Name,
                TournamentId = t.TournamentId,
                IsPaid = t.IsPaid,
                MembersCount = t.Members.Count,
                Members = t.Members.Select(m => new TeamMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    PlayerName = m.User?.UserName ?? "لاعب",
                    IsCaptain = m.IsCaptain
                }).ToList()
            });
        }

        // 6. تحديث حالة الدفع
        public async Task<bool> UpdateTeamPaymentStatusAsync(int teamId, bool isPaid)
        {
            var team = await _dbContext.Teams.FindAsync(teamId);
            if (team == null) return false;

            team.IsPaid = isPaid;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // 7. حذف الفريق
        public async Task<bool> DeleteTeamAsync(int teamId)
        {
            var team = await _dbContext.Teams.FindAsync(teamId);
            if (team == null) return false;

            _dbContext.Teams.Remove(team);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
