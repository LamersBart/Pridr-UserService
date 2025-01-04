using UserService.Enums;

namespace UserService.DTOs;

public class ProfileUpdateDto
{
    public required Sexuality Sexuality { get; set; }
    
    public required LookingFor LookingFor  { get; set; }
    
    public required RelationStatus RelationStatus  { get; set; }
    
    public int? Age  { get; set; }
    
    public double? Weight  { get; set; }
    
    public double? Height  { get; set; }
    
    public string? UserName { get; set; }
    
    // public int? PartnerUserId  { get; set; }
}