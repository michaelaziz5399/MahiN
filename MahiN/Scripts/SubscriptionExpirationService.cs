using MahiN.DataBase.Data;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Scripts
{
    public class SubscriptionExpirationService(
        IServiceScopeFactory _scopeFactory,
        ILogger<SubscriptionExpirationService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 1. تنفيذ الفحص فوراً أول ما الخدمة تبدأ/تستيقظ
                await CheckAndExpireSubscriptionsAsync(stoppingToken);

                // 2. حساب الوقت المتبقي حتى 12:00 منتصف الليل بتوقيت مصر
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                var egyptNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);
                var nextRunTime = egyptNow.Date.AddDays(1); // منتصف الليل القادم بتوقيت مصر

                var delay = nextRunTime - egyptNow;

                _logger.LogInformation("سيعمل فحص الاشتراكات القادم بعد {Hours} ساعة و {Minutes} دقيقة.",
                    (int)delay.TotalHours, delay.Minutes);

                // الانتظار حتى منتصف الليل
                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task CheckAndExpireSubscriptionsAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("جاري فحص وتحديث الاشتراكات المنتهية...");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // جلب توقيت مصر الحالي بدون وقت (تاريخ اليوم فقط)
                    var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                    var todayEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone).Date;

                    // جلب الاشتراكات النشطة التي انتهت قبل اليوم
                    var expiredSubscriptions = await dbContext.UserSubscriptions
                        .Where(s => s.IsActive && s.EndDate < todayEgypt)
                        .ToListAsync(stoppingToken);

                    if (expiredSubscriptions.Any())
                    {
                        foreach (var sub in expiredSubscriptions)
                        {
                            sub.IsActive = false;
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("تم إلغاء تفعيل {Count} اشتراك انتهى تاريخها.", expiredSubscriptions.Count);
                    }
                    else
                    {
                        _logger.LogInformation("لا توجد اشتراكات منتهية اليوم.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء فحص الاشتراكات المنتهية.");
            }
        }
    }
}