using MahiN.DataBase.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MahiN.DataBase.Models
{
    public class AppUser : IdentityUser
    {
        private const string BaseNfcUrl = "https://st-goerge-club.vercel.app/profile/";
        [Required]
        public string FullName { get; set; } = "";
        public Gender Gender { get; set; }
        public string NfcUrl => $"{BaseNfcUrl}{Id}";
        [Required]
        public string? PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public AgeGroup AgeGroup { get; set; }
        public DateOnly? Birthday { get; set; }
        public bool IsActive { get; set; } = false;
        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new HashSet<UserSubscription>();
    }
}