using MahiN.Services.SuspensionService;

namespace MahiN.Scripts
{
    public class SuspensionCleanupHostedService(
            IServiceProvider _serviceProvider,
            ILogger<SuspensionCleanupHostedService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("تم تشغيل خدمة التنظيف التلقائي للعقوبات المنتهية.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var suspensionRepository = scope.ServiceProvider.GetRequiredService<ISuspensionRepository>();

                        var closedCount = await suspensionRepository.AutoCloseExpiredSuspensionsAsync();

                        if (closedCount > 0)
                        {
                            _logger.LogInformation("تم إغلاق {Count} عقوبة منتهية بنجاح.", closedCount);
                        }
                        else
                        {
                            _logger.LogInformation("تم فحص العقوبات: لا توجد عقوبات منتهية حالياً.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "حدث خطأ أثناء تنفيذ عملية التنظيف التلقائي للعقوبات.");
                }

                // حساب التوقيت المتبقي للتنفيذ التالي (الساعة 3:00 صباحاً بتوقيت مصر)
                var delay = GetDelayUntilNextRun();
                _logger.LogInformation("التنظيف التلقائي القادم سيكون بعد: {Hours} ساعة و {Minutes} دقيقة.",
                    (int)delay.TotalHours, delay.Minutes);

                await Task.Delay(delay, stoppingToken);
            }
        }

        private static TimeSpan GetDelayUntilNextRun()
        {
            // تحديد المنطقة الزمنية لمصر (تتعامل مع التوقيت الصيفي/الشتوي تلقائياً)
            TimeZoneInfo egyptTimeZone;
            try
            {
                // لنظام Windows
                egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // لنظام Linux / Docker
                egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
            }

            // تحويل وقت UTC الحالي إلى توقيت مصر
            var nowInEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

            // تحديد موعد التشغيل القادم (الساعة 3:00 الفجر بتوقيت مصر)
            var nextRunInEgypt = nowInEgypt.Date.AddHours(3);

            // إذا كان الوقت الحالي بعد 3:00 فجراً، يتم جدولة التشغيل للغد الساعة 3:00 فجراً
            if (nowInEgypt >= nextRunInEgypt)
            {
                nextRunInEgypt = nextRunInEgypt.AddDays(1);
            }

            // إرجاع الفارق الزمني المطلوب للانتظار
            return nextRunInEgypt - nowInEgypt;
        }
    }
}
