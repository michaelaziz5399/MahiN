using MahiN.DataBase.Dtos.TeamDtos;
using MahiN.Services.TeamService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MahiN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController(ITeamRepository _teamRepository) : ControllerBase
    {
        // 1. إنشاء فريق جديد (الكابتن هو المستخدم المسجل حالياً)
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("لم يتم التعرف على هوية المستخدم.");

            try
            {
                var createdTeam = await _teamRepository.CreateTeamAsync(dto, userId);
                return CreatedAtAction(nameof(GetTeamById), new { id = createdTeam.Id }, createdTeam);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 2. إضافة لاعب إلى فريق
        [HttpPost("members")]
        public async Task<IActionResult> AddTeamMember([FromBody] AddTeamMemberDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var addedMember = await _teamRepository.AddTeamMemberAsync(dto);
                return Ok(addedMember);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 3. حذف لاعب من الفريق
        [HttpDelete("{teamId:int}/members/{userId}")]
        public async Task<IActionResult> RemoveTeamMember(int teamId, string userId)
        {
            try
            {
                var isSuccess = await _teamRepository.RemoveTeamMemberAsync(teamId, userId);
                if (!isSuccess)
                    return NotFound(new { message = "اللاعب غير موجود بهذا الفريق." });

                return Ok(new { message = "تم إزالة اللاعب من الفريق بنجاح." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 4. جلب بيانات فريق محدد بمعرفه
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTeamById(int id)
        {
            var team = await _teamRepository.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound(new { message = "الفريق غير موجود." });

            return Ok(team);
        }

        // 5. جلب جميع فرق بطولة معينة
        [HttpGet("tournament/{tournamentId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTeamsByTournament(int tournamentId)
        {
            var teams = await _teamRepository.GetTeamsByTournamentIdAsync(tournamentId);
            return Ok(teams);
        }

        // 6. تحديث حالة دفع رسوم الفريق (خاص بالأدمن فقط)
        [HttpPatch("{id:int}/payment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] bool isPaid)
        {
            var isSuccess = await _teamRepository.UpdateTeamPaymentStatusAsync(id, isPaid);
            if (!isSuccess)
                return NotFound(new { message = "الفريق غير موجود." });

            return Ok(new { message = "تم تحديث حالة الدفع بنجاح." });
        }

        // 7. حذف فريق بالكامل
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            try
            {
                var isSuccess = await _teamRepository.DeleteTeamAsync(id);
                if (!isSuccess)
                    return NotFound(new { message = "الفريق غير موجود." });

                return Ok(new { message = "تم حذف الفريق بنجاح." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
