namespace MahiN.DataBase.Enums
{
    public enum PaymentStatus
    {
        Unpaid = 1,       // لم يتم الدفع
        PartiallyPaid = 2,// دفع عربون 
        Paid = 3,         // تم الدفع بالكامل
        Refunded = 4      // تم استرداد المبلغ
    }
}
