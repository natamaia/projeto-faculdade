using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model
{
    public class UserClient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!; // Link to the main User model

        public long Cfp { get; set; } = 0;
        public string Address { get; set; } = null!;
        public long PhoneNumber { get; set; } = 0;
        public long Cep { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
