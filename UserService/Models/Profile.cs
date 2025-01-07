using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserService.Enums;

namespace UserService.Models;

public class Profile
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(60)]
    [Required]
    public required string KeyCloakId { get; set; }
    
    [Required]
    public required Sexuality Sexuality { get; set; }
    
    [Required]
    public required LookingFor LookingFor { get; set; }
    
    [Required]
    public required RelationStatus RelationStatus { get; set; }
    
    [Required]
    public required double Latitude { get; set; }
    
    [Required]
    public required double Longitude { get; set; }
    
    public int? Age { get; set; }
    
    public double? Weight { get; set; }
    
    public double? Height { get; set; }
    
    [MaxLength(30)]
    public string? UserName { get; set; }
    
    // public int? PartnerUserId { get; set; }

    public Profile()
    {
        Sexuality = Sexuality.Unknown;
        LookingFor = LookingFor.Friendship;
        Age = 0;
        Latitude = 0.0;
        Longitude = 0.0;
        Weight = 0.0;
        Height = 0.0;
        RelationStatus = RelationStatus.Unknown;
        // PartnerUserId = 0;
        UserName = "";
    }
}