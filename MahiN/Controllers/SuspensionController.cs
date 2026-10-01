using MahiN.DataBase.Dtos.SuspensionDtos;
using MahiN.Services.SuspensionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MahiN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SuspensionController(ISuspensionRepository _suspensionRepository) : ControllerBase
    {
        // 1. إضافة عقوبة جديدة للاعب
        [HttpPost]
        public async Task<IActionResult> CreateSuspension([FromBody] CreateSuspensionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _suspensionRepository.CreateSuspensionAsync(dto);
                return CreatedAtAction(nameof(GetActiveSuspensionsByPlayerId), new { playerId = result.PlayerId }, result);
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
                return StatusCode(500, new { message = "حدث خطأ غير متوقع أثناء إضافة العقوبة.", details = ex.Message });
            }
        }

        // 2. إلغاء عقوبة يدوياً (فك الحظر)
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> DeactivateSuspension(int id)
        {
            var isDeactivated = await _suspensionRepository.DeactivateSuspensionAsync(id);
            if (!isDeactivated)
                return NotFound(new { message = "لم يتم العثور على سجل العقوبة المحدد." });

            return Ok(new { message = "تم إلغاء العقوبة بنجاح." });
        }

        // 3. جلب جميع العقوبات النشطة للاعب معين
        [HttpGet("player/{playerId:string}/active")]
        public async Task<IActionResult> GetActiveSuspensionsByPlayerId(string playerId)
        {
            var suspensions = await _suspensionRepository.GetActiveSuspensionsByPlayerIdAsync(playerId);
            return Ok(suspensions);
        }

        // 4. جلب السجل التاريخي الكامل لعقوبات اللاعب
        [HttpGet("player/{playerId:string}/history")]
        public async Task<IActionResult> GetPlayerSuspensionHistory(string playerId)
        {
            var history = await _suspensionRepository.GetPlayerSuspensionHistoryAsync(playerId);
            return Ok(history);
        }

        // 5. فحص صلاحية مشاركة اللاعب في بطولة محددة
        [HttpGet("check-tournament-eligibility")]
        public async Task<IActionResult> CheckTournamentEligibility([FromQuery] string playerId, [FromQuery] int tournamentId)
        {
            var isSuspended = await _suspensionRepository.IsPlayerSuspendedFromTournamentsAsync(playerId, tournamentId);
            return Ok(new
            {
                playerId,
                tournamentId,
                isSuspended,
                canParticipate = !isSuspended
            });
        }

        // 6. فحص صلاحية حجز الملعب العادي للاعب
        [HttpGet("check-booking-eligibility/{playerId:string}")]
        public async Task<IActionResult> CheckBookingEligibility(string playerId)
        {
            var isSuspended = await _suspensionRepository.IsPlayerSuspendedFromBookingAsync(playerId);
            return Ok(new
            {
                playerId,
                isSuspendedFromBooking = isSuspended,
                canBookPlayground = !isSuspended
            });
        }

        // 7. فحص ما إذا كان الحساب محظوراً كلياً
        [HttpGet("check-account-status/{playerId:string}")]
        public async Task<IActionResult> CheckAccountStatus(string playerId)
        {
            var isBlocked = await _suspensionRepository.IsAccountFullyBlockedAsync(playerId);
            return Ok(new
            {
                playerId,
                isAccountBlocked = isBlocked
            });
        }

        // 8. إغلاق العقوبات المنتهية تلقائياً
        [HttpPost("run-auto-cleanup")]
        public async Task<IActionResult> RunAutoCleanup()
        {
            var closedCount = await _suspensionRepository.AutoCloseExpiredSuspensionsAsync();
            return Ok(new
            {
                message = "تمت عملية فحص وتحديث العقوبات المنتهية بنجاح.",
                closedSuspensionsCount = closedCount
            });
        }
    }
}
