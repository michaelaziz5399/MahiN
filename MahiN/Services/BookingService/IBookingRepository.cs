using MahiN.DataBase.Dtos.BookingDto;
using MahiN.DataBase.Enums;
using Microsoft.AspNetCore.Identity;

namespace MahiN.Services.BookingService
{
    public interface IBookingRepository
    {
        // == Standard CRUD ==
        Task<IEnumerable<BookingResponseAllDto>> GetAllBookingsAsync();
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto model);
        Task<BookingResponseDto> UpdateBookingAsync(int bookingId, CreateBookingDto model);
        Task<IdentityResult> DeleteBookingAsync(int bookingId);

        // 🎯 1. فحص توفر الملعب
        Task<List<string>> GetConflictingPlaygroundNamesAsync(CheckAvailabilityDto model);

        // 🎯 2. جلب حجوزات مستخدم معين
        Task<IEnumerable<BookingResponseAllDto>> GetBookingsByUserIdAsync(string userId);

        // 🎯 3. جلب حجوزات ملعب معين في يوم محدد
        Task<IEnumerable<BookingResponseAllDto>> GetBookingsByFacilityAndDateAsync(int facilityId, DateTime date);

        // 🎯 4. تغيير حالة الحجز (Pending, Confirmed, Cancelled)
        Task<bool> ChangeBookingStatusAsync(int bookingId, UpdateBookingStatusDto model);

        // 🎯 5. تحديث المبلغ المدفوع للحجز  
        Task<bool> UpdateBookingPaymentAsync(UpdatePaymentDto model);
    }
}
