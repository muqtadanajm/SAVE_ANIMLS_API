using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ReportModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("animalId")]
    [Required]
    public ObjectId AnimalId { get; set; }

    [BsonElement("reporterId")]
    [Required]
    public ObjectId ReporterId { get; set; }

    [BsonElement("date")]
    [Required]
    public DateTime Date { get; set; }

    [BsonElement("description")]
    [Required]
    public string Description { get; set; }

    [BsonElement("location")]
    public Location Location { get; set; }

    [BsonElement("severity")]
    public string Severity { get; set; }

    [BsonElement("images")]
    public List<string> Images { get; set; }

    [BsonElement("witnesses")]
    public List<string> Witnesses { get; set; }

    [BsonElement("status")]
    public string Status { get; set; }

    [BsonElement("followUpActions")]
    public List<string> FollowUpActions { get; set; }

    [BsonElement("comments")]
    public List<string> Comments { get; set; }
}
