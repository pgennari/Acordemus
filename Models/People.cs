using acordemus.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace acordemus.Models
{
    public class People
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement]
        public string? Name { get; set; }
        [BsonElement]
        public string? Email { get; set; }
        [BsonElement]
        public string? PhoneNumber { get; set; }
        [BsonElement]
        public string? SocialName { get; set; }
        [BsonElement]
        public DateTime? CreatedAt { get; set; }
        [BsonElement]
        public DateTime? UpdatedAt { get; set; }
        [BsonElement]
        public string? Otp { get; set; }
        [BsonElement]
        public DateTime? ExpiresAt { get; set; }
        // Navigation property for roles
        [BsonIgnore]
        public Dictionary<Condo, Role> Roles { get; set; } = new();

        // Additional properties can be added as needed
    }
}
