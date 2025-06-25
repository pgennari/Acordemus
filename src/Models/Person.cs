using acordemus.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace acordemus.Models
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        [BsonElement]
        public string? name { get; set; }
        [BsonElement]
        public string? email { get; set; }
        [BsonElement]
        public string? phoneNumber { get; set; }
        [BsonElement]
        public string? socialName { get; set; }
        [BsonElement]
        public string? cpf { get; set; }
        [BsonElement]
        public DateTime? CreatedAt { get; set; }
        [BsonElement]
        public DateTime? UpdatedAt { get; set; }
        [BsonElement]
        public string? UpdatedBy { get; set; }
        // Navigation property for roles
        [BsonIgnore]
        public Dictionary<Condo, Role> Roles { get; set; } = new();

        // Additional properties can be added as needed
    }
}
