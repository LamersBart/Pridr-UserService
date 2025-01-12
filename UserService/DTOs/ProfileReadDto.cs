using UserService.Enums;

namespace UserService.DTOs;

public class ProfileReadDto
{
    public required int Id { get; set; }
    
    public required string KeyCloakId { get; set; }
    
    public required Sexuality Sexuality { get; set; }
    
    public required LookingFor LookingFor  { get; set; }
    
    public required RelationStatus RelationStatus  { get; set; }
    
    public required double Latitude { get; set; }
    
    public required double Longitude { get; set; }
    
    public int? Age  { get; set; }
    
    public double? Weight  { get; set; }
    
    public double? Height  { get; set; }

    public string? UserName { get; set; }
    
}