using UserService.Enums;

namespace UserService.DTOs;

public class ProfileReadDto
{
    public required int Id { get; set; }
    
    public required int UserId { get; set; }
    
    public required Sexuality Sexuality { get; set; }
    
    public required DateTime? Birthday  { get; set; }
    
    public required double? Weight  { get; set; }
    
    public required double? Height  { get; set; }
    
    public required RelationStatus? RelationStatus  { get; set; }
    
    public required int? PartnerUserId  { get; set; }
    
    public required LookingFor LookingFor  { get; set; }
}