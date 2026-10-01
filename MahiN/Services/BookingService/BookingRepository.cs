using AutoMapper;
using AutoMapper.QueryableExtensions;
using MahiN.DataBase.Data;
using MahiN.DataBase.Dtos.BookingDto;
using MahiN.DataBase.Enums;
using MahiN.DataBase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.BookingService
{
    public class BookingRepository(ApplicationDbContext _dbContext 
        , IMapper _mapper) : IBookingRepository
    {
        // =========== { Standard CRUD } ===========
        public async Task<IEnumerable<BookingResponseAllDto>> GetAllBookingsAsync()
        {
            return await _dbContext.Bookings
                .AsNoTracking()
                .ProjectTo<BookingResponseAllDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _dbContext.Bookings
                .AsNoTracking()
                .ProjectTo<BookingResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto model)
        {
            // Check if the requested time slot is available for each playground
            var availabilityDto = _mapper.Map<CheckAvailabilityDto>(model);
            var hasOverlap = await GetConflictingPlaygroundNamesAsync(availabilityDto);
            if (hasOverlap.Any())
            {
                var names = string.Join(" و ", hasOverlap);
                throw new InvalidOperationException($"الملاعب التالية محجوزة بالفعل في هذا الوقت: ({names}). يرجى اختيار موعد آخر.");
            }


            // Map the CreateBookingDto to the Booking entity
            var booking = _mapper.Map<Booking>(model);

            // put the playgrounds in the booking
            if (model.PlaygroundIds != null && model.PlaygroundIds.Any())
            {
                foreach (var playgroundId in model.PlaygroundIds)
                {
                    booking.BookingPlaygrounds.Add(new BookingPlayground
                    {
                        PlaygroundId = playgroundId
                    });
                }
            }
            // Determine the payment status based on the amount paid and total amount
            if (booking.AmountPaid >= booking.TotalAmount && booking.TotalAmount > 0)
                booking.PaymentStatus = PaymentStatus.Paid;
            else if (booking.AmountPaid > 0)
                booking.PaymentStatus = PaymentStatus.PartiallyPaid;
            else
                booking.PaymentStatus = PaymentStatus.Unpaid;


            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();

            return await GetBookingByIdAsync(booking.Id) 
                ?? throw new InvalidOperationException("تعذر جلب بيانات الحجز بعد الإنشاء.");
        }

        public async Task<BookingResponseDto> UpdateBookingAsync(int bookingId, CreateBookingDto model)
        {
            var booking = await _dbContext.Bookings
                .Include(b => b.BookingPlaygrounds)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if(booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
            }

            // Check if the requested time slot is available for each playground
            var availabilityDto = _mapper.Map<CheckAvailabilityDto>(model);
            availabilityDto.ExcludeBookingId = bookingId;

            var hasOverlap = await GetConflictingPlaygroundNamesAsync(availabilityDto);
            if (hasOverlap.Any())
            {
                var names = string.Join(" و ", hasOverlap);
                throw new InvalidOperationException($"الملاعب التالية محجوزة بالفعل في هذا الوقت: ({names}). يرجى اختيار موعد آخر.");
            }

            // Update the booking properties with the new values from the model
            _mapper.Map(model, booking);

            // Determine the payment status based on the amount paid and total amount
            if (booking.AmountPaid >= booking.TotalAmount && booking.TotalAmount > 0)
                booking.PaymentStatus = PaymentStatus.Paid;
            else if (booking.AmountPaid > 0)
                booking.PaymentStatus = PaymentStatus.PartiallyPaid;
            else
                booking.PaymentStatus = PaymentStatus.Unpaid;


            // Clear existing playgrounds
            booking.BookingPlaygrounds.Clear();

            // Update the booking's playgrounds
            if (model.PlaygroundIds != null && model.PlaygroundIds.Any())
            {
                foreach (var playgroundId in model.PlaygroundIds)
                {
                    booking.BookingPlaygrounds.Add(new BookingPlayground
                    {
                        PlaygroundId = playgroundId
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
            return await GetBookingByIdAsync(booking.Id) 
                ?? throw new InvalidOperationException("تعذر جلب بيانات الحجز بعد التحديث.");
        }

        public async Task<IdentityResult> DeleteBookingAsync(int bookingId)
        {
            var booking =await _dbContext.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "BookingNotFound",
                    Description = $"Booking with ID {bookingId} not found."
                });
            }
            _dbContext.Bookings.Remove(booking);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0 
                ? IdentityResult.Success 
                : IdentityResult.Failed(new IdentityError
            {
                Code = "DeleteFailed",
                Description = $"Failed to delete booking with ID {bookingId}."
            });
        }


        // =========== { Check Slot Availability } ===========
        public async Task<List<string>> GetConflictingPlaygroundNamesAsync(CheckAvailabilityDto model)
        {
            if (model.PlaygroundIds == null || !model.PlaygroundIds.Any())
                return new List<string>();

            return await _dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.Status != BookingStatus.Canceled && b.Status != BookingStatus.Rejected)
                .Where(b => model.ExcludeBookingId == null || b.Id != model.ExcludeBookingId)
                .Where(b => model.StartDate < b.EndDate && model.EndDate > b.StartDate) // فحص التداخل في الوقت
                .SelectMany(b => b.BookingPlaygrounds)
                .Where(bp => model.PlaygroundIds.Contains(bp.PlaygroundId))
                .Select(bp => bp.Playground!.Name) // سحب اسم الملعب
                .Distinct()
                .ToListAsync();
        }
        // =========== { Get Bookings by User ID } ===========
        public async Task<IEnumerable<BookingResponseAllDto>> GetBookingsByUserIdAsync(string userId)
        {
            var bookings = await _dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.CreatedById == userId)
                .ProjectTo<BookingResponseAllDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return bookings;
        }

        // =========== { Get Bookings by Facility and Date } ===========
        public async Task<IEnumerable<BookingResponseAllDto>> GetBookingsByFacilityAndDateAsync(int facilityId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.BookingPlaygrounds.Any(bp => bp.PlaygroundId == facilityId))
                .Where(b => b.StartDate >= startOfDay && b.StartDate < endOfDay)
                .ProjectTo<BookingResponseAllDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // =========== { Change Booking Status } ===========
        public async Task<bool> ChangeBookingStatusAsync(int bookingId, UpdateBookingStatusDto model)
        {
            var booking = await _dbContext.Bookings.FindAsync(bookingId);
            if (booking == null)
                return false;

            booking.Status = model.Status;
            booking.ApprovedById = model.ApprovedById;

            return await _dbContext.SaveChangesAsync() > 0;
        }

        // =========== { Update Booking Payment } ===========
        public async Task<bool> UpdateBookingPaymentAsync(UpdatePaymentDto model)
        {
            var booking = await _dbContext.Bookings.FindAsync(model.BookingId);
            if (booking == null)
                return false;

            // 1. إضافة المبلغ الجديد للمبلغ المدفوع سابقاً (أو تعيينه مباشرة حسب بيزنس أپلكيشنك)
            booking.AmountPaid += model.NewPaymentAmount;

            // 2. تحديث حالة الدفع تلقائياً وبشكل آمن على السيرفر
            if (booking.AmountPaid >= booking.TotalAmount && booking.TotalAmount > 0)
                booking.PaymentStatus = PaymentStatus.Paid;
            else if (booking.AmountPaid > 0)
                booking.PaymentStatus = PaymentStatus.PartiallyPaid;
            else
                booking.PaymentStatus = PaymentStatus.Unpaid;

            return await _dbContext.SaveChangesAsync() > 0;
        }

    }
}
