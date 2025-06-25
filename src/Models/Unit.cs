using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using acordemus.Enums;

namespace acordemus.Models
{
    public class Unit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string? Block { get; set; }
        public string? Number { get; set; }
        public List<string> Owners { get; set; } = []; 

        private string _status;
        public string? Status
        {
            get => _status;
            set
            {
                if (!UnitStatus.AllowedStatus.Contains(value))
                    throw new ArgumentException($"Unit status invalid: {value}");
                _status = value;
            }
        }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
