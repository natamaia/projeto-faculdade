using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model
{
    public class UserClient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Username { get; set; } = null!;
        public int Cfp { get; set; } = 0;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int PhoneNumber { get; set; } = 0;
        public int Cep { get; set; } = 0;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}