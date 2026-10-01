using MahiN.DataBase.Data;
using MahiN.DataBase.Enums;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Scripts
{
    public class BookingAutoCompletedService(IServiceScopeFactory _scopeFactory 
        , ILogger<BookingAutoCompletedService> _logger) : BackgroundService

    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // تشغيل الفحص فوراً
                    await CheckAndCompleteBookingsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "حدث خطأ أثناء تشغيل دورة الفحص.");
                }

                // الانتظار للدورة القادمة (5 دقائق)
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CheckAndCompleteBookingsAsync(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // جلب توقيت مصر الحالي بغض النظر عن توقيت السيرفر
                    var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                    var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

                    var expiredBookings = await dbContext.Bookings
                        .Where(b => (b.Status == BookingStatus.Approved || b.Status == BookingStatus.Pending)
                                 && b.EndDate <= now)
                        .ToListAsync(stoppingToken);

                    if (expiredBookings.Any())
                    {
                        foreach (var booking in expiredBookings)
                        {
                            booking.Status = BookingStatus.Completed;
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("تم تحويل {Count} حجز منتهي إلى Completed تلقائياً.", expiredBookings.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الفحص التلقائي للحجوزات المنتهية.");
            }
        }
    }
}
