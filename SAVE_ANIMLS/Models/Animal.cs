using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

public class AnimalModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    [Required]
    public string Name { get; set; }

    [BsonElement("species")]
    [Required]
    public string Species { get; set; }

    [BsonElement("age")]
    public int Age { get; set; }

    [BsonElement("gender")]
    [Required]
    public string Gender { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("address")]
    public string Address { get; set; }

    [BsonElement("medicalCondition")]
    public string MedicalCondition { get; set; }

    [BsonElement("location")]
    public string Location { get; set; }

    [BsonElement("rescueStatus")]
    public string RescueStatus { get; set; }
}