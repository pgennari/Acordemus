using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace acordemus.Models
{
    public class Member
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [BsonElement]
        public string PersonId { get; set; } = string.Empty;
        [BsonElement]
        public string CondoId { get; set; } = string.Empty;
        [BsonElement]
        public List<Role> Roles { get; set; } = [];
        [BsonElement]
        public string CreatedBy { get; set; } = string.Empty;
        [BsonElement]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [BsonElement]
        public string? UpdatedBy { get; internal set; }
        [BsonElement]
        public DateTime UpdatedAt { get; internal set; }
    }
}
