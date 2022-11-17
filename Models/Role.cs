using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnHttpContext.Models;

public class Role
{
    [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string RoleId { get; set; } = null!;
    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = null!;
}