using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnHttpContext.Models;

public class User
{
    [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;
    [BsonElement("email"), BsonRepresentation(BsonType.String)]
    public string Email { get; set; } = null!;
    [BsonElement("password"), BsonRepresentation(BsonType.String)]
    public string Password { get; set; } = null!;
    [BsonElement("password_salt"), BsonRepresentation(BsonType.String)]
    public string PasswordSalt { get; set; } = null!;
    [BsonElement("roles")]
    public HashSet<string> Roles { get; set; } = new();
    [BsonElement("date_created"), BsonRepresentation(BsonType.DateTime)]
    public DateTime DateCreated { get; set; }
}