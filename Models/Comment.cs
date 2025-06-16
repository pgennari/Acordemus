using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace acordemus.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ExcerptId { get; set; }
        public string? ParentId { get; set; }
        public string? User { get; set; }
        public string? Content { get; set; }
        public List<string> Likes { get; set; }
        public List<string> Dislikes { get; set; }
        public DateTime CreatedAt { get; set; }
        [BsonIgnore]
        public List<Comment> Comments { get; set; } = new();
    }
}
