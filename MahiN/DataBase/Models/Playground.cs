using MahiN.DataBase.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahiN.DataBase.Models
{
    public class Playground
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public PlaygroundType PlaygroundType { get; set; }

        //  سعة الملعب/القطعة (عدد الأشخاص)
        public int Capacity { get; set; }

        [Column(TypeName =("decimal(18,2)"))]
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<BookingPlayground> BookingPlaygrounds { get; set; } = new HashSet<BookingPlayground>();
    }
}
