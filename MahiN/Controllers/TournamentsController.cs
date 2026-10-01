using MahiN.DataBase.Dtos.TournamentDtos;
using MahiN.Services.TournamentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MahiN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TournamentsController(ITournamentRepository _tournamentRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllTournaments([FromQuery] int? playgroundId, [FromQuery] string? status)
        {
            var tournaments = await _tournamentRepository.GetAllTournamentsAsync(playgroundId, status);
            return Ok(tournaments);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTournamentById(int id)
        {
            var tournament = await _tournamentRepository.GetTournamentByIdAsync(id);
            if (tournament == null)
            {
                return NotFound(new { Message = $"البطولة ذات المعرف (ID: {id}) غير موجودة." });
            }
            return Ok(tournament);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentDto tournamentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdTournament = await _tournamentRepository.CreateTournamentAsync(tournamentDto);
                return CreatedAtAction(nameof(GetTournamentById), new { id = createdTournament.Id }, createdTournament);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTournament(int id, [FromBody] CreateTournamentDto tournamentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var updatedTournament = await _tournamentRepository.UpdateTournamentAsync(id, tournamentDto);
                return Ok(updatedTournament);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            try
            {
                var isDeleted = await _tournamentRepository.DeleteTournamentAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new { Message = $"البطولة ذات المعرف (ID: {id}) غير موجودة." });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateTournamentStatus(int id, [FromBody] UpdateTournamentStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var isUpdated = await _tournamentRepository.UpdateTournamentStatusAsync(id , statusDto);
                if (!isUpdated)
                {
                    return NotFound(new { Message = $"البطولة ذات المعرف (ID: {id}) غير موجودة." });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
