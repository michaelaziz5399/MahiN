using MahiN.DataBase.Dtos.BookingDto;
using MahiN.DataBase.Enums;
using MahiN.Services.BookingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MahiN.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController(IBookingRepository bookingRepository) : ControllerBase
    {
        // =========== { Standard CRUD } ===========
        [HttpGet("GetAllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await bookingRepository.GetAllBookingsAsync();
            if(bookings == null || !bookings.Any())
            {
                return NotFound("No bookings found.");
            }
            return Ok(bookings);
        }

        [HttpGet("GetBookingById/{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await bookingRepository.GetBookingByIdAsync(id);
            if(booking == null)
            {
                return NotFound(new { message = $"Booking with ID {id} not found." });
            }
            return Ok(booking);
        }

        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await bookingRepository.CreateBookingAsync(model);
                return CreatedAtAction(nameof(GetBookingById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateBooking/{id}")]
        public async Task<IActionResult> UpdateBooking([FromRoute] int id, [FromBody] CreateBookingDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await bookingRepository.UpdateBookingAsync(id, model);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteBooking/{id}")]
        public async Task<IActionResult> DeleteBooking([FromRoute] int id)
        {
            try
            {
                await bookingRepository.DeleteBookingAsync(id);
                return Ok(new { message = $"Booking with ID {id} deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ========== { Check Availability } ===========
        [HttpPost("check-availability")]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityDto model)
        {

            var conflictingNames = await bookingRepository.GetConflictingPlaygroundNamesAsync(model);

            var isAvailable = !conflictingNames.Any();

            return Ok(new
            {
                isAvailable,
                conflictingPlaygrounds = conflictingNames,
                message = isAvailable
                    ? "الموعد متاح للحجز"
                    : $"الملاعب التالية محجوزة: ({string.Join(" و ", conflictingNames)})"
            });
        }

        // =========== { Get Bookings by User ID } ===========
        [HttpGet("GetBookingsByUserId/{userId}")]
        public async Task<IActionResult> GetBookingsByUserId(string userId)
        {
            var bookings = await bookingRepository.GetBookingsByUserIdAsync(userId);
            if (bookings == null || !bookings.Any())
            {
                return NotFound("No bookings found for the specified user.");
            }
            return Ok(bookings);
        }

        // =========== { Get Bookings by Facility and Date } ===========
        [HttpGet("GetBookingsByFacilityAndDate/{facilityId}/{date}")]
        public async Task<IActionResult> GetBookingsByFacilityAndDate(int facilityId, DateTime date)
        {
            var bookings = await bookingRepository.GetBookingsByFacilityAndDateAsync(facilityId, date);
            if (bookings == null || !bookings.Any())
            {
                return NotFound("No bookings found for the specified facility and date.");
            }
            return Ok(bookings);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateBookingStatus([FromRoute] int id, [FromBody] UpdateBookingStatusDto model)
        {
            try
            {
                var result = await bookingRepository.ChangeBookingStatusAsync(id, model);
                return Ok(new { message = "Booking status updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPatch("UpdatePayment")]
        public async Task<IActionResult> UpdateBookingPayment([FromBody] UpdatePaymentDto model)
        {
            try
            {
                var result = await bookingRepository.UpdateBookingPaymentAsync(model);
                if (!result)
                {
                    return NotFound(new { message = "Failed to update booking payment." });
                }
                return Ok(new { message = "Booking payment updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
