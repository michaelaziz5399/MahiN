using MahiN.DataBase.Dtos.PlaygroundDtos;
using MahiN.Services.PlaygroundService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MahiN.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaygroundController(IPlaygroundRepository _playgroundRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllPlaygrounds([FromQuery] bool onlyActive = false)
        {
            try
            {
                var playgrounds = await _playgroundRepository.GetAllPlaygroundsAsync(onlyActive);
                return Ok(playgrounds);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "حدث خطأ أثناء جلب الملاعب", details = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPlaygroundById(int id)
        {
            try
            {
                var playground = await _playgroundRepository.GetPlaygroundByIdAsync(id);
                if (playground == null)
                {
                    return NotFound(new { message = "لم يتم العثور على الملعب" });
                }
                return Ok(playground);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "حدث خطأ أثناء جلب الملعب", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayground([FromForm] CreatePlaygroundDto model)
        {
            try
            {
                var playground = await _playgroundRepository.CreatePlaygroundAsync(model);
                return CreatedAtAction(nameof(GetPlaygroundById), new { id = playground.Id }, playground);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "حدث خطأ أثناء إنشاء الملعب", details = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePlayground(int id, [FromForm] CreatePlaygroundDto model)
        {
            try
            {
                var playground = await _playgroundRepository.UpdatePlaygroundAsync(id, model);
                if (playground == null)
                {
                    return NotFound(new { message = "لم يتم العثور على الملعب" });
                }
                return Ok(playground);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "حدث خطأ أثناء تحديث الملعب", details = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePlayground(int id)
        {
            try
            {
                var result = await _playgroundRepository.DeletePlaygroundAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "لم يتم العثور على الملعب" });
                }
                return Ok(new { message = "تم حذف الملعب بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "حدث خطأ أثناء حذف الملعب", details = ex.Message });
            }
        }

        [HttpPatch("{id:int}/toggle-status")]
        public async Task<IActionResult> TogglePlaygroundStatus(int id)
        {
            try
            {
                var result = await _playgroundRepository.ToggleStatusAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "لم يتم العثور على الملعب" });
                }
                return Ok(new { message = "تم تحديث حالة الملعب بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "حدث خطأ أثناء تحديث حالة الملعب", details = ex.Message });
            }
        }
    }
}
