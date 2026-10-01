namespace MahiN.DataBase.Enums
{
    public enum RestrictedAction
    {
        TournamentsOnly = 1,      // المنع من البطولات فقط
        FreePlaygroundBooking = 2,// المنع من حجز الملاعب العادية
        AllClubActivities = 3,    // المنع من كافة أنشطة النادي (بطولات + حجز)
        FullAccountBlock = 4      // حظر كامل للحساب (في الحالات الصارخة)
    }
}
