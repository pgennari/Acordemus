using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace acordemus.Models
{
    public class Condo()
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string? Name { get; set; } 
        public string? ShortName { get; set; }
        public List<Document>? Documents { get; set; } = new();
        public List<Unit>? Units { get; set; } = new();
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }


    }
}
