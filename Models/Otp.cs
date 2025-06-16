using System.ComponentModel.DataAnnotations;

namespace acordemus.Models
{
    public class Otp
    {
        [Key]
        public string? peopleId { get; set; }
        public string otp { get; set; }
        public DateTimeOffset expiresAt { get; set; }
    }
}