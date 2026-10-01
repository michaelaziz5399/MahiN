using MahiN.DataBase.Dtos.MatchDtos;
using MahiN.Services.MatchService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MahiN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchController(IMatchRepository _matchRepository, ILogger<MatchController> _logger) : ControllerBase
    {
        // 1. إنشاء مباراة جديدة (خاص بالأدمن/منظم البطولة)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMatch([FromBody] CreateTournamentMatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var match = await _matchRepository.CreateMatchAsync(dto);
                return CreatedAtAction(nameof(GetMatchById), new { id = match.Id }, match);
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
                _logger.LogError(ex, "حدث خطأ أثناء إنشاء المباراة.");
                return StatusCode(500, new { message = "حدث خطأ غير متوقع في السيرفر." });
            }
        }

        // 2. تحديث نتيجة المباراة (خاص بالأدمن/منظم البطولة)
        [HttpPut("{id:int}/result")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMatchResult(int id, [FromBody] UpdateMatchResultDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedMatch = await _matchRepository.UpdateMatchResultAsync(id, dto);
                if (updatedMatch == null)
                    return NotFound(new { message = "المباراة غير موجودة." });

                return Ok(updatedMatch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث نتيجة المباراة.");
                return StatusCode(500, new { message = "حدث خطأ غير متوقع في السيرفر." });
            }
        }

        // 3. تسجيل حدث للمباراة (هدف، بطاقة صفراء/حمراء)
        [HttpPost("events")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMatchEvent([FromBody] CreateMatchEventDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var matchEvent = await _matchRepository.AddMatchEventAsync(dto);
                return Ok(matchEvent);
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
                _logger.LogError(ex, "حدث خطأ أثناء إدخال حدث المباراة.");
                return StatusCode(500, new { message = "حدث خطأ غير متوقع في السيرفر." });
            }
        }

        // 4. حذف حدث من المباراة
        [HttpDelete("events/{eventId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMatchEvent(int eventId)
        {
            var isSuccess = await _matchRepository.DeleteMatchEventAsync(eventId);
            if (!isSuccess)
                return NotFound(new { message = "الحدث غير موجود." });

            return Ok(new { message = "تم حذف الحدث بنجاح." });
        }

        // 5. جلب تفاصيل مباراة محددة
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMatchById(int id)
        {
            var match = await _matchRepository.GetMatchByIdAsync(id);
            if (match == null)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(match);
        }

        // 6. جلب جدول مباريات بطولة محددة
        [HttpGet("tournament/{tournamentId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMatchesByTournament(int tournamentId)
        {
            var matches = await _matchRepository.GetMatchesByTournamentIdAsync(tournamentId);
            return Ok(matches);
        }

        // 7. جلب أحداث مباراة معينة (Timeline للمباراة)
        [HttpGet("{matchId:int}/events")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMatchEvents(int matchId)
        {
            var events = await _matchRepository.GetMatchEventsAsync(matchId);
            return Ok(events);
        }

        // 8. حذف مباراة
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var isSuccess = await _matchRepository.DeleteMatchAsync(id);
            if (!isSuccess)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(new { message = "تم حذف المباراة بنجاح." });
        }
    }
}
