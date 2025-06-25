using acordemus.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace acordemus.Models
{
    public class Excerpt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string? DocumentId { get; set; }
        public string? ParentId { get; set; }

        private string _type;
        public string? Type
        {
            get => _type;
            set
            {
                if (!ExcerptType.AllowedTypes.Contains(value))
                    throw new ArgumentException($"Excert type invalid: {value}");
                _type = value;
            }
        }

        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? Order { get; set; }

        private string _status;
        public string? Status
        {
            get => _status; 
            set
            {
                if (!ExcerptStatus.AllowedStatus.Contains(value))
                    throw new ArgumentException($"Excerpt status invalid: {value}");
                _status = value;
            }
        }

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [BsonIgnore]
        public List<Excerpt> Excerpts { get; set; } = new();


        public List<string> Likes { get; set; } = new();
        public List<string> Dislikes { get; set; } = new();

        [BsonIgnore]
        public List<Comment> Comments { get; set; } = new();
    }
}
