using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserService.Enums;

namespace UserService.Models;

public class Profile
{
    [Key]
    [Required]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }
    
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
    
    public int? PartnerUserId { get; set; }
}