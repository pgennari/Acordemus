using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using acordemus.Enums;

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

        private string _type;
        public string? Type
        {
            get => _type;
            set
            {
                if (!DocumentType.AllowedTypes.Contains(value))
                    throw new ArgumentException($"Document type invalid: {value}");
                _type = value;
            }
        }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [BsonIgnore]
        public List<Excerpt> Excerpts { get; set; } = new();

    }
}
