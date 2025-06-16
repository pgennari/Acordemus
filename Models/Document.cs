using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace acordemus.Models
{
    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string? CondoId { get; set; } 
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [BsonIgnore]
        public List<Excerpt> Excerpts { get; set; } = new();

    }
}
