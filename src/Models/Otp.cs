using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace acordemus.Models
{
    public class Otp
    {
        [BsonId]
        public string email { get; set; }
        [BsonElement]
        public string? otp { get; set; }
        [BsonElement]
        public DateTimeOffset? expiresAt { get; set; }
    }
}