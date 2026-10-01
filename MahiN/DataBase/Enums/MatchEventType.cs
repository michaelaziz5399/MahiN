namespace MahiN.DataBase.Enums
{
    public enum MatchEventType
    {
        Goal = 1,                 // هدف عادي
        PenaltyGoal = 2,          // هدف من ركلة جزاء
        OwnGoal = 3,              // هدف عكسي
        YellowCard = 4,           // كارت أصفر
        SecondYellowCard = 5,     // كارت أصفر ثاني
        RedCard = 6,              // كارت أحمر مباشر
        Substitution = 7,         // تبديل
        MissedPenalty = 8,        // ركلة جزاء ضائعة
        Assist = 9,               // تمريرة حاسمة
        Foul = 10,                // خطأ / فاول
        FreeKick = 11             // ركلة حرة مباشرة
    }
}
