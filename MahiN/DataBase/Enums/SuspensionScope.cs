namespace MahiN.DataBase.Enums
{
    public enum SuspensionScope
    {
        CurrentTournamentOnly = 1,   // البطولة الحالية فقط
        NumberOfTournaments = 2,     // عدد محدد من البطولات
        TimePeriod = 3,              // فترة زمنية محددة (تاريخ بداية ونهاية)
        Permanent = 4                // حرمان دائم
    }
}
