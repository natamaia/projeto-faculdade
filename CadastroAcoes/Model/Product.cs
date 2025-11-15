using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;

        // Usar Decimal128 no MongoDB para valores monetários
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }

        public int Quantity { get; set; } = 0;

        // opcional: relacionar com vendedor
        public string? VendorId { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
