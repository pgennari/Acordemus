using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace acordemus.Models
{
    public class Invitation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [BsonElement]
        public string CondoId { get; set; } = string.Empty;
        [BsonElement]
        public string Email { get; set; } = string.Empty;
        [BsonElement]
        public string Token { get; set; } = Guid.NewGuid().ToString();
        [BsonElement]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [BsonElement]
        public string? CreatedBy { get; set; }
        [BsonElement]
        public DateTime? AcceptedAt { get; set; }
    }
}
