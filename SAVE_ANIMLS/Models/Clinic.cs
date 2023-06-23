using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

[BsonIgnoreExtraElements]
public class Clinic
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    [Required]
    public string Name { get; set; }

    [BsonElement("address")]
    [Required]
    public string Address { get; set; }

    [BsonElement("openingTime")]
    [Required]
    public string OpeningTime { get; set; }

    [BsonElement("closingTime")]
    [Required]
    public string ClosingTime { get; set; }

    [BsonElement("location")]
    [Required]
    public Location Location { get; set; }

    [BsonElement("veterinarian")]
    [Required]
    public string Veterinarian { get; set; }

    [BsonElement("services")]
    [Required]
    public List<Service> Services { get; set; }
}

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class Service
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }
}