using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SAVE_ANIMLS.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        [Required]
        public string Name { get; set; }

        [BsonElement("email")]
        [Required]
        public string Email { get; set; }

        [BsonElement("password")]
        [Required]
        public string Password { get; set; }

        [BsonElement("DateOfBirth")]
        [Required]
        public DateOnly DateOfBirth { get; set; }

        [BsonElement("address")]
        [Required]
        public string Address { get; set; }

        [BsonElement("educationLevel")]
        [Required]
        public string EducationLevel { get; set; }

        [BsonElement("imageProfile")]
        [Required]
        public string ImageProfile { get; set; }

        [BsonElement("role")]
        [Required]
        public UserRole Role { get; set; }

        [BsonElement("IsApproved")]
        public bool IsApproved { get; set; }
    }

    public enum UserRole
    {
        AdminUser,
        VeterinarianUser,
        AssistantUser,
        NormalUser
    }

    public class AdminUser : User
    {
        // Add additional properties or methods specific to the AdminUser
    }

    public class VeterinarianUser : User
    {
        // Add additional properties or methods specific to the VeterinarianUser
    }

    public class AssistantUser : User
    {
        // Add additional properties or methods specific to the AssistantUser
    }

    public class NormalUser : User
    {
        // Add additional properties or methods specific to the NormalUser
    }
}